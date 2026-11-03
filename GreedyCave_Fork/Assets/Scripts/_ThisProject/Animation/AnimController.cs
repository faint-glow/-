using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//（同类型物体）：使用同一份代码，且动画相同的物体，都使用同一份AnimationClip，但不是同一份Animator

//一个EventTriggerEnum对应一个方法
//每个（同类型物体）的每个AnimationClip都可以使用EventTriggerEnum每一个一次，也就是一个Clip可以使用一套方法
//由于使用同一份代码的在添加动画事件时，都是添加到同一个AniamtionClip，所以内部做了一些处理，用于区别。

//使用规则：
//如果同时只有一个人使用同一份代码：比如单机游戏中的玩家。
//那么这个代码就可以对他动画中的每一个AnimationClip添加EventTriggerEnum中的每一个方法,如果方法数量不够，可以添加枚举类型，且按照本脚本最下方的方法处理一下
//动态修改动画事件时：
//一个AnimationClip中不可以有重复的EventTriggerEnum，如果重复添加，则第二次将会把第一次的删除以后再重新添加动画事件
//因此EventTriggerEnum的时间是可以修改的

//多个（同类型物体）：比如此游戏中的史莱姆或者蝙蝠之类的，场景中有多个，且使用的是同一份脚本和AnimationClip
//那么这个代码就可以对他动画中的每一个AnimationClip添加EventTriggerEnum中的每一个方法,如果方法数量不够，可以添加枚举类型，且按照本脚本最下方的方法处理一下
//动态修改动画事件时：
//如果有（同类型物体）已经在对应AnimationClip中的“某个时间”“使用过某个EventTriggerEnum”
//那么其他同类型物体使用这个EventTriggerEnum时，就无法控制时间，时间只能是第一个同类型物体添加的时间
//如果同类型物体只剩下自己，则可以控制
//因此EventTriggerEnum的时间相当于是不可以修改的
//对应的效果的实现：可以先移除自己的这个相同的EventTriggerEnum，但是用另一个没有（同类型物体）使用的EventTriggerEnum方法

//不同EventTriggerEnum对同一个AnimationClip的同一个时间点添加方法，则两个方法都存在。值得注意的是，后添加的方法先被调用

public enum EventTriggerEnum
{
    EventTriggerOne,
    EventTriggerTwo,
    EventTriggerThree,
    EventTriggerFour,
    EventTriggerFive,
    EventTriggerSix,
    EventTriggerSeven,
    EventTriggerEight,
    EventTriggerNine,
    EventTriggerTen,

    EventTriggertStart,//一般由TimeType.Start使用，不建议使用这个
    EventTriggerEnd,//一般由TimeType.End使用，不建议使用这个
}
public enum TimeType
{
    Start,
    End,
}
//使用前必须要先设置objType
public class AnimController : MonoBehaviour
{
    private Animator anim;
    public Animator Anim
    {
        get
        {
            if (anim == null)
                anim = gameObject.GetComponent<Animator>();
            return anim;
        }
    }
    private string objType = null;
    private bool isRetainEventOnRunOut;
    //使用同一份动画的的物体 对应 他的事件信息（比如一个一个场景的所有史莱姆的（规定名字）都对应一个（AnimatorAided））
    public static Dictionary<string, AnimatorAided> animators = new Dictionary<string, AnimatorAided>();
    //拥有该脚本的一个物体（注意：不是一种），添加过的事件信息 对应 事件，用于删除所有事件
    //public Dictionary<string, Action> myAnimEvents = new Dictionary<string, Action>();
    private Dictionary<string, Dictionary<string, Action>> myAnimClipEvents = new Dictionary<string, Dictionary<string, Action>>();
    public void Initial(string objType,bool isRetainEventOnRunOut = true)
    {
        this.objType = objType;
        this.isRetainEventOnRunOut = isRetainEventOnRunOut;
    }
    //public void DeleteMyAllAnimation()
    //{
    //    foreach (var item in myAnimClipEvents)
    //    {
    //        AnimationClip clip = GetAnimationClip(Anim, item.Key);
    //        clip.events = default(AnimationEvent[]);
    //    }
    //}
    public void RemoveMyAllAnimationEvent()
    {
        foreach (var myAnimClipEventsPair in myAnimClipEvents)
        {
            RemoveClipAnimationEvent(myAnimClipEventsPair.Key,null,false);//不能再foreach里面移除key
        }
        myAnimClipEvents.Clear();
    }
    public void RemoveClipAnimationEvent(string clipName, HashSet<EventTriggerEnum> breakEvents, bool isRemoveKey = true)
    {
        if (!myAnimClipEvents.ContainsKey(clipName))
            return;
        foreach (var item in myAnimClipEvents[clipName])
        {
            string useClipName;
            EventTriggerEnum eventType;
            GetMyEventInfo(item.Key, out useClipName, out eventType);
            AnimationClip clip = GetAnimationClip(Anim, useClipName);
            if (clip == null)
                continue;
            if (breakEvents != null && breakEvents.Contains(eventType))
                continue;
            RemoveEvent(clip, eventType, item.Value);//注意：不能直接RemoveEventHandler，foreach中移除item会出错
        }
        if(isRemoveKey == true && myAnimClipEvents.Count <= 0)
            myAnimClipEvents.Remove(clipName);
    }
    private void AddMyEventInfo(string clipName, EventTriggerEnum eventType, Action action)
    {
        string content = clipName + ":" + eventType.ToString();

        if (!myAnimClipEvents.ContainsKey(clipName))
            myAnimClipEvents[clipName] = new Dictionary<string, Action>();
        myAnimClipEvents[clipName][content] = action;
    }
    private void RemoveMyEventInfo(string clipName, EventTriggerEnum eventType)
    {
        string content = clipName + ":" + eventType.ToString();
        if(myAnimClipEvents.ContainsKey(clipName) && myAnimClipEvents[clipName].ContainsKey(content))
        {
            myAnimClipEvents[clipName].Remove(content);
            if (myAnimClipEvents[clipName].Count <= 0)
                myAnimClipEvents.Remove(clipName);
        }
    }
    private void GetMyEventInfo(string content, out string clipName, out EventTriggerEnum eventType)
    {
        int index = content.LastIndexOf(":");//不用split是为了防止clip的名字自带:
        clipName = content.Substring(0, index);
        string eventTypeName = content.Substring(index + 1);
        eventType = (EventTriggerEnum)System.Enum.Parse(typeof(EventTriggerEnum), eventTypeName);
    }
    private void OnDestroy()
    {
        RemoveMyAllAnimationEvent();
    }

    public void AddEvent(string clipName, EventTriggerEnum funcEnum, float time, Action action)
    {
        AnimationClip clip = GetAnimationClip(Anim,clipName);
        if (clip == null)
            return;
        AddEvent(clip, funcEnum, time,action);
        AddMyEventInfo(clip.name, funcEnum, action);
    }
    public void AddEventHandler(string clipName, TimeType timeType, Action action)
    {
        AnimationClip clip = GetAnimationClip(Anim, clipName);
        if (clip == null)
            return;
        float time = GetTime(clip, timeType);
        EventTriggerEnum funcEnum = EventTriggerEnum.EventTriggertStart;
        if (timeType == TimeType.Start)
            funcEnum = EventTriggerEnum.EventTriggertStart;
        else
            funcEnum = EventTriggerEnum.EventTriggerEnd;

        AddEvent(clip, funcEnum, time, action);
        AddMyEventInfo(clip.name, funcEnum, action);
    }

    public void RemoveEventHandler(string clipName, EventTriggerEnum funcEnum, Action action)
    {
        AnimationClip clip = GetAnimationClip(Anim, clipName);
        if (clip == null)
            return;
        RemoveEvent(clip, funcEnum, action);
        RemoveMyEventInfo(clip.name, funcEnum);
    }

    private void AddEvent(AnimationClip clip, EventTriggerEnum funcEnum, float time, Action action)
    {
        if (!animators.ContainsKey(objType))
            animators[objType] = new AnimatorAided();
        //如果该对象的对应Clip已经使用过funcEnum，则要先删掉原来的
        Action olderAction = animators[objType].CheckIsHave(Anim, clip, funcEnum);
        if (olderAction != null)
        {
            RemoveEventHandler(clip.name, funcEnum, olderAction);
            //if (!animators.ContainsKey(objType))
            //    animators[objType] = new AnimatorAided();
        }
        if (animators[objType].AddEvent(Anim,clip, funcEnum , action))
        {
            SetEvent(clip, funcEnum.ToString(), time);
        }
    }

    private void RemoveEvent(AnimationClip clip, EventTriggerEnum funcEnum, Action action)
    {
        if (!animators.ContainsKey(objType))
            return;

        if(animators[objType].RemoveEvent(Anim, clip, funcEnum , action) && isRetainEventOnRunOut == false)
        {
            //DeleteEvent(clip, funcEnum.ToString());
        }

        //if (animators[objType].events.Count <= 0)
        //    animators.Remove(objType);
    }

    public static AnimationClip GetAnimationClip(Animator anim, string clipName)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == clipName)
                return clips[i];
        }
        return null;
    }

    private static float GetTime(AnimationClip clip, TimeType timeType)
    {
        if (timeType == TimeType.Start)
            return 0;
        else
            return (clip.length * 0.60f - 0.05f);//clip.length是按分钟的比值来的（比如：时间轴上显示0.30，那么clip.length就是0.5，也就是0.6的一半，如果是大于1秒的，整数部分按照秒计算，小数部分同前），我经过调整，可以按照时间轴显示的时间来使用
    }

    public static void SetEvent(AnimationClip clip, string funcName, float time)
    {
        AnimationEvent animationEvent = new AnimationEvent();
        animationEvent.functionName = funcName;
        animationEvent.stringParameter = clip.name;

        #region 时间处理的原理
        //时间的小数部分是以60的百分比计算帧率的。
        //比如animation面板0.15秒等于0.15*60 = 9。但是这个9是animation面板的第9帧。
        //想要变成0.15秒，对应animation面板的第15帧，只需要让15/60 = 0.25就可以获得我们15帧对应的0.25秒
        #endregion
        string timeStr = time.ToString("f2");
        int intNum = (int)time;
        float floatNum = float.Parse(timeStr.Substring(timeStr.IndexOf(".") + 1));
        floatNum = floatNum / 60;

        animationEvent.time = intNum + floatNum;
        clip.AddEvent(animationEvent);
    }
    public void DeleteEvent(AnimationClip clip, string funcName)
    {
        if (clip.events.Length <= 0)
            return;

        //删除动画的事件，Unity删除动画事件的API实在找不到。
        //唯一可以把动画事件删掉的就只找到clip.events = default(AnimationEvent[]);并且这个对单独的events[i]无效
        List<AnimationEvent> newEvent = new List<AnimationEvent>();//记录需要保留的方法
        for (int i = 0; i < clip.events.Length; i++)
        {
            if (clip.events[i].functionName != funcName)
                newEvent.Add(clip.events[i]);
        }

        clip.events = default(AnimationEvent[]);//删掉所有方法
        for (int i = 0; i < newEvent.Count; i++)//复原保留的方法
        {
            clip.AddEvent(newEvent[i]);
        }
    }
    public void PrintAllTypeCount()
    {
        Debug.Log(animators.Count);
    }
    public void PrintAnimatorCount()
    {
        if (!animators.ContainsKey(objType))
            return;
        animators[objType].PrintAnimatorCount(Anim,objType);
    }

    public void PrintEvent()
    {
        if (!animators.ContainsKey(objType))
            return;
        Debug.Log(anim.gameObject.name + ":");
        animators[objType].PrintEvents(Anim);
    }

    private void EventTriggertHandler(string clipName, EventTriggerEnum type)
    {
       
        if (Anim == null)
            return;
        if (objType == null)
            return;

        Action action = animators[objType].GetEvent(Anim, clipName, type);
        if (action != null)
        {
            action.Invoke();
        }
    }
    //*******添加枚举以后的处理方法，只看这下面，其他地方都不用管*******
    public void EventTriggertStart(string clipName)
    {
        EventTriggertHandler(clipName, EventTriggerEnum.EventTriggertStart);
    }
    public void EventTriggerEnd(string clipName)
    {
        EventTriggertHandler(clipName, EventTriggerEnum.EventTriggerEnd);
    }
    public void EventTriggerOne(string clipName)
    {
        EventTriggertHandler(clipName, EventTriggerEnum.EventTriggerOne);
    }
    public void EventTriggerTwo(string clipName)
    {
        EventTriggertHandler(clipName, EventTriggerEnum.EventTriggerTwo);
    }
    public void EventTriggerThree(string clipName)
    {
        EventTriggertHandler(clipName, EventTriggerEnum.EventTriggerThree);
    }
    public void EventTriggerFour(string clipName)
    {
        EventTriggertHandler(clipName, EventTriggerEnum.EventTriggerFour);
    }
    public void EventTriggerFive(string clipName)
    {
        EventTriggertHandler(clipName, EventTriggerEnum.EventTriggerFive);
    }
    public void EventTriggerSix(string clipName)
    {
        EventTriggertHandler(clipName, EventTriggerEnum.EventTriggerSix);
    }
    public void EventTriggerSeven(string clipName)
    {
        EventTriggertHandler(clipName, EventTriggerEnum.EventTriggerSeven);
    }
    public void EventTriggerEight(string clipName)
    {
        EventTriggertHandler(clipName, EventTriggerEnum.EventTriggerEight);
    }
    public void EventTriggerNine(string clipName)
    {
        EventTriggertHandler(clipName, EventTriggerEnum.EventTriggerNine);
    }
    public void EventTriggerTen(string clipName)
    {
        EventTriggertHandler(clipName, EventTriggerEnum.EventTriggerTen);
    }
}
