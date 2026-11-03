using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthenMaterial : Item
{
    public StrengthenMaterial(StrengthenMaterialInfo strengthenMaterialInfo, ItemQuality itemQuality,int count) 
        : base(strengthenMaterialInfo, itemQuality, count)
    {
        this.InitialStrengthenMaterial(strengthenMaterialInfo);
    }

    public StrengthenMaterial(StrengthenMaterial strengthenMaterial, int count)
    : base(strengthenMaterial, count)
    {
        this.InitialStrengthenMaterial(strengthenMaterial.itemInfo as StrengthenMaterialInfo);
    }

    private void InitialStrengthenMaterial(StrengthenMaterialInfo strengthenMaterialInfo)
    {
        if(strengthenMaterialInfo == null)
        {
            Debug.Log("propInfo为空");
            return;
        }
        this.materialInfo = strengthenMaterialInfo.materialInfo;
        this.itemBaseInfo = GetMaterialInfo(this.materialInfo);
    }

    public MaterialInfo[] materialInfo;

    private string GetMaterialInfo(MaterialInfo[] materialInfos)
    {
        string content = "";
        string lastContent = "";
        if (materialInfos != null)
        {
            for (int i = 0; i < materialInfos.Length; i++)
            {
                MaterialInfo materialInfo = materialInfos[i];
                if (materialInfo == null)
                    continue;
                if (lastContent != null && lastContent != "")
                    content += "\n";
                lastContent = materialInfo.propertyType.ToString() + ":" + materialInfo.minNum + "~" + materialInfo.maxNum + "("+ materialInfo.ratio*100 + "%)";
                content += lastContent;
            }
        }

        return content;
    }

    /// <summary>
    /// 返回在背包中一个单元格的最大数量，不写就默认为背包的默认值
    /// </summary>
    /// <returns></returns>
    public override int GetUpLimitCount()
    {
        return 3;
    }

    public override ItemCategory GetItemCategory()
    {
        return ItemCategory.Material;
    }
}
