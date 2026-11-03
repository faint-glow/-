using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Choice))]
public class ChoiceEditor : SelfAdaptionPropertyDrawer
{
    public override string GetSpreadName(SerializedProperty property)
    {
        return "选择";
    }

    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty choiceName = property.FindPropertyRelative("choiceName");
        SerializedProperty choiceType = property.FindPropertyRelative("choiceType");
        SerializedProperty talk = property.FindPropertyRelative("talk");
        //SerializedProperty taskInfo = property.FindPropertyRelative("taskInfo");

        EditorGUI.PropertyField(GetRectVertical(choiceName), choiceName);
        EditorGUI.PropertyField(GetRectVertical(choiceType), choiceType);

        if ((ChoiceType)choiceType.enumValueIndex == ChoiceType.继续对话)
            EditorGUI.PropertyField(GetRectVertical(talk), talk);
        //if ((ChoiceType)choiceType.enumValueIndex == ChoiceType.下个任务)
        //    EditorGUI.PropertyField(GetRect(taskInfo), taskInfo);
    }
}
