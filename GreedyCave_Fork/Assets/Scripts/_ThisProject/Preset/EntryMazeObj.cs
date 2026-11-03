using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class EntryMazeObj : MazeObj
{
    //private Tilemap tileMap;
    //private MapInfo mapInfo;
    //private string msgID = "进入迷宫";
    //private int msgNum = 0;
    private int targetLayerCount;

    public override string GetSmallMapSpritePath()
    {
        return "Sprite/SmallMapObj/Entry";
    }
    public override bool PresetEventTrigger(IPlayerAble player)
    {
        SelectTargetLayer();
        return true;
    }
    #region 网络
    //private void OnMsgRequest(MsgBase msgBase)
    //{
    //    MsgRequest msg = (MsgRequest)msgBase;
    //    if (msg.msgID != this.msgID)
    //        return;
    //    if (msg.requestID == PlayerMgr.Instance.GetPlayerData().ID)//处理自己发出的请求
    //    {
    //        if (msg.msgNum != this.msgNum)
    //        {
    //            PlayerMsgMgr.SendTip(msg.targetID, "响应超时");
    //            return;
    //        }
    //        if (msg.targetResults[msg.targetID] == RequestType.Refuse)
    //        {
    //            msg.isEndRequest = true;
    //            NetManager.Send(msg);
    //            return;
    //        }
    //        if(msg.responseCount >= msg.targetResults.Count)
    //        {
    //            Enter();
    //        }
    //    }
    //    else//处理别人发出的请求
    //    {
    //        UIManager.GetInstance().ShowPoorPanelAsync<RequestPanel>(RequestPanel.prefabsPath, PanelLayer.Max, null, (panel) =>
    //        {
    //            panel.SetRequestInfo(msg.content,
    //            () => //拒绝处理
    //            {
    //                if (msg.targetResults.ContainsKey(PlayerMgr.Instance.GetHostID()))
    //                {
    //                    string id = PlayerMgr.Instance.GetHostID();
    //                    msg.targetID = id;
    //                    msg.targetResults[id] = RequestType.Refuse;
    //                    NetManager.Send(msg);
    //                }
    //                if (panel != null)
    //                    UIManager.GetInstance().DestroyPoorPanel(RequestPanel.prefabsPath, panel);
    //            },
    //            () => //同意处理
    //            {
    //                if (msg.targetResults.ContainsKey(PlayerMgr.Instance.GetHostID()))
    //                {
    //                    string id = PlayerMgr.Instance.GetHostID();
    //                    msg.targetID = id;
    //                    msg.targetResults[id] = RequestType.Accept;
    //                    NetManager.Send(msg);
    //                }
    //                if (panel != null)
    //                    UIManager.GetInstance().DestroyPoorPanel(RequestPanel.prefabsPath, panel);
    //            });
    //        });
    //    }
    //}
    #endregion
    private void SelectTargetLayer()
    {
        if (!TaskMgr.GetInstance().IsFinishTak(TaskMgr.firstMainTreadTaskName))
        {
            UIManager.GetInstance().ShowTips("请完成主线任务\"新手引导\"后再进入迷宫");
            return;
        }

        //if (SceneMgr.GetInstance().GetCurrentScene().name == SceneName.MainCity)
        //{
        //    EnterController(1);
        //}
        //else if (SceneMgr.GetInstance().GetCurrentScene().name == SceneName.Lounge)
        //{
        //    EnterController(GameManager.Instance.GetMapInfo().LayerCount + 1);
        //}
        //else if (SceneMgr.GetInstance().GetCurrentScene().name == SceneName.MazeScene)
        //{
        //    EnterController(GameManager.Instance.GetMapInfo().LayerCount + 1);
        //}
        //else if(SceneMgr.GetInstance().GetCurrentScene().name == SceneName.BossRoom_1)
        //{
        //    GameManager.Instance.ChangeSceneAfterVignette(SceneName.MainCity);
        //}
        //else
        //{
        //    Debug.Log("场景有误");
        //}

        Enter();
    }

    public void EnterController(int layerCount)
    {
        targetLayerCount = layerCount;
        Enter();
        #region 网络

        //if (RoomManager.GetInstance().IsMeInRoom == false || RoomManager.GetInstance()._roomInfo.players.Count < 2)
        //{
        //    Enter();
        //}
        //else
        //{
        //    List<string> players = RoomManager.GetInstance()._roomInfo.players;
        //    msgNum++;

        //    string currentPlayerID = PlayerMgr.Instance.GetPlayerData().ID;

        //    MsgRequest msg = new MsgRequest();
        //    msg.msgID = this.msgID;
        //    msg.msgNum = this.msgNum;
        //    msg.requestID = currentPlayerID;
        //    msg.content = currentPlayerID + "请求进入迷宫";
        //    msg.targetResults = new Dictionary<string, RequestType>();
        //    for (int i = 0; i < players.Count; i++)
        //    {
        //        if (players[i] == currentPlayerID)
        //            continue;
        //        msg.targetResults[players[i]] = RequestType.Wait;
        //    }
        //    msg.isEndRequest = false;

        //    NetManager.Send(msg);
        //}

        #endregion
    }

    private void Enter()
    {
        if (SceneMgr.GetInstance().GetCurrentScene().name != SceneName.MainCity)
            GameManager.Instance.NextLayer();
        else
            UIManager.GetInstance().ShowPanel<SelectLayerPanel>(SelectLayerPanel.prefabsPath, PanelLayer.Mid_Up);
    }
}
