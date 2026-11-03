using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ValueSourceInfo))]
public class ValueSourceInfoEditor : SelfAdaptionPropertyDrawer
{
    public override void InitialProperty(SerializedProperty property)
    {
        SerializedProperty sourceValueType = property.FindPropertyRelative("sourceValueType");
        sourceValueType.enumValueIndex = (int)(SourceValueType)SourceValueType.BaseValue;
    }

    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty ratioType = property.FindPropertyRelative("ratioType");
        SerializedProperty sourceValueType = property.FindPropertyRelative("sourceValueType");
        SerializedProperty ratioSource = property.FindPropertyRelative("ratioSource");
        SerializedProperty ratio = property.FindPropertyRelative("ratio");

        ShowPropertyVertical(ratioType);
        ShowPropertyVertical(sourceValueType);
        ShowPropertyVertical(ratioSource);
        ShowPropertyVertical(ratio);
    }
}
