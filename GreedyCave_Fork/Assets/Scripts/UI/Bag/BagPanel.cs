using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BagPanel : CurrentBagPanel
{
    public static string prefabsPath = "BagPanel";//普通背包
    private string buttonPath = "";
    private string leftCurrentPanel = "";
    private string rightCurrentPanel = "";
    private Transform leftPanelFather;
    private Transform rightPanelFather;
    private Button currentButton;

    public override void ShowMeFirst()
    {
        //关闭按钮触发器
        Button closeButton = GetPanel("ClosePanel").GetUI<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.close);
            UIManager.GetInstance().HidePanel(BagPanel.prefabsPath);
        });
        leftPanelFather = GetPanel("Left").transform;
        rightPanelFather = GetPanel("Right").transform;

        InitialPanelChange();
    }
    public override void ShowMeEveryTime()
    {
        Button targetButton = GetPanel("ChangePanel/ItemBag").GetUI<Button>();
        ChangePanel<PlayerInfoPanel, ItemBagPanel>(targetButton, PlayerInfoPanel.prefabsPath, ItemBagPanel.prefabsPath);
    }
    protected override BagPanelType GetBagPanelType()
    {
        return BagPanelType.NormalPanel;
    }
    private void InitialPanelChange()
    {
        InitialPanelChangeAided<PlayerInfoPanel, ItemBagPanel>("ChangePanel/ItemBag", PlayerInfoPanel.prefabsPath, ItemBagPanel.prefabsPath);
        InitialSkillPanel("ChangePanel/Skill");
        InitialSkillPanel("ChangePanel/Magic");
        InitialSkillPanel("ChangePanel/Aided");


        //Button propertyButton = GetPanel("ChangePanel/Property").GetUI<Button>();
        //propertyButton.onClick.RemoveAllListeners();
        //propertyButton.onClick.AddListener(() => 
        //{
        //    AudioMgr.Instance.PlayEffect(AudioClipPath.change);
        //    UIManager.GetInstance().ShowTips("暂未开放"); 
        //});
        Button talentButton = GetPanel("ChangePanel/Talent").GetUI<Button>();
        talentButton.onClick.RemoveAllListeners();
        talentButton.onClick.AddListener(() => 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.change);
            UIManager.GetInstance().ShowTips("暂未开放"); 
        });
    }
    private void InitialSkillPanel(string buttonPath)
    {
        SkillDamageType skillType = SkillDamageType.物理;
        if (buttonPath == "ChangePanel/Skill")
            skillType = SkillDamageType.物理;
        else if (buttonPath == "ChangePanel/Magic")
            skillType = SkillDamageType.魔法;
        else if (buttonPath == "ChangePanel/Aided")
            skillType = SkillDamageType.辅助;
        else
            return;
        InitialPanelChangeAided<SkillInfoPanel, SkillPanel>(buttonPath, SkillInfoPanel.prefabsPath, SkillPanel.prefabsPath, null,
        (leftPanel_Always) =>
        {
            leftPanel_Always.SetSkillType(skillType);
        }, null, 
        (rigthPanelAlways) => 
        {
            rigthPanelAlways.SetSkillType(skillType);
        });
    }
    private void InitialPanelChangeAided<T1, T2>
        (
        string buttonPath,string leftPanelName,string rightPanelName,
        UnityAction<T1> leftPanelFirst = null,
        UnityAction<T1> leftPanelAlways = null,
        UnityAction<T2> rightPanelFirst = null,
        UnityAction<T2> rightPanelAlways = null
        ) where T1 : MapPanel where T2 : MapPanel
    {
        Button itemBagChange = GetPanel(buttonPath).GetUI<Button>();
        itemBagChange.onClick.RemoveAllListeners();
        itemBagChange.onClick.AddListener(() => 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.change);
            if (leftCurrentPanel == leftPanelName && rightCurrentPanel == rightPanelName && this.buttonPath == buttonPath)//如果当前界面已经是目标界面，就跳过
                return;
            this.buttonPath = buttonPath;
            ChangePanel<T1, T2>(itemBagChange, leftPanelName, rightPanelName, leftPanelFirst, leftPanelAlways, rightPanelFirst, rightPanelAlways);
        });
    }

    private void ActiveCurrentButton(Button button,bool isActive)
    {
        button.transform.GetChild(0).gameObject.SetActive(!isActive);
        button.transform.GetChild(1).gameObject.SetActive(isActive);
    }

    public void ChangePanel<T1,T2>
        (
        Button targetButton,string leftTarget,string rightTarget,
        UnityAction<T1> leftPanelFirst = null,
        UnityAction<T1> leftPanelAlways = null,
        UnityAction<T2> rightPanelFirst = null,
        UnityAction<T2> rightPanelAlways = null
        ) where T1 :MapPanel where T2:MapPanel
    {
        if (leftCurrentPanel != "")
            HidePanel(leftCurrentPanel);
        if (rightCurrentPanel != "")
            HidePanel(rightCurrentPanel);
        if(currentButton != null)
            ActiveCurrentButton(currentButton,false);

        ActiveCurrentButton(targetButton, true);
        currentButton = targetButton;

        ShowPanelAsync<T1>(leftTarget, leftPanelFather, leftPanelFirst, (targetPanel)=>
        {
            leftCurrentPanel = leftTarget;
            if (leftPanelAlways != null)
                leftPanelAlways.Invoke(targetPanel);
        });
        ShowPanelAsync<T2>(rightTarget, rightPanelFather, rightPanelFirst, (targetPanel)=>
        { 
            rightCurrentPanel = rightTarget;
            if (rightPanelAlways != null)
                rightPanelAlways.Invoke(targetPanel);
        });
    }

    public override Dictionary<DetailButtonType, DetailButton> GetDetailButtons(Item item)
    {
        return null;
    }

    public override string GetPanelName()
    {
        return BagPanel.prefabsPath;
    }
}
