using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MyDictionary/Item/NormalBox", fileName = "NewNormalBox")]
public class NormalBoxInfo : ContainerPropInfo
{
    public override Item GetItem(ItemQuality itemQuality, int level = 1)
    {
        return new NormalBox(this, itemQuality, 1);
    }
}
