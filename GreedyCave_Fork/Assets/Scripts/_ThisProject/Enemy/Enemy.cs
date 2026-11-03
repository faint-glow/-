using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum EnemyType
{
    Normal,
    Eltie,
    Goalkeeper,
    Boss,
}

public class Enemy : Warrior, IAnimtorAble, IEffectSourceAble
{
    private static BuffInfo elite;
    public static BuffInfo Elite
    {
        get
        {
            if (elite == null)
            {
                elite = ResMgr.GetInstance().Load<BuffInfo>("Buff/EnemyBuff/Elite");
            }
            return elite;
        }
    }
    private static BuffInfo goalkeeper;
    public static BuffInfo Goalkeeper
    {
        get
        {
            if (goalkeeper == null)
            {
                goalkeeper = ResMgr.GetInstance().Load<BuffInfo>("Buff/EnemyBuff/Goalkeeper");
            }
            return goalkeeper;
        }
    }
    protected EnemyObj mazeObj;
    private MazeGrid grid;//敌人对应的迷宫格子
    private Animator anim;
    private Vector3 lastPos;
    private EnemyType enemyType;
    private Buff enemyTypeBuff;
    //private Texture avataImage;
    public Sprite AvataImage
    {
        get 
        {
            if (enemyInfo == null)
                return null;
            return enemyInfo.avataImage;
        }
    }

    private EnemyAnimatorController animatorController;
    private Dictionary<int, HashSet<Warrior>> battleWarrior = new Dictionary<int, HashSet<Warrior>>()
    {
        {-1,new HashSet<Warrior>()},
        {1,new HashSet<Warrior>() },
    };//-1表示左边，1表示右边

    private int skillInterval;//怪物释放技能的cd，与技能自身cd共同作用
    //public int minSkillAttackCount = 2;
    //public int maxSkillAttackCount = 6;

    private List<Skill> skillReady = new List<Skill>();//准备好的技能
    private List<Skill> skillWait = new List<Skill>();//cd中的技能
    private EnemyInfo enemyInfo;

    public Enemy(MazeGrid grid, Transform father, EnemyInfo enemyInfo,GameObject enemyObj = null) : base(1, enemyInfo.propertyFactors)
    {
        if (enemyInfo == null)
        {
            Debug.Log("这。。这不对吧");
            return;
        }
        this.grid = grid;

        //超级特别注意：GameObject.Instantiate如果不在方法里就设置好初始位置，而是对返回的物体设置初始位置的话
        //物体的实际位置会晚至少一帧才会移动，虽然此时Debug他的位置是我们移动后的位置，但是如果此时对移动后的位置进行碰撞体检测是无效的
        //new GameObject也是一样的，但是new GameObject没办法在构造函数里设置初始位置，因此，这个检测是无效的
        GameObject obj = null;
        if (enemyObj != null)
            obj = enemyObj;
        else
            obj = GameObject.Instantiate(enemyInfo.obj, new Vector3(this.grid.x + 0.5f, this.grid.y + 0.105f, 0), Quaternion.identity, father);

        this.mazeObj = obj.GetComponent<EnemyObj>();
        if (this.mazeObj == null)
            this.mazeObj = obj.AddComponent<EnemyObj>();
        this.mazeObj.Initial(this);

        this.lastPos = this.mazeObj.transform.position;

        this.enemyInfo = enemyInfo;
        this.animatorController = new EnemyAnimatorController(this, this.mazeObj.gameObject.name);

        if (enemyInfo.skillInfoAndAnimInfo != null)
        {
            for (int i = 0; i < enemyInfo.skillInfoAndAnimInfo.Length; i++)
            {
                SkillInfo skillInfo = enemyInfo.skillInfoAndAnimInfo[i].skillInfo;
                AnimationMgrInfo animationMgrInfo = enemyInfo.skillInfoAndAnimInfo[i].animationMgrInfo;
                Skill skill = new Skill(this, animationMgrInfo, skillInfo);
                this.skillReady.Add(skill);
            }
        }
        this.skillInterval = Random.Range(enemyInfo.minSkillAttackCount, enemyInfo.maxSkillAttackCount + 1);
        //this.avataImage = AvatarImage.GetStaticTex(obj, new Vector3(0, 0.5f, -1f));

        SortingGroup sortingGroup = GetAnimator().gameObject.GetComponent<SortingGroup>();
        if (sortingGroup == null)
            sortingGroup = GetAnimator().gameObject.AddComponent<SortingGroup>();
        sortingGroup.sortingLayerName = PlayerMgr.PlayerLayerName;
        sortingGroup.sortingOrder = PlayerMgr.PlayerLayerOrder;
    }
    public void PresetEventTrigger(IPlayerAble player)
    {
        player.StartBattle(this);
    }
    public GameObject GetEnemyObj()
    {
        if (mazeObj == null)
            return null;
        return mazeObj.gameObject;
    }
    public Animator GetAnimator()
    {
        if (GetEnemyObj() == null)
            return null;
        if (anim == null)
            anim = GetEnemyObj().GetComponentInChildren<Animator>();
        return anim;
    }
    public EnemyType GetEnemyType()
    {
        return this.enemyType;
    }
    public void SetEnemyType(EnemyType enemyType)
    {
        if (enemyTypeBuff != null)
        {
            RemoveBuff(enemyTypeBuff);
            enemyTypeBuff = null;
        }
        this.enemyType = enemyType;
        if (enemyType == EnemyType.Eltie)
            enemyTypeBuff = new Buff(Enemy.Elite, new List<IEffectSourceAble>() { this });
        else if (enemyType == EnemyType.Goalkeeper)
            enemyTypeBuff = new Buff(Enemy.Goalkeeper, new List<IEffectSourceAble>() { this });
        if (enemyTypeBuff != null)
            this.AddBuff(this, enemyTypeBuff);

        this.RecoverState_Handler();
    }
    protected override Vector3 StartBattle(Warrior warrior)
    {
        Vector3 targetPos = GetBattlePos(warrior);
        animatorController.DirControl(GetWarriorPos(), targetPos);
        animatorController.StartBattle();
        return targetPos;
    }
    //玩家退出战斗的处理
    protected override void DecreaseWarrior(Warrior warrior)
    {
        foreach (KeyValuePair<int, HashSet<Warrior>> item in battleWarrior)
        {
            if (item.Value.Contains(warrior))
            {
                item.Value.Remove(warrior);
                break;
            }
        }
    }
    //所有战斗玩家逃跑调用
    protected override void Win()
    {
        animatorController.EndBattle();
        RecoverState_Handler();
    }
    protected override void EndBattle(EndBattleType endBattleType)
    {
        if(endBattleType == EndBattleType.Die)
        {
            this.battleInfo.ForeachAllOptionalTarget(LockTargetType.Enemy, (warrior) =>
             {

             });
        }
    }
    protected override void Die()
    {
        if (mazeObj == null)
            return;
        if (grid != null)//boss是预设好的，没有grid
            MazeGridMgr.ReleaseGrid(grid);

        OnDie();
        GameObject.Destroy(mazeObj.gameObject);
    }
    protected virtual void OnDie()
    {
        
    }
    private Vector3 GetBattlePos(Warrior player)
    {
        Vector3 targetPos = GetWarriorPos();
        float numX = player.GetWarriorPos().x - GetWarriorPos().x;
        int index = 0;
        if (Mathf.Abs(numX) <= 0.2)//玩家在这个敌人的上下方时
        {
            float numY = player.GetWarriorPos().y - GetWarriorPos().y;
            index = numY > 0.2 ? 1 : -1;//如果玩家在上方，则让玩家在右边进行战斗,反之，在左边战斗
        }
        else
        {
            index = numX > 0.2 ? 1 : -1;
        }
        if (battleWarrior[index].Count >= 2)//如果某一边已经站了两个人。则要换到另一边去
            index = -index;
        if (battleWarrior[index].Count == 1)
            targetPos.y += GetBattlePosYTwo();
        else if (battleWarrior[index].Count < 1)
            targetPos.y += GetBattlePosYOne();
        targetPos.x += index * GetBattlePosX();

        battleWarrior[index].Add(player);
        return targetPos;
    }
    //这将成为1的倍数，用于控制X方向的偏移
    protected virtual int GetBattlePosX()
    {
        return 1;
    }
    //某一边第一个来到的玩家的Y方向偏移
    protected virtual float GetBattlePosYOne()
    {
        return 0;
    }
    //某一边已经有一个人时，这将成为另一个来到时Y方向的偏移
    protected virtual float GetBattlePosYTwo()
    {
        return 0.4f;
    }
    public override Vector3 GetWarriorPos()
    {
        if (mazeObj != null)
            lastPos = mazeObj.gameObject.transform.position;
        return lastPos;
    }
    public override MyAnimatorController GetAnimatorController()
    {
        return animatorController;
    }
    public override int GetDir()
    {
        return animatorController.GetDir();
    }
    public override Booty GetBooty()
    {
        Booty targetBooty = enemyInfo.booty;

        Equip equip = null;
        if (GetEnemyType() == EnemyType.Eltie)
            equip = TreasureBoxObj.OpenExcellentTreasureBox();
        else if (GetEnemyType() == EnemyType.Goalkeeper)
            equip = TreasureBoxObj.OpenValuableTreasureBox();
        else if (GetEnemyType() == EnemyType.Boss)
            equip = TreasureBoxObj.OpenKingTreasureBox();
        if (equip != null)
        {
            targetBooty = Booty.CopyBooty(enemyInfo.booty);
            targetBooty.itemList.Add(new ItemInfoAndRatio(equip.itemInfo,1));
        }

        return targetBooty;
    }
    public override void AfterChangeProperty(PropertyType propertyType,float value, float overflow)
    {
        FightPanel panel = UIManager.GetInstance().GetExistPanel(FightPanel.prefabsPath) as FightPanel;
        if (panel == null)
            return;

        panel.RefreshPanel();
    }
    public override string GetName()
    {
        return enemyInfo.enemyName;
    }
    public override float GetNormalAttackTriggerTime()
    {
        return enemyInfo.normalAttackTriggerTime;
    }
    public override void AfterNormalAttack()
    {
        skillInterval -= 1;

        for (int i = 0; i < skillWait.Count; i++)
        {
            skillWait[i].SkillCurrentCD -= 1;
            if (skillWait[i].SkillCurrentCD <= 0)
            {
                skillReady.Add(skillWait[i]);
                skillWait.RemoveAt(i);
                i--;//循环内部移除元素，需要处理一下
            }
        }
        if (skillInterval <= 0)
        {
            if (skillReady.Count > 0)
            {
                int index = Random.Range(0, skillReady.Count);
                Skill useSkill = skillReady[index];
                if (useSkill != null)
                {
                    useSkill.OnTriggerEnd(this, ()=> 
                    {
                        skillInterval = Random.Range(enemyInfo.minSkillAttackCount, enemyInfo.maxSkillAttackCount + 1);
                        skillWait.Add(useSkill);
                        useSkill.SkillCurrentCD = useSkill.SkillCD;
                        skillReady.RemoveAt(index);
                    });
                }
            }
        }
    }
    public override Dictionary<WearEquipType, List<Equip>> GetEquips()
    {
        return null;
    }
    public override GameObject GetObj()
    {
        if (mazeObj == null)
            return null;
        return mazeObj.gameObject;
    }
    public override int GetOriginalDir()
    {
        return Warrior.left;
    }
    public override AnimationMgrInfo GetNormalAttackAnimaMgrInfo()
    {
        if(enemyInfo == null || enemyInfo.NormalAttackInfo == null || enemyInfo.NormalAttackInfo.animationMgrInfo == null)
            return base.GetNormalAttackAnimaMgrInfo();

        return enemyInfo.NormalAttackInfo.animationMgrInfo;
    }
    public override SkillInfo GetNormalAttackSkillInfo()
    {
        if (enemyInfo == null || enemyInfo.NormalAttackInfo == null || enemyInfo.NormalAttackInfo.skillInfo == null)
            return base.GetNormalAttackSkillInfo();

        return enemyInfo.NormalAttackInfo.skillInfo;
    }

    public override float AdjustReward_Property(float targetValue, ref PropertyType targetType)
    {
        targetValue += GetLevel() * targetValue * 0.1f *  Random.Range(0.9f,1.1f);
        if (enemyType == EnemyType.Eltie)
            targetValue *= 1.5f;
        if (enemyType == EnemyType.Goalkeeper)
            targetValue *= 3f;
        return targetValue;
    }
}
