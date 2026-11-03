using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStrengthPanel : ItemBagDrageUp
{
    public override IDragAble SetItem_Drag(IDragAble obj)
    {
        Item item = obj as Item;
        Item overflowItem = null;
        if (item.holder != this.Holder)
        {
            if (item.holder.GetHolderType() == HolderType.Farrier || this.Holder.GetHolderType() == HolderType.Farrier)
            {
                Item newItem = item.holder.RemoveItem(item);
                overflowItem = this.Holder.AddItem(newItem);
            }
        }

        return overflowItem;
    }
}
