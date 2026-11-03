using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterMainPanel : ContainerPanel<RoomPanel, RoomInfo>
{
    public static string prefabsPath = "SetupPanel/Room/EnterMainPanel";

    //public static RoomInfo SelectRoomInfo;
    private Transform mainPanel;
    private RoomMgrPanel roomMgrPanel;
    public override void ShowMeFirst()
    {
        roomMgrPanel = (RoomMgrPanel)UIManager.GetInstance().GetExistPanel(RoomMgrPanel.prefabsPath);

        mainPanel = GetPanel("Main/Scroll/Viewport/Content").transform;

        Button createButon = GetPanel("Main/Button").GetUI<Button>();
        createButon.onClick.RemoveAllListeners();
        createButon.onClick.AddListener(CreateRoom);

        Button enterButton = GetPanel("Main/Button (1)").GetUI<Button>();
        enterButton.onClick.RemoveAllListeners();
        enterButton.onClick.AddListener(EnterRoom);

        NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);
        NetManager.AddMsgListener("MsgGetRoomInfos", OnMsgGetRoomInfos);
    }
    public override void ShowMeEveryTime()
    {
        RoomPanel.SelectRoomPanel = null;
        GetRoomInfo();
    }

    public override IHolderAble GetHolder()
    {
        return null;
    }
    public override void DestroyMe()
    {
        NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
        NetManager.RemoveMsgListener("MsgGetRoomInfos", OnMsgGetRoomInfos);
    }

    public override string GetCellPath()
    {
        return RoomPanel.prefabsPath;
    }

    public override Transform GetContainterTrans()
    {
        return mainPanel;
    }

    public override ScrollRect GetScrollRect()
    {
        return null;
    }
    private void CreateRoom()
    {
        if (RoomManager.GetInstance().IsMeInRoom == true)
        {
            UIManager.GetInstance().ShowTips("当前正在房间中，不可再次创建房间");
            return;
        }

        MsgCreateRoom msg = new MsgCreateRoom();
        msg.roomInfo = new RoomInfo();
        msg.roomInfo.roomName = "新房间";
        msg.roomInfo.playerMaxCount = 3;
        
        NetManager.Send(msg);
    }
    private void EnterRoom()
    {
        if (RoomManager.GetInstance().IsMeInRoom == true)
        {
            UIManager.GetInstance().ShowTips("当前正在房间中，不可再次进入房间");
            return;
        }

        MsgEnterRoom msg = new MsgEnterRoom();
        if(RoomPanel.SelectRoomPanel != null)
            msg.roomInfo = RoomPanel.SelectRoomPanel._roomInfo;
        if(msg.roomInfo == null)
        {
            UIManager.GetInstance().ShowTips("未选中房间");
            return;
        }
        NetManager.Send(msg);
    }
    private void GetRoomInfo()
    {
        MsgGetRoomInfos msg = new MsgGetRoomInfos();
        NetManager.Send(msg);
    }

    private void OnMsgCreateRoom(MsgBase msgBase)
    {
        MsgCreateRoom msgCreateRoom = (MsgCreateRoom)msgBase;
        if (msgCreateRoom.result == 0)
        {
            RoomManager.GetInstance().CreateRoom(msgCreateRoom.roomInfo);
        }
        else
            UIManager.GetInstance().ShowTips("创建失败");
    }

    //可能收到来自自己的进入房间请求和别人的进入房间请求
    private void OnMsgEnterRoom(MsgBase msgBase)
    {
        MsgEnterRoom msgEnterRoom = (MsgEnterRoom)msgBase;

        if (msgEnterRoom.result == 0)
            RoomManager.GetInstance().EnterRoom(msgEnterRoom.roomInfo, msgEnterRoom.enterPlayerID);
        else
            UIManager.GetInstance().ShowTips(msgEnterRoom.errInfo);
    }

    private void OnMsgGetRoomInfos(MsgBase msgBase)
    {
        MsgGetRoomInfos msgRoomInfos = (MsgGetRoomInfos)msgBase;
        if (msgRoomInfos == null || msgRoomInfos.roomInfos == null)
        {
            UIManager.GetInstance().ShowTips("获取房间信息失败");
            return;
        }
        if (msgRoomInfos.roomInfos.Count <= 0)
            UIManager.GetInstance().ShowTips("房间数为0");

        RefreshPanel(msgRoomInfos.roomInfos);
    }

    public void RefreshPanel(List<RoomInfo> roomInfoList)
    {
        RefreshContainer(roomInfoList, (cell) =>
        {

        });
    }

}
