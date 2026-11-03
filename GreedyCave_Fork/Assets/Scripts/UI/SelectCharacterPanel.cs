using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectCharacterPanel : ContainerPanel<CharacterButton, SelectCharacterInfo>
{
    public static string prefabsPath = "SelectCharacterPanel";

    private Transform childFather;
    private TMP_InputField name_inputField;
    private Button confirmButton;
    private Button cancelButton;

    private CharacterButton currentCharacterButton;
    public override void ShowMeFirst()
    {
        childFather = GetPanel("Scroll View/Viewport/Content").transform;
        name_inputField = GetPanel("Text (TMP)/InputField (TMP)").transform.GetComponent<TMP_InputField>();
        confirmButton = GetPanel("ConfirmButton").GetUI<Button>();
        cancelButton = GetPanel("CancelButton").GetUI<Button>();

        name_inputField.onValueChanged.AddListener((content)=> 
        {
            if (content.Length > 6)
            {
                name_inputField.text = content.Substring(0, 6);
                UIManager.GetInstance().ShowTips("名字最长6个字");
            }
        });
        confirmButton.onClick.AddListener(() =>
        {
            //SelectDataPanel.ClearRunTimeData();

            PlayerData playerData = new PlayerData();
            SelectCharacterInfo info = currentCharacterButton.GetSelectCharacterInfo();
            playerData.playerName = name_inputField.text;
            playerData.gender = info.gender;
            playerData.currentSceneName = SceneName.MainCity;
            PlayerMgr.Instance.SetPlayerData(playerData);

            SelectDataPanel.LoadPlayerData(playerData);
            //TaskMgr.ReCreate();


            //GameManager.Instance.ChangeScene(SceneName.MainCity);
            //TaskMgr.GetInstance().LoadSaveInfo();

            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            UIManager.GetInstance().DestroyPanel(SelectCharacterPanel.prefabsPath);
        });
        cancelButton.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            UIManager.GetInstance().DestroyPanel(SelectCharacterPanel.prefabsPath);
            UIManager.GetInstance().ShowPanelAsync<StartPanel>(StartPanel.prefabsPath);
        });
    }
    public override void ShowMeEveryTime()
    {
        SelectCharacterInfo[] selectCharacterInfos = ResMgr.GetInstance().LoadAll<SelectCharacterInfo>("Player/SelectCharacterInfo");
        List<SelectCharacterInfo> newSelectCharacterInfos = new List<SelectCharacterInfo>(selectCharacterInfos);
        RefreshContainer(newSelectCharacterInfos);
    }
    protected override void AfterRefresh()
    {
        CharacterButton characterButton = (GetCell(0) as CharacterButton);
        SetCurrentCharacterButton(characterButton);
    }
    public void SetCurrentCharacterButton(CharacterButton characterButton)
    {
        if (currentCharacterButton != null)
            currentCharacterButton.CancelSelect();

        if(characterButton != null)
        {
            characterButton.Select();
            currentCharacterButton = characterButton;
        }
    }
    public override string GetCellPath()
    {
        return CharacterButton.prefabsPath;
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