using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialInfo
{
    public PropertyType propertyType;
    public float minNum;
    public float maxNum;
    [Header("总成功率的占比，所有元素比率合计应等于1")]
    public float ratio;

    public bool MyEquals(MaterialInfo other)
    {
        if (propertyType != other.propertyType)
            return false;
        if (minNum != other.minNum)
            return false;
        if (maxNum != other.maxNum)
            return false;
        return true;
    }
}
[CreateAssetMenu(menuName = "MyDictionary/Item/StrengthenMaterial", fileName = "NewStrengthenMaterial")]
public class StrengthenMaterialInfo : ItemInfo
{
    public MaterialInfo[] materialInfo;

    public override Item GetItem(ItemQuality itemQuality, int level = 1)
    {
        return new StrengthenMaterial(this, itemQuality, 1);
    }

    public override string GetSpecialType()
    {
        return PropType.Material.ToString();
    }
}
