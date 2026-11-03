using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscardPanel : CurrentBagPanel,IHolderAble
{
    public static string prefabsPath = "DiscardPanel";
    private Transform leftPanelFather;
    private Transform rightPanelFather;

    private ContainerBagPanel boxBagPanel;
    private ItemBagPanel itemBagPanel;

    private BagMgr<Item> bagMgr = new BagMgr<Item>();
    public override void ShowMeFirst()
    {
        Button closeButton = GetPanel("ClosePanel").GetUI<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.close);
            UIManager.GetInstance().HidePanel(DiscardPanel.prefabsPath);
        });
    }
    public override void ShowMeEveryTime()
    {
        rightPanelFather = GetPanel("Right/Main").transform;
        leftPanelFather = GetPanel("Left/Main").transform;
        ShowPanelAsync<ContainerBagPanel>(ContainerBagPanel.prefabsPath_Discard, leftPanelFather);//先显示这个面板，那么这个面板会先被隐藏
        ShowPanelAsync<ItemBagPanel>(ItemBagPanel.prefabsPath_Discard, rightPanelFather, (panel) => { this.itemBagPanel = panel; });//等待物品背包被显示后，刷新过玩家背包，再显示其他背包进行移除魔法球类型的物品的操作
        ShowPanelAsync<ContainerBagPanel>(ContainerBagPanel.prefabsPath_Discard, leftPanelFather, (panel) => { this.boxBagPanel = panel; }, (panel) => { panel.SetHolder(this); });
    }
    public BagMgr<Item> GetBagMgr()
    {
        return bagMgr;
    }
    public Bag<Item> GetBag()
    {
        return bagMgr.GetMainBag();
    }
    public HolderType GetHolderType()
    {
        return HolderType.Discard;
    }
    public Item AddItem(Item item)
    {
        item.holder = this;
        Item ovelflow = bagMgr.GetMainBag().AddItem(item);
        if (boxBagPanel != null)
            boxBagPanel.RefreshPanel();
        return ovelflow;
    }
    public Item RemoveItem(Item item, int count = 1)
    {
        Item ovelflow = bagMgr.GetMainBag().RemoveItem(item, count);
        if (boxBagPanel != null)
            boxBagPanel.RefreshPanel();
        return ovelflow;
    }
    public int GetCoin()
    {
        return 0;
    }
    public void ChangeCoin(int count)
    {
        
    }
    public override void HideMe()
    {
        bagMgr.GetMainBag().Clear();//隐藏的时候清空背包
    }

    protected override BagPanelType GetBagPanelType()
    {
        return BagPanelType.DiscardPanel;
    }
    public override Dictionary<DetailButtonType, DetailButton> GetDetailButtons(Item item)
    {
        Dictionary<DetailButtonType, DetailButton> detailButton = new Dictionary<DetailButtonType, DetailButton>();
        if (item.holder.GetHolderType() == HolderType.Player)
        {
            detailButton[DetailButtonType.Up] = null;
            detailButton[DetailButtonType.Middle] = null;
            detailButton[DetailButtonType.Down] = new DetailButton("丢弃", Color.white, () =>
            {
                UIManager.GetInstance().ShowPanel<AbandonPanel>(AbandonPanel.prefabsPath, PanelLayer.Max, null, (panel) =>
                {
                    panel.SetItem(boxBagPanel.GetHolder(), item);
                });

                UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
            });
        }
        else
        {
            detailButton[DetailButtonType.Up] = null;
            detailButton[DetailButtonType.Middle] = null;
            detailButton[DetailButtonType.Down] = new DetailButton("取出", Color.white, () =>
            {
                UIManager.GetInstance().ShowPanel<AbandonPanel>(AbandonPanel.prefabsPath, PanelLayer.Max, null, (panel) =>
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
        return DiscardPanel.prefabsPath;
    }

    public string GetBagName()
    {
        return "丢弃背包";
    }
    public int GetItemPrice(Item item)
    {
        if (item == null)
            return 0;
        return item.GetPrice();
    }

    public void RefreshBag()
    {

    }

    public Dictionary<string, string> GetForbidItemDic()
    {
        return null;
    }
}
