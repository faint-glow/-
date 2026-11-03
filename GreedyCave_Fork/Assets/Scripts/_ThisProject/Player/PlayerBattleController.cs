using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBattleController : Warrior, IAnimtorAble
{
    private PlayerController playerController;
    private PlayerAnimatorController animatorController;
    private Vector3 lastPosition;//战斗前的位置
    private Vector3 battleEnemyPos;

    private bool isRuning;
    private float runProcess;

    public PlayerBattleController(PlayerController playerController) : base(1, playerController._playerInfo.propertyFactors)
    {
        this.playerController = playerController;
        this.animatorController = new PlayerAnimatorController(this);

        EventCenter.GetInstance().AddEventListener<Scene>(EventCenterEnum.切换场景后.ToString(), ControllPhysiology);
    }
    private void ControllPhysiology(Scene scene)
    {
        if (scene.name == SceneName.MazeScene)
            physiologyMgr.Start();
        else
            physiologyMgr.End();
    }
    public override int GetDir()
    {
        return playerController.GetDir();
    }
    public override Vector3 GetWarriorPos()
    {
        return playerController.GetPlayerPos();
    }
    public PlayerAnimatorController GetPlayerAnimatorController()
    {
        if (animatorController == null)
            animatorController = new PlayerAnimatorController(this);
        return animatorController;
    }
    public override MyAnimatorController GetAnimatorController()
    {
        if (animatorController == null)
            animatorController = new PlayerAnimatorController(this);
        return animatorController;
    }
    public Animator GetAnimator()
    {
        return playerController.GetAnimator();
    }

    public override void AfterChangeProperty(PropertyType propertyType, float value, float overflow)
    {
        if (property.IsExperienceFull())
            this.LevelUpOne(overflow);

        if (propertyType == PropertyType.金币 && value != 0)
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.changeCoin);
        }

        if (propertyType != PropertyType.饱腹度)//饱腹度会一直随时间变化，会触发多次
        {
            if (SceneMgr.GetInstance().GetCurrentScene().name == SceneName.MainCity)
                RecoverState();
            playerController.RefreshStatePanel();
        }
    }

    protected override Vector3 StartBattle(Warrior warrior)
    {
        Enemy enemy = warrior as Enemy;
        runProcess = 0;
        isRuning = false;

        Vector3 battlePos = enemy.StartBattleHandler(this);//获取一个战斗位置
        battlePos += new Vector3(0, -0.001f, 0);
        lastPosition = playerController.GetPlayerPos();//记录玩家战斗前的位置
        battleEnemyPos = warrior.GetWarriorPos();

        playerController.BattleAnim();//战斗动画

        Vector3 battlePosition = battlePos;
        playerController.DirControl(battlePosition, enemy.GetWarriorPos());//把敌人位置传入，可以控制玩家朝向
        playerController.NormalMoveToTarget(battlePosition);

        if (IsHost())
        {
            UIManager.GetInstance().ShowPanelAsync<FightPanel>(FightPanel.prefabsPath, PanelLayer.Mid, null, (fightPanel) =>
            {
                fightPanel.SetEnemy(warrior as Enemy);
            });
        }
        return GetWarriorPos();
    }
    public override float GetNormalAttackTriggerTime()
    {
        return 0.37f;
    }
    public float GetRunProcess()
    {
        return runProcess;
    }
    public void StartRunAway()
    {
        if (isRuning == true)
            return;
        isRuning = true;

        playerController.SetDir(-playerController.GetDir());
        playerController.WalkAnim();
        float runTime = 3;
        MonoMgr.GetInstance().CountdownTimer(runTime, null,
            (countdownTimerInfo) =>
            {
                if (isRuning == false)
                    countdownTimerInfo.isBreak = true;
                Animator anim = GetAnimator();
                if (anim != null)
                    countdownTimerInfo.speed = anim.speed;
                runProcess = countdownTimerInfo.CurrentTime / runTime;
            },
            (countdownTimerInfo) =>
            {
                if (isRuning == false)
                    return;
                MoveToTarget(lastPosition, () =>
                {
                    RunAwayHandler();
                });//逃跑结束调用方法释放资源
            });
    }
    //获胜调用
    protected override void Win()
    {
        this.battleInfo.ForeachAllDieTarget(LockTargetType.Enemy, (enemyWarrior) =>
         {
             PlayerMgr.Instance.UseBooty(enemyWarrior.GetBooty(), enemyWarrior);
             EventCenter.GetInstance().EventTrigger<Warrior>(EventCenterEnum.击败.ToString(), enemyWarrior);

         });
        MoveToTarget(battleEnemyPos + new Vector3(-GetDir() * 0.001f, 0, 0), null);//战斗胜利前往敌人所在格子
    }
    protected override void Die()
    {
        bool isBreak = false;
        //bool isShowFX = false;
        //float lastClickTime = -1;
        //GameObject obj = null;
        UIManager.GetInstance().HideAllBasePanel(new HashSet<string>()
        {
            TestButtonsPanel.prefabsPath,

            SceneNamePanel.prefabsPath,
            FightPanel.prefabsPath,
            PlayerStatePanelMgr.prefabsPath,
            TaskPanel.prefabsPath,
            MoveControlPanel.prefabsPath,
            OperatePanel.prefabsPath,
        });
        SetProperty(PropertyType.金币, 0);

        RemoveContainter(PlayerMgr.Instance.GetBag());//立即清空背包，防止玩家手动闪退
        PlayerMgr.Instance.TakeOffEquips((equip) =>
        {
            if (equip == null)
                return false;
            if (equip.isNeverDrop == true)
                return false;
            return true;//死亡，全部移除
        });
        RemoveContainter(PlayerMgr.Instance.GetBag());//先清空背包再脱装备是因为，如果背包已满，脱装会失败

        if (isRuning)
            playerController.SetDir(-GetDir());
        GetAnimatorController().SetTrigger("Die");
        UIManager.GetInstance().ShowTips("您已阵亡，双击结束动画");
        //添加玩家输出禁用
        InputMgr.GetInstance().AddForbidPlayerInput("Die", () =>
        {
            return isBreak;//isBreak由下面的倒计时方法结束调用
        });
        MonoMgr.GetInstance().DelayAction(1.5f, () =>
         {
             GameManager.Instance.ChangeSceneAfterVignette(SceneName.MainCity);
             isBreak = true;
         });

        isRuning = false;
        runProcess = 0;
    }
    
    protected override void EndBattle(EndBattleType endBattleType)//在所有相关方法的最后调用，比如Win，RunAway，Die等
    {
        UIManager.GetInstance().HidePanel(FightPanel.prefabsPath);
        PlayerMgr.Instance.ClearCD();
    }
    //战斗结束以后传入目标移动位置
    private void MoveToTarget(Vector3 targetPos, Action action)
    {
        playerController.WalkAnim();//走路动画
        playerController.DirControl(playerController.GetPlayerPos(), targetPos);//把敌人位置传入，可以控制玩家朝向
        playerController.NormalMoveToTarget(targetPos, () =>
         {
             isRuning = false;
             playerController.StopAnim();//休息动画
             if (action != null)
                 action.Invoke();
             PlayerMgr.Instance.SavePlayerData(false);//战斗获胜和逃跑时会触发
         });
    }

    private void RemoveContainter(Bag<Item> containter)
    {
        if (containter == null)
            return;
        containter.RemoveBagItems((item) =>
        {
            if (item == null)
                return false;
            if (item.isNeverDrop == true)
            {
                if (item.GetSpecialType() == ContainerType.MagicBall.ToString())//如果遇到魔法球，就继续删除内容物
                {
                    ContainerProp prop = item as ContainerProp;
                    if (prop != null)
                        RemoveContainter(prop.GetBag());
                }
                return false;//绝不掉落的物品自己本身不掉落，但是如果他里面有东西，里面的是可以掉落的
            }
            return true;
        });
    }
    //别人获取战利品，没人能从玩家身上扒东西
    public override Booty GetBooty()
    {
        return null;
    }
    public override string GetName()
    {
        return PlayerMgr.Instance.GetPlayerData().playerName;
    }
    public bool IsHost()
    {
        return playerController.IsHost();
    }
    public override Dictionary<WearEquipType, List<Equip>> GetEquips()
    {
        if (playerController == null)
            return null;

        return playerController.GetEquips();
    }
    public override GameObject GetObj()
    {
        return playerController.GetPlayerObject();
    }
    public override void LevelUp(int level, float overflowExperience, bool isFull = false)
    {
        base.LevelUp(level, overflowExperience, isFull);
        EventCenter.GetInstance().EventTrigger<int>(EventCenterEnum.升级.ToString(), GetLevel());
    }
    public override int GetOriginalDir()
    {
        return Warrior.left;
    }
}
