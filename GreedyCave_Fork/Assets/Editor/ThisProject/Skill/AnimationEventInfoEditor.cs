using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(AnimationEventInfo))]
public class AnimationEventInfoEditor : SelfAdaptionPropertyDrawer
{
    public override string GetSpreadName(SerializedProperty property)
    {
        return "需要的clip中就算不触发事件，也需要至少一个包含该clip名字的此对象";
    }
    public override void InitialProperty(SerializedProperty property)
    {
        SerializedProperty haveEvent = property.FindPropertyRelative("haveEvent");
        haveEvent.boolValue = true;
    }
    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty clipName = property.FindPropertyRelative("clipName");
        SerializedProperty haveEvent = property.FindPropertyRelative("haveEvent");
        SerializedProperty skilltriggerTime = property.FindPropertyRelative("skilltriggerTime");
        SerializedProperty triggerTypes = property.FindPropertyRelative("triggerTypes");
        SerializedProperty eventTriggerEnum = property.FindPropertyRelative("eventTriggerEnum");


        ShowPropertyVertical(clipName, "动画片段的名字");
        ShowPropertyVertical(haveEvent, "是否拥有动画事件");
        if(haveEvent.boolValue == true)
        {
            ShowPropertyVertical(skilltriggerTime, "触发事件的时间");
            ShowPropertyVertical(eventTriggerEnum, "触发的技能效果对应的事件");
            ShowPropertyVertical(triggerTypes, "触发效果的类型");
        }
    }
}
