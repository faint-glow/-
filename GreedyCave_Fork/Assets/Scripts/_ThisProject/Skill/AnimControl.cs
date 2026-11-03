using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimControl:Control,IFixedAnimControlAble
{
    private string stateName;
    private float speed;
    private bool isStartControl;
    public AnimControl(AnimControlInfo animControlInfo) :base(animControlInfo) 
    {
        this.stateName = animControlInfo.stateName;
        this.speed = animControlInfo.speed;
    }
    public override ControlType GetControlType()
    {
        return ControlType.AnimControl;
    }
    protected override void SetApplyInfo(ApplyInfo applyInfo)
    {
        isStartControl = false;
        fxMgr.UseFX(this.applyInfo, GetCurrentControlTime());
    }
    protected override void StartControl()
    {
        applyInfo.target.GetAnimatorController().SetFixedClipSpeed(stateName, this.speed,this);
        applyInfo.target.GetAnimatorController().Play(stateName);
    }
    public override void OnStop()
    {
        applyInfo.target.GetAnimatorController().RemoveFixedClipSpeed(stateName,this);
    }
    protected override void Countdown(CountdownTimerInfo countdownTimerInfo)
    {
        if (isStartControl == false)
        {
            StartControl();
            isStartControl = true;
        }
    }
    
}
