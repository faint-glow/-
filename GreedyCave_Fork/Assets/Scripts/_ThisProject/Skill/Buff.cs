using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffCategory
{
    无,
    增益,
    负面,
    装备,
    生理状态,
    技能,
}
public enum OverlayType
{
    Refresh,
    Overlay,
    None,
}
public interface IEffectSourceAble
{
    public string GetName();
}
public enum AddBuffType
{
    Refresh,//刷新
    Superpose,//叠加
}
public class ApplyInfo
{
    public ApplyInfo(Warrior user, Warrior target)
    {
        this.user = user;
        this.target = target;
    }
    public Warrior user;//使用者
    public Warrior target;//作用目标
}

[System.Serializable]
public class Buff : IEffectSourceAble, ICollectAble<Buff>,IGetAble
{
    public Buff(BuffInfo buffInfo,List<IEffectSourceAble> effectSource)
    {
        if (buffInfo == null)
        {
            Debug.Log("buffInfo为空");
            return;
        }
        this.buffInfo = buffInfo;
        this.buffName = buffInfo.buffName;
        this.liveTime = buffInfo.liveTime;
        this.existType = buffInfo.existType;
        this.buffCategoryList = buffInfo.buffCategoryList;
        this.overlayType = buffInfo.overlayType;
        this.count = 1;

        allEffect = buffInfo.effects;
        for (int i = 0; i < buffInfo.effects.Count; i++)
        {
            Effect effect = buffInfo.effects[i];
            AddEffect(effectDic, effect);

            if (effect.triggerTypes.Contains(TriggerType.存在触发))
            {
                PropertyType targetType = effect.targetProperty;
                if (!effectExistDic.ContainsKey(targetType))
                    effectExistDic[targetType] = new List<Effect>();
                effectExistDic[targetType].Add(effect);
            }
        }
        fxMgr = buffInfo.fxMgr;

        SetEffectSource(effectSource);
    }
    private BuffInfo buffInfo;
    private string buffName;
    public float liveTime;
    public ExistType existType;

    public int overlayType;//刷新机制，是否可以叠加
    //public ApplyTargetType applyTargetType;//用于判断该buff施加给某个行为的触发者还是被触发者

    //private bool isRefreshTime;
    private ApplyInfo applyInfo;//施加信息，持有该buff的一定是applyInfo.target
    private Dictionary<PropertyType, float> adjustInjuredAllValue = new Dictionary<PropertyType, float>();
    public CountdownTimerInfo countdownTimerInfo = new CountdownTimerInfo();

    private int count = 1;
    public List<BuffCategory> buffCategoryList;
    private HashSet<string> buffCategoryHashSet;
    //全部效果
    private List<Effect> allEffect;
    //一个效果可能由多个触发方式，每个触发方式都有这个效果的索引
    private Dictionary<TriggerType, List<Effect>> effectDic = new Dictionary<TriggerType, List<Effect>>();
    //把所有存在触发的效果添加到另一个字典中，方便访问
    private Dictionary<PropertyType, List<Effect>> effectExistDic = new Dictionary<PropertyType, List<Effect>>();
    //private List<FXInfo> fxInfoList = new List<FXInfo>();
    private EffectFXMgr fxMgr;
    private List<IEffectSourceAble> effectSources;

    private static List<string> paths = new List<string>()
    {
        "Buff",
    };
    private static Dictionary<string, BuffInfo> allBuffInfoDic;
    public static Dictionary<string, BuffInfo> AllBuffInfoDic
    {
        get
        {
            if (allBuffInfoDic == null)
                allBuffInfoDic = ToolsMgr.LoadAllResourceInfo<BuffInfo>(paths);
            return allBuffInfoDic;
        }
    }
    public float GetExistTriggerValue(PropertyType propertyType)
    {
        float value = 0;
        if (GetEffectExistDic().ContainsKey(propertyType))
        {
            List<Effect> effects = GetEffectExistDic()[propertyType];
            for (int j = 0; j < effects.Count; j++)
                value += effects[j].GetValue(this.applyInfo);
        }
        return value;
    }
    public string GetBuffInfoText()
    {
        return buffInfo.GetBuffInfoText();
    }
    public bool IsContainExistTrigger()
    {
        return effectDic.ContainsKey(TriggerType.存在触发);
    }
    public Dictionary<PropertyType, List<Effect>> GetEffectExistDic()
    {
        return effectExistDic;
    }
    //buff的原本名字+buff的来源物品的名字，仅仅供Warrior添加buff的地方使用
    public string GetBuffName_Extra()
    {
        if (effectSources == null || effectSources.Count <= 0)
        {
            Debug.Log("sourceAble为空");
            return buffName;
        }
        string targetName = buffName;
        foreach (var item in effectSources)
        {
            if(item != null)
                targetName += "_" + item.GetName();
        }
        return targetName;
    }

    private void SetEffectSource(List<IEffectSourceAble> effectSource)
    {
        if (effectSource != null)
            this.effectSources = new List<IEffectSourceAble>(effectSource);
        else
            this.effectSources = new List<IEffectSourceAble>();
        if (GetLastEffectSource().GetName() != this.GetName())//避免Effect和Buff一直循环把这个IEffectSourceAble传递下去
            this.effectSources.Add(this);
        else//并且把最后一个替换为自己
            SetLastEffectSource(this);
    }

    public void UseBuff(ApplyInfo applyInfo)
    {
        this.applyInfo = applyInfo;
        this.count = 1;
        this.adjustInjuredAllValue.Clear();
        if (countdownTimerInfo != null)
            this.countdownTimerInfo.Reset();
    }

    private IEffectSourceAble GetLastEffectSource()
    {
        if (effectSources == null || effectSources.Count <= 0)
            return null;
        return effectSources[effectSources.Count - 1];
    }
    private void SetLastEffectSource(IEffectSourceAble effectSourceAble)
    {
        if (effectSources == null || effectSources.Count <= 0)
            return;
        effectSources[effectSources.Count - 1] = effectSourceAble;
    }
    public void UseFX()
    {
        fxMgr.UseFX(this.applyInfo);//buff这种持续存在的，使用特效，使用者和使用对象都是自己
    }
    //如果是不会因时间而消失的Buff，那么就只会调用触发CountdownStart();
    public void StartCountdown()
    {
        if (existType == ExistType.Time)
            MonoMgr.GetInstance().CountdownTimer(liveTime, CountdownStart, Countdown, CountdownEnd);
        else if(existType == ExistType.Exist_WithHolder && effectDic.ContainsKey(TriggerType.间隙触发))
            MonoMgr.GetInstance().CountdownTimer(float.MaxValue, CountdownStart, Countdown, CountdownEnd);
    }
    public void AddAdjustInjuredValue(PropertyType propertyType,float value)
    {
        BattleInfo.AddDicValue(adjustInjuredAllValue,propertyType, value);
        Debug.Log(adjustInjuredAllValue[propertyType]);
    }
    public float GetAdjustInjuredValue(PropertyType propertyType)
    {
        return BattleInfo.GetDicValue(adjustInjuredAllValue, propertyType);
    }
    public float GetAllAdjustInjuredValue()
    {
        float value = 0;
        foreach (var item in adjustInjuredAllValue)
            value += item.Value;
        return value;
    }

    public void RefreshBuff()
    {
        //isRefreshTime = true;
        countdownTimerInfo.CurrentTime = 0;
        adjustInjuredAllValue.Clear();
        CountdownStart(countdownTimerInfo);
    }

    public void ReleaseCountdown()
    {
        if (countdownTimerInfo != null)
        {
            countdownTimerInfo.isBreak = true;
            countdownTimerInfo.isInfluenceEndAction = true;
        }
    }
    public void AddEffect(Dictionary<TriggerType, List<Effect>> effectDic, Effect effect)
    {
        for (int i = 0; i < effect.triggerTypes.Count; i++)
        {
            TriggerType type = effect.triggerTypes[i];
            if (!effectDic.ContainsKey(type))
                effectDic[type] = new List<Effect>();
            effectDic[type].Add(effect);
        }
    }
    public List<Effect> GetAllEffects()
    {
        return allEffect;
    }
    public List<Effect> GetEffects(TriggerType type)
    {
        if (!effectDic.ContainsKey(type))
            effectDic[type] = new List<Effect>();

        return effectDic[type];
    }
    protected virtual void CountdownStart(CountdownTimerInfo countdownTimerInfo)
    {
        CountDownStartEffect(TriggerType.间隙触发);
    }
    private void CountDownStartEffect(TriggerType triggerType)
    {
        if (effectDic.ContainsKey(triggerType))
        {
            List<Effect> effect = effectDic[triggerType];

            for (int i = 0; i < effect.Count; i++)
            {
                effect[i].CountdownStart();
            }
        }
    }
    protected virtual void Countdown(CountdownTimerInfo countdownTimerInfo)
    {
        this.countdownTimerInfo = countdownTimerInfo;
        this.CountdownEffect(countdownTimerInfo, TriggerType.间隙触发);
    }

    private void CountdownEffect(CountdownTimerInfo countdownTimerInfo, TriggerType triggerType)
    {
        if (effectDic.ContainsKey(triggerType))
        {
            List<Effect> effect = effectDic[triggerType];
            for (int i = 0; i < effect.Count; i++)
            {
                effect[i].Countdown(countdownTimerInfo.CurrentTime,this.applyInfo);
            }
        }
        return;
    }
    protected virtual void CountdownEnd(CountdownTimerInfo countdownTimerInfo)
    {
        CountdownEndEffect(TriggerType.间隙触发);
        RemoveThisBuff();
    }
    public void RemoveThisBuff()
    {
        applyInfo.target.RemoveBuff(this);
    }
    private void CountdownEndEffect(TriggerType triggerType)
    {
        if (effectDic.ContainsKey(triggerType))
        {
            List<Effect> effect = effectDic[triggerType];

            for (int i = 0; i < effect.Count; i++)
            {
                effect[i].CountdownEnd();
            }
        }
    }

    private HashSet<string> GetBuffCategoryHashSet()
    {
        if(buffCategoryHashSet == null || buffCategoryHashSet.Count <= 0)
        {
            buffCategoryHashSet = new HashSet<string>();
            for (int i = 0; i < buffCategoryList.Count; i++)
            {
                if (!buffCategoryHashSet.Contains(buffCategoryList[i].ToString()))
                    buffCategoryHashSet.Add(buffCategoryList[i].ToString());
            }
        }
        if(effectDic.ContainsKey(TriggerType.存在触发) && effectDic[TriggerType.存在触发].Count > 0)//如果外界忘记预设存在触发，这里补充
        {
            if (!buffCategoryHashSet.Contains(TriggerType.存在触发.ToString()))
                buffCategoryHashSet.Add(TriggerType.存在触发.ToString());
        }
        if(buffCategoryHashSet.Count <= 0)
            buffCategoryHashSet.Add(BuffCategory.无.ToString());//放入背包的物品，必须要有至少一个分类

        return buffCategoryHashSet;
    }

    public void UseGrid()
    {
        foreach (var item in effectDic)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                item.Value[i].SetEffectSource(this.effectSources);
            }
        }
        this.StartCountdown();
        this.UseFX();
    }

    public HashSet<string> GetCategory()
    {
        return GetBuffCategoryHashSet();
    }
    public List<Buff> SplitRules(int countUpperLimit)
    {
        return new List<Buff>() { this};
    }
    public void ReverseSpltRules(Buff item)
    {
        
    }
    public string GetName()
    {
        return buffName;
    }
    public bool IsCanCombine()
    {
        return true;
    }
    public bool IsSame(Buff item)
    {
        return item.GetBuffName_Extra() == GetBuffName_Extra();
    }
    public Buff StackRules(Buff item, int countUpperLimit)
    {
        RefreshBuff();
        return null;
    }
    public Buff DecreaseRules(int count)
    {
        this.count = 0;
        this.fxMgr.DestoryAllFXObj();
        this.ReleaseCountdown();
        return null;
    }
    public int GetCount()
    {
        return this.count;
    }
    public int Compare(Buff item)
    {
        return  0;
    }
}
