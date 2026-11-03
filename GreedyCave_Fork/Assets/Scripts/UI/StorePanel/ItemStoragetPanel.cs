using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStoragetPanel: ItemBagDrageUp
{
    public override IDragAble SetItem_Drag(IDragAble obj)
    {
        Item item = obj as Item;
        if (item == null)
            return null;
        if(item.IsLocked == true)
        {
            UIManager.GetInstance().ShowTips(Item.lockedTip);
            return null;
        }

        if (item.holder != this.Holder)
        {
            if (item.holder.GetHolderType() == HolderType.Merchant || this.Holder .GetHolderType() == HolderType.Merchant)
                UIManager.GetInstance().ShowPanelAsync<TradePanel>(TradePanel.prefabsPath, PanelLayer.Max, null,
                (panel) => { panel.SetItem(Holder, item); });
        }

        return null;
    }

}
