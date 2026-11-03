using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemInfo : ScriptableObject,INameAble
{
    public string itemName;
    public Sprite itemSprite;
    public bool isNeverDrop;//是否绝不掉落
    public int basePrice = 100;
    [Header("可能出现的品质及其概率，概率总和不为1，也会按总概率值的占比来计算概率")]
    public List<ItemQuality> itemQualitys = new List<ItemQuality>() { ItemQuality.White};
    [TextArea(2, 5)]//最小显示两行，字多了以后最多显示5行，5行之后变成滚动条
    public string itemBaseInfo;
    [TextArea(2, 5)]
    public string itemExtraInfo;

    /// <summary>
    /// 返回最低级别的品质
    /// </summary>
    public ItemQuality GetLostQuality()
    {
        ItemQuality lostQuality = ItemQuality.Red;
        for (int i = 0; i < itemQualitys.Count; i++)
        {
            if ((int)itemQualitys[i] < (int)lostQuality)
            {
                lostQuality = itemQualitys[i];
            }
        }
        return lostQuality;
    }
    /// <summary>
    /// 获取该物品有可能出现的品质的物品，品质概率为默认概率值
    /// </summary>
    /// <param name="level">物品等级</param>
    /// <returns></returns>
    public Item GetItem(int level = 1)
    {
        ItemQuality targetQuality;
        Dictionary<ItemQuality, float> newRatioDic = new Dictionary<ItemQuality, float>();
        for (int i = 0; i < itemQualitys.Count; i++)
        {
            targetQuality = itemQualitys[i];
            newRatioDic[targetQuality] = RewardLibrary.baseItemQualityRatio[targetQuality];
        }
        targetQuality = RewardLibrary.GetRandomType<ItemQuality>(newRatioDic);

        return GetItem(targetQuality, level);
    }
    //获取指定品质的物品
    public abstract Item GetItem(ItemQuality itemQuality, int level = 1);
    //public abstract ItemType GetItemType();
    public abstract string GetSpecialType();

    public string GetName()
    {
        return itemName;
    }
}

[System.Serializable]
public class ItemQualityAndRatio
{
    public ItemQualityAndRatio(ItemQuality itemQuality, float fallRatio)
    {
        this.itemQuality = itemQuality;
        this.fallRatio = fallRatio;
    }

    public ItemQuality itemQuality;
    public float fallRatio;
}
