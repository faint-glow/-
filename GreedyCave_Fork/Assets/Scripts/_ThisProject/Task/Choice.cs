using System;
using UnityEngine;

public enum ChoiceType
{
    继续对话,
    离开,
    任务处理,
    领取任务,
    完成对话,
    完成任务,
    下一阶段,//实际效果与完成任务相同，为了便于理解，增加一个枚举类型
    下个任务,//领取奖励+下个任务
    执行方法,
}

[System.Serializable]
public class Choice: SelfAdaptationClassMgr,IGetAble
{
    public string choiceName;
    public ChoiceType choiceType;
    public TalkInfo talk;
    [HideInInspector][NonSerialized] public Task task;
    [HideInInspector][NonSerialized] public Action action;
    public void SetLeave(string choiceName)
    {
        this.choiceType = ChoiceType.离开;
        this.choiceName = choiceName;
    }

    public void SetOperateTask(Task task)
    {
        this.choiceType = ChoiceType.任务处理;
        this.choiceName = task.GetTaskName();
        this.task = task;
    }
    public void SetReveiceTask(string choiceName,Task task)
    {
        this.choiceType = ChoiceType.领取任务;
        this.choiceName = choiceName;
        this.task = task;
    }
    public void SetFinishTask(string choiceName,Task task)
    {
        this.choiceType = ChoiceType.完成任务;
        this.choiceName = choiceName;
        this.task = task;
    }

    public void SetAction(string choiceName,Action action)
    {
        this.choiceType = ChoiceType.执行方法;
        this.choiceName = choiceName;
        this.action = action;
    }
}
