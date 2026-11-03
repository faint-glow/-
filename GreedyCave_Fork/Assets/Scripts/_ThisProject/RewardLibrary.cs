using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardLibrary
{
    private ItemTypeLirary itemTypeLibrary = new ItemTypeLirary();
    public RewardLibrary(string equipPath)
    {
        EquipInfo[] equipInfos = ResMgr.GetInstance().LoadAll<EquipInfo>(equipPath);
        for (int i = 0; i < equipInfos.Length; i++)
        {
            AddItem(equipInfos[i]);
        }
    }
    public void AddItem(ItemInfo item)
    {
        itemTypeLibrary.AddItem(item);
    }
    /// <summary>
    /// 获取随机Item，对应类型如果设置为枚举类型为无,字符型为""则为随机
    /// </summary>
    /// <param name="itemQuality"></param>
    /// <param name="specialType"></param>
    /// <returns></returns>
    public Item GetRandomItem(ItemQuality itemQuality, string specialType,int level = 1)
    {
        return itemTypeLibrary.GetRandomItem(ItemType.无, itemQuality, specialType, level);
    }

    public static Dictionary<ItemQuality, float> baseItemQualityRatio = new Dictionary<ItemQuality, float>()
    {
        {ItemQuality.White,0.35f},
        {ItemQuality.Green,0.3f},
        {ItemQuality.Blue,0.25f},
        {ItemQuality.Purple,0.05f},
        {ItemQuality.Orange,0.035f},
        {ItemQuality.Red,0.015f},
        {ItemQuality.无,0},
    };
    public static Dictionary<ItemInfo, float> ItemInfoRatioToDictionary(List<ItemInfoAndRatio> itemInfoAndRatioList)
    {
        Dictionary<ItemInfo, float> itemInfoRatio = new Dictionary<ItemInfo, float>();
        for (int i = 0; i < itemInfoAndRatioList.Count; i++)
        {
            ItemInfo itemInfo = itemInfoAndRatioList[i].itemInfo;
            itemInfoRatio[itemInfo] = itemInfoAndRatioList[i].fallRatio;
        }
        return itemInfoRatio;
    }
    public static Dictionary<T, float> RatioObjListToDic<T>(List<T> ratioObjList) where T: IRatioObjAble
    {
        Dictionary<T, float> ratioObjAbleDic = new Dictionary<T, float>();
        for (int i = 0; i < ratioObjList.Count; i++)
        {
            if (ratioObjList[i] == null)
                continue;
            ratioObjAbleDic[ratioObjList[i]] = ratioObjList[i].GetRatio();
        }
        return ratioObjAbleDic;
    }
    public static Dictionary<T, float> AdjustRatio<T>(Dictionary<T,float> ratioDic, T type,float addRatio)
    {
        if (!ratioDic.ContainsKey(type))
        {
            Debug.Log("不包含该键");
            return ratioDic;
        }
        if (ratioDic.Count <= 1)//数量小于等于1没法调整
            return ratioDic;

        addRatio = Mathf.Clamp(addRatio, -1, 1);//addRatio是需要增加的概率值，比如我现在占比为0.1，addRatio为0.2，则经过此算法，T占比将为0.3

        float allNum = 0;
        foreach (var item in ratioDic)
        {
            allNum += item.Value;
        }
        float typeRatio = ratioDic[type] / allNum;

        Dictionary<T, float> newRatioDic = new Dictionary<T, float>(ratioDic);//复制一份用于返回，保证原数据不被破坏

        if (typeRatio + addRatio >= 1)//如果目标概率大于等于1，则之后的算法无效，没有意义，而且分母可能为0,。特殊处理
            newRatioDic = new Dictionary<T, float>() { { type, 1 } };
        else if (typeRatio + addRatio <= 0)//如果目标概率小于等于0，没有意义，特殊处理
            newRatioDic[type] = 0;
        else
        {
            float adjustNum = (addRatio * allNum) / (1 - typeRatio - addRatio);//数学列方程化简可得此式
            newRatioDic[type] += adjustNum;
        }

        #region 测试概率
        //Debug.Log("原概率" + typeRatio);
        //allNum = 0;
        //foreach (var item in newRatioDic)
        //{
        //    allNum += item.Value;
        //}
        //Debug.Log("现概率" + newRatioDic[type] / allNum);
        #endregion

        return newRatioDic;
    }

    //根据所有item的概率占总概率的占比来随机，也就是总概率数值可以大于1或者小于1
    public static T GetRandomType<T>(Dictionary<T, float> ratioDic)
    {
        Dictionary<float, T> edges = new Dictionary<float, T>();
        float num = 0;

        T type = default(T);
        //遍历枚举类型，根据宝箱设置的初始概率设置临界值

        foreach (var item in ratioDic)
        {
            type = item.Key;

            if (ratioDic[type] <= 0)//对类似 ItemQuality.无 情况的处理
                continue;

            num += ratioDic[type];
            edges[num] = type;
        }

        float randomNum = UnityEngine.Random.Range(0, num);
        //遍历所有临界值，随机数第一次小于临界值时的TreasureBoxType就是对应宝箱
        foreach (var item in edges)
        {
            if (randomNum < item.Key)
            {
                type = item.Value;
                break;
            }
        }
        return type;
    }
}

public class ItemTypeLirary
{
    private Dictionary<ItemType, ItemQualityLibrary> itemQualityLibrary = new Dictionary<ItemType, ItemQualityLibrary>();

    public void AddItem(ItemInfo item)
    {
        ItemType type = ItemType.无; //item.GetItemType();//item.itemType;
        if (!itemQualityLibrary.ContainsKey(type))
            itemQualityLibrary[type] = new ItemQualityLibrary();

        itemQualityLibrary[type].AddItem(item);
        //Debug.Log(itemQualityLibrary.Count);
    }

    //获取随机Item
    public Item GetRandomItem(ItemType itemType, ItemQuality itemQuality,string specialType,int level)
    {
        if (itemQualityLibrary.Count <= 0)
        {
            Debug.Log("itemQualityLibrary内容物数量为0");
            return null;
        }

        ItemQualityLibrary itemlibrary;
        if (itemType == ItemType.无)
        {
            int num = UnityEngine.Random.Range(0, itemQualityLibrary.Count);
            itemlibrary = itemQualityLibrary.ElementAt(num).Value;//通过下标访问元素
        }
        else
        {
            if (!itemQualityLibrary.ContainsKey(itemType))
            {
                return null;
            }
            itemlibrary = itemQualityLibrary[itemType];
        }
        return itemlibrary.GetRandomItem(itemQuality, specialType, level);
    }
}

public class ItemQualityLibrary
{
    private Dictionary<ItemQuality, ItemLibrary> itemLibrary = new Dictionary<ItemQuality, ItemLibrary>();

    public void AddItem(ItemInfo item)
    {
        for (int j = 0; j < item.itemQualitys.Count; j++)//不同品质的都创建一个副本
        {
            ItemQuality quality = item.itemQualitys[j];
            if (!itemLibrary.ContainsKey(quality))
                itemLibrary[quality] = new ItemLibrary();

            itemLibrary[quality].AddItem(item);
        }
    }

    //获取随机Item
    public Item GetRandomItem(ItemQuality itemQuality,string specialType,int level)
    {
        if (itemLibrary.Count <= 0)
        {
            Debug.Log("itemLibrary内容物数量为0");
            return null;
        }

        ItemLibrary itemlibrary;
        if (itemQuality == ItemQuality.无)
        {
            int num = UnityEngine.Random.Range(0, itemLibrary.Count);
            itemlibrary = itemLibrary.ElementAt(num).Value;//通过下标访问元素
        }
        else
        {
            if (!itemLibrary.ContainsKey(itemQuality))
            {
                //Debug.Log("itemLibrary\"" + itemQuality + "\"内容物数量为0");
                return null;
            }

            itemlibrary = itemLibrary[itemQuality];
        }

        return itemlibrary.GetRandomItem(specialType, itemQuality, level);
    }

}


public class ItemLibrary
{
    //这个字典是具体的Item类型(Sword，Cane,Medicament等)对应的列表，用于获取随机物品
    private Dictionary<string, List<ItemInfo>> itemListLibrary = new Dictionary<string, List<ItemInfo>>();

    public void AddItem(ItemInfo item)
    {
        string type = item.GetSpecialType();
        if (!itemListLibrary.ContainsKey(type))
            itemListLibrary[type] = new List<ItemInfo>();

        itemListLibrary[type].Add(item);
    }

    ////获取随机Item
    //public Item GetRandomItem()
    //{  
    //    if(itemListLibrary.Count <= 0)
    //    {
    //        Debug.Log("itemListLibrary内容物数量为0");
    //        return null;
    //    }

    //    int num = Random.Range(0, itemListLibrary.Count);
    //    List<Item> items = itemListLibrary.ElementAt(num).Value;//通过下标访问元素
    //    num = Random.Range(0, items.Count);
    //    return CreateItem(items[num]);
    //}

    //获取随机Item
    public Item GetRandomItem(string specialType,ItemQuality itemQuality, int level)
    {
        if (itemListLibrary.Count <= 0)
        {
            Debug.Log("itemListLibrary内容物数量为0");
            return null;
        }

        int num;
        List<ItemInfo> items;
        if (specialType == "" || specialType == null)
        {
            num = UnityEngine.Random.Range(0, itemListLibrary.Count);
            items = itemListLibrary.ElementAt(num).Value;//通过下标访问元素
        }
        else
        {
            if (!itemListLibrary.ContainsKey(specialType))
            {
                //Debug.Log("itemListLibrary \"" + specialType + "\"数量为0");
                return null;
            }
            items = itemListLibrary[specialType];
        }

        num = UnityEngine.Random.Range(0, items.Count);
        return CreateItem(items[num], itemQuality, level);
    }

    private Item CreateItem(ItemInfo item,ItemQuality itemQuality,int level)
    {
        return item.GetItem(itemQuality, level);
    }
}
