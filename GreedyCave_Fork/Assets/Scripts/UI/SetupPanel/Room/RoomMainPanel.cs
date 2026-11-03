using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomMainPanel :MapPanel
{
    public static string prefabsPath = "SetupPanel/Room/RoomMainPanel";

    private Transform playerInfoPanelFather;
    private List<PlayerInfoButton> playerInfoButtons = new List<PlayerInfoButton>();
    private RoomMgrPanel roomMgrPanel;

    public override void ShowMeFirst()
    {
        roomMgrPanel = (RoomMgrPanel)UIManager.GetInstance().GetExistPanel(RoomMgrPanel.prefabsPath);

        NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);

        Button mainButton = GetPanel("Main").GetUI<Button>();
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(ReleaseOperiateButton);

        Button leaveButton = GetPanel("Main/Button").GetUI<Button>();
        leaveButton.onClick.RemoveAllListeners();
        leaveButton.onClick.AddListener(LeaveRoom);

        playerInfoPanelFather = GetPanel("Main/Content").transform;
        for (int i = 0; i < playerInfoPanelFather.childCount; i++)
        {
            playerInfoButtons.Add(playerInfoPanelFather.GetChild(i).GetComponent<PlayerInfoButton>());
        }
    }

    public override void ShowMeEveryTime()
    {
        //RoomManager.GetInstance().RefershPlayers();//刷新房间玩家，并且从服务器获取数据。然后会调用此脚本的RefreshPanel
        RefreshPanel();
    }

    public override void DestroyMe()
    {
        NetManager.RemoveMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
    }

    public void RefreshPanel()
    {
        int index = 0;

        foreach (var item in RoomManager.GetInstance().Players)
        {
            playerInfoButtons[index].SetInfo(item.Value);
            index++;
        }

        for (int i = index; i < playerInfoButtons.Count; i++)
        {
            playerInfoButtons[i].SetInfo(null);
        }
    }

    private void ReleaseOperiateButton()
    {
        if(PlayerInfoButton.operiatePlayerInfoButton != null)
        {
            PlayerInfoButton.operiatePlayerInfoButton.CloseOperiatePanel();
        }
    }

    private void LeaveRoom()
    {
        if (RoomManager.GetInstance().IsMeInRoom == false)
        {
            UIManager.GetInstance().ShowTips("不在房间中，无法退出");
            return;
        }

        MsgLeaveRoom msg = new MsgLeaveRoom();
        NetManager.Send(msg);
    }

    private void OnMsgLeaveRoom(MsgBase msgBase)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        if(msg.result == 0)
            RoomManager.GetInstance().LeaveRoom(msg.roomInfo, msg.leavePlayerID);
        else
            UIManager.GetInstance().ShowTips("离开失败");
    }
}
