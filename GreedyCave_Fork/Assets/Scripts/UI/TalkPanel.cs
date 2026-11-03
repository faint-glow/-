using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface ISpeakAble
{
    public Texture GetDynamicAvatar();
    public TalkInfo GetBaseTalkInfo();
    public List<Choice> GetChoices();
    public string GetSpeakerName();
    public string Relationship();
}

public class TalkPanel : ContainerPanel<TalkButton, Choice>
{
    [HideInInspector] public static string prefabsPath = "TalkPanel";

    //private TalkInfo talkInfo;
    private ISpeakAble speaker;
    private TalkInfo currentTalkInfo;
    private List<Choice> currentChoices;
    private int speakInfoIndex;

    private Button breakButton;
    private RawImage speakerTexture;
    private TMP_Text speakerNameText;
    private TMP_Text relationshipText;
    private TMP_Text speakWordsText;
    private Transform choiceButtonParent;

    private bool isSpeaking = false;
    private bool isBreakSpeak = false;

    public override void ShowMeFirst()
    {
        breakButton = ThisPanel.GetUI<Button>();
        breakButton.onClick.AddListener(BreakOrNext);
        speakerTexture = GetPanel("Back/Main/Image/RawImage").GetUI<RawImage>();
        speakerNameText = GetPanel("Back/Main/Image/Info/Name").GetUI<TMP_Text>();
        relationshipText = GetPanel("Back/Main/Image/Info/Relationship").GetUI<TMP_Text>();
        speakWordsText = GetPanel("Back/Main/Scroll View/Viewport/ContentText (TMP)").GetUI<TMP_Text>();
        choiceButtonParent = GetPanel("Back/Buttons").transform;
    }
    public override IHolderAble GetHolder()
    {
        return null;
    }
    public ISpeakAble GetCurrentTalkNPC()
    {
        return this.speaker;
    }
    public void SetSpeak(ISpeakAble speaker)
    {
        if (speaker == null)
        {
            Debug.Log("speaker为空");
            return;
        }
        if (speaker.GetBaseTalkInfo() == null)
        {
            Debug.Log("talkInfo为空");
            return;
        }

        this.RefreshContainer(null);
        this.speaker = speaker;

        this.speakInfoIndex = 0;
        this.currentTalkInfo = this.speaker.GetBaseTalkInfo();
        this.currentChoices = this.speaker.GetChoices();
        this.SpeakHandler();
    }

    public void ContinueSpeak(TalkInfo talkInfo)
    {
        if (talkInfo == null)
        {
            Debug.Log("talkInfo为空");
            return;
        }

        this.RefreshContainer(null);
        this.speakInfoIndex = 0;
        this.currentTalkInfo = talkInfo;
        this.currentChoices = currentTalkInfo.choices;
        this.SpeakHandler();
    }

    private void BreakOrNext()
    {
        if (this.currentTalkInfo == null)
            return;
        if (this.currentTalkInfo.speakInfos == null)
            return;

        if (isSpeaking == true)
        {
            isBreakSpeak = true;
        }
        else
        {
            if (this.speakInfoIndex >= currentTalkInfo.speakInfos.Count - 1)
                return;

            this.speakInfoIndex++;
            this.SpeakHandler();
        }
    }

    public static ISpeakAble GetCurrentSpeaker(SpeakInfo speakInfo)
    {
        ISpeakAble currentSpeaker;
        if (speakInfo.speakName == "Player")
            currentSpeaker = PlayerMgr.Instance;
        else
            currentSpeaker = NPC.GetNPC(speakInfo.speakName);

        return currentSpeaker;
    }
    private void SpeakHandler()
    {
        if (currentTalkInfo.speakInfos == null || currentTalkInfo.speakInfos.Count <= 0)
        {
            Debug.Log("SpeakInfo设置不正确");
            return;
        }

        List<SpeakInfo> speakInfos = currentTalkInfo.speakInfos;
        SpeakInfo speakInfo = speakInfos[this.speakInfoIndex];
        ISpeakAble currentSpeaker = TalkPanel.GetCurrentSpeaker(speakInfo);
        if (currentSpeaker == null)
            currentSpeaker = this.speaker;

        this.speakerTexture.texture = currentSpeaker.GetDynamicAvatar();
        this.speakerNameText.text = currentSpeaker.GetSpeakerName();
        //this.relationshipText.text = speaker.Relationship();

        Speak(speakInfo.content, speakInfos.Count);

    }

    private void Speak(string content, int allContentCount)
    {
        StartCoroutine(DoSpeak(content, currentChoices, allContentCount));
    }

    IEnumerator DoSpeak(string content, List<Choice> choices, int allSpeakCount, float intervalTime = 0.05f)
    {
        speakWordsText.text = "";
        isSpeaking = true;
        for (int i = 0; i < content.Length; i++)
        {
            speakWordsText.text += content[i];
            float time = intervalTime;
            yield return new WaitUntil(() =>
            {
                time -= Time.deltaTime;

                if (time <= 0 || isBreakSpeak == true)
                    return true;
                return false;
            });
            if (isBreakSpeak == true)
            {
                speakWordsText.text = content;
                isBreakSpeak = false;
                break;
            }
        }
        this.isSpeaking = false;
        if (this.speakInfoIndex >= allSpeakCount - 1)
            RefreshContainer(choices);
    }

    public override ScrollRect GetScrollRect()
    {
        return null;
    }
    public override string GetCellPath()
    {
        return TalkButton.prefabsPath;
    }

    public override Transform GetContainterTrans()
    {
        return choiceButtonParent;
    }


}
