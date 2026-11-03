using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectDataPanel : ContainerPanel<PlayerDataButton, PlayerData>
{
    public static string prefabsPath = "SelectDataPanel";

    private Transform childFather;
    private Button confirmButton;
    private Button deleteButton;
    private Button cancelButton;

    private PlayerDataButton currentPlayerDataButton;
    public override void ShowMeFirst()
    {
        childFather = GetPanel("Scroll View/Viewport/Content").transform;
        //name_inputField = GetPanel("Text (TMP)/InputField (TMP)").transform.GetComponent<TMP_InputField>();
        confirmButton = GetPanel("ConfirmButton").GetUI<Button>();
        deleteButton = GetPanel("DeleteButton").GetUI<Button>();
        cancelButton = GetPanel("CancelButton").GetUI<Button>();

        confirmButton.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);

            if (currentPlayerDataButton == null)
                return;
            ClearRunTimeData();

            PlayerData playerData = currentPlayerDataButton.GetSelectPlayerData();
            PlayerMgr.Instance.GetPlayerDataMgr().SetPlayerData(playerData);
            if (playerData == null)
                SelectCaracter();
            else
            {
                LoadPlayerData(playerData);
            }
        });
        deleteButton.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);

            if (currentPlayerDataButton == null)
                return;
            if (currentPlayerDataButton == null)
                return;
            PlayerData playerData = currentPlayerDataButton.GetSelectPlayerData();
            if (playerData == null)
            {
                UIManager.GetInstance().ShowTips("没有选用有效存档");
                return;
            }
            else
            {
                UIManager.GetInstance().ShowPanelAsync<RequestPanel>(RequestPanel.prefabsPath, PanelLayer.Top_Up, null, (panel) =>
                {
                    panel.SetRequestInfo("你确定要删除该存档: \"" + "Lv." + playerData.level + " " + playerData.playerName + "\" 吗", CancelDelete, ConfirmDelete);
                });
            }
        });
        cancelButton.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);

            UIManager.GetInstance().DestroyPanel(SelectDataPanel.prefabsPath);
            UIManager.GetInstance().ShowPanelAsync<StartPanel>(StartPanel.prefabsPath);
        });
    }
    public override void ShowMeEveryTime()
    {
        Refresh();
    }
    private void Refresh()
    {
        List<PlayerData> playerDatas = PlayerMgr.Instance.GetPlayerDataMgr().GetAllPlayerData();
        RefreshContainer(playerDatas);
    }
    private void SelectCaracter()
    {
        UIManager.GetInstance().ShowPanelAsync<SelectCharacterPanel>(SelectCharacterPanel.prefabsPath, PanelLayer.Top);
        UIManager.GetInstance().DestroyPanel(SelectDataPanel.prefabsPath);
    }
    public static void LoadPlayerData(PlayerData playerData)
    {
        PlayerMgr.Instance.LoadBag(playerData);
        PlayerMgr.Instance.LoadEquip(playerData);
        PlayerMgr.Instance.LoadDecorate(playerData);
        PlayerMgr.Instance.LoadSkillController(playerData);
        PlayerMgr.Instance.LoadShortcutController(playerData);

        GameManager.Instance.LoadMapInfo(playerData);

        PlayerMgr.Instance.GetPlayerDataMgr().RemovePlayerChangeSceneEvent();//有些NPC在场景切换时也要保存数据，但是切换存档时，会让商人的所有物品初始化，（而此时，已经时物品初始化后的样子，并且玩家存档已经切换为目标存档）如果不把切换场景时保存数据关闭，则切换存档会导致目标存档的NPC是初始化后的数据
        GameManager.Instance.ChangeScene(playerData.currentSceneName, null, () =>
        {
            if (playerData.currentSceneName == SceneName.MazeScene)
                GameManager.Instance.LoadMaze(playerData);
            if (SceneController.Instance != null)
                SceneController.Instance.LoadSaveData(playerData.sceneSaveInfoDic);

            PlayerMgr.Instance.LoadBasePlayerData(playerData);
        });
        PlayerMgr.Instance.GetPlayerDataMgr().AddPlayerChangeSceneEvent();

        TaskMgr.GetInstance().LoadSaveInfo();
        UIManager.GetInstance().DestroyPanel(SelectDataPanel.prefabsPath);
    }
    public static void ClearRunTimeData()
    {
        PlayerMgr.Instance.ClearRunTimeData();
        TaskMgr.GetInstance().ClearRunTimeData();
        if (SceneController.Instance != null)
            SceneController.Instance.ClearRunTimeData();
    }

    protected override void AfterRefresh()
    {
        PlayerDataButton characterButton = (GetCell(0) as PlayerDataButton);
        SetCurrentPlayerDataButton(characterButton);
    }
    public void SetCurrentPlayerDataButton(PlayerDataButton characterButton)
    {
        if (currentPlayerDataButton != null)
            currentPlayerDataButton.CancelSelect();

        if (characterButton != null)
        {
            characterButton.Select();
            currentPlayerDataButton = characterButton;
            PlayerMgr.Instance.GetPlayerDataMgr().SetCurrentDataIndex(currentPlayerDataButton.Index);
        }
    }
    private void CancelDelete()
    {
        UIManager.GetInstance().HidePanel(RequestPanel.prefabsPath);
    }
    private void ConfirmDelete()
    {
        if (currentPlayerDataButton == null)
            return;

        PlayerMgr.Instance.GetPlayerDataMgr().DeletePlayerData(currentPlayerDataButton.Index);
        UIManager.GetInstance().HidePanel(RequestPanel.prefabsPath);
        if (PlayerMgr.Instance.GetPlayerDataMgr().IsHaveData(currentPlayerDataButton.Index) == false)
            currentPlayerDataButton.SetItem_Container(null);
    }
    public override string GetCellPath()
    {
        return PlayerDataButton.prefabsPath;
    }
    public override Transform GetContainterTrans()
    {
        return childFather;
    }
    public override IHolderAble GetHolder()
    {
        return null;
    }
    public override ScrollRect GetScrollRect()
    {
        return null;
    }
}
