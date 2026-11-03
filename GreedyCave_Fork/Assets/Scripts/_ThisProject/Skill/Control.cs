using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////这个要在创建集合时给构造函数提供，IComparer的优先级大于IComparable，只要提供了这个，就不会使用默认的IComparable排序
////SortedSet<ControlTest> sortedSet = new SortedSet<ControlTest>(new ReverseSort<ControlTest>());
//public class ReverseSort<T> : IComparer<T>//逆序
//    where T : IComparable<T>//只有自己实现的了自定义正序的，才可以使用这个逆序
//{
//    public int Compare(T x, T y)
//    {
//        if (x.CompareTo(y) > 0)
//            return -1;
//        else if (x.CompareTo(y) < 0)
//            return 1;
//        else
//            return 0;
//    }
//}
////实现了IComparable就可以直接使用
////SortedSet<ControlTest> sortedSet = new SortedSet<ControlTest>();
//public class ControlTest : IComparable<ControlTest>
//{
//    public float animatorControlLevel;//动画控制等级
//    public int CompareTo(ControlTest other)//自己实现正序排列，反向排列使用上面的ReverseSort
//    {
//        if (animatorControlLevel > other.animatorControlLevel)
//            return 1;
//        else if (animatorControlLevel < other.animatorControlLevel)
//            return -1;
//        else
//            return 0;
//    }
//}

public abstract class Control
{
    public Control(ControlInfo controlInfo)
    {
        this.controlInfo = controlInfo;
        this.fxMgr = controlInfo.effectFXMgr;
    }
    protected ControlInfo controlInfo;
    protected ApplyInfo applyInfo;
    protected EffectFXMgr fxMgr;
    protected bool isUse = false;
    protected CountdownTimerInfo countdownTimerInfo;
    public ControlType _ControlType
    {
        get { return GetControlType(); }
    }
    public abstract ControlType GetControlType();
    public int ControlLevel
    {
        get { return controlInfo.controlLevel; }
    }
    public float GetAllControlTime()
    {
        return controlInfo.controlTime;
    }
    public float GetCurrentControlTime()
    {
        if (this.countdownTimerInfo == null)
            return 0;
        return this.countdownTimerInfo.CurrentTime;
    }
    public int Compare(Control control)
    {
        if (this.controlInfo.controlLevel > control.controlInfo.controlLevel)
            return 1;
        else if(this.controlInfo.controlLevel < control.controlInfo.controlLevel)
            return -1;
        return 0;
    }
    public void SetApplyInfoHandler(ApplyInfo applyInfo)
    {
        this.applyInfo = applyInfo;
        this.isUse = false;
        this.SetApplyInfo(applyInfo);
        //开始倒计时
        if (controlInfo.isNeverEnd == false)
            MonoMgr.GetInstance().CountdownTimer(controlInfo.controlTime, StartCountdownHandler, CountdownHandler, CountdownEndHandler);
    }
    public void UseControl()
    {
        if(controlInfo.isNeverEnd == false)
            this.isUse = true;
        else
            StartControl();
    }
    public string GetControlName()
    {
        return controlInfo.controlName;
    }
    protected virtual void StartControl()
    {
        
    }
    public void StopHandler()
    {
        if (isUse == false)//如果现在没有使用，就不调用
            return;
        isUse = false;
        OnStop();
    }
    public virtual void OnStop()
    {

    }
    private void StartCountdownHandler(CountdownTimerInfo countdownTimerInfo)
    {
        this.countdownTimerInfo = countdownTimerInfo;
    }
    private void CountdownHandler(CountdownTimerInfo countdownTimerInfo)
    {
        if (isUse == false)
            return;
        Countdown(countdownTimerInfo);
    }
    private void CountdownEndHandler(CountdownTimerInfo countdownTimerInfo)
    {
        applyInfo.target.RemoveControl(this);//从warrior中移除
        if (isUse == false)
            return;
        CountdownEnd(countdownTimerInfo);
    }

    protected virtual void SetApplyInfo(ApplyInfo applyInfo)
    {

    }
    protected virtual void Countdown(CountdownTimerInfo countdownTimerInfo)
    {

    }
    protected void CountdownEnd(CountdownTimerInfo countdownTimerInfo)
    {

    }

}
