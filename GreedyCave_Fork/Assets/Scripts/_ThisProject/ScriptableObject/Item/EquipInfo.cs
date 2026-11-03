using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ScriptObject上面调用编辑器修改需要调用EditorUtility.SetDirty，不可调用EditorSceneManager.MarkSceneDirty
//因为MarkSceneDirty顾名思义是标记场景为已修改，但是编辑ScriptObject并不属于场景内数据，所以如果修改只可调用EditorUtility.SetDirty，不然会造成数据改动未生效。

[CreateAssetMenu(menuName ="MyDictionary/Item/EquipInfo",fileName = "NewEquip")]
public class EquipInfo : ItemInfo
{
    public EquipType equipType;
    public PropertyValue[] propertyFactors;
    public List<BuffInfo> buffInfo;
    public override Item GetItem(ItemQuality itemQuality, int level = 1)
    {
        Equip equip = new Equip(this, itemQuality,level);
        return equip;
    }

    //public override ItemType GetItemType()
    //{
    //    return ItemType.CannotCombine;
    //}

    public override string GetSpecialType()
    {
        return equipType.ToString();
    }
}
