using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyDictionary/Task/TaskInfo", fileName = "NewTaskInfo")]
public class TaskInfo : ScriptableObject
{
    //public string promoterName;//发起者的名字
    public string taskName;
    public bool isMainThread = false;
    public string nextTaskName;
    public ConditionMgr openConditions;
    public List<TaskStepInfo> taskSteps;

    //获取发起者的名字
    public string GetTaskPromoterName()
    {
        if (this.taskSteps == null)
            return "";
        if (this.taskSteps.Count < 1)
            return "";
        if (this.taskSteps[0].receiveTalkInfo == null)
            return "";
        return this.taskSteps[0].receiveTalkInfo.NPC_Name;
    }
}


