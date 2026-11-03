using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ContainerType
{
    NormalBox,
    MagicBall,
}

public class ContainerProp : Item, IHolderAble
{
    public ContainerProp(ContainerPropInfo containterPropInfo, ItemQuality itemQuality, int count) :
    base(containterPropInfo, itemQuality, count)
    {
        this.InitialBox_Normal(containterPropInfo);
    }
    public ContainerProp(ContainerProp containterProp, int count)//用于复制
       : base(containterProp, count)
    {
        this.InitialBox_Normal(containterProp.itemInfo as ContainerPropInfo);
    }
    private void InitialBox_Normal(ContainerPropInfo containterPropInfo)
    {
        if (containterPropInfo == null)
        {
            Debug.Log("boxInfo为空");
            return;
        }
        this.containterType = containterPropInfo.containterType;
        this.capacity = containterPropInfo.capacity;
        this.bagMgr = new BagMgr<Item>(capacity);
        this.itemBaseInfo = "容量：" + containterPropInfo.capacity;
    }
    protected ContainerType containterType;
    protected int capacity;
    protected BagMgr<Item> bagMgr;
    public override ItemSaveData GetItemSaveData()
    {
        ItemSaveData itemSaveData = base.GetItemSaveData();
        itemSaveData.bagSaveData = PlayerDataMgr.GetBagSaveData(this.bagMgr.GetMainBag());
        return itemSaveData;
    }
    public override void LoadItemSaveData(ItemSaveData itemSaveData)
    {
        base.LoadItemSaveData(itemSaveData);
        this.bagMgr.SetMainBag(PlayerDataMgr.LoadBag(itemSaveData.bagSaveData, this));
    }
    public override Dictionary<DetailButtonType, DetailButton> DetailButtons
    {
        get
        {
            SetDetailButtons();
            return detailButtons;
        }
    }
    public ContainerType GetContainterType()
    {
        return containterType;
    }
    protected override void Initial_DecreaseRules(Item newItem)
    {
        ContainerProp prop = newItem as ContainerProp;
        if (prop == null)
            return;
        if (this.GetItemType() == ItemType.CannotCombine)//如果是不可堆叠的物品，就把背包复制
            prop.bagMgr = this.bagMgr;
    }
    public BagMgr<Item> GetBagMgr()
    {
        return bagMgr;
    }
    public override ItemType GetItemType()
    {
        if (bagMgr.GetMainBag().Count > 0)
            return ItemType.CannotCombine;
        else
            return ItemType.CanCombine;
    }
    public override ItemCategory GetItemCategory()
    {
        return ItemCategory.Sundary;
    }
    public HolderType GetHolderType()
    {
        return HolderType.Box;
    }
    public Item AddItem(Item item)
    {
        if (item == null)
            return null;

        item.holder = this;
        Item overflow = this.bagMgr.GetMainBag().AddItem(item);//内部已经把item更换过索引，对Item的处理必须在Add之前
        if (overflow != null)
            UIManager.GetInstance().ShowTips("该目标背包已满");

        RefreshBagPanel();
        return overflow;
    }
    public Item RemoveItem(Item item, int count = 1)
    {
        Item newItem = this.bagMgr.GetMainBag().RemoveItem(item, count);
        RefreshBagPanel();
        return newItem;
    }
    private void RefreshBagPanel()
    {
        BoxPanel boxPanel = (BoxPanel)UIManager.GetInstance().GetExistPanel(BoxPanel.prefabsPath);
        if (boxPanel == null)
            return;
        ContainerBagPanel boxBagPanel = (ContainerBagPanel)boxPanel.GetExistPanel(ContainerBagPanel.prefabsPath_Bag);
        if (boxBagPanel == null)
            return;
        if (boxBagPanel.gameObject.activeInHierarchy)//如果没有显现就不用刷新。界面每次显现的时候内部会刷新
            boxBagPanel.RefreshPanel();
    }
    public int GetCoin()
    {
        return 0;
    }
    public void ChangeCoin(int count)
    {

    }
    //public List<Item> GetBagItemAll()
    //{
    //    return this.bagMgr.GetMainBag().GetAllBagItems();
    //}
    public List<Item> GetBagItems(ItemCategory itemCategory)
    {
        return this.bagMgr.GetMainBag().GetCategoryBagItems(itemCategory.ToString(), null);
    }
    public Bag<Item> GetBag()
    {
        return this.bagMgr.GetMainBag();
    }

    public virtual string GetBagName()
    {
        return "箱子";
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

    public virtual Dictionary<string, string> GetForbidItemDic()
    {
        return null;
    }
}
