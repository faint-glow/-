using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperatePanel : BasePanel
{
    public static string prefabsPath = "OperatePanel";

    private Image satietyImage;
    public override void ShowMeFirst()
    {
        Button bagPanelButton = GetPanel("Main/Bag").GetUI<Button>();
        bagPanelButton.onClick.RemoveAllListeners();
        bagPanelButton.onClick.AddListener(() => 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.open);
            UIManager.GetInstance().ShowPanel<BagPanel>(BagPanel.prefabsPath);
        });

        Button setupPanelButton = GetPanel("Main/Setup").GetUI<Button>();
        setupPanelButton.onClick.RemoveAllListeners();
        setupPanelButton.onClick.AddListener(()=> 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            UIManager.GetInstance().ShowPanel<SetupPanel>(SetupPanel.prefabsPath); 
        });

        Button satietyButton = GetPanel("Main/PhysicsState/Satiety").GetUI<Button>();
        Button restButton = GetPanel("Main/PhysicsState/Rest").GetUI<Button>();
        satietyButton.onClick.AddListener(() => { UIManager.GetInstance().ShowTips("暂未开放"); });
        restButton.onClick.AddListener(() => { UIManager.GetInstance().ShowTips("暂未开放"); });

        satietyImage = GetPanel("Main/PhysicsState/Satiety/SatietyImage/Mask").GetUI<Image>();
    }

    private void Update()
    {
        float satietyUpLimit = PlayerMgr.Instance.GetWarrior().GetCurrentProperty(PropertyType.饱腹度上限);
        float satiety = PlayerMgr.Instance.GetWarrior().GetOriginalProperty(PropertyType.饱腹度);

        satietyImage.fillAmount = 1 - (satiety / satietyUpLimit);
    }
}
