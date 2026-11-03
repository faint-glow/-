using System.Collections.Generic;
using UnityEngine;

public enum ApplyTargetType
{
    User,
    Target,
}
public enum ValueType
{
    property,
    damage,
    injured,
    loss,
}
public enum PlusOrMinus
{
    正,
    负,
}
public enum SourceValueType
{
    OriginalValue,
    BaseValue,
    CurrentValue,
    CurrentLossValue,
    CurrentValueRatio,
    CurrentLossValueRatio,
}
public enum LossValueType
{
    已损生命值,
    已损魔法值,
}
public interface IValueAble
{
    //public bool IsPlus();
    //public bool IsMinus();
    public float GetValue(ApplyInfo applyInfo, IEffectSourceAble effectSource);
    //public string GetAutoValueInfo();

}

[System.Serializable]
public class Value:SelfAdaptationClassMgr
{
    public ValueType valueType;
    public PlusOrMinus plusOrMinus;
    public float baseValue;//基础值
    public Value_Property value_Property;
    public Value_Damage value_Damage;
    public Value_Injured value_Injured;
    //public Value_Loss value_Loss;

    public virtual float GetValue(ApplyInfo applyInfo,IEffectSourceAble effectSource)
    {
        float value = 0;
        IValueAble valueAble = GetIValueAble();
        if(valueAble != null)
            value = baseValue + valueAble.GetValue(applyInfo, effectSource);
        return value;
    }
    //public string GetAutoValueInfo()
    //{
    //    string content = "";
    //    content = "(";
    //    if (baseValue != 0)
    //        content += baseValue;
    //    IValueAble valueAble = GetIValueAble();
    //    if (valueAble != null)
    //    {
    //        if (baseValue != 0)
    //            content += "+";
    //        content += valueAble.GetAutoValueInfo();
    //    }
    //    content  += ")";
    //    return content;
    //}
    //public string GetAutoEffectInfo()
    //{
    //    string content = "";

    //    IValueAble valueAble = GetIValueAble();
    //    if (baseValue > 0 || valueAble.IsPlus())
    //        content = "增加";
    //    else if (baseValue < 0 || valueAble.IsMinus())
    //        content = "减少";
    //    else
    //        Debug.Log("双值为0,出现这个说明设置有问题");
    //    return content;
    //}
    private IValueAble GetIValueAble()
    {
        IValueAble valueAble = null;
        if (valueType == ValueType.property)
            valueAble = value_Property;
        else if (valueType == ValueType.damage)
            valueAble = value_Damage;
        else if(valueType == ValueType.injured)
            valueAble = value_Injured;
        //else if (valueType == ValueType.loss)
        //    valueAble = value_Loss;
        return valueAble;
    }
}
[System.Serializable]
public class Value_Property:IValueAble
{
    public List<ValueSourceInfo> valueSourceInfos;
    public float GetValue(ApplyInfo applyInfo,IEffectSourceAble effectSource)
    {
        if (valueSourceInfos == null)
            return 0;
        float value = 0;
        for (int i = 0; i < valueSourceInfos.Count; i++)
        {
            ValueSourceInfo valueSourceInfoElement = valueSourceInfos[i];
            Warrior warrior = Warrior.GetWarrior(applyInfo, valueSourceInfoElement.ratioType);
            if (warrior == null)
            {
                Debug.Log("warrior为null");
                return 0;
            }
            float sourceValue = 0;
            if (valueSourceInfoElement.sourceValueType == SourceValueType.OriginalValue)
                sourceValue = warrior.GetOriginalProperty(valueSourceInfoElement.ratioSource);
            else if (valueSourceInfoElement.sourceValueType == SourceValueType.BaseValue)
                sourceValue = warrior.GetBaseProperty(valueSourceInfoElement.ratioSource);
            else if(valueSourceInfoElement.sourceValueType == SourceValueType.CurrentValue)
                sourceValue = warrior.GetCurrentProperty(valueSourceInfoElement.ratioSource, effectSource);
            else if(valueSourceInfoElement.sourceValueType == SourceValueType.CurrentLossValue)
                sourceValue = warrior.GetCurrentLossProperty(valueSourceInfoElement.ratioSource, effectSource);
            else if(valueSourceInfoElement.sourceValueType == SourceValueType.CurrentValueRatio)
                sourceValue = warrior.GetCurrentRatioProperty(valueSourceInfoElement.ratioSource, effectSource);
            else if (valueSourceInfoElement.sourceValueType == SourceValueType.CurrentLossValueRatio)
                sourceValue = warrior.GetCurrentRatioLossProperty(valueSourceInfoElement.ratioSource, effectSource);
            value += valueSourceInfoElement.ratio * sourceValue;
        }
        //Debug.Log(warrior + ":总生命值：" + warrior.GetCurrentProperty(PropertyType.生命值上限, effectSource));
        //Debug.Log(warrior + ":当前生命值：" + warrior.GetCurrentProperty(PropertyType.生命值, effectSource));
        //Debug.Log(warrior + ":已损生命值" + sourceValue);
        return value;
    }
}
[System.Serializable]
public class ValueSourceInfo: SelfAdaptationClassMgr
{
    public ApplyTargetType ratioType = ApplyTargetType.User;//比率来源的对象
    public SourceValueType sourceValueType = SourceValueType.BaseValue;
    public PropertyType ratioSource;//比率来源的对象的对应属性
    public float ratio;//比率
}
[System.Serializable]
public class Value_Damage: IValueAble
{
    public PropertyType damageType;
    //public float baseValue;//基础值
    public float ratio;//比率
    public float GetValue(ApplyInfo applyInfo, IEffectSourceAble effectSource)
    {
        float damgeValue = applyInfo.user.GetBattleInfo().GetDamageValue(damageType);
        float value = Mathf.Abs(damgeValue) * ratio;
        return value;
    }
    //public string GetAutoValueInfo()
    //{
    //    string content = "";
    //    content += "造成伤害的 " + -(ratio * 100) + "%";//这里不使用Mathf.Abs是因为可以提示开发者预设的正负号是否有问题
    //    return content;
    //}
    //public bool IsPlus()
    //{
    //    return ratio > 0;
    //}
    //public bool IsMinus()
    //{
    //    return ratio < 0;
    //}
}
[System.Serializable]
public class Value_Injured : IValueAble
{
    public PropertyType damageType;//应当是InjuredType，懒得改了
    public float ratio;//比率
    public float GetValue(ApplyInfo applyInfo, IEffectSourceAble effectSource)
    {
        float damgeValue = applyInfo.user.GetBattleInfo().GetInjuredValue(damageType);
        float value = Mathf.Abs(damgeValue) * ratio;
        return value;
    }
}

//[System.Serializable]
//public class Value_Loss : IValueAble
//{
//    public ApplyTargetType ratioType = ApplyTargetType.User;//比率来源的对象
//    public LossValueType lossValueType;
//    public float ratio;//比率
//    public float GetValue(ApplyInfo applyInfo, IEffectSourceAble effectSource)
//    {
//        Warrior warrior = Warrior.GetWarrior(applyInfo,ratioType);
//        return GetLossValue(warrior, lossValueType, effectSource) * ratio;
//    }
//    private float GetLossValue(Warrior warrior,LossValueType lossValueType, IEffectSourceAble effectSource)
//    {
//        if (warrior == null)
//        {
//            Debug.Log("warrior为null");
//            return 0;
//        }
//        float value = 0;
//        if (lossValueType == LossValueType.已损生命值)
//            value = warrior.GetCurrentProperty(PropertyType.生命值上限, effectSource) - warrior.GetCurrentProperty(PropertyType.生命值, effectSource);
//        else if(lossValueType == LossValueType.已损魔法值)
//            value = warrior.GetCurrentProperty(PropertyType.魔法值上限, effectSource) - warrior.GetCurrentProperty(PropertyType.魔法值, effectSource);

//        Debug.Log(warrior + ":总生命值：" + warrior.GetCurrentProperty(PropertyType.生命值上限, effectSource) );
//        Debug.Log(warrior + ":当前生命值："+ warrior.GetCurrentProperty(PropertyType.生命值, effectSource) + ":" + value);
//        Debug.Log(warrior + ":已损生命值" + value);
//        return value;
//    }
//}