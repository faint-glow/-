using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TaskButton : MapPanel, ISetAble<IGetAble>
{
    public static string prefabsPath = "Button/TaskButton";
    private Task task;
    public MapPanel FatherPanel { get; set; }
    public int Index { get; set; }
    private Button taskButton;
    private TMP_Text taskContent;
    public override void ShowMeFirst()
    {
        taskButton = ThisPanel.GetUI<Button>();
        taskContent = GetPanel("Text (TMP)").GetUI<TMP_Text>();
    }
    public void IsMask(bool isMask)
    {
        taskButton.enabled = isMask;
    }
    public void SetItem_Container(IGetAble obj)
    {
        Task task = obj as Task;
        taskButton.onClick.AddListener(() =>
        {
            Debug.Log("寻路待写");
        });
        string richColor = ToolsMgr.white;
        ConditionMgrState conditionMgrState = task.GetConditionMgrState();
        if (conditionMgrState == null)
            return;

        task.RefreshTaskStepState(conditionMgrState);
        string taskTitle = "[支线]：";
        if (task.IsMainThread() == true)
            taskTitle = "[主线]：";
        if (task.IsSatisfied(conditionMgrState))
            richColor = ToolsMgr.green;
        taskContent.text = ToolsMgr.GetColorText(taskTitle + task.GetTaskName() + GetTipTitle_Satisfied(task, conditionMgrState), richColor) + "\n";
        taskContent.text += task.GetTaskTip(conditionMgrState);
    }
    private string GetTipTitle_Satisfied(Task task, ConditionMgrState conditionMgrState)
    {
        string tipTitle = "";
        if (conditionMgrState.IsSatisfied())
        {
            if (task.GetCurrentSetpState() == TaskStepState.Receive)
                tipTitle = "...[可接取]";
            else if (task.GetCurrentSetpState() == TaskStepState.Finish)
                tipTitle = "...[可完成]";
        }
        return tipTitle;
    }
}
