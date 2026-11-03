using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class TestButtonInfo : IGetAble
{
    public TestButtonInfo(UnityAction action, string actionName = "")
    {
        this.actionName = actionName;
        this.action = action;
    }
    public string actionName;
    public UnityAction action;
}
public class TestButton : MapPanel, ISetAble<IGetAble>
{
    public static string prefabsPath = "Test/TestButton";

    public int Index { get; set; }
    public MapPanel FatherPanel { get; set; }

    private Button actionButton;
    private TMP_Text actionName;
    public override void ShowMeFirst()
    {
        actionButton = ThisPanel.GetUI<Button>();
        actionName = GetPanel("ActionName").GetUI<TMP_Text>();
    }
    public void SetItem_Container(IGetAble obj)
    {
        TestButtonInfo testButtonInfo = obj as TestButtonInfo;
        if (testButtonInfo == null || testButtonInfo.action == null)
        {
            actionName.text = "空";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() =>
            {
                UIManager.GetInstance().ShowTips("该按钮未设置事件");
            });
            return;
        }
        if (testButtonInfo.actionName == "")
            actionName.text = "方法:" + Index;
        else
            actionName.text = testButtonInfo.actionName;
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(testButtonInfo.action);
    }
}
