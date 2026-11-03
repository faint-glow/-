using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyDictionary/Item/ScrollProp", fileName = "NewScrollProp")]
public class ScrollPropInfo : ShortcutItemInfo
{
    public ScrollType scrollType;
    public override Item GetItem(ItemQuality itemQuality, int level = 1)
    {
        return new ScrollProp(this, itemQuality, 1);
    }
    public override string GetSpecialType()
    {
        return scrollType.ToString();
    }
}
