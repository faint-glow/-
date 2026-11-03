using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Parameters
{
    [Header("普通攻击伤害在动画中的触发时间")]
    public float normalAttackTriggerTime;
    [Header("注意这里的值都是只对应属性的成长系数")]//[UnityEngine.Serialization.FormerlySerializedAs("propertyFactors")]修改变量名
    public PropertyValue[] propertyFactors;
}

[System.Serializable]
//必须要可序列化才可以显示在编辑器面板中。并且里面的字段也必须是公有的才可以显示，私有的不能显示
public class PropertyValue
{
    public PropertyValue()
    {

    }
    public PropertyValue(PropertyType propertyType, float baseValue)
    {
        this.propertyType = propertyType;
        this.baseValue = baseValue;
    }
    public PropertyType propertyType;
    public float baseValue;
    public float growValue;
}