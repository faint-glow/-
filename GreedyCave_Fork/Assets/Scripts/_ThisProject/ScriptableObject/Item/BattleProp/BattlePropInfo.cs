using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattlePropInfo : ShortcutItemInfo
{
    public List<Effect> effects = new List<Effect>();
    public abstract override Item GetItem(ItemQuality itemQuality, int level = 1);
    public abstract override string GetSpecialType();
}
