using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : BaseManager<RoomManager>
{
    public static int MaxCount = 3;
    private RoomInfo roomInfo;
    private Dictionary<string, IPlayerAble> players = new Dictionary<string, IPlayerAble>();
    private RoomMgrPanel roomMgrPanel;
    public RoomMgrPanel _roomMgrPanel
    {
        get
        {
            if(roomMgrPanel == null)
                roomMgrPanel = (RoomMgrPanel)UIManager.GetInstance().GetExistPanel(RoomMgrPanel.prefabsPath);
            return roomMgrPanel;
        }
    }
    public int GetPlayerCount()
    {
        if (IsMeInRoom == false)
            return 0;
        return _roomInfo.players.Count;
    }

    public RoomManager()
    {
        NetManager.AddMsgListener("MsgGetPlayerData", OnMsgGetPlayerData);
    }

    public Dictionary<string, IPlayerAble> Players
    {
        get { return players; }
        set { players = value; }
    }

    public RoomInfo _roomInfo
    {
        get { return roomInfo; }
    }

    public bool IsMeInRoom
    {
        get 
        {
            if (roomInfo == null)
                return false;
            return roomInfo.IsContainsPlayer(PlayerMgr.Instance.GetPlayerData().ID); 
        }
    }

    //如果我不在游戏页面，其他玩家在此期间，进入，然后退出，就会出现先删除该玩家，然后加入该玩家。就会出错，需要刷新
    public void RefershPlayers()
    {
        Players.Clear();
        if (roomInfo == null)
        {
            if (_roomMgrPanel != null)
                _roomMgrPanel.RefreshRoom();
            return;
        }
        MsgGetPlayerData msg = new MsgGetPlayerData();
        msg.targetIDs = roomInfo.players;
        NetManager.Send(msg);
    }

    public void CreateRoom(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
        string id = PlayerMgr.Instance.GetPlayerData().ID;
        Players[id] = PlayerMgr.Instance;

        _roomMgrPanel.ShowRoomMainPanel();
    }

    //分自己进入别人的房间和别人进入自己当前所在房间
    public void EnterRoom(RoomInfo roomInfo,string enterPlayerID)
    {
        MsgGetPlayerData msg = new MsgGetPlayerData();//获取玩家数据填充players
        msg.targetIDs = new List<string>();

        if (enterPlayerID == PlayerMgr.Instance.GetPlayerData().ID)
        {
            msg.targetIDs = roomInfo.players;//获取房间所有人的信息，此时信息包括自己
            this.roomInfo = roomInfo;

            for (int i = 0; i < roomInfo.players.Count; i++)//先设置为null。占用空间，如果还没收到数据，就已经退出，就会利用没有这个key的条件而不生成新玩家
            {
                Players[roomInfo.players[i]] = null;
            }
        }
        else
        {
            this.roomInfo = roomInfo;
            Players[enterPlayerID] = null;//先设置为null。占用空间，如果还没收到数据，就已经退出，就会利用没有这个key的条件而不生成新玩家
            msg.targetIDs.Add(enterPlayerID);//获取加入房间的其他玩家的数据
        }

        NetManager.Send(msg);
        _roomMgrPanel.ShowRoomMainPanel();
    }

    public void LeaveRoom(RoomInfo roomInfo,string leavePlayerID)
    {
        if (leavePlayerID == PlayerMgr.Instance.GetPlayerData().ID)
        {
            foreach (var item in Players)
            {
                if (item.Key == PlayerMgr.Instance.GetPlayerData().ID)
                    continue;

                if(item.Value != null)
                    item.Value.DestoryPlayer();
            }

            this.Players.Clear();
            this.roomInfo = null;
            if (_roomMgrPanel != null)
                _roomMgrPanel.ShowEnterMainPanel();

            UIManager.GetInstance().ShowPanelAsync<PlayerStatePanelMgr>(PlayerStatePanelMgr.prefabsPath, PanelLayer.Bot, null, (panel) =>
            {
                panel.RemoveAllPlayerStates();

                panel.ShowPlayerState(PlayerMgr.Instance);

            });
        }
        else
        {
            if (this.Players.ContainsKey(leavePlayerID))
            {
                if (Players[leavePlayerID] != null)
                {
                    UIManager.GetInstance().ShowPanelAsync<PlayerStatePanelMgr>(PlayerStatePanelMgr.prefabsPath, PanelLayer.Bot, null, (panel) =>
                    {
                        panel.RemovePlayerState(players[leavePlayerID]);
                        Players[leavePlayerID].DestoryPlayer();

                        this.Players.Remove(leavePlayerID);
                    });
                }
            }

            this.roomInfo = roomInfo;

            if (_roomMgrPanel != null)
                _roomMgrPanel.RefreshRoom();
        }
    }

    public void OnMsgGetPlayerData(MsgBase msgBase)
    {
        MsgGetPlayerData msg = (MsgGetPlayerData)msgBase;

        for (int i = 0; i < msg.playerDatas.Count; i++)
        {
            PlayerData data = msg.playerDatas[i];
            string id = data.ID;
            if (id == PlayerMgr.Instance.GetPlayerData().ID)
            {
                Players[id] = PlayerMgr.Instance;
            }
            else
            {
                if(Players.ContainsKey(data.ID))//如果加入时，还没收到这个信息，就已经退出。那么Players就不会有这个ID
                {
                    SyncPlayerMgr syncPlayer = new SyncPlayerMgr(data);
                    string syncePlayerID = syncPlayer.GetPlayerData().ID;
                    Players[syncePlayerID] = syncPlayer;
                }
            }
        }

        UIManager.GetInstance().ShowPanelAsync<PlayerStatePanelMgr>(PlayerStatePanelMgr.prefabsPath, PanelLayer.Bot, null, (panel) =>
        {
            panel.RemoveAllPlayerStates();
            panel.ShowAllPlayerStates(players);
        });

        if (_roomMgrPanel != null)
            _roomMgrPanel.RefreshRoom();
    }

}
