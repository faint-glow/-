using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScrollType
{
    BackCity
}
public class ScrollProp : ShortcutItem//Item, IShortcutAble
{
    public ScrollProp(ScrollPropInfo scrollPropInfo, ItemQuality itemQuality, int count)
    : base(scrollPropInfo, itemQuality, count)
    {

    }
    public ScrollProp(ScrollProp scrollProp, int count)
    : base(scrollProp, count)
    {

    }
    public override ItemCategory GetItemCategory()
    {
        return ItemCategory.Sundary;
    }
    public override void SetDetailButtons()
    {
        base.SetDetailButtons();
        detailButtons[DetailButtonType.Middle] = null;
        detailButtons[DetailButtonType.Down] = new DetailButton("使用", Color.white, () =>
        {
            IPlayerAble player = holder as IPlayerAble;
            if (player == null)
                return;
            OnTriggerEnd(player.GetWarrior(),null);
            UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
            UIManager.GetInstance().HidePanel(BagPanel.prefabsPath);
        });
    }
    //public void OnTriggerStart(Warrior warrior)
    //{
    //}
    //public void OnTrigger(Warrior warrior)
    //{
    //}
    protected override bool OnTriggerEndAided(Warrior warrior)
    {
        if (warrior.IsBattleing() == true)
        {
            UIManager.GetInstance().ShowTips("战斗中不可进行该操作");
            return false;
        }
        if (PlayerStateCheck() == true)
        {
            UIManager.GetInstance().ShowPanelAsync<RequestPanel>(RequestPanel.prefabsPath, PanelLayer.Max, null, (panel) =>
            {
                panel.SetRequestInfo("您将失去除了已穿戴的紫色品质及以上的装备和魔法球之外的所有物品，你确定离开吗", Refuse, Agree);
            });
        }
        return true;
    }
    private void Refuse()
    {
        UIManager.GetInstance().HidePanel(RequestPanel.prefabsPath);
    }

    private void Agree()
    {
        UIManager.GetInstance().HidePanel(RequestPanel.prefabsPath);
        UseProp(() =>
        {
            PlayerMgr.Instance.RemoveItem(this, 1);
            BackCity();
        });
    }

    private void BackCity()
    {
        GameManager.Instance.ChangeSceneAfterVignette(SceneName.MainCity);
        PlayerMgr.Instance.TakeOffEquips((equip)=> 
        {
            if (equip == null)
                return false;
            if (equip.isNeverDrop == true)
                return false;
            if ((int)equip.itemQuality >= (int)ItemQuality.Purple)
                return false;
            return true;
        });
        PlayerMgr.Instance.GetBag().RemoveBagItems((item) =>
        {
            if (item == null)
                return false;
            if (item.isNeverDrop == true)
                return false;
            return true;
        });
    }

}
