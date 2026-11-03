using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetController : BaseManager<NetController>
{
    public NetController()
    {
        //NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        //NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        //NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        //NetManager.AddMsgListener("MsgKick", OnMsgKick);

        //RoomManager.GetInstance();//激活房间管理
    }
    
    //连接成功时调用
    private void OnConnectSucc(string err)
    {
        Debug.Log("OnConnectSucc");
    }
    //连接失败时调用
    private void OnConnectFail(string err)
    {
        Debug.Log("OnConnectFail" + err);
    }
    //关闭连接
    private void OnConnectClose(string err)
    {
        Debug.Log("OnConnectClose");
    }
    private void OnMsgKick(MsgBase msgBase)
    {
        Debug.Log("被踢下线");
        UIManager.GetInstance().ShowTips("被踢下线");
        UIManager.GetInstance().ShowPanelAsync<KickPanel>(KickPanel.prefabsPath, PanelLayer.Max);
    }
}
