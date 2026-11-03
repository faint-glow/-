using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysticalMerchant : Merchant
{
    public static string merchantName = "…Ò√ÿ…Ã»À";
    protected override Dictionary<string,int> InitialBag()
    {
        Dictionary<string, int> originalItemDic = new Dictionary<string, int>();


        LoungeSupply[] loungeSupplys = GameManager.Instance.GetMapInfo()._loungeSupplys;
        for (int i = 0; i < loungeSupplys.Length; i++)
        {
            LoungeSupply loungeSupply = loungeSupplys[i];
            Item item = loungeSupply.itemInfo.GetItem(1);
            item.SetItemCount(loungeSupply.count);
            AddItem(item);

            if (!originalItemDic.ContainsKey(item.GetName()))
                originalItemDic[item.GetName()] = loungeSupply.count;
        }

        return originalItemDic;
    }
}
