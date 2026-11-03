using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyDictionary/Task/TaksStepInfo", fileName = "NewTaksStepInfo")]
public class TaskStepInfo : ScriptableObject
{
    //public TalkInfo receiveTalkInfo;
    //public TalkInfo finishTalkInfo;
    public TaskStepState startTaskState;
    public NPCInfo_TalkInfo receiveTalkInfo;
    public NPCInfo_TalkInfo finishTalkInfo;

    public ConditionMgr finishConditions;
    public Booty booty;
}
[System.Serializable]
public class NPCInfo_TalkInfo
{
    public string NPC_Name;
    public string sceneName;
    [Header("如果目标场景有预设好的目标NPC，可以不管")]
    public Vector3 targetPos;

    public TalkInfo talkInfo;
}
