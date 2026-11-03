using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IRatioObjAble
{
    public float GetRatio();
}
public interface IBootyHolderAble
{
    public Vector3 GetPosition();
    public float AdjustReward_Property(float targetValue, ref PropertyType targetType);
}

[System.Serializable]
public class Booty: IRatioObjAble
{
    public float bootyRatio = 1;
    public List<Reward_Property> reward_Property;
    [SerializeField]
    public List<ItemInfoAndRatio> itemList = new List<ItemInfoAndRatio>();
    
    public static Booty CopyBooty(Booty originalBooty)
    {
        Booty targetBooty = new Booty();
        if (originalBooty == null)
            return targetBooty;
        targetBooty.bootyRatio = originalBooty.bootyRatio;
        targetBooty.reward_Property = new List<Reward_Property>(originalBooty.reward_Property);
        targetBooty.itemList = new List<ItemInfoAndRatio>(originalBooty.itemList);

        return targetBooty;
    }

    public float GetRatio()
    {
        return bootyRatio;
    }
    public static Booty GetRandomBooty(List<Booty> bootys)
    {
        Dictionary<Booty,float> bootyRatioDic = RewardLibrary.RatioObjListToDic<Booty>(bootys);
        Booty booty = RewardLibrary.GetRandomType<Booty>(bootyRatioDic);
        return booty;
    }

    public static HashSet<string> GetAllItemName(Booty booty)
    {
        HashSet<string> nameHash = new HashSet<string>();
        for (int i = 0; i < booty.itemList.Count; i++)
        {
            if(booty.itemList[i] != null && booty.itemList[i].itemInfo != null)
            {
                string itemName = booty.itemList[i].itemInfo.GetName();
                if (nameHash.Contains(itemName) == false)
                    nameHash.Add(itemName);
            }
        }
        return nameHash;
    }
    
    public List<Reward_Property> GetReward_Property()
    {
        return reward_Property;
    }
    /// <summary>
    /// 此方法每次调用获取的都是随机的Item列表
    /// </summary>
    /// <returns></returns>
    public List<Item> GetItems()
    {
        List<Item> newItemInfo = new List<Item>();

        for (int i = 0; i < itemList.Count; i++)
        {
            float value = Random.Range(0, 1f);
            if (value <= itemList[i].fallRatio)
            {
                if (itemList[i] == null || itemList[i].itemInfo == null)
                {
                    Debug.Log("存在booty的物品列表中含有空物体，请检查预设");
                    continue;
                }
                Item item = itemList[i].itemInfo.GetItem();
                //item.SetItemCount(10);
                newItemInfo.Add(item);
            }
        }
        return newItemInfo;
    }

}
[System.Serializable]
public class Reward_Property: IShowSpriteAble
{
    public PropertyType rewardType = PropertyType.金币;
    public float baseValue;
    public float fluctuationRatio = 0;

    private Sprite coinSprite;
    public Sprite CoinSprite
    {
        get 
        {
            if (coinSprite == null)
                coinSprite = ResMgr.GetInstance().Load<Sprite>("Sprite/ExtraSprite/金币");
            return coinSprite; 
        }
    }

    private Sprite expSprite;
    public Sprite ExpSprite
    {
        get
        {
            if (expSprite == null)
                expSprite = ResMgr.GetInstance().Load<Sprite>("Sprite/ExtraSprite/经验");
            return expSprite;
        }
    }

    public Vector3 CenterPosOffset()
    {
        return Vector3.zero;
    }

    public Vector3 GetShowScale()
    {
        return Vector3.one;
    }

    public Sprite GetSprite()
    {
        if (rewardType == PropertyType.金币)
            return CoinSprite;
        else if(rewardType == PropertyType.经验值)
            return ExpSprite;
        return null;
    }

    public float GetValue()
    {
        float maxFluctuation = baseValue * fluctuationRatio;
        if (fluctuationRatio == 0)
            return baseValue;
        return Random.Range(baseValue - maxFluctuation, baseValue + maxFluctuation + 1);
    }
}

[System.Serializable]
public class ItemInfoAndRatio: IRatioObjAble
{
    public ItemInfoAndRatio(ItemInfo itemInfo, float fallRatio)
    {
        this.itemInfo = itemInfo;
        this.fallRatio = fallRatio;
    }

    public float GetRatio()
    {
        return fallRatio;
    }

    public ItemInfo itemInfo;
    [Range(0, 1)] public float fallRatio = 1;
}
