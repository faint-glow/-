using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyType
{
    软体,
}

public enum EndBattleType
{
    Win,
    Run,
    Die,
}
public enum LockTargetType
{
    Enemy,
    Team,
}
public interface IWarriorBehaviourAble
{
    //public void GetApplyTarget();
    //public List<Warrior> GetLockTargets(Warrior user);
    //public void EffectTrigger(TriggerType triggerType);
}

public abstract class Warrior : IPropertyAble, IBootyHolderAble
{
    public Warrior(int level, PropertyValue[] propertyFactors)
    {
        this.property = new Property(this, level, propertyFactors);
        this.battleInfo = new BattleInfo(this, null);

        if (property.IsContainProperty(PropertyType.饱腹度上限))
            this.physiologyMgr = new PhysiologyMgr(this);

        warriorControlMgr = new WarriorControlMgr(this);
    }
    private BodyType bodyType;
    private Bag<Buff> buffBag = new Bag<Buff>();
    private System.Action afterAddBuffAction;
    private System.Action afterRmoveBuffAction;

    private Dictionary<BodyType, List<string>> normalAttackAudioDic = new Dictionary<BodyType, List<string>>();

    public static int right = 1;
    public static int left = -1;

    protected PhysiologyMgr physiologyMgr;
    protected Property property;
    protected BattleInfo battleInfo;
    protected WarriorControlMgr warriorControlMgr;

    private bool isBattleing = false;
    public bool IsBattleing()
    {
        return isBattleing;
    }
    private AnimationMgrInfo normalAttackAnimMgrInfo;
    public AnimationMgrInfo NormalAttackAnimMgrInfo
    {
        get
        {
            if (normalAttackAnimMgrInfo == null)
            {
                normalAttackAnimMgrInfo = ResMgr.GetInstance().Load<AnimationMgrInfo>("SkillInfo/NormalAttack");
            }
            return normalAttackAnimMgrInfo;
        }
    }
    private SkillInfo normalAttackSkillInfo;
    public SkillInfo NormalAttackSkillInfo
    {
        get
        {
            if (normalAttackSkillInfo == null)
                normalAttackSkillInfo = ResMgr.GetInstance().Load<SkillInfo>("SkillInfo/PhysiceNormalAttack");
            return normalAttackSkillInfo;
        }
    }
    private Skill normalAttackSkill;
    public Skill NormalAttackSkill
    {
        get
        {
            if (normalAttackSkill == null)
                normalAttackSkill = new Skill(this, GetNormalAttackAnimaMgrInfo(), GetNormalAttackSkillInfo());
            return normalAttackSkill;
        }
    }
    public abstract GameObject GetObj();
    public abstract Vector3 GetWarriorPos();

    public abstract float GetNormalAttackTriggerTime();

    public abstract MyAnimatorController GetAnimatorController();
    public PhysiologyMgr GetPhysiologyMgr()
    {
        return physiologyMgr;
    }
    public float GetCurrentAnimationSpeed()
    {
        return GetAnimatorController().GetCurrentAnimationSpeed();
    }
    public string GetCurrentAnimation()
    {
        return GetAnimatorController().GetCurrentAnimation();
    }
    //普通攻击需要的动画信息
    public virtual AnimationMgrInfo GetNormalAttackAnimaMgrInfo()
    {
        return NormalAttackAnimMgrInfo;
    }
    //普通攻击需要的技能信息
    public virtual SkillInfo GetNormalAttackSkillInfo()
    {
        return NormalAttackSkillInfo;
    }
    public Skill GetNormalAttackSkill()
    {
        return NormalAttackSkill;
    }
    public Vector3 StartBattleHandler(Warrior warrior)
    {
        this.battleInfo.ClearBattleInfo();

        this.isBattleing = true;
        Vector3 targetPos = StartBattle(warrior);
        this.battleInfo.AddOptionalTarget(this.CheckLockTargetType(warrior), warrior);
        this.NormalAttackAnimMgrInfo.SetFirstEventTriggerTime(GetNormalAttackTriggerTime());

        this.GetNormalAttackSkill().AddNormalAttackEvent();//激活普通攻击
        return targetPos;
    }
    private LockTargetType CheckLockTargetType(Warrior target)
    {
        return LockTargetType.Enemy;//这里可以根据具体需要修改，因为这个游戏只有单挑，所以每个人都是敌人
    }
    private void EndBattleHandler(EndBattleType endBattleType)
    {
        //暂缓一帧再清空,因为要触发伤害造成的特效
        MonoMgr.GetInstance().DelayAction(Time.deltaTime, () =>
        {
            EndBattle(endBattleType);
            isBattleing = false;
            buffBag.RemoveBagItems((buff) =>
            {
                if (buff.GetCategory().Contains(BuffCategory.装备.ToString()))
                    return false;//战斗结束不移除装备类的buff
                if (buff.GetCategory().Contains(BuffCategory.生理状态.ToString()))
                    return false;//战斗结束不移除生理状态类的buff
                if (buff.GetCategory().Contains(BuffCategory.技能.ToString()))
                    return false;
                return true;
            });
            this.CheckAllClamp();
            if (afterRmoveBuffAction != null)
                afterRmoveBuffAction.Invoke();

            this.battleInfo.ForeachAllOptionalTarget(LockTargetType.Enemy, (Warrior enemy) =>
            {
                if (endBattleType == EndBattleType.Die)
                    enemy.battleInfo.AddDieTarget(LockTargetType.Enemy, this);

                enemy.DecreaseWarriorHandler(this);//我的所有对手释放我这个锁定目标
            });
            this.battleInfo.ForeachAllOptionalTarget(LockTargetType.Team, (Warrior team) =>
            {
                team.battleInfo.AddDieTarget(LockTargetType.Team, this);
            });

            if (endBattleType != EndBattleType.Win)
                this.battleInfo.ClearBattleInfo();
        });
    }
    private int DecreaseWarriorHandler(Warrior warrior)
    {
        this.DecreaseWarrior(warrior);
        this.battleInfo.RemoveOptionalTarget(this.CheckLockTargetType(warrior), warrior);
        if (this.battleInfo.GetOptionalTargetCount(LockTargetType.Enemy) <= 0)
            WinHanldler();
        return (this.battleInfo.GetOptionalTargetCount(LockTargetType.Enemy));
    }
    protected void RunAwayHandler()
    {
        RunAway();
        EndBattleHandler(EndBattleType.Run);
    }
    public void WinHanldler()
    {
        Win();
        EndBattleHandler(EndBattleType.Win);
    }
    private void DieHandler()
    {
        Die();
        EndBattleHandler(EndBattleType.Die);
    }
    protected virtual Vector3 StartBattle(Warrior warrior)
    {
        return GetWarriorPos();
    }
    protected virtual void EndBattle(EndBattleType endBattleType)
    {

    }
    protected virtual void RunAway()
    {

    }
    protected virtual void DecreaseWarrior(Warrior warrior)
    {

    }
    protected virtual void Win()
    {

    }
    protected virtual void Die()
    {

    }
    public void SetControl(Control control)
    {
        warriorControlMgr.SetControl(control);//currentControl[control._ControlType] = control;
    }
    public void AddControl(Warrior user, Control control)
    {
        warriorControlMgr.AddControl(user, control);
    }
    public void RemoveControl(Control control)
    {
        warriorControlMgr.RemoveControl(control);
    }

    //原始数值
    public float GetOriginalProperty(PropertyType propertyType)
    {
        return property.GetOriginalProperty(propertyType);
    }
    //原始数值+装备数值
    public float GetBaseProperty(PropertyType propertyType)
    {
        float value = property.GetBaseProperty(propertyType);
        return value;
    }
    //原始数值+装备数值+Buff数值
    //第二个参数是为了防止无限循环的，来自于Buff的效果中的Value必须填写，其他都写null即可
    public float GetCurrentProperty(PropertyType propertyType, IEffectSourceAble effectSource = null)
    {
        float value = property.GetCurrentProperty(propertyType, effectSource);
        return value;
    }
    public float GetCurrentRatioProperty(PropertyType propertyType, IEffectSourceAble effectSource = null)
    {
        float value = 0;
        if (Property.UpLimitMap.ContainsKey(propertyType))
            value = GetCurrentProperty(propertyType, effectSource) / GetCurrentProperty(Property.UpLimitMap[propertyType], effectSource);

        return value;
    }
    public float GetCurrentRatioLossProperty(PropertyType propertyType, IEffectSourceAble effectSource = null)
    {
        float value = 0;
        value = GetCurrentRatioProperty(propertyType, effectSource);
        value = 1 - value;
        return value;
    }
    public float GetCurrentLossProperty(PropertyType propertyType, IEffectSourceAble effectSource = null)
    {
        float value = 0;
        if (Property.UpLimitMap.ContainsKey(propertyType))
            value = GetCurrentProperty(Property.UpLimitMap[propertyType], effectSource) - GetCurrentProperty(propertyType, effectSource);
        return value;
    }
    //接口IPropertyAble中的方法，供给Property类调用
    public float GetExtraProperty_Equip(PropertyType propertyType)//获取装备加持过的对应属性
    {
        return GetAllEquipProperty(GetEquips(), propertyType);
    }
    //接口IPropertyAble中的方法，供给Property类调用
    public float GetExtraProperty_Buff(PropertyType propertyType, IEffectSourceAble effectSource)//获取buff加持后的对应属性
    {
        return GetAllBuffProperty(buffBag.GetCategoryBagItems(TriggerType.存在触发.ToString()), propertyType, effectSource);
    }
    private float GetAllEquipProperty(Dictionary<WearEquipType, List<Equip>> equipDic, PropertyType propertyType)
    {
        float value = 0;
        if (equipDic != null)
        {
            foreach (var item in equipDic)
            {
                List<Equip> equip = item.Value;
                for (int i = 0; i < equip.Count; i++)
                {
                    if (equip[i] == null)
                        continue;
                    value += equip[i].GetCurrentProperty(propertyType);
                }
            }
        }
        return value;
    }
    private float GetAllBuffProperty(List<Buff> buffs, PropertyType propertyType, IEffectSourceAble effectSource)
    {
        float value = 0;
        for (int i = 0; i < buffs.Count; i++)
        {
            Buff buff = buffs[i];
            if (effectSource == buff)
                continue;
            value += buff.GetExistTriggerValue(propertyType);
        }
        return value;
    }

    public BattleInfo GetBattleInfo()
    {
        return battleInfo;
    }
    public void WearEquip()
    {
        //if (SceneMgr.GetInstance().GetCurrentScene().name == SceneName.MainCity)
        //    RecoverState();
        //else
            AfterChangeProperty(PropertyType.无, 0, 0);
    }
    public void TakeOffEquip()
    {
        CheckAllClamp();
    }
    public Bag<Buff> GetBuffBag()
    {
        return buffBag;
    }
    /// <summary>
    /// 注意此方法应当由buff类进行调用，因为有些效果需要buff自己判断应给给谁施加buff
    /// </summary>
    /// <param name="buff"></param>
    public void AddBuff(Warrior user, Buff buff)
    {
        buff.UseBuff(new ApplyInfo(user, this));
        buffBag.AddItem(buff);
        this.GetAnimatorController().CheckCurrentClipSpeed();//有些buff影响动画播放速度，添加时立即检测
        if (this.afterAddBuffAction != null)
            this.afterAddBuffAction.Invoke();

        AfterChangeProperty(PropertyType.无, 0, 0);
    }
    public void RemoveBuff_Category(string category)
    {
        buffBag.RemoveBagItems((buff) =>
        {
            if (buff.GetCategory().Contains(category))
                return true;
            return false;
        });
    }
    public void RemoveBuff(Buff buff)
    {
        buffBag.RemoveItem(buff);
        this.CheckAllClamp();//有些buff增加生命值上限，buff结束以后，要限制生命值
        this.GetAnimatorController().CheckCurrentClipSpeed();//有些buff影响动画播放速度，移除时立即检测
        if (this.afterRmoveBuffAction != null)
            this.afterRmoveBuffAction.Invoke();

        AfterChangeProperty(PropertyType.无, 0, 0);
    }
    public void AddAfterAddBuffAction(System.Action action)
    {
        if (this.afterAddBuffAction == null)
            this.afterAddBuffAction = action;
        else
            this.afterAddBuffAction += action;
    }
    public void AddAfterRemoveBuffAction(System.Action action)
    {
        if (this.afterRmoveBuffAction == null)
            this.afterRmoveBuffAction = action;
        else
            this.afterRmoveBuffAction += action;
    }
    public void RemoveAfterAddBuffAction(System.Action action)
    {
        if (this.afterAddBuffAction == null)
            return;
        this.afterAddBuffAction -= action;
    }
    public void RemoveAfterRemoveBuffAction(System.Action action)
    {
        if (this.afterRmoveBuffAction == null)
            return;
        this.afterRmoveBuffAction -= action;
    }
    public abstract Dictionary<WearEquipType, List<Equip>> GetEquips();
    public static Warrior GetWarrior(ApplyInfo applyInfo, ApplyTargetType objectType)
    {
        if (applyInfo == null)
            return null;

        Warrior warrior;
        if (objectType == ApplyTargetType.User)
            warrior = applyInfo.user;
        else
            warrior = applyInfo.target;

        return warrior;
    }
    public bool IsCanSelect(LockTargetInfoMgr lockTargetInfoMgr)
    {
        return this.battleInfo.IsCanSelect(lockTargetInfoMgr);
    }
    public List<Warrior> GetFixedOptionalTargetByInfo(LockTargetInfoMgr lockTargetInfoMgr, LockTargetType lockTargetType)
    {
        return battleInfo.GetFixedOptionalTargetByInfo(lockTargetInfoMgr, lockTargetType);
    }
    public List<Warrior> GetAllRandomTargetByInfo(LockTargetInfoMgr lockTargetInfoMgr)
    {
        return battleInfo.GetAllRandomTargetByInfo(lockTargetInfoMgr);
    }
    public List<Warrior> GetRandomTargetByInfo(LockTargetInfoMgr lockTargetInfoMgr, LockTargetType lockTargetType)
    {
        return battleInfo.GetRandomTargetByInfo(lockTargetInfoMgr, lockTargetType);
    }
    public Warrior GetRandomEnemyTarget()
    {
        Warrior target = this.battleInfo.LockTarget;
        if (target == null && this.battleInfo.GetDieTargetCount(LockTargetType.Enemy) > 0)
            target = this.battleInfo.GetLastDieTarget(LockTargetType.Enemy); //BeatEnemyWarrior[battleInfo.BeatEnemyWarrior.Count - 1];//攻击已经打败的敌人，用于触发特效

        if (target == null)
        {
            Debug.Log("无锁定目标");
            return null;
        }
        return target;
    }

    public void StartWarriorBehaviour()
    {
        this.battleInfo.StartBehaviour();
    }
    /// <summary>
    /// 技能受伤
    /// </summary>
    /// <param name="user">攻击者</param>
    /// <param name="skill">技能</param>
    /// <returns></returns>
    public float Injured_Skill(Warrior user, Effect effect)
    {
        if (user == null || effect == null)
            return 0;

        user.battleInfo.ClearDamageValue();//每次效果清除上一次使用者的技能效果
        this.battleInfo.ClearInjuredValue();

        bool isCrit = false;
        Dictionary<PropertyType, float> damageValueDic = effect.GetDamageValue(new ApplyInfo(user, this));
        Dictionary<PropertyType, float> adjustDamageValueDic = new Dictionary<PropertyType, float>(); //effect.GetDamageValue(new ApplyInfo(user, this));
        if (damageValueDic == null)
            return 0;
        foreach (var item in damageValueDic)//foreach中无法修改值和移除key
        {
            PropertyType damageType = item.Key;
            float value = item.Value;
            value = GetRealValue(user, damageType, value);
            isCrit = CheckCrit(user, damageType);//是否暴击
            if (isCrit == true)
            {
                value = value * Mathf.Clamp(user.GetCurrentProperty(PropertyType.暴击效果), 1.5f, float.MaxValue);//暴击效果至少都是大于等于1.5倍的,防止外界设置的时候有误
            }
            float injuredFreeValue = value * Mathf.Clamp(this.GetCurrentProperty(PropertyType.免伤率), 0, 1);
            value -= injuredFreeValue;//注意value一定是负值，injuredFreeValue也是负值

            adjustDamageValueDic[damageType] = value;
        }
        user.battleInfo.AddHitTarget(user.CheckLockTargetType(this), this);
        user.battleInfo.DamageValueDic = adjustDamageValueDic;
        this.battleInfo.InjuredValueDic = adjustDamageValueDic;
        Skill skill = effect.GetLastEffectSource() as Skill;
        if (skill != null && user.battleInfo.GetAllHitTargetCount() > 0)
        {
            if (skill._skillType == SkillType.技能)
            {
                skill.EffectTrigger(TriggerType.技能造成伤害前触发, this);//触发技能本身的命中效果
                user.EffectTrigger(TriggerType.技能造成伤害前触发, this);//使用者身上可能拥有技能命中触发效果的buff所以也要触发
                this.EffectTrigger(TriggerType.受到技能伤害前触发, this);
            }
            else if (skill._skillType == SkillType.普攻)
            {
                skill.EffectTrigger(TriggerType.普攻造成伤害前触发, this);//触发技能本身的命中效果
                user.EffectTrigger(TriggerType.普攻造成伤害前触发, this);//触发使用者的普攻命中触发效果
                this.EffectTrigger(TriggerType.受到普攻伤害前触发, this);
            }
        }
        damageValueDic = new Dictionary<PropertyType, float>(this.battleInfo.InjuredValueDic);

        foreach (var item in damageValueDic)
        {
            ChangeProperty_Auto_Anim(PropertyType.生命值, item.Value, item.Key, isCrit);
        }

        if (skill != null && user.battleInfo.GetAllHitTargetCount() > 0)
        {
            if (skill._skillType == SkillType.技能)
            {
                skill.EffectTrigger(TriggerType.技能造成伤害后触发, this);//触发技能本身的命中效果
                user.EffectTrigger(TriggerType.技能造成伤害后触发, this);//使用者身上可能拥有技能命中触发效果的buff所以也要触发
                this.EffectTrigger(TriggerType.受到技能伤害后触发, this);
            }
            else if (skill._skillType == SkillType.普攻)
            {
                skill.EffectTrigger(TriggerType.普攻造成伤害后触发, this);
                user.EffectTrigger(TriggerType.普攻造成伤害后触发, this);//触发使用者的普攻命中触发效果
                this.EffectTrigger(TriggerType.受到普攻伤害后触发, this);

                user.AfterNormalAttack();
            }
        }
        return 0;
    }


    //检查命中
    public bool CheckHit(Warrior user, Effect effect)
    {
        float baseHitRatio = effect.triggerRatio;
        float differenceRatio = user.GetCurrentProperty(PropertyType.命中率) - this.GetCurrentProperty(PropertyType.闪避率);

        return Random.Range(0, 1.0f) < (baseHitRatio + differenceRatio);
    }
    //检查暴击
    private bool CheckCrit(Warrior user, PropertyType attackType)
    {
        if (attackType == PropertyType.攻击力)
        {
            float critRatio = user.GetCurrentProperty(PropertyType.暴击率, null);
            float resistCritRatio = this.GetCurrentProperty(PropertyType.抗爆率, null);
            return Random.Range(0, 1.0f) < (critRatio - resistCritRatio);
        }
        return false;
    }
    private float GetRealValue(Warrior user, PropertyType attackType, float value)
    {
        PropertyType defenceType = Property.AttackDefenceMap(attackType);
        PropertyType penetrateType = Property.AttackPenetrateMap(attackType);
        if (defenceType != PropertyType.无 && penetrateType != PropertyType.无)
        {
            float penetrateRatio = user.GetCurrentProperty(penetrateType, null);//攻击者的穿透值比率
            float defenceValue = this.GetCurrentProperty(defenceType, null);//受击者的防御力
            penetrateRatio = Mathf.Clamp(penetrateRatio, 0, 1);//穿透率必须在0到1之间
            value = value + defenceValue * (1 - penetrateRatio);//受伤方法：传入此处的value必须且一定是负值，property的值一定是正值
        }

        value = Mathf.Clamp(value, float.MinValue, -1);
        return value;
    }

    /// <summary>
    /// 触发buff
    /// </summary>
    /// <param name="triggerType">触发什么类型的Buff</param>
    /// <param name="target">触发buff的单位</param>
    public void EffectTrigger(TriggerType triggerType, Warrior target)
    {
        List<Buff> buffs = this.buffBag.GetBagItems();
        for (int i = 0; i < buffs.Count; i++)
        {
            List<Effect> effects = buffs[i].GetEffects(triggerType);
            for (int j = 0; j < effects.Count; j++)
            {
                effects[j].EffectTrigger(new ApplyInfo(this, target));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="propertyType">目标修改的属性</param>
    /// <param name="value">目标修改值</param>
    /// <param name="damageType">如果是修改生命值，那么伤害类型是魔法还是物理</param>
    /// <param name="isCrit">是否产生暴击效果</param>
    /// <returns>返回溢出值</returns>
    public float ChangeProperty_Auto_Anim(PropertyType propertyType, float value, PropertyType damageType = PropertyType.攻击力, bool isCrit = false)
    {
        float overflow = 0;
        if (propertyType == PropertyType.生命值)
        {
            if (IsDie())
                return overflow;
            if (value >= 0)//回血
                overflow = ChangeProperty_Anim_Two(PropertyType.生命值, value, Color.green);
            else//扣血
            {
                int textSize = 40;
                Color color = Color.white;
                if (isCrit == true)
                {
                    textSize = 55;
                    color = new Color32(255, 140, 0, 255);//橙色
                }
                else
                {
                    if (damageType == PropertyType.攻击力)//白色
                        color = Color.white;
                    else if (damageType == PropertyType.魔力)//紫色
                        color = new Color32(255, 0, 255, 255);
                }
                overflow = ChangeProperty_Anim_One(PropertyType.生命值, value, color, -GetDir(), textSize);
                if (property.IsDie())
                    DieHandler();
            }
            return overflow;
        }
        if (propertyType == PropertyType.魔法值)
        {
            if (value >= 0)//回蓝
                overflow = ChangeProperty_Anim_Two(PropertyType.魔法值, value, Color.blue);
            else//扣蓝
                overflow = ChangeProperty_Anim_Two(PropertyType.魔法值, value, new Color(128, 0, 128));

            return overflow;
        }

        overflow = ChangerProperty(propertyType, value);
        return overflow;
    }
    private float ChangeProperty_Anim_One(PropertyType propertyType, float value, Color color, int dir = 1, int textSize = 40)
    {
        ChangerProperty(propertyType, value);
        BattleValuePanelMgr.GetInstance().ShowBattleValue_Damage(this, new Vector3(0, 2, 0), value, color, dir, textSize);
        return 0;
    }

    private float ChangeProperty_Anim_Two(PropertyType propertyType, float value, Color color)
    {
        ChangerProperty(propertyType, value);
        BattleValuePanelMgr.GetInstance().ShowBattleValue_AddState(this, new Vector3(0, 2, 0), value, color);
        return value;
    }
    public void RecoverState_Handler()
    {
        RecoverState();
        AfterChangeProperty(PropertyType.无, 0, 0);
    }
    public void RecoverState()
    {
        property.RecoverState();
    }
    public void CheckAllClamp()
    {
        property.CheckAllClamp();
        AfterChangeProperty(PropertyType.无, 0, 0);
    }
    /// <summary>
    /// 修改属性值，返回溢出值，返回的溢出值仅对有上限值的属性有效
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public float ChangerProperty(PropertyType propertyType, float value)
    {
        float overflow = property.ChangerProperty(propertyType, value);
        AfterChangeProperty(propertyType, value, overflow);
        return overflow;
    }

    public void SetProperty(PropertyType propertyType, float value)
    {
        property.SetProperty(propertyType, value);
        AfterChangeProperty(propertyType, value, 0);
    }
    public PropertyType CheckPropertyEnough(PropertyType propertyType, float value)
    {
        return property.CheckPropertyEnough(propertyType, value);
    }
    public virtual void AfterChangeProperty(PropertyType propertyType, float value, float overflow)
    {

    }
    //传入目标位置，自动确定vector1朝向vector2方向
    public static int GetDir(Vector3 vector1, Vector3 vector2)
    {
        int dir;
        if (vector1.x - vector2.x > 0.1f)
            dir = left;
        else if (vector1.x - vector2.x < -0.1f)
            dir = right;
        else
            dir = 0;
        return dir;
    }
    //传入目标位置，自动确定玩家方向
    public static void SetDir(Warrior warrior, Vector3 originalScale, int dir)
    {
        Transform trans = warrior.GetAnimatorController().GetAnimator().gameObject.transform;
        Vector3 newScale = originalScale;
        newScale.x *= dir * warrior.GetOriginalDir();//如果原来物体的是朝向左边的，需要处理一下，让世界坐标的左边为左边
        trans.localScale = newScale;
    }
    public abstract int GetOriginalDir();
    //-1为左，1为右
    public abstract int GetDir();

    public abstract Booty GetBooty();

    public abstract string GetName();

    public int GetLevel()
    {
        return property.GetLevel();
    }

    public bool IsDie()
    {
        return property.IsDie();
    }

    /// <summary>
    /// 升1级
    /// </summary>
    /// <param name="overflowExperience">溢出经验值</param>
    public void LevelUpOne(float overflowExperience, bool isFull = false)
    {
        LevelUp(property.GetLevel() + 1, overflowExperience, isFull);
        AfterChangeProperty(PropertyType.无, 0, 0);
    }
    /// <summary>
    /// 升级
    /// </summary>
    /// <param name="level">目标等级</param>
    /// <param name="overflowExperience">溢出经验值</param>
    public virtual void LevelUp(int level, float overflowExperience, bool isFull = false)
    {
        property.LevelUp(level, overflowExperience, isFull);
        AfterChangeProperty(PropertyType.无, 0, 0);
    }
    public bool IsContainProperty(PropertyType propertyType)
    {
        return property.IsContainProperty(propertyType);
    }
    public static Warrior GetApplyTarget(ApplyInfo applyInfo, ApplyTargetType targetType)
    {
        if (applyInfo == null)
            return null;
        Warrior warrior = applyInfo.user;
        if (targetType == ApplyTargetType.User)
            warrior = applyInfo.user;
        else
            warrior = applyInfo.target;
        return warrior;
    }
    public static string GetApplyTargetTypeInfo(ApplyTargetType objType)
    {
        string content = "";
        if (objType == ApplyTargetType.User)
            content = "自身";
        else
            content = "目标";
        return content;
    }

    public Vector3 GetPosition()
    {
        return GetWarriorPos();
    }
    public virtual void AfterNormalAttack()
    {

    }

    public virtual float AdjustReward_Property(float targetValue, ref PropertyType targetType)
    {
        return targetValue;
    }

}
