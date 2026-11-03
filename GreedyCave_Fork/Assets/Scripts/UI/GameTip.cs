using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTip : MapPanel
{
    public static string prefabsPath = "GameTip";

    private TMP_Text contentText;
    private Animator anim;
    private AnimController animationEvent;
    
    public override void ShowMeFirst()
    {
        contentText = GetPanel("Text (TMP)").GetUI<TMP_Text>();
        anim = GetComponent<Animator>();

        animationEvent = anim.gameObject.GetComponent<AnimController>();
        if (animationEvent == null)
            animationEvent = anim.gameObject.AddComponent<AnimController>();
        animationEvent.Initial("GameTip");

        animationEvent.AddEvent("TipAnimation", EventTriggerEnum.EventTriggertStart, 1.5f, HidePanel);
    }
    public override void ShowMeEveryTime()
    {
        transform.localPosition = Vector3.zero;
    }
    public void SetContent(string content)
    {
        contentText.text = content;
    }
    public void HidePanel()
    {
        UIManager.GetInstance().HideTips(this);
    }
}
