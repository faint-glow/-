using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffPanel : ContainerPanel<BuffButton, Buff>
{
    public static string prefabsPath = "BuffPanel";

    private Warrior warrior;
    private Bag<Buff> buffBag;
    private Transform itemFather;
    private int index;
    public override void ShowMeFirst()
    {
        itemFather = GetPanel("Scroll View/Viewport/Content").transform;
    }
    public void SetWarrior(Warrior warrior)
    {
        if(this.warrior != null)
        {
            this.warrior.RemoveAfterAddBuffAction(Refresh);
            this.warrior.RemoveAfterRemoveBuffAction(Refresh);
        }

        this.warrior = warrior;
        this.buffBag = this.warrior.GetBuffBag();
        this.Refresh();
        this.warrior.AddAfterAddBuffAction(Refresh);
        this.warrior.AddAfterRemoveBuffAction(Refresh);
    }
    public void Refresh()
    {
        RefreshContainer(buffBag.GetBagItems());
    }
    public override string GetCellPath()
    {
        return BuffButton.prefabsPath;
    }
    public override Transform GetContainterTrans()
    {
        return itemFather;
    }
    public override IHolderAble GetHolder()
    {
        return null;
    }
    public override ScrollRect GetScrollRect()
    {
        return null;
    }

    public override void HideMe()
    {
        if (warrior == null)
            return;

        this.warrior.RemoveAfterAddBuffAction(Refresh);
        this.warrior.RemoveAfterRemoveBuffAction(Refresh);
        this.warrior = null;
    }
}
