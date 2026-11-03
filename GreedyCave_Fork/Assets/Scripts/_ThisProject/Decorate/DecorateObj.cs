using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DecorateObj : MonoBehaviour, IPresetObjAble,ISpeakAble
{
    public IDecorateAble decorateAble;
    protected BoxCollider2D coll;
    protected SpriteRenderer sr;

    protected TalkInfo baseTalkInfo;
    protected IPlayerAble currentPlayer;
    private void Awake()
    {
        TryGetComponent<BoxCollider2D>(out coll);
        if (coll == null)
            coll = gameObject.AddComponent<BoxCollider2D>();
        coll.isTrigger = true;

        TryGetComponent<SpriteRenderer>(out sr);
        if (sr == null)
            sr = gameObject.AddComponent<SpriteRenderer>();

        sr.sortingOrder = 5;
    }

    public BoxCollider2D GetBoxCollider2D()
    {
        return coll;
    }

    public void SetDecorate(IDecorateAble IDecorateAble,float alpha = 0.6f)
    {
        this.decorateAble = IDecorateAble;
        this.coll.size = IDecorateAble.GetSize() - new Vector2(0.2f, 0.2f);
        this.sr.sprite = IDecorateAble.GetSprite();
        this.sr.color = new Color(1, 1, 1, alpha);

        this.SetBaseTalkInfo();
    }
    protected virtual void SetBaseTalkInfo()
    {
        baseTalkInfo = ScriptableObject.CreateInstance<TalkInfo>();
        baseTalkInfo.speakInfos = new List<SpeakInfo>();
        baseTalkInfo.choices = new List<Choice>();

        SpeakInfo speakInfo = new SpeakInfo();
        speakInfo.speakName = decorateAble.GetName();
        speakInfo.content = "你想对她做什么...";
        Choice choice_1 = new Choice();
        Choice choice_2 = new Choice();
        choice_1.SetAction("返回", Quit);
        choice_2.SetAction("回收", Recycle);

        baseTalkInfo.speakInfos.Add(speakInfo);
        baseTalkInfo.choices.Add(choice_1);
        baseTalkInfo.choices.Add(choice_2);
    }
    private void Quit()
    {
        UIManager.GetInstance().HidePanel(TalkPanel.prefabsPath);
    }
    private void Recycle()
    {
        if (decorateAble == null)
            Debug.Log("不对劲");
        if (decorateAble.IsCanRecycle(out string tip) == false)
            UIManager.GetInstance().ShowTips(tip);
        else
        {
            if (currentPlayer.RecycleDecorate(this) == false)
                return;
            SceneController.Instance.SetAstarEventGrid((int)transform.position.x, (int)transform.position.y, E_Node_Type.Walk);
            UIManager.GetInstance().HidePanel(TalkPanel.prefabsPath);
            Destroy(gameObject);
        }
    }

    public IDecorateAble Confirm()
    {
        decorateAble = decorateAble.GetHolder().RemoveItem(decorateAble as Item,1) as IDecorateAble;
        this.sr.color = new Color(1, 1, 1, 1f);
        return decorateAble;
    }

    public virtual bool PresetEventTrigger(IPlayerAble player)
    {
        currentPlayer = player;
        UIManager.GetInstance().ShowPanel<TalkPanel>(TalkPanel.prefabsPath, PanelLayer.Top_Down, null, (TalkPanel panel) =>
        {
            panel.SetSpeak(this);
        });
        return true;
    }

    public virtual bool IsAstarEvent()
    {
        return decorateAble.IsAstarEvent();
    }

    public Texture GetDynamicAvatar()
    {
        return decorateAble.GetSprite().texture;
    }

    public TalkInfo GetBaseTalkInfo()
    {
        return baseTalkInfo;
    }

    public List<Choice> GetChoices()
    {
        List<Choice> choices = new List<Choice>(baseTalkInfo.choices);
        Choice addChoice_1 = new Choice();
        addChoice_1.SetAction("使用",()=> 
        {
            if(currentPlayer != null)
                decorateAble.PresetEventTrigger(currentPlayer);
        });
        choices.Add(addChoice_1);
        return choices;
    }

    public string GetSpeakerName()
    {
        return decorateAble.GetName();
    }

    public string Relationship()
    {
        return "";
    }
}
