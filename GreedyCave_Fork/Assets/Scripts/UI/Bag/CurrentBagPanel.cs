using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BagPanelType
{
    NormalPanel,
    StorePanel,
    StrengthPanel,
    BoxPanel,
    DiscardPanel,
}
public abstract class CurrentBagPanel : BasePanel
{
    public static CurrentBagPanel currentBasePanel;
    public static BagPanelType currentBagPanelType;

    public override void ShowMeEveryTimesHandler()
    {
        base.ShowMeEveryTimesHandler();
        SetCurrentBagPanel(this, GetBagPanelType());
    }
    protected abstract BagPanelType GetBagPanelType();

    public static void HideCurrentBagPanel()
    {
        if(currentBasePanel != null)
            UIManager.GetInstance().HidePanel(currentBasePanel.GetPanelName());
    }
    private void SetCurrentBagPanel(CurrentBagPanel basePael, BagPanelType bagPanelType)
    {
        if (currentBasePanel != null && currentBasePanel.GetPanelName() != GetPanelName())
            UIManager.GetInstance().HidePanel(currentBasePanel.GetPanelName());
        currentBasePanel = basePael;
        currentBagPanelType = bagPanelType;
    }
    //返回null则使用item默认的详细信息按钮，也就是正常打开背包的效果
    public abstract string GetPanelName();
    public abstract Dictionary<DetailButtonType, DetailButton> GetDetailButtons(Item item);
}
