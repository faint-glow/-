using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ValuePropertys
{
    public SerializedProperty valueType;
    public SerializedProperty plusOrMinus;
    public SerializedProperty baseValue;
    public SerializedProperty value_Property;
    public SerializedProperty value_Damage;
    public SerializedProperty value_Injured;
    //public SerializedProperty value_Loss;
}

[CustomPropertyDrawer(typeof(Value))]
public class ValueEditor : SelfAdaptionPropertyDrawer
{
    //private ValuePropertys valuePropertys = new ValuePropertys();
    public override string GetSpreadName(SerializedProperty property)
    {
        return "Value值：" + GetAutoValueInfo(property, PropertyType.无);
    }
    public override void InitialProperty(SerializedProperty property)
    {
        //SerializedProperty value_Property = property.FindPropertyRelative("value_Property");
        //SerializedProperty ratioType = value_Property.FindPropertyRelative("ratioType");
        //SerializedProperty sourceValueType = value_Property.FindPropertyRelative("sourceValueType");
        //ratioType.enumValueIndex = (int)ApplyTargetType.User;
        //sourceValueType.enumValueIndex = (int)SourceValueType.BaseValue;
    }
    private static ValuePropertys GetValueProperty(SerializedProperty property)
    {
        ValuePropertys valueProperty = new ValuePropertys();
        valueProperty.valueType = property.FindPropertyRelative("valueType");
        valueProperty.plusOrMinus = property.FindPropertyRelative("plusOrMinus");
        valueProperty.baseValue = property.FindPropertyRelative("baseValue");
        valueProperty.value_Property = property.FindPropertyRelative("value_Property");
        valueProperty.value_Damage = property.FindPropertyRelative("value_Damage");
        valueProperty.value_Injured = property.FindPropertyRelative("value_Injured");
        //valueProperty.value_Loss = property.FindPropertyRelative("value_Loss");
        return valueProperty;
    }
    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ValuePropertys valuePropertys = GetValueProperty(property);

        EditorGUI.PropertyField(GetRectVertical(valuePropertys.valueType), valuePropertys.valueType);
        EditorGUI.PropertyField(GetRectVertical(valuePropertys.plusOrMinus), valuePropertys.plusOrMinus);
        EditorGUI.PropertyField(GetRectVertical(valuePropertys.baseValue), valuePropertys.baseValue);

        if ((ValueType)valuePropertys.valueType.enumValueIndex == ValueType.property)
        {
            EditorGUI.PropertyField(GetRectVertical(valuePropertys.value_Property), valuePropertys.value_Property, true);
        }
        else if ((ValueType)valuePropertys.valueType.enumValueIndex == ValueType.damage)
        {
            EditorGUI.PropertyField(GetRectVertical(valuePropertys.value_Damage), valuePropertys.value_Damage, true);
        }
        else if ((ValueType)valuePropertys.valueType.enumValueIndex == ValueType.injured)
        {
            EditorGUI.PropertyField(GetRectVertical(valuePropertys.value_Injured), valuePropertys.value_Injured, true);
        }
        //else if ((ValueType)valuePropertys.valueType.enumValueIndex == ValueType.loss)
        //{
        //    EditorGUI.PropertyField(GetRectVertical(valuePropertys.value_Loss), valuePropertys.value_Loss, true);
        //}
    }

    public static string GetAutoValueInfo(SerializedProperty property, PropertyType targetType)
    {
        ValuePropertys valuePropertys = GetValueProperty(property);
        string content = "";
        content = "(";
        if (valuePropertys.baseValue.floatValue != 0)
        {
            float value = valuePropertys.baseValue.floatValue;
            if (Property.RatioValueDic.Contains(targetType))
            {
                value *= 100;
                content += GetDisPlayValue(value, valuePropertys) + "%";
            }
            else
            {
                content += GetDisPlayValue(value, valuePropertys);
            }
        }
        if ((ValueType)valuePropertys.valueType.enumValueIndex == ValueType.property)
        {
            SerializedProperty valueSourceInfos = valuePropertys.value_Property.FindPropertyRelative("valueSourceInfos");
            for (int i = 0; i < valueSourceInfos.arraySize; i++)
            {
                SerializedProperty valueSourceInfo = valueSourceInfos.GetArrayElementAtIndex(i);
                SerializedProperty ratioType = valueSourceInfo.FindPropertyRelative("ratioType");//比率来源的对象
                SerializedProperty sourceValueType = valueSourceInfo.FindPropertyRelative("sourceValueType");
                SerializedProperty ratioSource = valueSourceInfo.FindPropertyRelative("ratioSource");//比率来源的对象的对应属性
                SerializedProperty ratio = valueSourceInfo.FindPropertyRelative("ratio");//比率
                if (ratio.floatValue != 0)
                {
                    string sourceValueTypeText = "未重写";
                    string ratioText = "";

                    if (sourceValueType.enumValueIndex == (int)SourceValueType.OriginalValue)
                        sourceValueTypeText = "原始";
                    else if (sourceValueType.enumValueIndex == (int)SourceValueType.BaseValue)
                        sourceValueTypeText = "基础";
                    else if (sourceValueType.enumValueIndex == (int)SourceValueType.CurrentValue)
                        sourceValueTypeText = "当前";
                    else if (sourceValueType.enumValueIndex == (int)SourceValueType.CurrentLossValue)
                        sourceValueTypeText = "已损";
                    else if (sourceValueType.enumValueIndex == (int)SourceValueType.CurrentValueRatio)
                    {
                        sourceValueTypeText = "当前";
                        ratioText = "率";
                    }
                    else if (sourceValueType.enumValueIndex == (int)SourceValueType.CurrentLossValueRatio)
                    {
                        sourceValueTypeText = "已损";
                        ratioText = "率";
                    }
                    if (i == 0)
                    {
                        if (ratio.floatValue != 0 && valuePropertys.baseValue.floatValue != 0)
                            content += "+";
                    }
                    else
                    {
                        if (ratio.floatValue != 0)
                            content += "+";
                    }
                    content += Warrior.GetApplyTargetTypeInfo((ApplyTargetType)ratioType.enumValueIndex) + "的" + sourceValueTypeText + ((PropertyType)ratioSource.enumValueIndex).ToString() + ratioText + GetDisPlayValue(ratio.floatValue, valuePropertys) * 100 + "%";
                }
            }
        }
        else if ((ValueType)valuePropertys.valueType.enumValueIndex == ValueType.damage)
        {
            SerializedProperty ratio = valuePropertys.value_Damage.FindPropertyRelative("ratio");
            SerializedProperty damageType = valuePropertys.value_Damage.FindPropertyRelative("damageType");

            if (ratio.floatValue != 0)
            {
                if (valuePropertys.baseValue.floatValue != 0)
                    content += "+";
                content += "造成" + Property.GetAttackTypeText((PropertyType)damageType.enumValueIndex) + "伤害的" + GetDisPlayValue(ratio.floatValue, valuePropertys) * 100 + "%";
            }
        }
        else if ((ValueType)valuePropertys.valueType.enumValueIndex == ValueType.injured)
        {
            SerializedProperty ratio = valuePropertys.value_Injured.FindPropertyRelative("ratio");
            SerializedProperty damageType = valuePropertys.value_Injured.FindPropertyRelative("damageType");

            if (ratio.floatValue != 0)
            {
                if (valuePropertys.baseValue.floatValue != 0)
                    content += "+";
                content += "受到" + Property.GetAttackTypeText((PropertyType)damageType.enumValueIndex) + "伤害的" + GetDisPlayValue(ratio.floatValue, valuePropertys) * 100 + "%";
            }
        }
        //else if ((ValueType)valuePropertys.valueType.enumValueIndex == ValueType.loss)
        //{
        //    SerializedProperty ratioType = valuePropertys.value_Loss.FindPropertyRelative("ratioType");
        //    SerializedProperty lossValueType = valuePropertys.value_Loss.FindPropertyRelative("lossValueType");
        //    SerializedProperty ratio = valuePropertys.value_Loss.FindPropertyRelative("ratio");
        //    if (ratio.floatValue != 0)
        //    {
        //        if (valuePropertys.baseValue.floatValue != 0)
        //            content += "+";
        //        content += Warrior.GetApplyTargetTypeInfo((ApplyTargetType)ratioType.enumValueIndex) + "的" + ((LossValueType)lossValueType.enumValueIndex).ToString() + GetDisPlayValue(ratio.floatValue, valuePropertys) * 100 + "%";
        //    }
        //}
        content += ")";
        return content;
    }
    //不适用Mathf.Abs是因为某些情况显示会有问题
    //比如造成伤害，此时ratio应该是负的，但是此时如果使用Mathf.Abs不管ratio正负，显示的都是造成xxx伤害，那么显示正确，但是代码执行却无效
    private static float GetDisPlayValue(float originalValue, ValuePropertys valuePropertys)
    {
        if (valuePropertys.plusOrMinus.enumValueIndex == (int)(PlusOrMinus.负))
            originalValue = -originalValue;

        return originalValue;
    }

    public static string GetAutoEffectPlusInfo(SerializedProperty property)
    {
        ValuePropertys valuePropertys = GetValueProperty(property);
        string content = "";
        if (valuePropertys.plusOrMinus.enumValueIndex == (int)(PlusOrMinus.正))
            content = "增加";
        else
            content = "减少";
        return content;
    }
    public static bool IsPlus(SerializedProperty property)
    {
        ValuePropertys valuePropertys = GetValueProperty(property);
        return valuePropertys.plusOrMinus.enumValueIndex == (int)(PlusOrMinus.正);
    }
}
