using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxPanel : ItemBagDrageUp
{
    public override IDragAble SetItem_Drag(IDragAble obj)
    {
        Item item = obj as Item;
        if (item == null)
            return null;

        if (item.holder != this.Holder)
        {
            if (item.holder.GetHolderType() == HolderType.Box || this.Holder.GetHolderType() == HolderType.Box)
                UIManager.GetInstance().ShowPanelAsync<DepositPanel>(DepositPanel.prefabsPath, PanelLayer.Max, null,
                (panel) => { panel.SetItem(Holder, item); });
        }
        return null;
    }
}
