using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Condition))]
public class ConditionEditor : SelfAdaptionPropertyDrawer
{
    public override string GetSpreadName(SerializedProperty property)
    {
        return "条件";
    }
    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty conditionType = property.FindPropertyRelative("conditionType");
        SerializedProperty isOverrideTip = property.FindPropertyRelative("isOverrideTip");
        SerializedProperty conditionTip = property.FindPropertyRelative("conditionTip");

        SerializedProperty condition_Level = property.FindPropertyRelative("condition_Level");
        SerializedProperty condition_Task = property.FindPropertyRelative("condition_Task");
        SerializedProperty condition_Beat = property.FindPropertyRelative("condition_Beat");
        SerializedProperty condition_Talk = property.FindPropertyRelative("condition_Talk");
        SerializedProperty condition_Collection = property.FindPropertyRelative("condition_Collection");

        EditorGUI.PropertyField(GetRectVertical(conditionType), conditionType);

        if ((ConditionType)conditionType.enumValueIndex == ConditionType.等级)
        {
            EditorGUI.PropertyField(GetRectVertical(condition_Level), condition_Level, true);//****最后面写true，可以把类的子物体也弄出来***
        }
        else if ((ConditionType)conditionType.enumValueIndex == ConditionType.任务)
        {
            EditorGUI.PropertyField(GetRectVertical(condition_Task), condition_Task, true);
        }
        else if ((ConditionType)conditionType.enumValueIndex == ConditionType.击杀)
        {
            EditorGUI.PropertyField(GetRectVertical(condition_Beat), condition_Beat, true);
        }
        else if ((ConditionType)conditionType.enumValueIndex == ConditionType.对话)
        {
            EditorGUI.PropertyField(GetRectVertical(condition_Talk), condition_Talk, true);
        }
        else if ((ConditionType)conditionType.enumValueIndex == ConditionType.收集)
        {
            EditorGUI.PropertyField(GetRectVertical(condition_Collection), condition_Collection, true);
        }

        EditorGUI.PropertyField(GetRectVertical(isOverrideTip), isOverrideTip, new GUIContent("是否重写任务提示"));
        if (isOverrideTip.boolValue == true)
            EditorGUI.PropertyField(GetRectVertical(conditionTip), conditionTip, true);
    }

}