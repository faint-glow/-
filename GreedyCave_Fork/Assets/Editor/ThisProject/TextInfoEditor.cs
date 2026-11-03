using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TextInfoProperty
{
    public SerializedProperty isUse;
    public SerializedProperty isOverride;
    public SerializedProperty textInfo;
    public SerializedProperty textInfoContent;
    public SerializedProperty overrideTextInfo;
    public SerializedProperty overrideTextInfoContent;

}
[CustomPropertyDrawer(typeof(TextInfo))]
public class TextInfoEditor : SelfAdaptionPropertyDrawer
{
    public override string GetSpreadName(SerializedProperty property)
    {
        return "信息";
    }
    public static TextInfoProperty GetTextInfoProperty(SerializedProperty property)
    {
        TextInfoProperty textInfoProperty = new TextInfoProperty();
        textInfoProperty.isUse = property.FindPropertyRelative("isUse");
        textInfoProperty.isOverride = property.FindPropertyRelative("isOverride");
        textInfoProperty.textInfo = property.FindPropertyRelative("textInfo");
        textInfoProperty.textInfoContent = textInfoProperty.textInfo.FindPropertyRelative("content");
        textInfoProperty.overrideTextInfo = property.FindPropertyRelative("overrideTextInfo");
        textInfoProperty.overrideTextInfoContent = textInfoProperty.overrideTextInfo.FindPropertyRelative("content");

        return textInfoProperty;
    }

    public override void InitialProperty(SerializedProperty property)
    {
        TextInfoProperty textInfoProperty = GetTextInfoProperty(property);
        textInfoProperty.isUse.boolValue = true;
    }
    
    public static bool IsUseTextInfo(SerializedProperty property)
    {
        TextInfoProperty textInfoProperty = GetTextInfoProperty(property);
        return textInfoProperty.isUse.boolValue;
    }
    public static void SetAudioTextInfo(SerializedProperty property,string content)
    {
        TextInfoProperty textInfoProperty = GetTextInfoProperty(property);
        textInfoProperty.textInfoContent.stringValue = content;
    }

    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        TextInfoProperty textInfoProperty = GetTextInfoProperty(property);
        ShowPropertyVertical(textInfoProperty.isUse, "是否使用该内容于游戏中");
        ShowPropertyHorizontal(textInfoProperty.isOverride,30,"是否重写");
        if (textInfoProperty.isOverride.boolValue == false)
        {
            if (GUI.Button(GetRectFreeHorizontal(0, 200), new GUIContent("复制并重写(当前为自动)")))
            {
                textInfoProperty.isOverride.boolValue = true;
                textInfoProperty.overrideTextInfoContent.stringValue = textInfoProperty.textInfoContent.stringValue;
            }
        }
        else
        {
            if (GUI.Button(GetRectFreeHorizontal(0, 200), new GUIContent("自动(当前为重写)")))
            {
                textInfoProperty.isOverride.boolValue = false;
            }
        }

        if (textInfoProperty.isOverride.boolValue == true)
            ShowPropertyVertical(textInfoProperty.overrideTextInfo);
        else
            ShowPropertyVertical(textInfoProperty.textInfo);
    }
}
