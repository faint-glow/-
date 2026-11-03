using System.Collections.Generic;
using UnityEngine;

public enum TriggerType
{
    存在触发,//用于默认的生命值/攻击力加成等,随Buff存在而存在
    间隙触发,
    升级触发,
    受击触发,
    释放技能对所有目标触发,
    普攻命中敌人触发,
    技能命中敌人触发,
    使用道具触发,
    战斗结束触发,
    死亡触发,
    动画触发,
    普攻命中队友触发,
    技能命中队友触发,
    使用道具命中敌人触发,
    使用道具命中队友触发,

    修改属性触发,
    施加buff触发,
    普攻造成伤害后触发,
    技能造成伤害后触发,
    普攻造成伤害前触发,
    技能造成伤害前触发,
    释放技能时触发,

    受到普攻伤害前触发,
    受到技能伤害前触发,

    受到普攻伤害后触发,
    受到技能伤害后触发,
}

public enum EffectType
{
    //顺序千万千万千万不能改，想要增加新的，就在最下面直接加就行，否则会修改已经配置好的ScriptableObject
    修改属性 = 0,
    施加buff = 1,
    造成伤害 = 2,
    触发特效 = 3,//辅助类效果，主要是音乐和特效
    控制效果 = 4,
    调整造成伤害 = 5,
    调整受到伤害 = 6,
    清除当前Buff = 7,
}
[System.Serializable]
public class TextInfoElement
{
    [TextArea(2, 5)]
    public string content;
}
[System.Serializable]
public class TextInfo : SelfAdaptationClassMgr
{
    public bool isUse = true;
    public bool isOverride;
    public TextInfoElement textInfo;
    public TextInfoElement overrideTextInfo;

    public string GetTextInfo()
    {
        if (isUse == false)
            return "";
        if (isOverride == true)
            return overrideTextInfo.content;
        else
            return textInfo.content;
    }
}

//ApplyInfo的对应位置为（施加者，施加目标）
//Effect只对效果的持有者产生效果
//也就是只对"施加目标"产生效果，不对"施加者"产生效果
//如果要对"施加者"产生效果，那么在触发条件时就应当设置"施加目标"为"施加者"

//当Effect存在于buff中时，user为buff的施加者，
//如果buff的ObjectType为user，那就是对施加者本身施加buff，那么此时的ApplyInfo为(user,user)
//如果buff的ObjectType为target,target为user的击中目标,如果没用击中目标，则buff施加无效,此时的applyInfo就是(user,target)

//当Effect存在于技能中时
//如果是技能释放触发，则user为施加者，target也为施加者。此时的applyInfo为(user,user)
//如果是技能命中触发，则user为施加者，target为击中的目标。此时的applyInfo为(user,target)

//当Effect存在于药剂中时，

//使用Effect必须要SetApplyInfo
[System.Serializable]
public class Effect : SelfAdaptationClassMgr
{
    public TextInfo effectInfo;
    public EffectCondition effectCondition;
    public List<TriggerType> triggerTypes = new List<TriggerType>();//什么情况下会触发该效果
    public float intervalTime = -1;//如果intervalTime小于等于0则，不会调用TriggerType.间隙触发
    public float triggerRatio;
    public ApplyTargetType applyTargetType;//用于判断该效果对施加者还是被施加者生效
    public EffectType effectType;
    public PropertyType targetProperty;
    public Value value;
    public List<ControlInfo> controlInfos = new List<ControlInfo>();
    public List<BuffInfo> buffInfos = new List<BuffInfo>();//用于外部添加
    public EffectFXMgr fxMgr;
    public EffectAudioMgr audioMgr;
    [System.NonSerialized] private List<IEffectSourceAble> effectSources = new List<IEffectSourceAble>();
    private int count = 1;
    public static void AddEffect(Dictionary<TriggerType, List<Effect>> effectDic, Effect effect)
    {
        for (int i = 0; i < effect.triggerTypes.Count; i++)
        {
            TriggerType type = effect.triggerTypes[i];
            if (!effectDic.ContainsKey(type))
                effectDic[type] = new List<Effect>();

            effectDic[type].Add(effect);
        }
    }
    public static string GetEffectDicInfoText(List<Effect> effects, string targetType = "")
    {
        string content = "";
        string lastEffectCount = "";
        for (int i = 0; i < effects.Count; i++)
        {
            if (lastEffectCount != "")
                content += "\n";
            string currentContent_Effect = targetType + effects[i].GetEffectInfoText();
            content += currentContent_Effect;
            lastEffectCount = currentContent_Effect;
        }
        return content;
    }

    public static string GetEffectDicInfoText(Dictionary<TriggerType, List<Effect>> effectDic, string targetType = "")
    {
        string content = "";
        string lastContent = "";
        //string lastEffectCount = "";
        foreach (var item in effectDic)
        {
            if (lastContent != "")
                content += "\n";
            List<Effect> effects = item.Value;
            lastContent = Effect.GetEffectDicInfoText(effects, targetType);
            content += lastContent;

        }
        return content;
    }
    public IEffectSourceAble GetLastEffectSource()
    {
        if (effectSources == null || effectSources.Count <= 0)
            return null;
        return effectSources[effectSources.Count - 1];
    }

    public string GetEffectInfoText()
    {
        return effectInfo.GetTextInfo();
    }

    public void SetEffectSource(List<IEffectSourceAble> effectSources)
    {
        this.effectSources = new List<IEffectSourceAble>(effectSources);
    }
    public void CountdownStart()
    {
        count = 1;
        IntervalStart();
    }
    public void Countdown(float currentTime, ApplyInfo applyInfo)
    {
        if (intervalTime <= 0)
            return;
        if (currentTime >= intervalTime * count)
        {
            count++;
            Interval(applyInfo);
        }
    }
    public void CountdownEnd()
    {
        IntervalEnd();
    }
    public void SetIntervalTime(float intervalTime)
    {
        this.intervalTime = intervalTime;
    }
    //这是由Buff触发的，目前只有 间隙触发 才可以被触发
    protected virtual void IntervalStart()
    {

    }
    //这是由Buff触发的，目前只有 间隙触发 才可以被触发
    protected virtual void Interval(ApplyInfo applyInfo)
    {
        if (triggerTypes.Contains(TriggerType.间隙触发))
        {
            EffectTrigger(applyInfo);
        }
    }
    //这是由Buff触发的，目前只有 间隙触发 才可以被触发
    protected virtual void IntervalEnd()
    {

    }
    //applyInfo.user对ApplyInfo的target触发效果
    public virtual void EffectTrigger(ApplyInfo applyInfo)
    {
        if (effectCondition.Check(this, applyInfo) == false)
            return;

        if (effectType == EffectType.施加buff && CheckTrigger() == true)
            AddBuff(applyInfo);
        else if (effectType == EffectType.修改属性 && CheckTrigger() == true)
            ChangeProperty(applyInfo);
        else if (effectType == EffectType.造成伤害 && CheckHit(applyInfo) == true)
            Damage(applyInfo);
        else if (effectType == EffectType.控制效果 && CheckTrigger() == true)
            AddControl(applyInfo);
        else if (effectType == EffectType.调整受到伤害 && CheckTrigger() == true)
            AdjustInjuredValue(applyInfo);
        else if (effectType == EffectType.清除当前Buff && CheckTrigger() == true)
            RemoveThisBuff();
        TriggerAideEffect(applyInfo);
    }
    private bool CheckTrigger()
    {
        return Random.Range(0, 1.0f) < triggerRatio;
    }
    private bool CheckHit(ApplyInfo applyInfo)
    {
        if (applyInfo == null || applyInfo.target == null || applyInfo.user == null)
            return false;
        return applyInfo.target.CheckHit(applyInfo.user, this);
    }
    //存在触发时被调用的方法
    public float GetValue(ApplyInfo applyInfo)
    {
        return value.GetValue(applyInfo, GetLastEffectSource());
    }
    private void AddControl(ApplyInfo applyInfo)
    {
        Warrior target = Warrior.GetApplyTarget(applyInfo, applyTargetType);
        if (target == null)
            return;
        for (int i = 0; i < controlInfos.Count; i++)
        {
            if (controlInfos[i] == null)
                continue;
            Control control = controlInfos[i].GetControl();
            target.AddControl(applyInfo.user,control);
        }
    }
    //使用者对目标施加buff
    private void AddBuff(ApplyInfo applyInfo)
    {
        Warrior target = Warrior.GetApplyTarget(applyInfo, applyTargetType);
        if (target == null)
            return;
        for (int i = 0; i < buffInfos.Count; i++)
        {
            if (buffInfos[i] == null)
                continue;
            Buff buff = new Buff(buffInfos[i], this.effectSources);
            target.AddBuff(applyInfo.user, buff);
        }
    }
    //使用者对目标修改属性
    private void ChangeProperty(ApplyInfo applyInfo)
    {
        Warrior target = Warrior.GetApplyTarget(applyInfo, applyTargetType);
        if (target == null)
            return;
        float value = this.value.GetValue(applyInfo, GetLastEffectSource());
        target.ChangeProperty_Auto_Anim(targetProperty, value);
    }
    private void AdjustInjuredValue(ApplyInfo applyInfo)
    {
        float value = this.value.GetValue(applyInfo, GetLastEffectSource());
        if(applyInfo.target.GetBattleInfo().InjuredValueDic.ContainsKey(targetProperty))
        {
            applyInfo.user.GetBattleInfo().InjuredValueDic[targetProperty] -= value;
            Buff holderBuff = GetLastEffectSource() as Buff;
            if (holderBuff != null)
                holderBuff.AddAdjustInjuredValue(targetProperty, value);
        }
    }
    private void RemoveThisBuff()
    {
        Buff holderBuff = GetLastEffectSource() as Buff;
        if (holderBuff != null)
            holderBuff.RemoveThisBuff();
    }
    //施加对象受伤，传入攻击者和攻击效果
    public void Damage(ApplyInfo applyInfo)
    {
        Warrior target = Warrior.GetApplyTarget(applyInfo, applyTargetType);
        if (target == null)
            return;

        target.Injured_Skill(applyInfo.user, this);
    }
    public Dictionary<PropertyType, float> GetDamageValue(ApplyInfo applyInfo)
    {
        Dictionary<PropertyType, float> damageValueDic = new Dictionary<PropertyType, float>();
        if (GetLastEffectSource() == null)
            return damageValueDic;
        if (!damageValueDic.ContainsKey(targetProperty))
            damageValueDic[targetProperty] = 0;

        damageValueDic[targetProperty] += value.GetValue(applyInfo, GetLastEffectSource());
        return damageValueDic;
    }
    /// <summary>
    /// 触发辅助效果，包括音效，特效等
    /// </summary>
    public void TriggerAideEffect(ApplyInfo applyInfo)
    {
        TriggerFX(applyInfo);
        PlayClip();
    }
    public void TriggerFX(ApplyInfo applyInfo)
    {
        if (fxMgr == null)
            return;
        fxMgr.UseFX(applyInfo);
    }
    public void PlayClip()
    {
        audioMgr.Play();
    }
}

