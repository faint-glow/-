using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayerMgr : IPlayerAble
{
    private GameObject player;
    private Animator anim;
    private PlayerController playerController;
    private PlayerEquipController playerEquipController;
    private PlayerDataMgr playerDataMgr;
    private Dictionary<string, Skill> skills = new Dictionary<string, Skill>();
    private Queue<Vector3> nextPosQue = new Queue<Vector3>();
    private bool isMoving = false;
    public SyncPlayerMgr(PlayerData playerData)
    {
        //playerDataMgr = new PlayerDataMgr(playerData);
        //playerEquipController = new PlayerEquipController();
        //playerController = new PlayerController(this);

        ////CreatePlayerObj(playerData);

        //if (SceneMgr.GetInstance().GetCurrentScene().name == "MainCity")
        //    GetWarrior().RecoverState();

        //GameManager.Instance.InitialPlayerPos(this);//待修改
        ////MonoMgr.GetInstance().AddUpdateListener(SyncUpdate);
    }


    public void AddNextPos(Vector3 targetPos)
    {
        nextPosQue.Enqueue(targetPos);
    }

    private void SyncUpdate()
    {
        if (playerController.IsBattleing())
        {
            if (nextPosQue.Count > 0)
            {
                nextPosQue.Clear();
            }
        }

        if (nextPosQue.Count > 0 && isMoving == false)
        {
            Vector3 targetPos = nextPosQue.Peek();
            isMoving = true;
            playerController.WalkAnim();
            playerController.DirControl(playerController.GetPlayerPos(), targetPos);//传入目标位置，自动确定玩家方向
            playerController.NormalMoveToTarget(targetPos, () =>
            {
                isMoving = false;
                if (playerController.IsBattleing())
                    return;

                nextPosQue.Dequeue();
                if (nextPosQue.Count <= 0)
                    playerController.StopAnim();
            },
            () =>
            {
                return playerController.IsBattleing();
            });
        }
    }
    public GameObject GetPlayerObject()
    {
        //if (player == null)
        //{
        //    if (GetPlayerData().gender == Gender.男)
        //        player = ResMgr.GetInstance().Load<GameObject>("Player/NewPlayer(man)");
        //    else
        //        player = ResMgr.GetInstance().Load<GameObject>("Player/NewPlayer(woman)");
        //}
        return null;
    }
    public Animator GetAnimator()
    {
        if (anim == null)
        {
            anim = player.GetComponentInChildren<Animator>();
        }
        return anim;
    }
    public void DestoryPlayer()
    {
        if (player != null)
            GameObject.Destroy(player);

        MonoMgr.GetInstance().RemoveUpdateListener(SyncUpdate);
    }
    public Equip GetEquip(WearEquipType equipType)
    {
        return playerEquipController.GetEquip(equipType);
    }
    public PlayerData GetPlayerData()
    {
        return playerDataMgr.GetPlayerData();
    }
    public void SavePlayerData()
    {
        //playerDataMgr.SaveData(GetWarrior());
    }

    public void SetPlayerPos(Vector3 targetPos)
    {
        playerController.SetPlayerPos(targetPos);
    }

    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    //public void CreatePlayerObj(PlayerData playerData)
    //{
    //    if (playerData.gender == Gender.男)
    //        player = ResMgr.GetInstance().Load<GameObject>("Player/NewPlayer(man)");
    //    else
    //        player = ResMgr.GetInstance().Load<GameObject>("Player/NewPlayer(woman)");

    //    //playerController = new PlayerController(player, this);
    //}

    public Warrior GetWarrior()
    {
        return playerController.GetWarrior();
    }

    public Texture GetStaticAvatar()
    {
        throw new System.NotImplementedException();
    }

    //public Texture GetStaticAvatar()
    //{
    //    return playerController.GetStaticAvatar();
    //}

    public void StartBattle(Enemy enemy)
    {
        playerController.StartBattle(enemy);
    }
    public void NormalAttack()
    {

    }

    public void TriggerSkill(string skillInfoName)
    {
        //if (!skills.ContainsKey(skillInfoName))
        //{
        //    SkillInfo skillInfo = Skill.GetSkillInfo(skillInfoName);
        //    Skill_Instant skill = new Skill_Instant(GetWarrior(), SkillController.PlayerPhysicsAnimInfo, skillInfo);
        //    skills[skillInfoName] = skill;
        //}
        //skills[skillInfoName].TriggerSkill(GetWarrior(), null);
    }

    public Dictionary<WearEquipType, List<Equip>> GetEquips()
    {
        return null;//playerEquipController.EquipDic;
    }

    public Vector3Int GetGirdPos()
    {
        return ToolsMgr.GetGirdPos(player.transform.position);
    }
    public PlayerAnimatorController GetPlayerAnimContoller()
    {
        return GetWarrior().GetAnimatorController() as PlayerAnimatorController;
    }

    //public bool IsBelongMe(IBelongAble belongAble)
    //{
    //    if (belongAble == null)
    //        return false;
    //    if (belongAble.GetHolder() == null && belongAble.GetWarrior() == null)
    //        return false;

    //    if (belongAble.GetHolder() != null)
    //        return belongAble.GetHolder() == PlayerMgr.Instance;

    //    if (belongAble.GetWarrior() != null)
    //        return belongAble.GetWarrior() == PlayerMgr.Instance.GetWarrior();

    //    return false;
    //}
    public PlayerAnimState GetPlayerAnimState()
    {
        return playerController.GetPlayerAnimState();
    }

    public void OpenTreasureBox(TreasureBoxObj box)
    {
        throw new System.NotImplementedException();
    }

    public void OpenJar(JarObj jar)
    {
        throw new System.NotImplementedException();
    }

    public void SavePlayerData(bool isShowTip)
    {
        throw new System.NotImplementedException();
    }

    public bool RecycleDecorate(DecorateObj decorateObj)
    {
        throw new System.NotImplementedException();
    }

    public Texture GetStaticAvatar(bool isRebulid = false)
    {
        return null;
    }
}
