using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyDictionary/Item/ContainterProp", fileName = "NewContainterProp")]
public class ContainerPropInfo : ItemInfo
{
    public ItemType itemType;
    public ContainerType containterType;
    public int capacity;
    public override Item GetItem(ItemQuality itemQuality, int level = 1)
    {
        return new ContainerProp(this, itemQuality, 1);
    }
    //public override ItemType GetItemType()
    //{
    //    return itemType;
    //}
    public override string GetSpecialType()
    {
        return containterType.ToString();
    }
}
