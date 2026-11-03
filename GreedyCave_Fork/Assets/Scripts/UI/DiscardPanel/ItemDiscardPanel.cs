using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDiscardPanel : ItemBagDrageUp
{
    public override IDragAble SetItem_Drag(IDragAble obj)
    {
        Item item = obj as Item;
        if (item == null)
            return null;

        if (item.holder != this.Holder)
        {
            if (item.holder.GetHolderType() == HolderType.Discard || this.Holder.GetHolderType() == HolderType.Discard)
                UIManager.GetInstance().ShowPanelAsync<AbandonPanel>(AbandonPanel.prefabsPath, PanelLayer.Max, null,
                (panel) => { panel.SetItem(Holder, item); });
        }
        return null;
    }
}
