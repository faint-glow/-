using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorControlMgr
{
    private Warrior warrior;
    public WarriorControlMgr(Warrior warrior)
    {
        this.warrior = warrior;
    }
    protected Dictionary<ControlType, Control> defaultControl = new Dictionary<ControlType, Control>();
    protected Dictionary<ControlType, Control> DefaultControl
    {
        get
        {
            if (defaultControl == null)
                defaultControl = new Dictionary<ControlType, Control>();
            if (defaultControl.Count <= 0)
            {
                AnimControlInfo animControlInfo = ResMgr.GetInstance().Load<AnimControlInfo>("Control/Idle");
                Control animControl = animControlInfo.GetControl();
                animControl.SetApplyInfoHandler(new ApplyInfo(warrior, warrior));
                defaultControl[ControlType.AnimControl] = animControl;
                defaultControl[ControlType.ShiftControl] = null;
            }
            return defaultControl;
        }
    }
    protected Dictionary<ControlType, Control> currentControl = new Dictionary<ControlType, Control>();
    protected Dictionary<ControlType, SortedList> extraControl = new Dictionary<ControlType, SortedList>();

    public void UseDefaultControl(ControlType controlType)
    {
        if (DefaultControl[controlType] != null)
            DefaultControl[controlType].UseControl();
    }
    public void SetControl(Control control)
    {
        if (control != null)
            currentControl[control._ControlType] = control;
    }

    public void AddControl(Warrior user, Control control)
    {
        if (currentControl == null || currentControl.Count <= 0)
        {
            SetControl(DefaultControl[ControlType.AnimControl]);
            SetControl(DefaultControl[ControlType.ShiftControl]);
        }
        //控制级别小于当前行为的直接丢弃
        if (currentControl[control._ControlType] != null && currentControl[control._ControlType].Compare(control) > 1)
            return;
        //buff判断，比如霸体或者其他什么情况

        if (!extraControl.ContainsKey(control._ControlType))
            extraControl[control._ControlType] = new SortedList();

        SortedList sortedList = extraControl[control._ControlType];
        int maxLevel = -1;
        if (sortedList.Count > 0)
            maxLevel = (int)sortedList.GetKey(sortedList.Count - 1);

        if (!sortedList.ContainsKey(control.ControlLevel))
            sortedList[control.ControlLevel] = new List<Control>();

        control.SetApplyInfoHandler(new ApplyInfo(user, this.warrior));//为该控制效果预设初值
        List<Control> controls = sortedList[control.ControlLevel] as List<Control>;
        controls.Add(control);
        if (maxLevel >= 0 && control.ControlLevel > maxLevel)//如果以前存在最高级别的控制，并且新来的等级更高，就把以前的停下
        {
            List<Control> oldMaxControls = sortedList[maxLevel] as List<Control>;
            for (int i = 0; i < oldMaxControls.Count; i++)
            {
                if (oldMaxControls[i] != null)
                    oldMaxControls[i].StopHandler();
            }
        }
        
        if (control.ControlLevel >= maxLevel)//新来的控制只要控制等级只要大于等于以前的最高级控制的，都可以直接使用
        {
            control.UseControl();
        }
    }
    public void RemoveControl(Control control)
    {
        if (!extraControl.ContainsKey(control._ControlType))
            return;
        SortedList sortedList = extraControl[control._ControlType];
        if (!sortedList.ContainsKey(control.ControlLevel))
            return;
        List<Control> controls = sortedList[control.ControlLevel] as List<Control>;
        control.StopHandler();
        controls.Remove(control);
        int oldMaxLevel = (int)sortedList.GetKey(sortedList.Count - 1);
        if (controls.Count <= 0)//如果该级别的控制数量为0
        {
            sortedList.Remove(control.ControlLevel);//移除该级别的控制
            if (sortedList.Count > 0)//如果额外控制效果数量大于0
            {
                int currentMaxLevel = (int)sortedList.GetKey(sortedList.Count - 1);
                if (currentMaxLevel != oldMaxLevel)//检查当前的最大级别是否已经更换，如果更换，就使用更低一级的，如果没有，则不处理
                {
                    List<Control> currentMaxControl = sortedList[currentMaxLevel] as List<Control>;
                    for (int i = 0; i < currentMaxControl.Count; i++)
                    {
                        if (currentMaxControl[i] != null)
                            currentMaxControl[i].UseControl();
                    }
                }
            }
            else
            {
                extraControl.Remove(control._ControlType);
            }
        }
        if (extraControl.Count <= 0)
        {
            UseDefaultControl(control.GetControlType());
        }
    }
}
