//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public enum TaskStepState
//{
//    Receive,//接受任务阶段
//    Process,//任务进行阶段
//    Finish,//完成任务领取奖励
//    End,//判定改阶段已经结束
//}
//[System.Serializable]
//public class TaskSaveInfo
//{
//    public string promoterName;
//    public string taskName;
//    public int index = 0;
//    public TaskStepState taskStepState;
//    public Dictionary<string, int> beatDic;
//    public HashSet<string> talkHashSet;
//}
//public class Task : IGetAble, IConditionHolderAble
//{
//    public Task(TaskInfo taskInfo)
//    {
//        this.taskInfo = taskInfo;
//        this.index = 0;
//        this.taskStepState = TaskStepState.Receive;
//    }
//    private TaskInfo taskInfo;
//    private int index = 0;//用于索引任务做到第几个阶段

//    //当前阶段的任务状态
//    private TaskStepState taskStepState;//用于检阅任务阶段的状态

//    private Dictionary<ConditionType, List<Condition>> finishConditionsDic;//完成任务的条件类型对应所有条件，不包含“且”，“或”关系
//    private Dictionary<string, int> beatDic;//打败的敌人字典
//    private HashSet<string> talkHashSet;//已完成对话的目标NPC

//    public TaskSaveInfo GetTaskSaveInfo()
//    {
//        TaskSaveInfo taskInfo = new TaskSaveInfo();
//        taskInfo.promoterName = GetTaskPromoterName();
//        taskInfo.taskName = GetTaskName();
//        taskInfo.index = index;
//        taskInfo.taskStepState = taskStepState;
//        taskInfo.beatDic = beatDic;
//        taskInfo.talkHashSet = talkHashSet;
//        return taskInfo;
//    }
//    public void LoadTaskSaveInfo(TaskSaveInfo taskSaveInfo)
//    {
//        this.index = taskSaveInfo.index;
//        this.taskStepState = taskSaveInfo.taskStepState;
//        this.beatDic = taskSaveInfo.beatDic;
//        this.talkHashSet = taskSaveInfo.talkHashSet;
//    }

//    //获取任务条件的状态
//    public ConditionMgrState GetConditionMgrState()
//    {
//        TaskStepInfo taskStep = GetCurrentTaskStepInfo();
//        if (taskStep != null)
//        {
//            ConditionMgr conditionMgr = null;

//            if (this.GetCurrentSetpState() == TaskStepState.Receive)//如果还没有领取，条件状态就是开启任务的条件
//                conditionMgr = this.taskInfo.openConditions;
//            else
//                conditionMgr = taskStep.finishConditions;

//            if (conditionMgr != null)
//            {
//                ConditionMgrState conditionMgrState = conditionMgr.GetConditionMgrState(this, PlayerMgr.Instance);
//                return conditionMgrState;
//            }
//        }
//        Debug.Log("ConditionMgr为空");
//        return null;
//    }
//    public bool IsMainThread()
//    {
//        if (taskInfo == null)
//            Debug.Log("任务缺失任务信息");
//        return taskInfo.isMainThread;
//    }
//    public string GetNextMainTreadTaskName()
//    {
//        if (this.IsMainThread() == false)
//        {
//            Debug.Log("请不要对非主线任务进行主线任务操作");
//            return "";
//        }
//        return taskInfo.nextTaskName;
//    }
//    //完成当前任务阶段
//    public void FinishTaskStep()
//    {
//        RemoveCollectItem();
//        PlayerMgr.Instance.UseBooty(GetBooty(),PlayerMgr.Instance.GetWarrior());
//        SetNextTaskStep();
//    }
//    //如果任务有收集条件，会把收集条件需要的东西都移除
//    private void RemoveCollectItem()
//    {
//        Dictionary<ConditionType, List<Condition>> allCondition = GetFinishConditions();

//        if (allCondition == null || !allCondition.ContainsKey(ConditionType.收集))
//            return;
//        HashSet<Condition> checkedConditions = new HashSet<Condition>();//已经检查过不满足收费条件的列表
//        List<Condition> mustChargeConditions = new List<Condition>();//没有可以替换的条件的收集类型条件
//        List<List<Condition>> satisfiedConditions = new List<List<Condition>>();//内层List装的是同一个"或条件下"可以相互替换，且满足提交任务的条件
//        for (int i = 0; i < allCondition[ConditionType.收集].Count; i++)//遍历本次任务所有与收集有关的条件
//        {
//            Condition currentCondition = allCondition[ConditionType.收集][i];
//            if (checkedConditions.Contains(currentCondition))//已经被检查过的
//                continue;

//            bool isFree = false;//如果队伍中有不是收集类型的，并且满足完成条件,则该"或条件"下的所有"收费"任务免费
//            if (currentCondition.conditionContainter != null && currentCondition.conditionContainter.replaceableCondition.Count > 1)
//            {
//                satisfiedConditions.Add(new List<Condition>());
//                List<Condition> teammateConditions = currentCondition.conditionContainter.replaceableCondition;
//                for (int j = 0; j < teammateConditions.Count; j++)
//                {
//                    Condition currentTeammateCondition = teammateConditions[j];

//                    if (currentTeammateCondition.conditionType == ConditionType.收集)
//                    {
//                        if (!checkedConditions.Contains(currentTeammateCondition))
//                            checkedConditions.Add(currentTeammateCondition);//已经检查过的不用再查
//                    }
//                    //满足完成条件
//                    if (currentTeammateCondition.Check(this, PlayerMgr.Instance) == true)
//                    {
//                        if (currentTeammateCondition.conditionType == ConditionType.收集)
//                            satisfiedConditions[i].Add(currentTeammateCondition);
//                        else//队友不是收集类型
//                            isFree = true;//这里不要break，如果不把同一组下的收集类型条件全部添加到已检查，之后又会再遍历一次,一直重复
//                    }
//                }
//                if (isFree == true)//如果满足免费条件，那么这一组就不需要付费
//                    satisfiedConditions.RemoveAt(i);
//            }
//            else
//            {
//                mustChargeConditions.Add(currentCondition);
//            }
//        }
//        ChargingMgr(mustChargeConditions, satisfiedConditions);
//    }

//    private void ChargingMgr(List<Condition> mustChargeConditions, List<List<Condition>> satisfiedConditions)
//    {
//        if (mustChargeConditions != null)
//        {
//            for (int i = 0; i < mustChargeConditions.Count; i++)
//                Charging(mustChargeConditions[i]);
//        }
//        if (satisfiedConditions != null)
//        {
//            for (int i = 0; i < satisfiedConditions.Count; i++)
//            {
//                List<Condition> replaceAbleList = satisfiedConditions[i];
//                Debug.Log(replaceAbleList.Count);
//                Charging(replaceAbleList[0]);
//                //Charging(replaceAbleList[replaceAbleList.Count - 1]);
//            }
//        }
//    }

//    private void Charging(Condition targetCondition)
//    {
//        //扣费
//        Condition_Collect condition_Collect = targetCondition.condition_Collection;//获取收集条件
//        int needCount = condition_Collect.count;//获取目标收集量
//        if (condition_Collect != null)
//        {
//            DesignItems<Item> designItems = condition_Collect.GetDesignItems();//获取收集信息
//            if (designItems == null)
//                return;
//            for (int i = 0; i < designItems.itemList.Count; i++)//访问所有收集信息
//            {
//                Item item = designItems.itemList[i];
//                if (needCount < item.GetCount())
//                {
//                    item.holder.RemoveItem(item, needCount);
//                    needCount -= needCount;
//                }
//                else
//                {
//                    item.holder.RemoveItem(item, item.GetCount());
//                    needCount -= item.GetCount();
//                }
//                if (needCount <= 0)
//                    break;
//            }
//        }
//    }
//    //切换任务阶段
//    private void SetNextTaskStep()
//    {
//        index++;

//        if (index >= taskInfo.taskSteps.Count)
//        {
//            SetTaskState(TaskStepState.End);//暂时没用
//            TaskMgr.GetInstance().FinishTask(this);
//            return;
//        }
//        if (beatDic != null)
//            beatDic.Clear();//进入下一阶段，已经击败过的怪物不算了
//        if (talkHashSet != null)
//            talkHashSet.Clear();//进入下一阶段，已经对话过的NPC也不算了

//        SetTaskState(TaskStepState.Process);

//        SetFinishConditions();//任务阶段改变，重新设置完成任务需要的条件
//        TaskMgr.GetInstance().RefreshProcessTask();
//    }
//    //获取任务提示
//    public string GetTaskTip(ConditionMgrState conditionMgrState)
//    {
//        string tip = "";
//        if (conditionMgrState != null)
//            tip = conditionMgrState.GetTip(this, PlayerMgr.Instance);

//        return tip;
//    }
//    public string GetTaskTip()
//    {
//        return GetTaskTip(GetConditionMgrState());
//    }
//    //获取当前任务阶段是否满足完成条件
//    public bool IsSatisfied(ConditionMgrState conditionMgrState)
//    {
//        if (conditionMgrState == null)
//        {
//            Debug.Log(GetTaskName() + "：conditionMgrState为空");
//            return false;
//        }
//        return conditionMgrState.IsSatisfied();
//    }
//    //获取当前任务阶段是否满足完成条件
//    public bool IsSatisfied()
//    {
//        ConditionMgrState conditionMgrState = GetConditionMgrState();
//        return IsSatisfied(conditionMgrState);
//    }
//    //刷新任务状态
//    public void RefreshTaskStepState(ConditionMgrState conditionMgrState)
//    {
//        if (taskStepState == TaskStepState.Process || taskStepState == TaskStepState.Finish)
//        {
//            if (conditionMgrState.IsSatisfied() == true)
//                SetTaskState(TaskStepState.Finish);
//            else
//                SetTaskState(TaskStepState.Process);
//        }
//    }
//    //设置任务状态
//    public void SetTaskState(TaskStepState state)
//    {
//        taskStepState = state;
//    }
//    //获取发起者的名字
//    public string GetTaskPromoterName()
//    {
//        if (this.taskInfo == null)
//            return "";

//        return this.taskInfo.GetTaskPromoterName();
//    }
//    //获取任务的名字
//    public string GetTaskName()
//    {
//        if (this.taskInfo == null)
//            return "";
//        return this.taskInfo.taskName;
//    }
//    //获取当前任务阶段的奖励
//    public Booty GetBooty()
//    {
//        return GetCurrentTaskStepInfo().booty;
//    }
//    //获取当前任务阶段的信息
//    public TaskStepInfo GetCurrentTaskStepInfo()
//    {
//        if (this.taskInfo == null)
//            return null;
//        if (this.taskInfo.taskSteps == null)
//            return null;
//        if (index >= this.taskInfo.taskSteps.Count)
//            return null;

//        return this.taskInfo.taskSteps[index];
//    }
//    //获取当前任务阶段的状态
//    public TaskStepState GetCurrentSetpState()
//    {
//        return taskStepState;
//    }
//    //判断该任务是否包含某个类型的完成条件
//    public bool IsHaveConditionType(ConditionType type)
//    {
//        return GetFinishConditions().ContainsKey(type);
//    }
//    //设置获取完成当前阶段的任务的所有条件
//    private void SetFinishConditions()
//    {
//        TaskStepInfo currentTaskStepInfo = GetCurrentTaskStepInfo();
//        if (currentTaskStepInfo == null)
//            return;
//        finishConditionsDic = currentTaskStepInfo.finishConditions.GetAllConditionTypes();
//        return;
//    }
//    //获取完成当前阶段的任务的所有条件，不处理"且","或"关系
//    public Dictionary<ConditionType, List<Condition>> GetFinishConditions()
//    {
//        if (finishConditionsDic == null)
//            SetFinishConditions();

//        return finishConditionsDic;
//    }
//    //判断任务是否满足接取条件
//    public bool IsAccessibility()
//    {
//        ConditionMgr conditionMgr = this.taskInfo.openConditions;
//        if (conditionMgr == null)//条件为null，说明不需要条件，之间返回true
//            return true;

//        ConditionMgrState conditionMgrState = conditionMgr.GetConditionMgrState(this, PlayerMgr.Instance);
//        return conditionMgrState.IsSatisfied();
//    }

//    /// <summary>
//    /// 遍历对应类型的所有条件
//    /// </summary>
//    /// <param name="type">对应类型</param>
//    /// <param name="func">返回true时跳出循环</param>
//    public void ForeachAllConditions_Type(ConditionType type, Func<Condition, bool> func)
//    {
//        Dictionary<ConditionType, List<Condition>> dic = GetFinishConditions();
//        if (dic == null)
//            return;
//        if (!dic.ContainsKey(type))
//            return;

//        List<Condition> conditions = dic[type];
//        for (int i = 0; i < conditions.Count; i++)
//        {
//            if (func.Invoke(conditions[i]) == true)//返回true时跳出循环
//                break;
//        }
//    }

//    /// <summary>
//    /// 添加击败的对手
//    /// </summary>
//    /// <param name="warrior">传入warrior是以后有可能要区分是否是精英怪</param>
//    /// <param name="count"></param>
//    public void AddTargetBeat(Warrior warrior, int count = 1)
//    {
//        string targetName = warrior.GetName();
//        ForeachAllConditions_Type(ConditionType.击杀, (conditions) =>
//        {
//            if (conditions.condition_Beat != null && conditions.condition_Beat.targetName == targetName)
//            {
//                if (beatDic == null)
//                    beatDic = new Dictionary<string, int>();
//                if (!beatDic.ContainsKey(targetName))
//                    beatDic[targetName] = 0;

//                beatDic[targetName] += count;
//                return true;//跳出循环
//            }
//            return false;
//        });
//    }

//    //作为IConditionHolderAble被Condition访问，以获得目标敌人的击败数
//    public int GetTargetBeatCount(string targetName)
//    {
//        if (beatDic == null)
//            return 0;
//        if (!beatDic.ContainsKey(targetName))
//            return 0;

//        return beatDic[targetName];
//    }
//    //获取任务中所有需要对话的人
//    public HashSet<string> GetTargetTalkNPCs()
//    {
//        HashSet<string> targetTalkNPCs = new HashSet<string>();
//        ForeachAllConditions_Type(ConditionType.对话, (condition) =>
//        {
//            if (condition.condition_Talk != null || condition.condition_Talk.talkInfo_Before != null)
//            {
//                if (condition.condition_Talk.talkInfo_Before.NPC_Name == null || condition.condition_Talk.talkInfo_Before.NPC_Name == "")
//                    Debug.Log(this.GetTaskName() + "：对话任务NPC名字为空，请检查");

//                targetTalkNPCs.Add(condition.condition_Talk.talkInfo_Before.NPC_Name);
//            }
//            return false;
//        });
//        return targetTalkNPCs;
//    }
//    //获取与任务目标对话时的对话信息
//    public TalkInfo GetTalkInfo_Task_Before(string npcName)//获取对话任务的对话信息
//    {
//        TalkInfo targetTalkInfo = null;
//        ForeachAllConditions_Type(ConditionType.对话, (condition) =>
//         {
//             if (condition.condition_Talk != null || condition.condition_Talk.talkInfo_Before != null)
//             {
//                 if (condition.condition_Talk.talkInfo_Before.NPC_Name == null || condition.condition_Talk.talkInfo_Before.NPC_Name == "")
//                     Debug.Log(this.GetTaskName() + "：对话任务NPC名字为空，请检查");

//                 if (npcName == condition.condition_Talk.talkInfo_Before.NPC_Name)
//                     targetTalkInfo = condition.condition_Talk.talkInfo_Before.talkInfo;
//             }
//             return false;
//         });
//        return targetTalkInfo;
//    }
//    //获取对话任务完成（还没有领奖）后，与任务目标对话时的对话信息
//    public TalkInfo GetTalkInfo_Task_After(string npcName)//获取对话任务的对话信息
//    {
//        TalkInfo targetTalkInfo = null;
//        ForeachAllConditions_Type(ConditionType.对话, (condition) =>
//        {
//            if (condition.condition_Talk != null || condition.condition_Talk.talkInfo_After != null)
//            {
//                if (condition.condition_Talk.talkInfo_After.NPC_Name == null || condition.condition_Talk.talkInfo_After.NPC_Name == "")
//                    Debug.Log(this.GetTaskName() + "：对话任务NPC名字为空，请检查");

//                if (npcName == condition.condition_Talk.talkInfo_After.NPC_Name)
//                    targetTalkInfo = condition.condition_Talk.talkInfo_After.talkInfo;
//            }
//            return false;
//        });
//        return targetTalkInfo;
//    }

//    //添加完成对话的目标
//    public void AddTalk_Task(ISpeakAble speakAble)
//    {
//        if (talkHashSet == null)
//            talkHashSet = new HashSet<string>();

//        if (!talkHashSet.Contains(speakAble.GetSpeakerName()))
//            talkHashSet.Add(speakAble.GetSpeakerName());
//    }
//    //获取目标人物是否已经完成对话
//    public bool GetTargetTalkState(string targetName)
//    {
//        if (talkHashSet == null)
//            return false;
//        if (!talkHashSet.Contains(targetName))
//            return false;

//        return true;
//    }
//    //获取当前任务阶段的发起人
//    public string GetReceiveTalkNPC_Name()
//    {
//        return GetCurrentTaskStepInfo().receiveTalkInfo.NPC_Name;
//    }
//    //获取当前任务阶段的领奖人
//    public string GetFinishTalkNPC_Name()
//    {
//        return GetCurrentTaskStepInfo().finishTalkInfo.NPC_Name;
//    }
//    //获取当前任务阶段的发起人对话信息
//    public TalkInfo GetReveiveTalkInfo()
//    {
//        return GetCurrentTaskStepInfo().receiveTalkInfo.talkInfo;
//    }
//    //获取当前任务阶段的领奖人对话信息
//    public TalkInfo GetFinishTalkInfo()
//    {
//        return GetCurrentTaskStepInfo().finishTalkInfo.talkInfo;
//    }
//}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TaskStepState
{
    Receive,//接受任务阶段
    Process,//任务进行阶段
    Finish,//完成任务领取奖励
    End,//判定改阶段已经结束
}
[System.Serializable]
public class TaskSaveInfo
{
    public string promoterName;
    public string taskName;
    public int index = 0;
    public TaskStepState taskStepState;
    public Dictionary<string, int> beatDic;
    public HashSet<string> talkHashSet;
}
public class Task : IGetAble, IConditionHolderAble
{
    public Task(TaskInfo taskInfo)
    {
        this.taskInfo = taskInfo;
        this.index = 0;
        if (taskInfo.taskSteps.Count <= 0)
        {
            Debug.Log("任务" + taskInfo.name + ":没有任何一个任务阶段");
            this.taskStepState = TaskStepState.Receive;
            return;
        }
        this.taskStepState = taskInfo.taskSteps[0].startTaskState;
    }
    private TaskInfo taskInfo;
    private int index = 0;//用于索引任务做到第几个阶段

    //当前阶段的任务状态
    private TaskStepState taskStepState;//用于检阅任务阶段的状态

    private Dictionary<ConditionType, List<Condition>> finishConditionsDic;//完成任务的条件类型对应所有条件，不包含“且”，“或”关系
    private Dictionary<string, int> beatDic;//打败的敌人字典
    private HashSet<string> talkHashSet;//已完成对话的目标NPC

    public TaskSaveInfo GetTaskSaveInfo()
    {
        TaskSaveInfo taskInfo = new TaskSaveInfo();
        taskInfo.promoterName = GetTaskPromoterName();
        taskInfo.taskName = GetTaskName();
        taskInfo.index = index;
        taskInfo.taskStepState = taskStepState;
        taskInfo.beatDic = beatDic;
        taskInfo.talkHashSet = talkHashSet;
        return taskInfo;
    }
    public void LoadTaskSaveInfo(TaskSaveInfo taskSaveInfo)
    {
        this.index = taskSaveInfo.index;
        this.taskStepState = taskSaveInfo.taskStepState;
        this.beatDic = taskSaveInfo.beatDic;
        this.talkHashSet = taskSaveInfo.talkHashSet;
    }

    //获取任务条件的状态
    public ConditionMgrState GetConditionMgrState()
    {
        TaskStepInfo taskStep = GetCurrentTaskStepInfo();
        if (taskStep != null)
        {
            ConditionMgr conditionMgr = null;

            if (this.GetCurrentSetpState() == TaskStepState.Receive)//如果还没有领取，条件状态就是开启任务的条件
                conditionMgr = this.taskInfo.openConditions;
            else
                conditionMgr = taskStep.finishConditions;

            if (conditionMgr != null)
            {
                ConditionMgrState conditionMgrState = conditionMgr.GetConditionMgrState(this, PlayerMgr.Instance);
                return conditionMgrState;
            }
        }
        Debug.Log("ConditionMgr为空");
        return null;
    }
    public bool IsMainThread()
    {
        if (taskInfo == null)
            Debug.Log("任务缺失任务信息");
        return taskInfo.isMainThread;
    }
    public string GetNextMainTreadTaskName()
    {
        if (this.IsMainThread() == false)
        {
            Debug.Log("请不要对非主线任务进行主线任务操作");
            return "";
        }
        return taskInfo.nextTaskName;
    }
    //完成当前任务阶段
    public void FinishTaskStep()
    {
        RemoveCollectItem();
        PlayerMgr.Instance.UseBooty(GetBooty(), PlayerMgr.Instance.GetWarrior());
        SetNextTaskStep();
    }
    //如果任务有收集条件，会把收集条件需要的东西都移除
    private void RemoveCollectItem()
    {
        Dictionary<ConditionType, List<Condition>> allCondition = GetFinishConditions();

        if (allCondition == null || !allCondition.ContainsKey(ConditionType.收集))
            return;
        HashSet<Condition> checkedConditions = new HashSet<Condition>();//已经检查过不满足收费条件的列表
        List<Condition> mustChargeConditions = new List<Condition>();//没有可以替换的条件的收集类型条件
        List<List<Condition>> satisfiedConditions = new List<List<Condition>>();//内层List装的是同一个"或条件下"可以相互替换，且满足提交任务的条件
        for (int i = 0; i < allCondition[ConditionType.收集].Count; i++)//遍历本次任务所有与收集有关的条件
        {
            Condition currentCondition = allCondition[ConditionType.收集][i];
            if (checkedConditions.Contains(currentCondition))//已经被检查过的
                continue;

            bool isFree = false;//如果队伍中有不是收集类型的，并且满足完成条件,则该"或条件"下的所有"收费"任务免费
            if (currentCondition.conditionContainter != null && currentCondition.conditionContainter.replaceableCondition.Count > 1)
            {
                satisfiedConditions.Add(new List<Condition>());
                List<Condition> teammateConditions = currentCondition.conditionContainter.replaceableCondition;
                for (int j = 0; j < teammateConditions.Count; j++)
                {
                    Condition currentTeammateCondition = teammateConditions[j];

                    if (currentTeammateCondition.conditionType == ConditionType.收集)
                    {
                        if (!checkedConditions.Contains(currentTeammateCondition))
                            checkedConditions.Add(currentTeammateCondition);//已经检查过的不用再查
                    }
                    //满足完成条件
                    if (currentTeammateCondition.Check(this, PlayerMgr.Instance) == true)
                    {
                        if (currentTeammateCondition.conditionType == ConditionType.收集)
                            satisfiedConditions[i].Add(currentTeammateCondition);
                        else//队友不是收集类型
                            isFree = true;//这里不要break，如果不把同一组下的收集类型条件全部添加到已检查，之后又会再遍历一次,一直重复
                    }
                }
                if (isFree == true)//如果满足免费条件，那么这一组就不需要付费
                    satisfiedConditions.RemoveAt(i);
            }
            else
            {
                mustChargeConditions.Add(currentCondition);
            }
        }
        ChargingMgr(mustChargeConditions, satisfiedConditions);
    }

    private void ChargingMgr(List<Condition> mustChargeConditions, List<List<Condition>> satisfiedConditions)
    {
        if (mustChargeConditions != null)
        {
            for (int i = 0; i < mustChargeConditions.Count; i++)
                Charging(mustChargeConditions[i]);
        }
        if (satisfiedConditions != null)
        {
            for (int i = 0; i < satisfiedConditions.Count; i++)
            {
                List<Condition> replaceAbleList = satisfiedConditions[i];
                Debug.Log(replaceAbleList.Count);
                Charging(replaceAbleList[0]);
                //Charging(replaceAbleList[replaceAbleList.Count - 1]);
            }
        }
    }

    private void Charging(Condition targetCondition)
    {
        //扣费
        Condition_Collect condition_Collect = targetCondition.condition_Collection;//获取收集条件
        int needCount = condition_Collect.count;//获取目标收集量
        if (condition_Collect != null)
        {
            DesignItems<Item> designItems = condition_Collect.GetDesignItems();//获取收集信息
            if (designItems == null)
                return;
            for (int i = 0; i < designItems.itemList.Count; i++)//访问所有收集信息
            {
                Item item = designItems.itemList[i];
                if (needCount < item.GetCount())
                {
                    item.holder.RemoveItem(item, needCount);
                    needCount -= needCount;
                }
                else
                {
                    item.holder.RemoveItem(item, item.GetCount());
                    needCount -= item.GetCount();
                }
                if (needCount <= 0)
                    break;
            }
        }
    }
    //切换任务阶段
    private void SetNextTaskStep()
    {
        index++;

        if (index >= taskInfo.taskSteps.Count)
        {
            SetTaskState(TaskStepState.End);//暂时没用
            TaskMgr.GetInstance().FinishTask(this);
            return;
        }
        if (beatDic != null)
            beatDic.Clear();//进入下一阶段，已经击败过的怪物不算了
        if (talkHashSet != null)
            talkHashSet.Clear();//进入下一阶段，已经对话过的NPC也不算了

        //SetTaskState(TaskStepState.Process);
        SetTaskState(GetCurrentTaskStepInfo().startTaskState);
        SetFinishConditions();//任务阶段改变，重新设置完成任务需要的条件
        TaskMgr.GetInstance().RefreshProcessTask();
    }
    //获取任务提示
    public string GetTaskTip(ConditionMgrState conditionMgrState)
    {
        string tip = "";
        if (conditionMgrState != null)
            tip = conditionMgrState.GetTip(this, PlayerMgr.Instance);

        return tip;
    }
    public string GetTaskTip()
    {
        return GetTaskTip(GetConditionMgrState());
    }
    //获取当前任务阶段是否满足完成条件
    public bool IsSatisfied(ConditionMgrState conditionMgrState)
    {
        if (conditionMgrState == null)
        {
            Debug.Log(GetTaskName() + "：conditionMgrState为空");
            return false;
        }
        return conditionMgrState.IsSatisfied();
    }
    //获取当前任务阶段是否满足完成条件
    public bool IsSatisfied()
    {
        ConditionMgrState conditionMgrState = GetConditionMgrState();
        return IsSatisfied(conditionMgrState);
    }
    //刷新任务状态
    public void RefreshTaskStepState(ConditionMgrState conditionMgrState)
    {
        if (taskStepState == TaskStepState.Process || taskStepState == TaskStepState.Finish)
        {
            if (conditionMgrState.IsSatisfied() == true)
                SetTaskState(TaskStepState.Finish);
            else
                SetTaskState(TaskStepState.Process);
        }
    }
    //设置任务状态
    public void SetTaskState(TaskStepState state)
    {
        taskStepState = state;
    }
    //获取发起者的名字
    public string GetTaskPromoterName()
    {
        if (this.taskInfo == null)
            return "";

        return this.taskInfo.GetTaskPromoterName();
    }
    //获取任务的名字
    public string GetTaskName()
    {
        if (this.taskInfo == null)
            return "";
        return this.taskInfo.taskName;
    }
    //获取当前任务阶段的奖励
    public Booty GetBooty()
    {
        return GetCurrentTaskStepInfo().booty;
    }
    //获取当前任务阶段的信息
    public TaskStepInfo GetCurrentTaskStepInfo()
    {
        if (this.taskInfo == null)
            return null;
        if (this.taskInfo.taskSteps == null)
            return null;
        if (index >= this.taskInfo.taskSteps.Count)
            return null;

        return this.taskInfo.taskSteps[index];
    }
    //获取当前任务阶段的状态
    public TaskStepState GetCurrentSetpState()
    {
        return taskStepState;
    }
    //判断该任务是否包含某个类型的完成条件
    public bool IsHaveConditionType(ConditionType type)
    {
        return GetFinishConditions().ContainsKey(type);
    }
    //设置获取完成当前阶段的任务的所有条件
    private void SetFinishConditions()
    {
        TaskStepInfo currentTaskStepInfo = GetCurrentTaskStepInfo();
        if (currentTaskStepInfo == null)
            return;
        finishConditionsDic = currentTaskStepInfo.finishConditions.GetAllConditionTypes();
        return;
    }
    //获取完成当前阶段的任务的所有条件，不处理"且","或"关系
    public Dictionary<ConditionType, List<Condition>> GetFinishConditions()
    {
        if (finishConditionsDic == null)
            SetFinishConditions();

        return finishConditionsDic;
    }
    //判断任务是否满足接取条件
    public bool IsAccessibility()
    {
        ConditionMgr conditionMgr = this.taskInfo.openConditions;
        if (conditionMgr == null)//条件为null，说明不需要条件，之间返回true
            return true;

        ConditionMgrState conditionMgrState = conditionMgr.GetConditionMgrState(this, PlayerMgr.Instance);
        return conditionMgrState.IsSatisfied();
    }

    /// <summary>
    /// 遍历对应类型的所有条件
    /// </summary>
    /// <param name="type">对应类型</param>
    /// <param name="func">返回true时跳出循环</param>
    public void ForeachAllConditions_Type(ConditionType type, Func<Condition, bool> func)
    {
        Dictionary<ConditionType, List<Condition>> dic = GetFinishConditions();
        if (dic == null)
            return;
        if (!dic.ContainsKey(type))
            return;

        List<Condition> conditions = dic[type];
        for (int i = 0; i < conditions.Count; i++)
        {
            if (func.Invoke(conditions[i]) == true)//返回true时跳出循环
                break;
        }
    }

    /// <summary>
    /// 添加击败的对手
    /// </summary>
    /// <param name="warrior">传入warrior是以后有可能要区分是否是精英怪</param>
    /// <param name="count"></param>
    public void AddTargetBeat(Warrior warrior, int count = 1)
    {
        string targetName = warrior.GetName();
        ForeachAllConditions_Type(ConditionType.击杀, (conditions) =>
        {
            if (conditions.condition_Beat != null && conditions.condition_Beat.targetName == targetName)
            {
                if (beatDic == null)
                    beatDic = new Dictionary<string, int>();
                if (!beatDic.ContainsKey(targetName))
                    beatDic[targetName] = 0;

                beatDic[targetName] += count;
                return true;//跳出循环
            }
            return false;
        });
    }

    //作为IConditionHolderAble被Condition访问，以获得目标敌人的击败数
    public int GetTargetBeatCount(string targetName)
    {
        if (beatDic == null)
            return 0;
        if (!beatDic.ContainsKey(targetName))
            return 0;

        return beatDic[targetName];
    }
    //获取任务中所有需要对话的人
    public HashSet<string> GetTargetTalkNPCs()
    {
        HashSet<string> targetTalkNPCs = new HashSet<string>();
        ForeachAllConditions_Type(ConditionType.对话, (condition) =>
        {
            if (condition.condition_Talk != null || condition.condition_Talk.talkInfo_Before != null)
            {
                if (condition.condition_Talk.talkInfo_Before.NPC_Name == null || condition.condition_Talk.talkInfo_Before.NPC_Name == "")
                    Debug.Log(this.GetTaskName() + "：对话任务NPC名字为空，请检查");

                targetTalkNPCs.Add(condition.condition_Talk.talkInfo_Before.NPC_Name);
            }
            return false;
        });
        return targetTalkNPCs;
    }
    //获取与任务目标对话时的对话信息
    public TalkInfo GetTalkInfo_Task_Before(string npcName)//获取对话任务的对话信息
    {
        TalkInfo targetTalkInfo = null;
        ForeachAllConditions_Type(ConditionType.对话, (condition) =>
        {
            if (condition.condition_Talk != null || condition.condition_Talk.talkInfo_Before != null)
            {
                if (condition.condition_Talk.talkInfo_Before.NPC_Name == null || condition.condition_Talk.talkInfo_Before.NPC_Name == "")
                    Debug.Log(this.GetTaskName() + "：对话任务NPC名字为空，请检查");

                if (npcName == condition.condition_Talk.talkInfo_Before.NPC_Name)
                    targetTalkInfo = condition.condition_Talk.talkInfo_Before.talkInfo;
            }
            return false;
        });
        return targetTalkInfo;
    }
    //获取对话任务完成（还没有领奖）后，与任务目标对话时的对话信息
    public TalkInfo GetTalkInfo_Task_After(string npcName)//获取对话任务的对话信息
    {
        TalkInfo targetTalkInfo = null;
        ForeachAllConditions_Type(ConditionType.对话, (condition) =>
        {
            if (condition.condition_Talk != null || condition.condition_Talk.talkInfo_After != null)
            {
                if (condition.condition_Talk.talkInfo_After.NPC_Name == null || condition.condition_Talk.talkInfo_After.NPC_Name == "")
                    Debug.Log(this.GetTaskName() + "：对话任务NPC名字为空，请检查");

                if (npcName == condition.condition_Talk.talkInfo_After.NPC_Name)
                    targetTalkInfo = condition.condition_Talk.talkInfo_After.talkInfo;
            }
            return false;
        });
        return targetTalkInfo;
    }

    //添加完成对话的目标
    public void AddTalk_Task(ISpeakAble speakAble)
    {
        if (talkHashSet == null)
            talkHashSet = new HashSet<string>();

        if (!talkHashSet.Contains(speakAble.GetSpeakerName()))
            talkHashSet.Add(speakAble.GetSpeakerName());
    }
    //获取目标人物是否已经完成对话
    public bool GetTargetTalkState(string targetName)
    {
        if (talkHashSet == null)
            return false;
        if (!talkHashSet.Contains(targetName))
            return false;

        return true;
    }
    //获取当前任务阶段的发起人
    public string GetReceiveTalkNPC_Name()
    {
        return GetCurrentTaskStepInfo().receiveTalkInfo.NPC_Name;
    }
    //获取当前任务阶段的领奖人
    public string GetFinishTalkNPC_Name()
    {
        return GetCurrentTaskStepInfo().finishTalkInfo.NPC_Name;
    }
    //获取当前任务阶段的发起人对话信息
    public TalkInfo GetReveiveTalkInfo()
    {
        return GetCurrentTaskStepInfo().receiveTalkInfo.talkInfo;
    }
    //获取当前任务阶段的领奖人对话信息
    public TalkInfo GetFinishTalkInfo()
    {
        return GetCurrentTaskStepInfo().finishTalkInfo.talkInfo;
    }
}
