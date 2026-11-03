using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupPanel : MapPanel
{
    public static string prefabsPath = "SetupPanel/SetupPanel";
    public override void ShowMeFirst()
    {
        Button setupPanelClose = GetPanel("Close").GetUI<Button>();
        setupPanelClose.onClick.RemoveAllListeners();
        setupPanelClose.onClick.AddListener(() => 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            UIManager.GetInstance().HidePanel(SetupPanel.prefabsPath);
        });

        Button networkingPanel = GetPanel("Main/ButtonPanel/NetworkingButton").GetUI<Button>();
        networkingPanel.onClick.RemoveAllListeners();
        networkingPanel.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            UIManager.GetInstance().ShowTips("暂不开放此功能");
            //UIManager.GetInstance().ShowPanelAsync<NetworkingPanel>(NetworkingPanel.prefabsPath); 
        });
        Button roomPanel = GetPanel("Main/ButtonPanel/RoomButton").GetUI<Button>();
        roomPanel.onClick.RemoveAllListeners();
        roomPanel.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);

            if (NetManager.IsConnect() == false)
            {
                UIManager.GetInstance().ShowTips("暂不开放此功能");
                //UIManager.GetInstance().ShowTips("未联网，请联网后进行该操作");
                return;
            }
            UIManager.GetInstance().ShowPanelAsync<RoomMgrPanel>(RoomMgrPanel.prefabsPath);
        });

        Button savePanel = GetPanel("Main/ButtonPanel/SaveButton").GetUI<Button>();
        savePanel.onClick.RemoveAllListeners();
        savePanel.onClick.AddListener(() => 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            PlayerMgr.Instance.SavePlayerData();
        });

        Button audioPanel = GetPanel("Main/ButtonPanel/AudioButton").GetUI<Button>();
        audioPanel.onClick.RemoveAllListeners();
        audioPanel.onClick.AddListener(() => 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            UIManager.GetInstance().ShowPanelAsync<AudioPanel>(AudioPanel.prefabsPath);
        });

        Button signOutPanel = GetPanel("Main/ButtonPanel/SignOutButton").GetUI<Button>();
        signOutPanel.onClick.RemoveAllListeners();
        signOutPanel.onClick.AddListener(()=> 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            UIManager.GetInstance().ShowPanelAsync<SignOutPanel>(SignOutPanel.prefabsPath); 
        });
    }
}
