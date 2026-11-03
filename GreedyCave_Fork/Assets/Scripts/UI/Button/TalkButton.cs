using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TalkButton : MapPanel, ISetAble<IGetAble>
{
    public static string prefabsPath = "Button/TalkButton";
    public MapPanel FatherPanel { get; set; }
    public int Index { get; set; }
    private Button button;
    private TMP_Text buttonNameText;

    public override void ShowMeFirst()
    {
        button = ThisPanel.GetUI<Button>();
        buttonNameText = GetPanel("ButtonName").GetUI<TMP_Text>();
    }

    public void SetItem_Container(IGetAble obj)
    {
        Choice choice = obj as Choice;
        if (choice == null)
            return;
        buttonNameText.text = choice.choiceName;
        button.onClick.AddListener(() =>
        {
            ChoiceTrigger(choice);
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
        });
    }

    public void ChoiceTrigger(Choice choice)
    {
        TalkPanel talkPanel = FatherPanel as TalkPanel;
        ISpeakAble currentTalkNPC = talkPanel.GetCurrentTalkNPC();
        Task task = choice.task;

        if (choice.choiceType == ChoiceType.离开)
            UIManager.GetInstance().HidePanel(TalkPanel.prefabsPath);
        else if (choice.choiceType == ChoiceType.任务处理)
        {
            this.TaskOperateMgr(choice, talkPanel);
        }//以任务名字为名的按钮，点击后根据任务状态选择对话
        else if (choice.choiceType == ChoiceType.领取任务)//领取任务的实际执行
        {
            TaskMgr.GetInstance().AddProcessTask(choice.task);
            UIManager.GetInstance().HidePanel(TalkPanel.prefabsPath);
        }
        else if (choice.choiceType == ChoiceType.完成任务)//领取奖励的实际执行
        {
            task.FinishTaskStep();
            UIManager.GetInstance().HidePanel(TalkPanel.prefabsPath);
        }
        else if (choice.choiceType == ChoiceType.完成对话)//完成对话任务中的对话，这种对话，是需要回去领奖的对话
        {
            task.AddTalk_Task(currentTalkNPC);
            EventCenter.GetInstance().EventTrigger(EventCenterEnum.对话.ToString());
            UIManager.GetInstance().HidePanel(TalkPanel.prefabsPath);
        }
        else if(choice.choiceType == ChoiceType.下一阶段)//这里内部效果与完成任务的效果完全一致，但是为了方便区分，就增加一个枚举，便于理解
        {
            task.FinishTaskStep();
            UIManager.GetInstance().HidePanel(TalkPanel.prefabsPath);
        }
        else if(choice.choiceType == ChoiceType.执行方法)
        {
            if (choice.action != null)
                choice.action.Invoke();
            else
                Debug.Log("方法为空");
        }
    }

    public void TaskOperateMgr(Choice choice, TalkPanel talkPanel)
    {
        ISpeakAble currentTalkNPC = talkPanel.GetCurrentTalkNPC();
        Task task = choice.task;
        TalkInfo talk = null;
        if (task.GetCurrentSetpState() == TaskStepState.Receive)//正在接收任务的阶段
        {
            talk = ReceiveTaskOperiate(task, currentTalkNPC);
        }
        else if (task.GetCurrentSetpState() == TaskStepState.Process)//正在执行任务的阶段
        {
            talk = ProcessTaskOperiate(task, currentTalkNPC);
        }
        else if (task.GetCurrentSetpState() == TaskStepState.Finish)//如果任务是完成状态，也就是可以领取奖励的阶段
        {
            talk = FinishTaskOperiate(task, currentTalkNPC);
        }
        else if (task.GetCurrentSetpState() == TaskStepState.End)
        {
            Debug.Log("任务已完成，如果你看见这个，说明触发bug");
            return;
        }
        if (talk == null)
        {
            Debug.Log("任务:" + "\""+ task.GetTaskName() + "\"" + "缺失任务阶段" +"\"" + task.GetCurrentSetpState() + "\"" + "的对话信息");
            UIManager.GetInstance().HidePanel(TalkPanel.prefabsPath);
        }
        else
            talkPanel.ContinueSpeak(talk);
    }

    //如果一个角色同时有多个身份，比如领取奖励阶段，既是任务发起人，也是任务领奖人，则优先级为
    //任务领奖人 > 任务目标 > 任务阶段发起者 > 任务总发起者
    private TalkInfo TaskOperiate
        (
        Task task, ISpeakAble currentTalkNPC,
        Func<Task, ISpeakAble, TalkInfo> promoterNPC_Operiate,
        Func<Task, ISpeakAble, TalkInfo> receiveNPC_Operiate,
        Func<Task, ISpeakAble, TalkInfo> talkTargetNPC_Operiate,
        Func<Task, ISpeakAble, TalkInfo> finishNPC_Operiate
        )
    {
        TalkInfo talkInfo = null;

        //注意这里的顺序是决定一个NPC同时拥有多个身份时优先处理哪个身份的任务效果

        //自己作为阶段性任务的结束人的处理
        if (finishNPC_Operiate != null && task.GetFinishTalkNPC_Name() == currentTalkNPC.GetSpeakerName())
            talkInfo = finishNPC_Operiate.Invoke(task, currentTalkNPC);
        //自己作为对话任务的对话目标的处理
        else if (talkTargetNPC_Operiate != null && task.IsHaveConditionType(ConditionType.对话) && task.GetTargetTalkNPCs().Contains(currentTalkNPC.GetSpeakerName()))
            talkInfo = talkTargetNPC_Operiate.Invoke(task, currentTalkNPC);
        //自己作为阶段性任务发起人的处理(总发起人是第一阶段的发起人)
        else if (receiveNPC_Operiate != null && task.GetReceiveTalkNPC_Name() == currentTalkNPC.GetSpeakerName())
            talkInfo = receiveNPC_Operiate.Invoke(task, currentTalkNPC);
        //自己作为总发起人的处理
        else if (promoterNPC_Operiate != null && task.GetTaskPromoterName() == currentTalkNPC.GetSpeakerName())
            talkInfo = promoterNPC_Operiate.Invoke(task, currentTalkNPC);

        if (talkInfo != null)
        {
            for (int i = 0; i < talkInfo.choices.Count; i++)
                talkInfo.choices[i].task = task;
        }
        return talkInfo;
    }

    #region 模板
    //private TalkInfo ReceiveTaskOperiate(Task task, ISpeakAble currentTalkNPC)
    //{
    //    Func<Task, ISpeakAble, TalkInfo> promoterNPC_Operiate = null;
    //    Func<Task, ISpeakAble, TalkInfo> receiveNPC_Operiate = null;
    //    Func<Task, ISpeakAble, TalkInfo> talkTargetNPC_Operiate = null;
    //    Func<Task, ISpeakAble, TalkInfo> finishNPC_Operiate = null;

    //    //处理
    //    receiveNPC_Operiate = (task, currentTalkNPC) =>
    //    {
    //        TalkInfo talkInfo = null;

    //        return talkInfo;
    //    };

    //    //结束处理

    //    return TaskOperiate(task, currentTalkNPC, promoterNPC_Operiate, receiveNPC_Operiate, talkTargetNPC_Operiate, finishNPC_Operiate);
    //}
    #endregion

    //任务的第一阶段发起人，则是总发起人，第二阶段及以后的阶段，任务接取人都是上一阶段的领奖人，且此时直接从任务已经接取开始进行
    //任务在还没有领取时的情况，此时只有总发起人和阶段发起人可能拥有该任务的对话选项
    private TalkInfo ReceiveTaskOperiate(Task task, ISpeakAble currentTalkNPC)
    {
        Func<Task, ISpeakAble, TalkInfo> promoterNPC_Operiate = null;//与任务的总发起人对话
        Func<Task, ISpeakAble, TalkInfo> receiveNPC_Operiate = null;//与任务的阶段发起人对话
        Func<Task, ISpeakAble, TalkInfo> talkTargetNPC_Operiate = null;//与任务阶段的对话目标对话
        Func<Task, ISpeakAble, TalkInfo> finishNPC_Operiate = null;//与任务阶段的领奖人对话
        //处理
        promoterNPC_Operiate = (task, currentTalkNPC) =>
        {
            TalkInfo talkInfo = null;
            talkInfo = task.GetReveiveTalkInfo();
            return talkInfo;
        };//接取任务时，两个都应该是一样的
        receiveNPC_Operiate = (task, currentTalkNPC) =>
        {
            TalkInfo talkInfo = null;
            talkInfo = task.GetReveiveTalkInfo();
            return talkInfo;
        };
        //结束处理
        return TaskOperiate(task, currentTalkNPC, promoterNPC_Operiate, receiveNPC_Operiate, talkTargetNPC_Operiate, finishNPC_Operiate);
    }
    //任务正在进行阶段，此时任务的总发起人，任务的阶段发起人，任务阶段的对话目标，可能拥有该任务的处理选项
    private TalkInfo ProcessTaskOperiate(Task task, ISpeakAble currentTalkNPC)
    {
        Func<Task, ISpeakAble, TalkInfo> promoterNPC_Operiate = null;//与任务的总发起人对话
        Func<Task, ISpeakAble, TalkInfo> receiveNPC_Operiate = null;//与任务的阶段发起人对话
        Func<Task, ISpeakAble, TalkInfo> talkTargetNPC_Operiate = null;//与任务阶段的对话目标对话
        Func<Task, ISpeakAble, TalkInfo> finishNPC_Operiate = null;//与任务阶段的领奖人对话
        //处理
        promoterNPC_Operiate = (task, currentTalkNPC) =>
        {
            return GetTalkInfo_NotFinish(currentTalkNPC.GetSpeakerName(), task);
        };

        receiveNPC_Operiate = (task, currentTalkNPC) =>
        {
            return GetTalkInfo_NotFinish(currentTalkNPC.GetSpeakerName(), task);
        };

        talkTargetNPC_Operiate = (task, currentTalkNPC) =>
        {
            TalkInfo talkInfo = null;
            talkInfo = task.GetTalkInfo_Task_Before(currentTalkNPC.GetSpeakerName());
            if (talkInfo == null)
                Debug.Log(task.GetTaskName() + "：对话任务缺失在对话结束前的对话信息");
            return talkInfo;
        };

        //结束处理
        return TaskOperiate(task, currentTalkNPC, promoterNPC_Operiate, receiveNPC_Operiate, talkTargetNPC_Operiate, finishNPC_Operiate);
    }

    //任务满足领取条件，还没有领取的阶段。此时任务的总发起人，阶段性任务的当前发起人，任务阶段的对话目标，任务的领奖人会出现任务处理选项
    private TalkInfo FinishTaskOperiate(Task task, ISpeakAble currentTalkNPC)
    {
        Func<Task, ISpeakAble, TalkInfo> promoterNPC_Operiate = null;//与任务的总发起人对话
        Func<Task, ISpeakAble, TalkInfo> receiveNPC_Operiate = null;//与任务的阶段发起人对话
        Func<Task, ISpeakAble, TalkInfo> talkTargetNPC_Operiate = null;//与任务阶段的对话目标对话
        Func<Task, ISpeakAble, TalkInfo> finishNPC_Operiate = null;//与任务阶段的领奖人对话
        //处理
        promoterNPC_Operiate = (task, currentTalkNPC) =>
        {
            return GetTalkInfo_Finish(currentTalkNPC.GetSpeakerName(), task);
        };
        receiveNPC_Operiate = (task, currentTalkNPC) =>
        {
            return GetTalkInfo_Finish(currentTalkNPC.GetSpeakerName(), task);
        };
        talkTargetNPC_Operiate = (task, currentTalkNPC) =>
        {
            TalkInfo talkInfo = null;
            talkInfo = task.GetTalkInfo_Task_After(currentTalkNPC.GetSpeakerName());
            if (talkInfo == null)
                Debug.Log(task.GetTaskName() + "：对话任务缺失在对话结束后的对话信息");
            return talkInfo;
        };
        finishNPC_Operiate = (task, currentTalkNPC) =>
        {
            TalkInfo talkInfo = null;
            talkInfo = task.GetFinishTalkInfo();
            return talkInfo;
        };
        //结束处理
        return TaskOperiate(task, currentTalkNPC, promoterNPC_Operiate, receiveNPC_Operiate, talkTargetNPC_Operiate, finishNPC_Operiate);
    }

    private TalkInfo GetTalk_LeaveChoice(string currentSpeakName, string content)
    {
        TalkInfo talkInfo = ScriptableObject.CreateInstance<TalkInfo>();
        talkInfo.speakInfos = new List<SpeakInfo>();
        talkInfo.choices = new List<Choice>();

        SpeakInfo speakInfo = new SpeakInfo();
        speakInfo.speakName = currentSpeakName;
        speakInfo.content = content;
        talkInfo.speakInfos.Add(speakInfo);

        Choice newChoice = new Choice();
        newChoice.SetLeave("离开");
        talkInfo.choices.Add(newChoice);
        return talkInfo;
    }

    private TalkInfo GetTalkInfo_NotFinish(string currentSpeakName, Task task)
    {
        string content = "你的任务未完成\n" + task.GetTaskTip();
        return GetTalk_LeaveChoice(currentSpeakName, content);
    }

    private TalkInfo GetTalkInfo_Finish(string currentSpeakName, Task task)
    {
        string content = "你的任务已经完成，可以与" + task.GetFinishTalkNPC_Name() + "对话领取奖励";
        return GetTalk_LeaveChoice(currentSpeakName, content);
    }
}
