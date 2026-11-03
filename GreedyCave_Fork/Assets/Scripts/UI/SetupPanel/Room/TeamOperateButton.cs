using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum OperateType
{
    查看信息,
    踢出房间,
    离开房间,
    房主转交,
}
public class TeamOperateButton : MapPanel
{
    public static string prefabsPath = "SetupPanel/Room/TeamOperateButton";

    private TMP_Text buttonNameText;
    public override void ShowMeFirst()
    {
        buttonNameText = GetPanel("Text (TMP)").GetUI<TMP_Text>();
    }

    public override void ShowMeEveryTime()
    {
        RectTransform rectTrans = ThisPanel.transform.GetComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(210, 120);
    }

    public void SetOperiateType(OperateType operiateType)
    {
        buttonNameText.text = operiateType.ToString();
    }
    
}
