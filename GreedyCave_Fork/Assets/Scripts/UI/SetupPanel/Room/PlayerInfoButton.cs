using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerInfoButton : PoolPanel
{
    public static PlayerInfoButton operiatePlayerInfoButton;

    private GameObject infoObj;
    private TMP_Text nameText;

    private GameObject operiatePanelObj;
    private Transform teamOperiateButtonFather;

    private List<TeamOperateButton> operiatePanel = new List<TeamOperateButton>();

    bool isOperating = false;
    public override void ShowMeFirst()
    {
        infoObj = GetPanel("Info").transform.gameObject;
        nameText = GetPanel("Info/Text (TMP)").GetUI<TMP_Text>();

        operiatePanelObj = GetPanel("Info/OperatePanel").transform.gameObject;
        teamOperiateButtonFather = GetPanel("Info/OperatePanel/Content").transform;

        Button operiateButton = ThisPanel.GetUI<Button>();
        operiateButton.onClick.RemoveAllListeners();
        operiateButton.onClick.AddListener(OperiatePanel);
    }

    public override void ShowMeEveryTime()
    {
        isOperating = false;
    }

    public void OperiatePanel()
    {
        if (isOperating == false)
        {
            if(operiatePlayerInfoButton != null)
                operiatePlayerInfoButton.CloseOperiatePanel();

            operiatePanelObj.SetActive(true);
            ShowPoorPanelAsync<TeamOperateButton>(TeamOperateButton.prefabsPath, teamOperiateButtonFather, null, (panel) =>
              {
                  panel.SetOperiateType(OperateType.查看信息);
                  operiatePanel.Add(panel);
              });
            ShowPoorPanelAsync<TeamOperateButton>(TeamOperateButton.prefabsPath, teamOperiateButtonFather, null, (panel) =>
            {
                panel.SetOperiateType(OperateType.离开房间);
                operiatePanel.Add(panel);
            });
            isOperating = true;
            operiatePlayerInfoButton = this;
        }
        else
        {
            CloseOperiatePanel();
        }
    }


    public void CloseOperiatePanel()
    {
        if (operiatePlayerInfoButton == this)
            operiatePlayerInfoButton = null;

        operiatePanelObj.SetActive(false);
        isOperating = false;
        RecoveryOperiateButton();
    }

    private void RecoveryOperiateButton()
    {
        for (int i = 0; i < operiatePanel.Count; i++)
        {
            HidePanel(TeamOperateButton.prefabsPath, operiatePanel[i]);
        }
    }
    public void SetInfo(IPlayerAble player)
    {
        if (player == null)
        {
            infoObj.SetActive(false);
            return;
        }
        infoObj.SetActive(true);
        nameText.text = player.GetPlayerData().ID;
    }

    public override void HideMe()
    {
        CloseOperiatePanel();
    }

}
