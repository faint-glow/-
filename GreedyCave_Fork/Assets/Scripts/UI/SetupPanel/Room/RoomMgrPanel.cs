using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomMgrPanel :BasePanel
{
    public static string prefabsPath = "SetupPanel/Room/RoomMgrPanel";

    private EnterMainPanel enterMainPanel;
    private RoomMainPanel roomMainPanel;

    public override void ShowMeFirst()
    {
        Button closeButton = GetPanel("Close").GetUI<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(()=> { UIManager.GetInstance().HidePanel(RoomMgrPanel.prefabsPath); });
    }

    public override void ShowMeEveryTime()
    {
        if(RoomManager.GetInstance().IsMeInRoom)
            ShowRoomMainPanel();
        else
            ShowEnterMainPanel();
    }

    public void ShowEnterMainPanel()
    {
        ShowPanelAsync<EnterMainPanel>(EnterMainPanel.prefabsPath, transform,null,(panel)=> 
        {
            enterMainPanel = panel;
        });
        HidePanel(RoomMainPanel.prefabsPath);
    }

    public void ShowRoomMainPanel()
    {
        ShowPanelAsync<RoomMainPanel>(RoomMainPanel.prefabsPath, transform,null,(panel)=> 
        {
            roomMainPanel = panel;
        });
        HidePanel(EnterMainPanel.prefabsPath);
    }

    public void RefreshRoom()
    {
        if(roomMainPanel != null)
            roomMainPanel.RefreshPanel();
    }
}
