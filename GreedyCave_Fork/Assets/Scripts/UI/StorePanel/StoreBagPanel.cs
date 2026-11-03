using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreBagPanel : ContainerPanel<ItemButton, Item>
{
    public static string prefabsPath = "StoreBagPanel";

    private TargetPanel mainPanel;//item的直属父物体
    private ScrollRect itemScrollRect;
    private Merchant merchant;

    public override void ShowMeFirst()
    {
        itemScrollRect = GetPanel("Scroll View").GetUI<ScrollRect>();
        mainPanel = GetPanel("Scroll View/Viewport/BagContent");
    }

    public void SetMerchant(Merchant merchant)
    {
        if (merchant == null)
        {
            Debug.Log("商人为空");
            return;
        }
        this.merchant = merchant;
        this.merchant.GetBag().GetMainBag();
        this.RefreshPanel(this.merchant.GetBag().GetMainBag());
    }
    public override IHolderAble GetHolder()
    {
        return this.merchant;
    }
    public void RefreshPanel(List<Item> itemList)
    {
        this.RefreshContainer(itemList);
    }

    public override string GetCellPath()
    {
        return ItemButton.prefabsPath;
    }

    public override Transform GetContainterTrans()
    {
        return mainPanel.transform;
    }

    public override ScrollRect GetScrollRect()
    {
        return itemScrollRect;
    }

}
