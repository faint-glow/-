using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBagDrageUp : MonoBehaviour, IDragUpAble<IDragAble>
{
    ContainerPanel<ItemButton, Item> containter;
    ContainerPanel<ItemButton, Item> Containter
    {
        get
        {
            if (containter == null)
                containter = GetComponentInParent<ContainerPanel<ItemButton, Item>>();
            return containter;
        }
    }
    private IHolderAble holder;
    public IHolderAble Holder
    {
        get
        {
            if (Containter == null)
                Debug.Log("ÈÝÆ÷¹ÜÀíÎª¿Õ");
            holder = Containter.GetHolder();
            return holder;
        }
    }

    public abstract IDragAble SetItem_Drag(IDragAble obj);
}
