using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimationParameterInfo))]
public class AnimationParameterInfoEditor : SelfAdaptionPropertyDrawer
{
    public override string GetSpreadName(SerializedProperty property)
    {
        return "动画触发条件信息";
    }
    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty animationParameterType = property.FindPropertyRelative("animationParameterType");
        SerializedProperty parametersName = property.FindPropertyRelative("parametersName");
        SerializedProperty parameterBool = property.FindPropertyRelative("parameterBool");
        SerializedProperty parameterFloat = property.FindPropertyRelative("parameterFloat");

        EditorGUI.PropertyField(GetRectVertical(animationParameterType), animationParameterType, new GUIContent("参数类型"));
        EditorGUI.PropertyField(GetRectVertical(parametersName), parametersName, new GUIContent("参数名字"));
        if (animationParameterType.enumValueIndex == (int)AnimationParameterType.Bool)
            EditorGUI.PropertyField(GetRectVertical(parameterBool), parameterBool);
        if (animationParameterType.enumValueIndex == (int)AnimationParameterType.Float)
            EditorGUI.PropertyField(GetRectVertical(parameterFloat), parameterFloat);
    }
}
