using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TradeType
{
    购买,
    出售
}

public class TradePanel : ExchangePanel
{
    public static string prefabsPath = "TradePanel";

    public override string GetConfirmTextByOther()
    {
        return "出售";
    }
    public override string GetConfirmTextByPlayer()
    {
        return "购买";
    }

    public override string GetPrefabsPath()
    {
        return prefabsPath;
    }
    protected override void OperiateCoin(IHolderAble oldHolder, IHolderAble newHolder, Item item, int useCount)
    {
        int price = item.holder.GetItemPrice(item);
        oldHolder.ChangeCoin(price * useCount);
        newHolder.ChangeCoin(-price * useCount);
    }

    public override bool OperiateByPlayer(IHolderAble targetHolder, Item item)
    {
        int maxCount = targetHolder.GetCoin() / item.holder.GetItemPrice(item);
        if (maxCount < 1)
        {
            UIManager.GetInstance().ShowTips("您买不起哪怕一个");
            return false;
        }
        return true;
    }

    public override int GetSliderMaxValueByPlayer(IHolderAble targetHolder, Item item)
    {
        int maxCount = targetHolder.GetCoin() / item.holder.GetItemPrice(item);
        return Mathf.Min(item.GetCount(), maxCount);
    }
}
