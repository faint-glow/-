using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimtorAble
{
    public Animator GetAnimator();
}
public interface IFixedAnimControlAble
{
    public float GetAllControlTime();
    public float GetCurrentControlTime();
}
//Animator.ResetTrigger("");清空Trigger信号
public abstract class MyAnimatorController
{
    public static string NormalAttackClipName = "Attack";

    protected AnimController animationEvent;
    protected IFixedAnimControlAble currentFixedControl;
    protected Dictionary<string, float> fixedClipSpeed = new Dictionary<string, float>();
    protected Dictionary<string, float> ratioClipSpeed = new Dictionary<string, float>();
    private IAnimtorAble animAble;
    public MyAnimatorController(IAnimtorAble animAble, string objType)
    {
        this.animAble = animAble;
        CreateAnimController(objType);
        SetAnimationEvent();
    }
    private class AnimEvent
    {
        public string clipName;
        public EventTriggerEnum funcEnum;
        public Action action;
        public AnimEvent(string clipName, EventTriggerEnum funcEnum, Action action)
        {
            this.clipName = clipName;
            this.funcEnum = funcEnum;
            this.action = action;
        }
    }
    public Animator GetAnimator()
    {
        return animAble.GetAnimator();
    }

    protected virtual void SetAnimationEvent()
    {
        AnimationClip[] clips = GetAnimator().runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            AddAnimationEvent(clips[i].name, TimeType.Start, AnimationClipStart);
        }
    }
    public void CreateAnimController(string objType)
    {
        if (GetAnimator() == null) return;

        animationEvent = animAble.GetAnimator().gameObject.GetComponent<AnimController>();
        if (animationEvent == null)
            animationEvent = animAble.GetAnimator().gameObject.AddComponent<AnimController>();

        animationEvent.Initial(objType);
    }
    public float GetCurrentAnimationSpeed()
    {
        if (GetAnimator() == null) return 1;
        return GetAnimator().speed;
    }
    public string GetCurrentAnimation()
    {
        if (GetAnimator() == null) return "";
        AnimatorClipInfo[] clips = GetAnimator().GetCurrentAnimatorClipInfo(0);
        if (clips.Length <= 0)
            return "";

        string clipName = clips[0].clip.name;
        return clipName;
    }

    public void PrintEvent()
    {
        animationEvent.PrintAnimatorCount();
        animationEvent.PrintEvent();
    }
    public void PrintAnimatorCount()
    {
        animationEvent.PrintAnimatorCount();
    }

    /// <summary>
    /// 注意使用EventTriggertStart和EventTriggertEnd时因该使用另一个重载方法
    /// </summary>
    public void AddAnimationEvent(string clipName, EventTriggerEnum eventType, float triggerTime, Action action)
    {
        animationEvent.AddEvent(clipName, eventType, triggerTime, action);
    }
    public void AddAnimationEvent(string clipName, TimeType type, Action action)
    {
        animationEvent.AddEventHandler(clipName, type, action);
    }
    public void RemoveAnimationEvent(string clipName, EventTriggerEnum eventType, Action action)
    {
        animationEvent.RemoveEventHandler(clipName, eventType, action);
    }
    public void RemoveClipAnimationEvent(string clipName, HashSet<EventTriggerEnum> breakEvents)
    {
        animationEvent.RemoveClipAnimationEvent(clipName, breakEvents);
    }
    public void RemoveAllAnimationEvent()
    {
        animationEvent.RemoveMyAllAnimationEvent();
    }
    private void GetEventInfo(string content, out string clipName, out EventTriggerEnum eventType)
    {
        int index = content.LastIndexOf(":");//不用split是为了防止clip的名字自带:
        clipName = content.Substring(0, index);
        string eventTypeName = content.Substring(index + 1);
        eventType = (EventTriggerEnum)System.Enum.Parse(typeof(EventTriggerEnum), eventTypeName);
    }
    public void SetTrigger(string triggerName)
    {
        if (GetAnimator() == null) return;
        GetAnimator().SetTrigger(triggerName);
    }
    public void SetBool(string triggerName, bool isTrue)
    {
        if (GetAnimator() == null) return;
        GetAnimator().SetBool(triggerName, isTrue);
    }
    public void SetFloat(string triggerName, float num)
    {
        if (GetAnimator() == null) return;
        GetAnimator().SetFloat(triggerName, num);
    }
    public void ResetTrigger(string triggerName)
    {
        if (GetAnimator() == null) return;
        GetAnimator().ResetTrigger(triggerName);
    }
    public void Play(string stateName, int layer = 0, float ratio = 0)
    {
        if (GetAnimator() == null) return;
        int stateid = Animator.StringToHash(stateName);
        bool hasAction = GetAnimator().HasState(0, stateid);
        if (hasAction)
        {
            //单一动作重复调用时需要使用Play方法而且需把所有参数填写完整
            GetAnimator().Play(stateName, layer, ratio);//注意这里的名字，不是动画片段的名字，而是动画状态机的那个长方体的名字
        }
    }
    public void AnimationClipStart()
    {
        if (GetAnimator().speed == 0)
            return;
        CheckCurrentClipSpeed();
    }
    private float GetSurplusTime(IFixedAnimControlAble controlAble)
    {
        if (controlAble == null)
            return 0;
        return controlAble.GetAllControlTime() - controlAble.GetCurrentControlTime();
    }
    public void SetFixedClipSpeed(string clipName, float speed, IFixedAnimControlAble controlAble)
    {
        if(this.currentFixedControl == null)
            this.currentFixedControl = controlAble;
        else if(GetSurplusTime(this.currentFixedControl) < GetSurplusTime(controlAble))
            this.currentFixedControl = controlAble;

        this.fixedClipSpeed[clipName] = speed;
        this.CheckCurrentClipSpeed();
    }
    public void RemoveFixedClipSpeed(string clipName, IFixedAnimControlAble controlAble)
    {
        if (controlAble != currentFixedControl)
            return;
        if (fixedClipSpeed.ContainsKey(clipName))
        {
            fixedClipSpeed.Remove(clipName);
            CheckCurrentClipSpeed();
        }
    }
    public void AddRatioClipSpeed(string clipName, float speed)
    {
        if (!ratioClipSpeed.ContainsKey(clipName))
            ratioClipSpeed[clipName] = 1;
        ratioClipSpeed[clipName] += speed;

        if (ratioClipSpeed[clipName] == 1)
            ratioClipSpeed.Remove(clipName);
    }
    public void RemoveRatioClipSpeed(string clipName, float speed)
    {
        if (!ratioClipSpeed.ContainsKey(clipName))
            ratioClipSpeed[clipName] = 1;
        ratioClipSpeed[clipName] -= speed;

        if (ratioClipSpeed[clipName] == 1)
            ratioClipSpeed.Remove(clipName);
    }
    public void CheckCurrentClipSpeed()
    {
        string currentClipName = GetCurrentAnimation();
        float speed = GetBaseSpeed();

        if (ratioClipSpeed.ContainsKey(currentClipName))
            speed *= ratioClipSpeed[currentClipName];
        if (fixedClipSpeed.ContainsKey(currentClipName))
            speed = fixedClipSpeed[currentClipName];

        SetAnimatorSpeed(speed);
    }
    protected virtual float GetBaseSpeed()
    {
        return 1;
    }
    //修改Animator的播放速度会影响整个状态机的速度
    public void SetAnimatorSpeed(float speed)
    {
        if (GetAnimator() == null) return;
        GetAnimator().speed = speed;
    }
}
