using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMsgMgr : BaseManager<PlayerMsgMgr>
{
    public static string msgNextLayerID = "下一层";
    public static int msgNextLayerNum = 0;
    public PlayerMsgMgr()
    {
        NetManager.AddMsgListener("MsgSendNextPos", OnMsgSendNextPos);

        NetManager.AddMsgListener("MsgRequest", OnMsgRequest);
        NetManager.AddMsgListener("MsgStartBattle", OnMsgStartBattle);
        NetManager.AddMsgListener("MsgNormalAttack", OnMsgNormalAttack);
        NetManager.AddMsgListener("MsgSkill", OnMsgSkill);
        NetManager.AddMsgListener("MsgTip", OnMsgTip);
    }

    public static void SendTip(string targetID,string content)
    {
        MsgTip msgTip = new MsgTip();
        msgTip.targetID = targetID;
        msgTip.content = content;
        NetManager.Send(msgTip);
    }

    private void OnMsgRequest(MsgBase msgBase)
    {
        MsgRequest msg = (MsgRequest)msgBase;
        if (msg.msgID != msgNextLayerID)
            return;

        if (msg.requestID == PlayerMgr.Instance.GetPlayerData().ID)//处理自己发出的请求
        {
            if (msg.msgNum != msgNextLayerNum)
            {
                PlayerMsgMgr.SendTip(msg.targetID, "响应超时");
                return;
            }

            if (msg.targetResults[msg.targetID] == RequestType.Refuse)
            {
                msg.isEndRequest = true;
                NetManager.Send(msg);
                return;
            }

            if (msg.responseCount >= msg.targetResults.Count)
            {
                GameManager.Instance.NextLayer();
            }

        }
        else//处理别人发出的请求
        {
            UIManager.GetInstance().ShowPoorPanelAsync<RequestPanel>(RequestPanel.prefabsPath, PanelLayer.Max, null, (panel) =>
            {
                panel.SetRequestInfo(msg.content,
                () => //拒绝处理
                {
                    if (msg.targetResults.ContainsKey(PlayerMgr.Instance.GetHostID()))
                    {
                        string id = PlayerMgr.Instance.GetHostID();
                        msg.targetID = id;
                        msg.targetResults[id] = RequestType.Refuse;
                        NetManager.Send(msg);
                    }

                    if (panel != null)
                        UIManager.GetInstance().DestroyPoorPanel(RequestPanel.prefabsPath, panel);
                },
                () => //同意处理
                {
                    if (msg.targetResults.ContainsKey(PlayerMgr.Instance.GetHostID()))
                    {
                        string id = PlayerMgr.Instance.GetHostID();
                        msg.targetID = id;
                        msg.targetResults[id] = RequestType.Accept;
                        NetManager.Send(msg);
                    }

                    if (panel != null)
                        UIManager.GetInstance().DestroyPoorPanel(RequestPanel.prefabsPath, panel);
                });
            });
        }

    }

    private void OnMsgSendNextPos(MsgBase msgBase)
    {
        MsgSendNextPos msg = (MsgSendNextPos)msgBase;
        if (PlayerMgr.Instance.IsHost(msg.id))
            return;

        Dictionary<string,IPlayerAble> players = RoomManager.GetInstance().Players;
        if (players.ContainsKey(msg.id))
        {
            SyncPlayerMgr player = players[msg.id] as SyncPlayerMgr;

            if(player != null)
                player.AddNextPos(new Vector3(msg.x, msg.y, msg.z));
        }
    }

    private void OnMsgStartBattle(MsgBase msgBase)
    {
        MsgStartBattle msg = (MsgStartBattle)msgBase;
        IPlayerAble player = RoomManager.GetInstance().Players[msg.behaviourID];
        //通过位置获取格子
        msg.x = Mathf.RoundToInt(msg.x);
        msg.y = Mathf.RoundToInt(msg.y);
        MazeGrid[,] mapGrid = GameManager.Instance.GetRandomMaze().MapGrids;
        if (mapGrid == null)
            return;
        MazeGrid grid = mapGrid[msg.x, msg.y];
        //player.StartBattle(grid.GridObj as Enemy);
    }

    private void OnMsgNormalAttack(MsgBase msgBase)
    {
        MsgNormalAttack msg = (MsgNormalAttack)msgBase;

        IPlayerAble player = RoomManager.GetInstance().Players[msg.behaviourID];
        player.NormalAttack();
    }

    private void OnMsgSkill(MsgBase msgBase)
    {
        MsgSkill msg = (MsgSkill)msgBase;
        IPlayerAble player = RoomManager.GetInstance().Players[msg.behaviourID];
        player.TriggerSkill(msg.skillInfoName);
    }

    private void OnMsgTip(MsgBase msgBase)
    {
        MsgTip msg = (MsgTip)msgBase;
        UIManager.GetInstance().ShowTips(msg.content);
    }
}
