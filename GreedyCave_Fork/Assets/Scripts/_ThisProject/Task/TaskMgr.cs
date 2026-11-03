using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum TaskType
//{
//    Talk,
//    Beat,
//    Collect,
//}

[System.Serializable]
public class TaskMgrSaveInfo
{
    public TaskSaveInfo currentMainTreadTaskSaveInfo;
    public List<TaskSaveInfo> allProcessTasksSaveInfo;
    public HashSet<string> finishTaskDic;
}
public class TaskMgr : BaseManager<TaskMgr>
{
    //第一个主线任务的名字
    public static string firstMainTreadTaskName = "新手引导";
    private Task currentMainTreadTask;
    //主线任务名字对应主线任务
    private Dictionary<string, Task> mainTreadTask = new Dictionary<string, Task>();
    //任务发起人的名字，对应任务
    private Dictionary<string, List<Task>> allTask = new Dictionary<string, List<Task>>();
    //所有正在进行的任务，用于访问所有正在进行的任务
    private List<Task> allProcessTasks = new List<Task>();
    //用于快速访问对应任务是否已经存在
    private HashSet<Task> allProcessTasksHashSet = new HashSet<Task>();
    //已经完成的任务的任务名字
    private HashSet<string> finishTaskDic = new HashSet<string>();
    public TaskMgr()
    {
        InitialAllTask();

        EventCenter.GetInstance().AddEventListener<int>(EventCenterEnum.升级.ToString(), LevelUp_Operate);
        EventCenter.GetInstance().AddEventListener<Warrior>(EventCenterEnum.击败.ToString(), Beat_Operate);
        EventCenter.GetInstance().AddEventListener(EventCenterEnum.对话.ToString(), Talk_Operate);
    }
    //动态加载所有任务文件，并初始化unAcceptedTaskDic
    public void InitialAllTask()
    {
        TaskInfo[] taskInfos = ResMgr.GetInstance().LoadAll<TaskInfo>("Task");
        for (int i = 0; i < taskInfos.Length; i++)
        {
            Task task = new Task(taskInfos[i]);
            AddTasks(task);
        }
    }
    public void ClearRunTimeData()
    {
        mainTreadTask.Clear();
        allTask.Clear();
        currentMainTreadTask = null;
        allProcessTasks.Clear();
        allProcessTasksHashSet.Clear();
        finishTaskDic.Clear();

        InitialAllTask();
    }
    public TaskMgrSaveInfo GetSaveInfo()
    {
        TaskMgrSaveInfo taskMgrInfo = new TaskMgrSaveInfo();
        if (this.currentMainTreadTask != null)
            taskMgrInfo.currentMainTreadTaskSaveInfo = this.currentMainTreadTask.GetTaskSaveInfo();
        taskMgrInfo.allProcessTasksSaveInfo = new List<TaskSaveInfo>();
        for (int i = 0; i < this.allProcessTasks.Count; i++)
        {
            if (this.allProcessTasks[i] != null)
                taskMgrInfo.allProcessTasksSaveInfo.Add(this.allProcessTasks[i].GetTaskSaveInfo());
        }
        taskMgrInfo.finishTaskDic = this.finishTaskDic;
        return taskMgrInfo;
    }
    
    public void LoadSaveInfo()
    {
        PlayerData playerData = PlayerMgr.Instance.GetPlayerDataMgr().GetPlayerData();
        if (playerData == null || playerData.taskMgrSaveInfo == null)
        {
            SetFirstMainTreadTask();
            return;
        }
        TaskMgrSaveInfo taskMgrInfo = playerData.taskMgrSaveInfo;
        if (taskMgrInfo == null)
            return;
        if (taskMgrInfo.currentMainTreadTaskSaveInfo != null)
        {
            currentMainTreadTask = GetTask(taskMgrInfo.currentMainTreadTaskSaveInfo.promoterName, taskMgrInfo.currentMainTreadTaskSaveInfo.taskName);
            SetMainTreadTask(currentMainTreadTask.GetTaskName());
            currentMainTreadTask.LoadTaskSaveInfo(taskMgrInfo.currentMainTreadTaskSaveInfo);
        }
        if (taskMgrInfo.allProcessTasksSaveInfo != null)
        {
            for (int i = 0; i < taskMgrInfo.allProcessTasksSaveInfo.Count; i++)
            {
                TaskSaveInfo taskSaveInfo = taskMgrInfo.allProcessTasksSaveInfo[i];
                Task task = GetTask(taskSaveInfo.promoterName, taskSaveInfo.taskName);
                if (task != null)
                {
                    AddTask(task, TaskStepState.Receive);
                    task.LoadTaskSaveInfo(taskSaveInfo);
                }
            }
        }
        finishTaskDic = taskMgrInfo.finishTaskDic;
    }
    //传入任务发起者的名字，和任务名字
    private Task GetTask(string promoterName, string taskName)
    {
        if (!allTask.ContainsKey(promoterName) || allTask[promoterName] == null)
            return null;

        for (int i = 0; i < allTask[promoterName].Count; i++)
        {
            if (allTask[promoterName][i].GetTaskName() == taskName)
                return allTask[promoterName][i];
        }
        return null;
    }
    public void SetFirstMainTreadTask()
    {
        SetMainTreadTask(firstMainTreadTaskName);
    }
    public void SetNextMainTreadTask()
    {
        SetMainTreadTask(currentMainTreadTask.GetNextMainTreadTaskName());
    }
    public void SetMainTreadTask(string targetTaskName)
    {
        if (!mainTreadTask.ContainsKey(targetTaskName))
        {
            Debug.Log("找不到主线任务，请确认任务:\"" + targetTaskName + "\"是否存在或未勾选主线标志");
            currentMainTreadTask = null;
            return;
        }
        //if (currentMainTreadTask != null)
        currentMainTreadTask = mainTreadTask[targetTaskName];
        AddTask(currentMainTreadTask, currentMainTreadTask.GetCurrentSetpState());
    }
    private void LevelUp_Operate(int level)
    {
        RefreshProcessTask();
    }
    public void Beat_Operate(Warrior bearWarrior)
    {
        for (int i = 0; i < allProcessTasks.Count; i++)
        {
            allProcessTasks[i].AddTargetBeat(bearWarrior, 1);
        }
        RefreshProcessTask();
    }
    public void Talk_Operate()
    {
        RefreshProcessTask();
    }
    public void RefreshProcessTask()
    {
        TaskPanel taskPanel = UIManager.GetInstance().GetExistPanel(TaskPanel.prefabsPath) as TaskPanel;
        if (taskPanel != null)
            taskPanel.Refresh();
    }
    private void AddTasks(Task task)
    {
        string taskPrometerName = task.GetTaskPromoterName();
        if (!allTask.ContainsKey(taskPrometerName))
            allTask[taskPrometerName] = new List<Task>();
        allTask[taskPrometerName].Add(task);

        if (task.IsMainThread())
        {
            string taskName = task.GetTaskName();
            if (mainTreadTask.ContainsKey(taskName))
                Debug.Log("任务：" + taskName + "名字重复");
            mainTreadTask[taskName] = task;
        }
    }
    //获取对应NPC应该发布的任务
    public List<Task> GetTasks(string NPC_Name)
    {
        if (allTask.ContainsKey(NPC_Name))
            return allTask[NPC_Name];

        return null;
    }
    public void ForeachAllProcessTasks(Func<Task, bool> func)
    {
        for (int i = 0; i < allProcessTasks.Count; i++)
        {
            if (func.Invoke(allProcessTasks[i]) == true)
                break;
        }
    }
    //或者某个类型的正在进行的任务
    public List<Task> GetTasks_Type(ConditionType type)
    {
        List<Task> tasks = new List<Task>();
        for (int i = 0; i < allProcessTasks.Count; i++)
        {
            if (allProcessTasks[i].IsHaveConditionType(type))
                tasks.Add(allProcessTasks[i]);
        }
        return tasks;
    }
    public List<Task> GetProcessTask()
    {
        //优先级：满足完成条件或者领取条件的主线任务 > 满足完成条件的普通任务 > 其他情况的主线任务 > 其他情况的普通任务
        for (int i = 1; i < allProcessTasks.Count; i++)//让已经完成的任务显示在最上面
        {
            Task task = allProcessTasks[i];
            ConditionMgrState state = task.GetConditionMgrState();
            if (state.IsSatisfied() == true || task.IsMainThread())//主线任务和满足完成条件的任务上游
            {
                int j;
                for (j = i; j > 0; j--)
                {
                    ConditionMgrState previousState = allProcessTasks[j - 1].GetConditionMgrState();
                    if (previousState.IsSatisfied() == false || (task.IsMainThread() == true && (previousState.IsSatisfied() == true || task.IsAccessibility())))
                        allProcessTasks[j] = allProcessTasks[j - 1];
                    else
                        break;
                }
                allProcessTasks[j] = task;
            }

        }
        return allProcessTasks;
    }
    //public void AddReveiveTask(Task task)
    //{
    //    AddTask(task, TaskStepState.Receive);
    //}
    public void AddProcessTask(Task task)
    {
        AddTask(task, TaskStepState.Process);
    }
    private void AddTask(Task task, TaskStepState taskSetpState)
    {
        if (task == null || finishTaskDic.Contains(task.GetTaskName()))
            return;
        if (!allProcessTasksHashSet.Contains(task))//如果已经存在，就不再添加，但是要修改任务状态
        {
            allProcessTasks.Add(task);
            allProcessTasksHashSet.Add(task);
        }
        task.SetTaskState(taskSetpState);
        RefreshProcessTask();
    }
    public bool IsFinishTak(string taskName)
    {
        return finishTaskDic.Contains(taskName);
    }
    public void FinishTask(Task task)
    {
        allProcessTasks.Remove(task);
        allProcessTasksHashSet.Remove(task);
        if (task.IsMainThread() == true)
            SetNextMainTreadTask();

        finishTaskDic.Add(task.GetTaskName());
        RefreshProcessTask();
    }
}
