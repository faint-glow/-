using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkingPanel : MapPanel
{
    public static string prefabsPath = "SetupPanel/NetworkingPanel";

    TMP_InputField serverPassWord;

    TMP_InputField account;
    TMP_InputField passWord;

    GameObject serverPanel;
    GameObject accountPanel;

    public override void ShowMeFirst()
    {
        InitialState();

        NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
        //NetManager.AddMsgListener("MsgLogin", OnMsgLogin);//测试期间关掉，否则会和输入管理的测试样例冲突
    }

    public override void DestroyMe()
    {
        NetManager.RemoveMsgListener("MsgRegister", OnMsgRegister);
        //NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
    }

    public override void ShowMeEveryTime()
    {
        ShowPanel(NetManager.IsConnect());
    }

    private void InitialState()
    {

        Button networkingPanelClose = GetPanel("Close").GetUI<Button>();
        networkingPanelClose.onClick.RemoveAllListeners();
        networkingPanelClose.onClick.AddListener(() => { UIManager.GetInstance().DestroyPanel(NetworkingPanel.prefabsPath); });

        serverPanel = GetPanel("ServerMain").transform.gameObject;
        accountPanel = GetPanel("AccountMain").transform.gameObject;

        serverPassWord = GetPanel("ServerMain/InputField (TMP)").transform.GetComponent<TMP_InputField>();
        account = GetPanel("AccountMain/InputField (TMP)").transform.GetComponent<TMP_InputField>();
        passWord = GetPanel("AccountMain/InputField (TMP) (1)").transform.GetComponent<TMP_InputField>();

        Button serverPassWordButton = GetPanel("ServerMain/Button").GetUI<Button>();
        serverPassWordButton.onClick.RemoveAllListeners();
        serverPassWordButton.onClick.AddListener(Connect);

        Button registerButton = GetPanel("AccountMain/Button").GetUI<Button>();
        registerButton.onClick.RemoveAllListeners();
        registerButton.onClick.AddListener(Register);

        Button loginButton = GetPanel("AccountMain/Button (1)").GetUI<Button>();
        loginButton.onClick.RemoveAllListeners();
        loginButton.onClick.AddListener(Login);
    }


    private void Connect()
    {
        if(serverPassWord.text == "1314")
        {
            NetManager.Connect("127.0.0.1", 9999);
            StartCoroutine(ConnetCoroutine());
        }
    }
    
    IEnumerator ConnetCoroutine()
    {
        while(NetManager.IsConnect() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        ShowPanel(true);
    }

    private void ShowPanel(bool isShowAccount)
    {
        serverPanel.SetActive(!isShowAccount);
        accountPanel.SetActive(isShowAccount);
    }

    private void Register()
    {
        MsgRegister msg = new MsgRegister();
        msg.id = account.text;
        msg.pw = passWord.text;
        NetManager.Send(msg);
    }

    public void OnMsgRegister(MsgBase msgBase)
    {
        MsgRegister msg = (MsgRegister)msgBase;
        if (msg.result == 0)
            UIManager.GetInstance().ShowTips("注册成功");
        else
            UIManager.GetInstance().ShowTips("注册失败");
    }

    private void Login()
    {
        MsgLogin msg = new MsgLogin();
        msg.id = account.text;
        msg.pw = passWord.text;
        NetManager.Send(msg);
    }

    public void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msg = (MsgLogin)msgBase;
        if (msg.result == 0)
        {
            NetManager.IsLogin = true;
            UIManager.GetInstance().ShowTips("登录成功");

            UIManager.GetInstance().ShowPanelAsync<PlayerStatePanelMgr>(PlayerStatePanelMgr.prefabsPath, PanelLayer.Bot, null, (panel) =>
            {
                panel.RemoveAllPlayerStates();
                PlayerMgr.Instance.SetPlayerData(msg.playerData);
                GameManager.Instance.ChangeSceneAfterVignette(SceneName.MainCity);
            });
        }
        else
            UIManager.GetInstance().ShowTips("登录失败");
    }

}
