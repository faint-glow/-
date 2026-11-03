//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;



//public class Prop : Item
//{
//    public Prop(PropInfo proInfo,ItemQuality itemQuality,int count)
//    : base(proInfo, itemQuality, count)
//    {
//        InitialProp(proInfo);
//    }
//    public Prop(Prop prop, int count) :base(prop, count)
//    {
//        this.InitialProp(prop.itemInfo as PropInfo);
//        this.effectDic = prop.effectDic;
//        this.propType = prop.propType;
//    }

//    private void InitialProp(PropInfo propInfo)
//    {
//        if (propInfo == null)
//        {
//            Debug.Log("propInfoÎª¿Õ");
//            return;
//        }

//        this.propType = propInfo.propType;
//    }
//    protected PropType propType;
//    protected Dictionary<TriggerType, List<Effect>> effectDic = new Dictionary<TriggerType, List<Effect>>();
//    public override ItemCategory GetItemCategory()
//    {
//        if (propType == PropType.Material)
//            return ItemCategory.Material;
//        else if (propType == PropType.Medicine)
//            return ItemCategory.Medicine;
//        else if (propType == PropType.Scroll)
//            return ItemCategory.Sundary;

//        return ItemCategory.ÎÞ;
//    }


//}
