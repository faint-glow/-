using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(AnimationTriggerInfo))]
public class AnimationTriggerInfoEditor : SelfAdaptionPropertyDrawer
{
    public override string GetSpreadName(SerializedProperty property)
    {
        return "动画触发信息";
    }
    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty animationTriggerType = property.FindPropertyRelative("animationTriggerType");
        SerializedProperty clipName = property.FindPropertyRelative("clipName");
        SerializedProperty delayTime = property.FindPropertyRelative("delayTime");
        SerializedProperty animationParameterInfos = property.FindPropertyRelative("animationParameterInfos");

        ShowPropertyVertical(animationTriggerType, "技能触发的阶段");
        if (animationTriggerType.enumValueIndex != (int)AnimationTriggerType.AfterClip)
            AddHideProperty(clipName);
        ShowPropertyVertical(clipName,"目标动画");
        ShowPropertyVertical(delayTime,"延迟时间");
        ShowPropertyVertical(animationParameterInfos,"目标设置的参数");
    }
}
