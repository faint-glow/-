using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignOutPanel : MapPanel
{
    public static string prefabsPath = "SetupPanel/Room/SignOutPanel";

    public override void ShowMeFirst()
    {
        Button closeButton = GetPanel("Close").GetUI<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Close);

        Button signOutButton = GetPanel("Main/Button").GetUI<Button>();
        signOutButton.onClick.RemoveAllListeners();
        signOutButton.onClick.AddListener(SignOut);

        Button returnButton = GetPanel("Main/Button (1)").GetUI<Button>();
        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(ReturnPanel);
    }

    private void SignOut()
    {
        AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
        Application.Quit();
    }

    public void Close()
    {
        AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
        UIManager.GetInstance().HidePanel(SignOutPanel.prefabsPath);
    }

    private void ReturnPanel()
    {
        AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);

        UIManager.GetInstance().HidePanel(SignOutPanel.prefabsPath);
        UIManager.GetInstance().HidePanel(SetupPanel.prefabsPath);
        UIManager.GetInstance().ShowPanelAsync<StartPanel>(StartPanel.prefabsPath);
    }
}
