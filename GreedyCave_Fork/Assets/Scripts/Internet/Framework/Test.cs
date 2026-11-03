using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public InputField idInput;
    public InputField pwInput;
    public InputField textInput;

    void Start()
    {
        //NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        //NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        //NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);

        //NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
        //NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        //NetManager.AddMsgListener("MsgKick", OnMsgKick);
        //NetManager.AddMsgListener("MsgGetText", OnMsgGetText);
        //NetManager.AddMsgListener("MsgSaveText", OnMsgSaveText);
        //NetManager.AddMsgListener("MsgTest", OnMsgTest);
    }

    private void Update()
    {
        NetManager.Update();
        //if(Input.GetKeyDown(KeyCode.O))
        //{
        //    MsgTest msg = new MsgTest();
        //    NetManager.Send(msg);
        //}

    }

    public void OnMsgTest(MsgBase msg)
    {
        MsgTest newMsg = (MsgTest)msg;
        Debug.Log(newMsg.word);
    }

    public void OnRegisterClick()
    {
        MsgRegister msg = new MsgRegister();
        msg.id = idInput.text;
        msg.pw = pwInput.text;
        NetManager.Send(msg);
    }

    public void OnMsgRegister(MsgBase msgBase)
    {
        MsgRegister msg = (MsgRegister)msgBase;
        if (msg.result == 0)
            Debug.Log("注册成功");
        else
            Debug.Log("注册失败");
    }

    public void OnLoginClick()
    {
        MsgLogin msg = new MsgLogin();
        msg.id = idInput.text;
        msg.pw = pwInput.text;
        NetManager.Send(msg);
    }

    public void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msg = (MsgLogin)msgBase;
        if (msg.result == 0)
        {
            Debug.Log("登录成功");
            MsgGetText msgGetText = new MsgGetText();
            NetManager.Send(msgGetText);
        }
        else
            Debug.Log("登录失败");
    }

    private void OnMsgKick(MsgBase msgBase)
    {
        Debug.Log("被踢下线");
    }

    private void OnMsgGetText(MsgBase msgBase)
    {
        MsgGetText msg = (MsgGetText)msgBase;
        textInput.text = msg.text;
    }

    public void OnSaveClick()
    {
        MsgSaveText msg = new MsgSaveText();
        msg.text = textInput.text;
        NetManager.Send(msg);
    }

    private void OnMsgSaveText(MsgBase msgBase)
    {
        MsgSaveText msg = (MsgSaveText)msgBase;
        if (msg.result == 0)
            Debug.Log("保存成功");
        else
            Debug.Log("保存失败");
    }

    public void OnMsgMove(MsgBase msgBase)
    {
        MsgMove msg = (MsgMove)msgBase;
        //消息处理
        Debug.Log("OnMsgMove msg.x = " + msg.x);
        Debug.Log("OnMsgMove msg.y = " + msg.y);
        Debug.Log("OnMsgMove msg.z = " + msg.z);
    }


    //主动连接
    public void OnConnectClick()
    {
        NetManager.Connect("127.0.0.1", 8888);
    }
    //玩家点击发送按钮
    public void OnMoveClick()
    {
        MsgMove msg = new MsgMove();
        msg.x = 120;
        msg.y = 123;
        msg.z = -6;
        Debug.Log("Send");
        NetManager.Send(msg);
    }
    //主动关闭
    public void OnCloseClick()
    {
        NetManager.Close();
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

}
