using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Medicine : BattleProp
{
    public Medicine(MedicinePropInfo medicineInfo, ItemQuality itemQuality, int count)
    : base(medicineInfo, itemQuality, count)
    {
        InitialMedicine(medicineInfo);
    }
    public Medicine(Medicine medicine, int count)
    : base(medicine, count)
    {
        InitialMedicine(medicine.itemInfo as MedicinePropInfo);
    }
    private void InitialMedicine(MedicinePropInfo medicineInfo)
    {
        
    }
    public override ItemCategory GetItemCategory()
    {
        return ItemCategory.Medicine;
    }
}


