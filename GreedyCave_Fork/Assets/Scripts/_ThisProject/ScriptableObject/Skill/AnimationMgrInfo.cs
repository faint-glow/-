using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationParameterType
{
    Trigger,
    Bool,
    Float,
}
public enum AnimationTriggerType
{
    TriggerStart,
    Trigger,
    TriggerEnd,
    AfterClip,
}

[CreateAssetMenu(menuName = "MyDictionary/Skill/AnimationMgrInfo", fileName = "NewAnimationMgrInfo")]
public class AnimationMgrInfo : ScriptableObject
{
    [Header("需要给技能持有者添加的动画事件")]
    public List<AnimationEventInfo> animationEventInfos;
    [Header("决定技能持有者触发的动画的时机")]
    public List<AnimationTriggerInfo> animationTriggerInfos;

    public void SetFirstEventTriggerTime(float triggerTime)
    {
        if (animationEventInfos == null || animationEventInfos.Count <= 0)
            return;
        animationEventInfos[0].skilltriggerTime = triggerTime;
    }
}
[System.Serializable]
public class AnimationParameterInfo:SelfAdaptationClassMgr
{
    public AnimationParameterType animationParameterType;
    public string parametersName;//用于触发使用技能的trigger的名字
    public bool parameterBool;
    public float parameterFloat;
}
[System.Serializable]
public class AnimationTriggerInfo: SelfAdaptationClassMgr
{
    public AnimationTriggerType animationTriggerType;
    public string clipName;
    public float delayTime = 0;
    public List<AnimationParameterInfo> animationParameterInfos;
}
[System.Serializable]
public class AnimationEventInfo: SelfAdaptationClassMgr
{
    public string clipName;//用于在该动画下添加技能的触发事件
    public bool haveEvent;
    public float skilltriggerTime;//技能触发事件的时间
    public EventTriggerEnum eventTriggerEnum;
    public TriggerType triggerType;

    public List<TriggerType> triggerTypes;
}
