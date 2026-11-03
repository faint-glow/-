using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyDictionary/Item/FoodProp", fileName = "NewFood")]
public class FoodPropInfo : BattlePropInfo
{
    public override Item GetItem(ItemQuality itemQuality, int level = 1)
    {
        return new Food(this, itemQuality, 1);
    }
    public override string GetSpecialType()
    {
        return PropType.Food.ToString();
    }
}
