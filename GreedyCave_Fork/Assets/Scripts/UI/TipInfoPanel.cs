using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface ITipInfoAble
{
    public bool IsHideOnRepeatShow();
    public string GetTipInfo();
}
public class TipInfoPanel : MapPanel
{
    public static string prefabsPath = "TipInfoPanel";

    private ITipInfoAble tipInfoAble;
    private TMP_Text contentText;
    public override void ShowMeFirst()
    {
        Button clickButton = ThisPanel.GetUI<Button>();
        clickButton.onClick.AddListener(() =>
        {
            HideThisPanel();
        });
        contentText = GetPanel("Scroll View/Viewport/Content/Text (TMP)").GetUI<TMP_Text>();
    }
    public void SetContent(ITipInfoAble tipInfoAble)
    {
        if (tipInfoAble == null)
            return;

        if (this.tipInfoAble != null && this.tipInfoAble == tipInfoAble && tipInfoAble.IsHideOnRepeatShow() == true)
        {
            HideThisPanel();
        }
        else
        {
            this.tipInfoAble = tipInfoAble;
            this.contentText.text = tipInfoAble.GetTipInfo();
        }
    }
    public void RefreshContent()
    {
        if (this.tipInfoAble == null)
            return;

        this.contentText.text = tipInfoAble.GetTipInfo();
    }
    private void HideThisPanel()
    {
        UIManager.GetInstance().HidePanel(TipInfoPanel.prefabsPath);
    }
    public override void HideMe()
    {
        this.tipInfoAble = null;
        this.contentText.text = "";
    }
}
