using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : MapPanel
{
    public static string prefabsPath = "StartPanel";
    public override void ShowMeFirst()
    {
        Button button = GetPanel("Button").GetUI<Button>();
        button.onClick.RemoveAllListeners();
        //PlayerData playerData = PlayerMgr.Instance.GetPlayerDataMgr().GetPlayerData();
        PlayerMgr.Instance.LoadAudioController();
        button.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            UIManager.GetInstance().ShowPanelAsync<SelectDataPanel>(SelectDataPanel.prefabsPath, PanelLayer.Top,null,(panel)=>
            {
                UIManager.GetInstance().DestroyPanel(StartPanel.prefabsPath);
            });
        });
    }

    public override void ShowMeEveryTime()
    {
        AudioMgr.Instance.PlayBGM(AudioClipPath.start);
        RectTransform rectTrans = transform.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = Vector2.zero;
    }
    public void StartGame(PlayerData playerData)
    {
        //if (playerData == null)
        //{
        //    SelectCaracter();
        //    return;
        //}
        //LoadPlayerData(playerData);
        UIManager.GetInstance().ShowPanelAsync<SelectDataPanel>(SelectDataPanel.prefabsPath,PanelLayer.Top);
        UIManager.GetInstance().DestroyPanel(StartPanel.prefabsPath);
    }
    //private void SelectCaracter()
    //{
    //    UIManager.GetInstance().ShowPanelAsync<SelectCharacterPanel>(SelectCharacterPanel.prefabsPath, PanelLayer.Top);
    //    UIManager.GetInstance().DestroyPanel(StartPanel.prefabsPath);
    //}
    //private void LoadPlayerData(PlayerData playerData)
    //{
    //    PlayerMgr.Instance.LoadBag(playerData);
    //    PlayerMgr.Instance.LoadEquip(playerData);
    //    PlayerMgr.Instance.LoadDecorate(playerData);
    //    PlayerMgr.Instance.LoadSkillController(playerData);
    //    PlayerMgr.Instance.LoadShortcutController(playerData);

    //    GameManager.Instance.LoadMapInfo(playerData);
    //    GameManager.Instance.ChangeScene(playerData.currentSceneName, null, () =>
    //    {
    //        if (playerData.currentSceneName == SceneName.MazeScene)
    //            GameManager.Instance.LoadMaze(playerData);
    //        if (SceneController.Instance != null)
    //            SceneController.Instance.LoadSaveData(playerData.sceneSaveInfoDic);
    //        PlayerMgr.Instance.LoadBasePlayerData(playerData);
    //    });
    //    UIManager.GetInstance().DestroyPanel(StartPanel.prefabsPath);
    //}
}
