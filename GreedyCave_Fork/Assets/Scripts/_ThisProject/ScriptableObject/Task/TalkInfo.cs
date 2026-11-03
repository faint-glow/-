using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyDictionary/Task/TalkInfo", fileName = "NewTalkInfo")]
public class TalkInfo : ScriptableObject
{
    public List<SpeakInfo> speakInfos;//[UnityEngine.Serialization.FormerlySerializedAs("talkContent")]//修改名字不丢失索引
    public List<Choice> choices;
}

[System.Serializable]
public class SpeakInfo
{
    public string speakName;
    [TextArea(2, 5)]
    public string content;
}
