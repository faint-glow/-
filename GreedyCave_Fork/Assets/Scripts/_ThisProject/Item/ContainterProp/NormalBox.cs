using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBox : ContainerProp, IDecorateAble
{
    public NormalBox(NormalBoxInfo normalBoxInfo, ItemQuality itemQuality, int count) :
        base(normalBoxInfo, itemQuality, count)
    {

    }
    public NormalBox(NormalBox normalBox, int count)//用于复制
       : base(normalBox, count)
    {

    }
    public IHolderAble GetHolder()
    {
        return holder;
    }
    public Vector2 GetSize()
    {
        return Vector2.one;
    }
    //public Sprite GetSprite()
    //{
    //    return itemSprite;
    //}
    public bool IsAstarEvent()
    {
        return true;
    }
    public bool PresetEventTrigger(IPlayerAble player)
    {
        UseMe();
        return true;
    }
    private void UseMe()
    {
        UIManager.GetInstance().ShowPanelAsync<BoxPanel>(BoxPanel.prefabsPath, PanelLayer.Top, null, (panel) =>
        {
            ContainerProp showItem = this;
            panel.SetBox(showItem);
        });
    }
    public override void SetDetailButtons()
    {
        base.SetDetailButtons();
        NormalBoxDetailButtons();
    }
    public void NormalBoxDetailButtons()
    {
        detailButtons[DetailButtonType.Middle] = null;
        detailButtons[DetailButtonType.Down] = new DetailButton("放置", Color.white, () =>
        {
            DecorateMgr.GetInstance().OpenDecoratePanel();
            DecorateMgr.GetInstance().SelectObj(this);
            UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
            UIManager.GetInstance().HidePanel(BagPanel.prefabsPath);
        });
    }

    public bool IsCanRecycle(out string tip)
    {
        tip = "";
        if (bagMgr.GetMainBag().Count > 0)
            tip = "该容器含有其他物品，不可回收";
        else
            return true;
        return false;
    }
}
