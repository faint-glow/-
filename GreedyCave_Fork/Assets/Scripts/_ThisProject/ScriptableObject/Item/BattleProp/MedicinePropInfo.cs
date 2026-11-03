using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyDictionary/Item/MedicinePropInfo", fileName = "NewMedicine")]
public class MedicinePropInfo : BattlePropInfo
{
    public override Item GetItem(ItemQuality itemQuality, int level = 1)
    {
        return new Medicine(this, itemQuality, 1);
    }
    public override string GetSpecialType()
    {
        return PropType.Medicine.ToString();
    }
}