using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectTarget
{
    Null,
    Buff,
    User,
    Target,
}
public enum EffectConditionType_Warrior
{
    
}
public enum EffectConditionType_Buff
{
     ‹µΩ…À∫¶,
}
[System.Serializable]
public class EffectCondition: SelfAdaptationClassMgr
{
    public EffectTarget effectTarget;
    public PlusOrMinus plusOrMinus;
    public EffectConditionType_Buff conditionType_Buff;
    public List<PropertyType> targetPropertyTypes;
    public SizeType sizeType;
    public float targetValue;
    public Value value;
    public bool Check(Effect effect,ApplyInfo applyInfo)
    {
        if (effectTarget == EffectTarget.Null)
            return true;
        if (effectTarget == EffectTarget.Buff)
            return Check_Buff(effect, applyInfo);
        return true;
    }

    private bool Check_Buff(Effect effect, ApplyInfo applyInfo)
    {
        Buff buff = effect.GetLastEffectSource() as Buff;
        if (buff == null)
            return true;
        float targetCheckValue = 0;
        for (int i = 0; i < targetPropertyTypes.Count; i++)
        {
            targetCheckValue += buff.GetAdjustInjuredValue(targetPropertyTypes[i]);
        }
        return Condition.CheckValue(this.sizeType,value.GetValue(applyInfo, buff), targetCheckValue);
    }
}
