//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//[CreateAssetMenu(menuName = "MyDictionary/Item/PropInfo", fileName = "NewProp")]
//public class PropInfo : ItemInfo
//{
//    public PropType propType;
//    public ItemType itemType;

//    public MedicinePropertys medicineProperty;
//    public MaterialPropertys materialPropertys;
//    public ScrollPropertys scrollPropertys;
//    public ContainterPropertys containterPropertys;
//    public override Item GetItem(ItemQuality itemQuality, int level = 1)
//    {
//        Prop prop = null;

//        //if (propType == PropType.Medicine)
//        //    prop = new Medicine(this,itemQuality,1);
//        //else if (propType == PropType.Material)
//        //    prop = new StrengthenMaterial(this, itemQuality, 1);
//        //else if(propType == PropType.Scroll)
//        //    prop = new ScrollProp(this, itemQuality, 1);
//        //else if (propType == PropType.Containter)
//        //    prop = new ContainterProp(this, itemQuality, 1);
//        //else
//        //    prop = new Prop(this, itemQuality, 1);

//        return prop;
//    }

//    public override ItemType GetItemType()
//    {
//        return itemType;
//    }

//    public override string GetSpecialType()
//    {
//        return propType.ToString();
//    }
//}
//[System.Serializable]
//public class BattlePropPropertys
//{
//    public ObjectType applyTargetType;//使用时默认对谁使用
//}
//[System.Serializable]
//public class MedicinePropertys: BattlePropPropertys
//{
//    public Effect[] effects;
//}


//[System.Serializable]
//public class MaterialPropertys
//{
//    public MaterialInfo[] materialInfo;
//}
//[System.Serializable]
//public class ScrollPropertys
//{
    
//}
//[System.Serializable]
//public class ContainterPropertys
//{
//    public ContainterType boxType;
//    public int capacity;
//}
