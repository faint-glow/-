using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grocer : Merchant
{
    public static string BoxPath = "Item/Sundry/Box";

    protected override Dictionary<string, int> InitialBag()
    {
        Dictionary<string, int> originalItemDic = new Dictionary<string, int>();

        ItemInfo[] boxInfos = ResMgr.GetInstance().LoadAll<ItemInfo>(BoxPath);
        for (int i = 0; i < boxInfos.Length; i++)
        {
            Item item = boxInfos[i].GetItem();
            item.isUnlimited = true;
            item.SetItemCount(99);
            this.AddItem(item);

            if (!originalItemDic.ContainsKey(item.GetName()))
                originalItemDic[item.GetName()] = 99;
        }
        ItemInfo magicBallInfos = ResMgr.GetInstance().Load<ItemInfo>("Item/Sundry/MagicBall/MagicBall");
        Item magicBall = magicBallInfos.GetItem();
        magicBall.SetItemCount(1);
        this.AddItem(magicBall);

        if (!originalItemDic.ContainsKey(magicBall.GetName()))
            originalItemDic[magicBall.GetName()] = 1;

        return originalItemDic;
    }

    public override bool IsSaveOnChangeScene()
    {
        return true;
    }
}
