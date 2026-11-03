using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorePanel : CurrentBagPanel
{
    public static string prefabsPath = "StorePanel";

    private ItemBagPanel itemBagPanel;
    private StoreBagPanel storeBagPanel;

    private Merchant merchant;
    public override void ShowMeFirst()
    {
        Button closeButton = GetPanel("ClosePanel").GetUI<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.close);
            UIManager.GetInstance().HidePanel(StorePanel.prefabsPath);
        });
    }
    protected override BagPanelType GetBagPanelType()
    {
        return BagPanelType.StorePanel;
    }
    public Merchant GetMerchant()
    {
        return this.merchant;
    }
    public void SetMerchant(Merchant merchant)
    {
        this.merchant = merchant;

        Transform storePanelFather = GetPanel("Left/Main").transform;
        ShowPanelAsync<StoreBagPanel>(StoreBagPanel.prefabsPath, storePanelFather, null, (panel) =>
        {
            storeBagPanel = panel;
            storeBagPanel.SetMerchant(this.merchant);
        });
        Transform bagPanelFather = GetPanel("Right/Main").transform;
        ShowPanelAsync<ItemBagPanel>(ItemBagPanel.prefabsPath_Storage, bagPanelFather, (panel) =>
        {
            itemBagPanel = panel;
        });
    }

    public override Dictionary<DetailButtonType, DetailButton> GetDetailButtons(Item item)
    {
        Dictionary<DetailButtonType, DetailButton> detailButton = new Dictionary<DetailButtonType, DetailButton>();
        if (item.holder.GetHolderType() == HolderType.Player)
        {
            detailButton[DetailButtonType.Up] = null;
            detailButton[DetailButtonType.Middle] = null;
            detailButton[DetailButtonType.Down] = new DetailButton("³öÊÛ", Color.white, () =>
            {
                if (item == null)
                    return;
                if(item.IsLocked == true)
                {
                    UIManager.GetInstance().ShowTips(Item.lockedTip);
                    return;
                }
                UIManager.GetInstance().ShowPanel<TradePanel>(TradePanel.prefabsPath, PanelLayer.Max, null, (panel) =>
                {
                    panel.SetItem(storeBagPanel.GetHolder(), item);
                });

                UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
            });
        }
        else
        {
            detailButton[DetailButtonType.Up] = null;
            detailButton[DetailButtonType.Middle] = null;
            detailButton[DetailButtonType.Down] = new DetailButton("¹ºÂò", Color.white, () =>
            {
                UIManager.GetInstance().ShowPanel<TradePanel>(TradePanel.prefabsPath, PanelLayer.Max, null, (panel) =>
                {
                    panel.SetItem(itemBagPanel.GetHolder(), item);
                });

                UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
            });
        }
        return detailButton;
    }

    public override string GetPanelName()
    {
        return StorePanel.prefabsPath;
    }

    public override void HideMe()
    {
        if (this.merchant != null)
            this.merchant.RefreshBag();
    }
}
