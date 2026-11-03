using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShortcutItemInfo : ItemInfo
{
    public float allCD;
    public abstract override Item GetItem(ItemQuality itemQuality, int level = 1);
    public abstract override string GetSpecialType();
}
