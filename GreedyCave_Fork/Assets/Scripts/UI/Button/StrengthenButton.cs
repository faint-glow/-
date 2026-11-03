//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class StrengthenButton : DragPanel, IDragUpAble<IDragAble>, IHolderAble
//{
//    private Item item;
//    public IHolderAble originalItemHolder;

//    private StrengthenPanel strengthenPanel;
//    private Button clickButton;
//    private Image itemImage;
//    private bool isMaterial = true;
//    private TMP_Text strengthenLevel;
//    public override void ShowMeFirst()
//    {
//        strengthenPanel = (StrengthenPanel)UIManager.GetInstance().GetExistPanel(StrengthenPanel.prefabsPath);
//        itemImage = transform.Find("Item").gameObject.GetComponent<Image>();
//        Transform strengthenLevelTrans = transform.Find("Text (TMP)");
//        if (strengthenLevelTrans != null)
//            this.strengthenLevel = strengthenLevelTrans.GetComponent<TMP_Text>();

//        itemImage.color = Color.clear;

//        clickButton = ThisPanel.GetUI<Button>();
//        clickButton.onClick.AddListener(() =>
//        {
//            if (item != null)
//            {
//                UIManager.GetInstance().ShowPanelAsync<DetailPanel>(DetailPanel.prefabsPath, PanelLayer.Top_Up, null, (panel) =>
//                   {
//                       panel.SetItem(item);
//                   });
//            }
//        });
//    }

//    public void SetButtonType(bool isMaterial)
//    {
//        this.isMaterial = isMaterial;
//    }
//    public bool IsEmpty()
//    {
//        return item == null;
//    }
//    public IDragAble SetItem_Drag(IDragAble obj)//实际效果
//    {
//        return AddMaterial(obj as Item);
//    }
//    //如果从我这个格子拖到的目标位置上有溢出物体，我就对这个溢出物体进行处理
//    public override void OperiateOverflowItem_Drag(IDragAble drageAble)
//    {
//        AddMaterial(drageAble as Item);
//    }
//    public Item AddItem(Item item)
//    {
//        return AddMaterial(item);
//    }
//    //实际的移除
//    public Item RemoveItem(Item item, int count = 1)
//    {
//        return RemoveMaterial();
//    }
//    //UI方面的添加
//    private Item AddMaterial(Item newItem)
//    {
//        if (newItem == null)
//        {
//            RemoveMaterial();
//            return null;
//        }

//        if (isMaterial && !(newItem is StrengthenMaterial))
//        {
//            UIManager.GetInstance().ShowTips("该格子只能放入材料");
//            return null;
//        }
//        if (!isMaterial && !(newItem is Equip))
//        {
//            UIManager.GetInstance().ShowTips("该格子只能放入装备");
//            return null;
//        }
//        if(newItem.IsLocked && newItem is StrengthenMaterial)
//        {
//            UIManager.GetInstance().ShowTips(Item.lockedTip);
//            return null;
//        }

//        if (newItem.holder.GetHolderType() != this.GetHolderType())
//        {
//            originalItemHolder = newItem.holder;
//            newItem = newItem.holder.RemoveItem(newItem);
//        }
//        Item oldItem = this.item;
//        if (this.item != null)
//            RemoveMaterial();

//        this.item = newItem;
//        this.item.holder = this;
//        this.itemImage.sprite = newItem.itemSprite;
//        this.itemImage.color = Color.white;
//        this.strengthenPanel.AddMaterial(newItem as StrengthenMaterial);
//        this.strengthenPanel.RefreshPanel();

//        if (strengthenLevel != null)
//        {
//            Equip equip = item as Equip;
//            if (equip != null && equip.GetStrengthenLevel() >= 1)
//                strengthenLevel.text = "+" + equip.GetStrengthenLevel();
//            else
//                strengthenLevel.text = "";
//        }
//        return oldItem;
//    }
//    //UI方面的移除
//    public Item RemoveMaterial()
//    {
//        if (this.item == null)
//            return null;

//        Item thisItem = this.item;
//        this.strengthenPanel.RemoveMaterial(this.item as StrengthenMaterial);

//        this.item = null;
//        this.itemImage.color = Color.clear;
//        this.strengthenPanel.RefreshPanel();

//        if(this.strengthenLevel != null)
//            this.strengthenLevel.text = "";
//        return thisItem;
//    }
//    public void Refresh()
//    {
//        if (strengthenLevel != null && item != null)
//        {
//            Equip equip = item as Equip;
//            if (equip != null && equip.GetStrengthenLevel() >= 1)
//                strengthenLevel.text = "+" + equip.GetStrengthenLevel();
//            else
//                strengthenLevel.text = "";
//        }
//    }

//    //归还给实际持有者
//    public void ReturnItem()
//    {
//        Item item = this.RemoveMaterial();
//        if (item != null)
//            originalItemHolder.AddItem(item);
//    }
//    public Item GetItem()
//    {
//        return this.item;
//    }
//    public HolderType GetHolderType()
//    {
//        return HolderType.Farrier;
//    }

//    public int GetCoin()
//    {
//        return 0;
//    }

//    public void ChangeCoin(int count)
//    {

//    }
//    public override ScrollRect GetScrollRect()
//    {
//        return null;
//    }

//    public override IDragAble GetItem_Drag()
//    {
//        return this.item;
//    }
//    public bool IsCapacityFull()
//    {
//        return false;
//    }
//    public Bag<Item> GetBag()
//    {
//        return null;
//    }
//    public BagMgr<Item> GetBagMgr()
//    {
//        return null;
//    }

//    public string GetBagName()
//    {
//        return "强化格子";
//    }

//    public int GetItemPrice(Item item)
//    {
//        if (item == null)
//            return 0;
//        return item.GetPrice();
//    }
//}



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StrengthenButton : DragPanel, IDragUpAble<IDragAble>, IHolderAble, ITemporaryBagAble
{
    private Item item;
    public IHolderAble originalItemHolder;

    private StrengthenPanel strengthenPanel;
    private Button clickButton;
    private Image itemImage;
    private bool isMaterial = true;
    private TMP_Text strengthenLevel;
    public override void ShowMeFirst()
    {
        strengthenPanel = (StrengthenPanel)UIManager.GetInstance().GetExistPanel(StrengthenPanel.prefabsPath);
        itemImage = transform.Find("Item").gameObject.GetComponent<Image>();
        Transform strengthenLevelTrans = transform.Find("Text (TMP)");
        if (strengthenLevelTrans != null)
            this.strengthenLevel = strengthenLevelTrans.GetComponent<TMP_Text>();

        itemImage.color = Color.clear;

        clickButton = ThisPanel.GetUI<Button>();
        clickButton.onClick.AddListener(() =>
        {
            if (item != null)
            {
                UIManager.GetInstance().ShowPanelAsync<DetailPanel>(DetailPanel.prefabsPath, PanelLayer.Top_Up, null, (panel) =>
                {
                    panel.SetItem(item);
                });
            }
        });
    }

    public void SetButtonType(bool isMaterial)
    {
        this.isMaterial = isMaterial;
    }
    public bool IsEmpty()
    {
        return item == null;
    }
    public IDragAble SetItem_Drag(IDragAble obj)//实际效果
    {
        return AddMaterial(obj as Item);
    }
    //如果从我这个格子拖到的目标位置上有溢出物体，我就对这个溢出物体进行处理
    public override void OperiateOverflowItem_Drag(IDragAble drageAble)
    {
        AddMaterial(drageAble as Item);
    }
    public Item AddItem(Item item)
    {
        return AddMaterial(item);
    }
    //实际的移除
    public Item RemoveItem(Item item, int count = 1)
    {
        return RemoveMaterial();
    }
    //UI方面的添加
    private Item AddMaterial(Item newItem)
    {
        if (newItem == null)
        {
            RemoveMaterial();
            return null;
        }

        if (isMaterial && !(newItem is StrengthenMaterial))
        {
            UIManager.GetInstance().ShowTips("该格子只能放入材料");
            return null;
        }
        if (!isMaterial && !(newItem is Equip))
        {
            UIManager.GetInstance().ShowTips("该格子只能放入装备");
            return null;
        }
        if (newItem.IsLocked && newItem is StrengthenMaterial)
        {
            UIManager.GetInstance().ShowTips(Item.lockedTip);
            return null;
        }

        if (newItem.holder.GetHolderType() != this.GetHolderType())
        {
            originalItemHolder = newItem.holder;
            newItem = newItem.holder.GetBagMgr().UseTemporaryBag(this, newItem);
            originalItemHolder.RefreshBag();

            //List<Item> mainBag = originalItemHolder.GetBagMgr().GetMainBag().GetBagItems();
            //Debug.Log(mainBag.Count);
            //for (int i = 0; i < mainBag.Count; i++)
            //{
            //    if (mainBag[i] == null)
            //        Debug.Log("Null");
            //    Debug.Log(mainBag[i].GetName());
            //}

            //newItem = newItem.holder.RemoveItem(newItem);
        }
        Item oldItem = this.item;
        if (this.item != null)
            RemoveMaterial();

        this.item = newItem;
        this.item.holder = this;
        this.itemImage.sprite = newItem.itemSprite;
        this.itemImage.color = Color.white;
        this.strengthenPanel.AddMaterial(newItem as StrengthenMaterial);
        this.strengthenPanel.RefreshPanel();

        if (strengthenLevel != null)
        {
            Equip equip = item as Equip;
            if (equip != null && equip.GetStrengthenLevel() >= 1)
                strengthenLevel.text = "+" + equip.GetStrengthenLevel();
            else
                strengthenLevel.text = "";
        }
        return oldItem;
    }
    //UI方面的移除
    public Item RemoveMaterial(bool isRemoveTemporaryBag = true)
    {
        if (this.item == null)
            return null;

        Item thisItem = this.item;
        this.strengthenPanel.RemoveMaterial(this.item as StrengthenMaterial);

        if(isRemoveTemporaryBag == true)
        {
            Dictionary<ITemporaryBagAble, Bag<Item>> dic = this.originalItemHolder.GetBagMgr().GetTemporaryBagDic();
            if (dic != null && dic.ContainsKey(this))
                dic[this].RemoveItem(thisItem, thisItem.GetCount());
        }

        this.item = null;
        this.itemImage.color = Color.clear;
        this.strengthenPanel.RefreshPanel();

        if (this.strengthenLevel != null)
            this.strengthenLevel.text = "";
        return thisItem;
    }
    public void Refresh()
    {
        if (strengthenLevel != null && item != null)
        {
            Equip equip = item as Equip;
            if (equip != null && equip.GetStrengthenLevel() >= 1)
                strengthenLevel.text = "+" + equip.GetStrengthenLevel();
            else
                strengthenLevel.text = "";
        }
    }

    //归还给实际持有者
    public void ReturnItem()
    {
        Item item = this.RemoveMaterial(false);
        if (originalItemHolder == null)
            return;
        if (item != null)
        {
            item.holder = originalItemHolder;
            originalItemHolder.GetBagMgr().UseEndTemporaryBag(this);//.AddItem(item);
            originalItemHolder.RefreshBag();
        }
        else
        {
            originalItemHolder.GetBagMgr().UseEndTemporaryBag(this);//.AddItem(item);
        }
    }
    public Item GetItem()
    {
        return this.item;
    }
    public HolderType GetHolderType()
    {
        return HolderType.Farrier;
    }

    public int GetCoin()
    {
        return 0;
    }

    public void ChangeCoin(int count)
    {

    }
    public override ScrollRect GetScrollRect()
    {
        return null;
    }

    public override IDragAble GetItem_Drag()
    {
        return this.item;
    }
    public bool IsCapacityFull()
    {
        return false;
    }
    public Bag<Item> GetBag()
    {
        return null;
    }
    public BagMgr<Item> GetBagMgr()
    {
        return null;
    }

    public string GetBagName()
    {
        return "强化格子";
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

