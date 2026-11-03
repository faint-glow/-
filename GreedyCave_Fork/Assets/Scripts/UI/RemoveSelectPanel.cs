using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public class RemoveSelectButtonInfo
//{
//    public Button button;
//    public bool isSelect;
//}
public enum RemoveSelectType
{
    ItemCategory,
    ItemQuality,
}
public class RemoveSelectPanel : MapPanel
{
    public static string prefabsPath = "RemoveSelectPanel";

    private IHolderAble targetHolder;
    private Dictionary<ItemCategory, bool> categorySelectDic = new Dictionary<ItemCategory, bool>();
    private Dictionary<ItemQuality, bool> qualitySelectDic = new Dictionary<ItemQuality, bool>() 
    {
        {ItemQuality.White,true },
        {ItemQuality.Green,true },
        {ItemQuality.Blue,true },
    };

    private Button confirm;
    private Button cancel;

    public override void ShowMeFirst()
    {
        Transform selectTransform = GetPanel("Main").transform;
        confirm = GetPanel("Confirm").GetUI<Button>();
        cancel = GetPanel("Cancel").GetUI<Button>();

        confirm.onClick.AddListener(()=>
        {
            if(targetHolder == null)//此时位丢弃
            {
                PlayerMgr.Instance.GetBagMgr().GetMainBag().RemoveBagItems((item) =>
                {
                    if (GetSelectInfo(item.itemQuality) == true && GetSelectInfo(item.GetItemCategory()) == true && item.IsLocked == false)
                        return true;

                    return false;
                });
                PlayerMgr.Instance.RefreshBagPanelHandler();
                UIManager.GetInstance().HidePanel(RemoveSelectPanel.prefabsPath);
            }
            else//此时为售卖
            {
                PlayerMgr.Instance.GetBagMgr().GetMainBag().ForeachAllItems((item)=> 
                {
                    if (GetSelectInfo(item.itemQuality) == true && GetSelectInfo(item.GetItemCategory()) == true && item.IsLocked == false)
                        ExchangePanel.ExchangeItem(targetHolder, item, item.GetCount());
                });
                UIManager.GetInstance().HidePanel(RemoveSelectPanel.prefabsPath);
            }
        });
        cancel.onClick.AddListener(()=> 
        {
            UIManager.GetInstance().HidePanel(RemoveSelectPanel.prefabsPath);
        });
    }
    public override void ShowMeEveryTime()
    {
        RemoveTargetHolder();
    }
    public void SetTargetHolder(IHolderAble targetHolder)
    {
        this.targetHolder = targetHolder;
    }
    public void RemoveTargetHolder()
    {
        this.targetHolder = null;
    }

    public void ChangeSelectInfo(RemoveSelectType type,int index)
    {
        if(type == RemoveSelectType.ItemQuality)
        {
            ItemQuality itemQuality = (ItemQuality)index;
            if (!qualitySelectDic.ContainsKey(itemQuality))
                qualitySelectDic[itemQuality] = false;
            qualitySelectDic[itemQuality] = !qualitySelectDic[itemQuality];
        }
        else if (type == RemoveSelectType.ItemCategory)
        {
            ItemCategory itemCategory = (ItemCategory)index;
            if (!categorySelectDic.ContainsKey(itemCategory))
                categorySelectDic[itemCategory] = false;

            categorySelectDic[itemCategory] = !categorySelectDic[itemCategory];
        }
    }
    public bool GetSelectInfo(RemoveSelectType removeSelectType,int index)
    {
        if(removeSelectType == RemoveSelectType.ItemQuality)
        {
            ItemQuality itemQuality = (ItemQuality)index;
            return GetSelectInfo(itemQuality);
        }
        else if(removeSelectType == RemoveSelectType.ItemCategory)
        {
            ItemCategory itemCategory = (ItemCategory)index;
            return GetSelectInfo(itemCategory);
        }
        return false;
    }
    private bool GetSelectInfo(ItemQuality itemQuality)
    {
        if (!qualitySelectDic.ContainsKey(itemQuality))
            qualitySelectDic[itemQuality] = false;

        return qualitySelectDic[itemQuality];
    }
    private bool GetSelectInfo(ItemCategory itemCategory)
    {
        if (!categorySelectDic.ContainsKey(itemCategory))
            categorySelectDic[itemCategory] = false;

        return categorySelectDic[itemCategory];
    }
}
