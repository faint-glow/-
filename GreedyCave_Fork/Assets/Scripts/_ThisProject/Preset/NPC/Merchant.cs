using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[RequireComponent(typeof(BoxCollider2D))]
public class Merchant : NPC, IHolderAble,ISavePresetObjAble
{
    private BagMgr<Item> bagMgr = new BagMgr<Item>();
    private Dictionary<string,int> originalItemDic;
    private void Start()
    {
        originalItemDic = InitialBag();
    }

    public void ClearRuntimeData()
    {
        bagMgr = new BagMgr<Item>();
        originalItemDic = InitialBag();
    }
    protected virtual Dictionary<string, int> InitialBag()
    {
        return null;
    }
    protected override void InitialBaseChoice_Code()
    {
        Choice choice = new Choice();
        choice.SetAction("购物", OpenStorePanel);

        AddBaseChoice_Code(choice);
    }
    public void ClearImportItem()
    {
        if (originalItemDic == null || originalItemDic.Count <= 0)
            return;
        bagMgr.GetMainBag().RemoveBagItems((item) => 
        {
            if (originalItemDic.ContainsKey(item.GetName()))
            {
                if (item.GetCount() > originalItemDic[item.GetName()])
                    item.SetItemCount(originalItemDic[item.GetName()]);
                return false;
            }
            return true;
        });
        
    }
    protected void OpenStorePanel()
    {
        UIManager.GetInstance().ShowPanelAsync<StorePanel>(StorePanel.prefabsPath, PanelLayer.Top,null, SetMerchant);
    }
    public void SetMerchant(StorePanel panel)
    {
        panel.SetMerchant(this);
    }
    public HolderType GetHolderType()
    {
        return HolderType.Merchant;
    }
    public Bag<Item> GetBag()
    {
        return bagMgr.GetMainBag();
    }
    public Item AddItem(Item item)
    {
        if (item == null)
            return null;

        item.holder = this;
        bagMgr.GetMainBag().AddItem(item);//商人背包无限,不处理添加失败的情况
        RefreshBagPanel();
        return null;
    }
    public Item RemoveItem(Item item, int count = 1)
    {
        Item newItem = bagMgr.GetMainBag().RemoveItem(item,count);
        RefreshBagPanel();
        return newItem;
    }
    private void RefreshBagPanel()
    {
        StorePanel bagPanel = (StorePanel)UIManager.GetInstance().GetExistPanel(StorePanel.prefabsPath);
        if (bagPanel == null)
            return;
        StoreBagPanel storeBagPanel = (StoreBagPanel)bagPanel.GetExistPanel(StoreBagPanel.prefabsPath);
        if (storeBagPanel == null)
            return;
        if (storeBagPanel.gameObject.activeInHierarchy)//如果没有显现就不用刷新。界面每次显现的时候内部会刷新
            storeBagPanel.RefreshPanel(bagMgr.GetMainBag().GetMainBag());
    }
    public int GetCoin()
    {
        return int.MaxValue;
    }
    public void ChangeCoin(int count)
    {
        
    }
    public int GetItemPrice(Item item)
    {
        if (item == null)
            return 0;
        int qualityValue = (int)item.itemQuality;
        float ratio = 1 + qualityValue * qualityValue / 10.0f;//售卖价格为物品原本价格的（2 + 质量的指数偏移）倍
        return (int)(item.GetPrice() * ratio);
    }
    public BagMgr<Item> GetBagMgr()
    {
        return bagMgr;
    }
    public string GetBagName()
    {
        return "商店";
    }
    public void RefreshBag()
    {
        ClearImportItem();
        GetBag().RefreshBag();
    }
    public string GetSaveTag()
    {
        return name_NPC;
    }
    public virtual string GetSaveContentInfo()
    {
        BagSaveData bagSaveData = PlayerDataMgr.GetBagSaveData(bagMgr.GetMainBag());
        string content = JsonConvert.SerializeObject(bagSaveData);
        return content;
    }
    public void LoadSaveInfo(string content)
    {
        BagSaveData bagSaveData = (BagSaveData)JsonConvert.DeserializeObject(content,typeof(BagSaveData));
        bagMgr.SetMainBag(PlayerDataMgr.LoadBag(bagSaveData, this));
    }
    //是否在切换场景时自动保存和自动加载
    public virtual bool IsSaveOnChangeScene()
    {
        return false;
    }
    public Dictionary<string, string> GetForbidItemDic()
    {
        return null;
    }

}
