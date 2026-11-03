using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EffectConditionProperty
{
    public SerializedProperty effectTarget;
    //public SerializedProperty plusOrMinus;
    public SerializedProperty conditionType_Buff;
    public SerializedProperty targetPropertyTypes;
    public SerializedProperty sizeType;
    //public SerializedProperty targetValue;
    public SerializedProperty value;
}

[CustomPropertyDrawer(typeof(EffectCondition))]
public class EffectConditionEditor : SelfAdaptionPropertyDrawer
{
    private static EffectConditionProperty GetEffectConditionProperty(SerializedProperty property)
    {
        EffectConditionProperty effectConditionProperty = new EffectConditionProperty();
        effectConditionProperty.effectTarget = property.FindPropertyRelative("effectTarget");
        //effectConditionProperty.plusOrMinus = property.FindPropertyRelative("plusOrMinus");
        effectConditionProperty.conditionType_Buff = property.FindPropertyRelative("conditionType_Buff");
        effectConditionProperty.targetPropertyTypes = property.FindPropertyRelative("targetPropertyTypes");
        effectConditionProperty.sizeType = property.FindPropertyRelative("sizeType");
        //effectConditionProperty.targetValue = property.FindPropertyRelative("targetValue");
        effectConditionProperty.value = property.FindPropertyRelative("value");
        return effectConditionProperty;
    }
    public override string GetSpreadName(SerializedProperty property)
    {
        return "效果触发条件";// GetConditionInfo();
    }
    public override void InitialProperty(SerializedProperty property)
    {

    }
    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EffectConditionProperty effectConditionProperty = GetEffectConditionProperty(property);
        ShowPropertyVertical(effectConditionProperty.effectTarget);
        if (effectConditionProperty.effectTarget.enumValueIndex != (int)EffectTarget.Null)
        {
            //ShowPropertyVertical(effectConditionProperty.plusOrMinus);
            ShowPropertyVertical(effectConditionProperty.conditionType_Buff);
            ShowPropertyVertical(effectConditionProperty.targetPropertyTypes);
            ShowPropertyVertical(effectConditionProperty.sizeType);
            //ShowPropertyVertical(effectConditionProperty.targetValue);
            ShowPropertyVertical(effectConditionProperty.value);
        }
    }
    public static string GetEffectConditionInfo(SerializedProperty property)
    {
        EffectConditionProperty effectConditionProperty = GetEffectConditionProperty(property);
        string content = "";
        if (effectConditionProperty.effectTarget.enumValueIndex == (int)EffectTarget.Null)
            content = "";
        else if (effectConditionProperty.effectTarget.enumValueIndex == (int)EffectTarget.Buff)
            content = GetEffectConditionInfo_Buff(effectConditionProperty);

        return content;
    }
    private static string GetEffectConditionInfo_Buff(EffectConditionProperty effectConditionProperty)
    {
        string content = "";
        content += "当";
        if (effectConditionProperty.effectTarget.enumValueIndex == (int)EffectTarget.Buff)
            content += "Buff";

        //float targetValue = effectConditionProperty.targetValue.floatValue;
        //if (IsPlus(effectConditionProperty) == false)
        //    targetValue = -targetValue;
        //content += GetAutoEffectPlusInfo(effectConditionProperty) + "的" + GetPropertyTypeInfo(effectConditionProperty) + "伤害" + ((SizeType)effectConditionProperty.sizeType.enumValueIndex).ToString() + targetValue + "时";

        content += GetAutoEffectPlusInfo(effectConditionProperty.value) + "的" + GetPropertyTypeInfo(effectConditionProperty) + "伤害" + GetSizeInfo(effectConditionProperty) + GetAutoValueInfo(effectConditionProperty) + "时";
        return content;
    }

    private static string GetSizeInfo(EffectConditionProperty effectConditionProperty)
    {
        SizeType sizeType = (SizeType)effectConditionProperty.sizeType.enumValueIndex;

        if (ValueEditor.IsPlus(effectConditionProperty.value) == false)//如果是负，则反转大小于情况
        {
            if (sizeType == SizeType.大于)
                sizeType = SizeType.小于;
            else if (sizeType == SizeType.大于等于)
                sizeType = SizeType.小于等于;
            if (sizeType == SizeType.小于)
                sizeType = SizeType.大于;
            else if (sizeType == SizeType.小于等于)
                sizeType = SizeType.大于等于;
        }
        return sizeType.ToString();
    }

    private static string GetPropertyTypeInfo(EffectConditionProperty effectConditionProperty)
    {
        string content = "";
        for (int i = 0; i < effectConditionProperty.targetPropertyTypes.arraySize; i++)
        {
            if (i > 0)
                content += "和";
            PropertyType propertyType = (PropertyType)effectConditionProperty.targetPropertyTypes.GetArrayElementAtIndex(i).enumValueIndex;
            content += Property.GetAttackTypeText(propertyType);
        }
        return content;
    }

    public static string GetAutoValueInfo(EffectConditionProperty effectConditionProperty)
    {
        return ValueEditor.GetAutoValueInfo(effectConditionProperty.value, PropertyType.无);
    }
    public static string GetAutoEffectPlusInfo(SerializedProperty value)
    {
        return ValueEditor.GetAutoEffectPlusInfo(value);
    }
}
