using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlType
{
    AnimControl,
    ShiftControl,
}
public abstract class ControlInfo : ScriptableObject
{
    public string controlName;
    public bool isNeverEnd;
    public float controlTime;//控制时间
    public int controlLevel;
    public EffectFXMgr effectFXMgr;

    public abstract Control GetControl();
    public string GetControlInfoText()
    {
        return "持续" + controlTime +"秒"+ controlName;
    }
}
