using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectLayerButtonInfo : IGetAble
{
    public int targetLayer;
}
public class SelectLayerButton : MapPanel, ISetAble<IGetAble>
{
    public static string prefabsPath = "Button/SelectLayerButton";

    public MapPanel FatherPanel { get; set; }
    public int Index { get; set; }
    private Button clickButton;
    private TMP_Text buttonText;

    public override void ShowMeFirst()
    {
        clickButton = ThisPanel.GetUI<Button>();
        buttonText = GetPanel("Text (TMP)").GetUI<TMP_Text>();
    }
    public void SetItem_Container(IGetAble obj)
    {
        SelectLayerButtonInfo buttonInfo = obj as SelectLayerButtonInfo;
        if (buttonInfo == null)
        {
            buttonText.text = "暂未开放";
            clickButton.onClick.RemoveAllListeners();
            clickButton.onClick.AddListener(() =>
            {
                UIManager.GetInstance().ShowTips("暂未开放");
            });
        }

        buttonText.text = "第" + buttonInfo.targetLayer + "层";
        if (PlayerMgr.Instance.GetPlayerData().maxLayerCount >= buttonInfo.targetLayer)
            buttonText.color = Color.white;
        else
            buttonText.color = Color.grey;

        clickButton.onClick.RemoveAllListeners();
        clickButton.onClick.AddListener(() =>
        {
            if (PlayerMgr.Instance.GetPlayerData().maxLayerCount >= buttonInfo.targetLayer)
            {
                GameManager.Instance.CreateNormalMapInfo(buttonInfo.targetLayer-1);

                GameManager.Instance.ChangeSceneAfterVignette(GameManager.Instance.GetMapInfo().LoungeSceneName);
                SelectLayerPanel selectLayerPanel = FatherPanel as SelectLayerPanel;
                if (selectLayerPanel != null)
                    selectLayerPanel.HidePanel();
            }
            else
            {
                UIManager.GetInstance().ShowTips("未解锁");
            }

            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
        });
    }
}
