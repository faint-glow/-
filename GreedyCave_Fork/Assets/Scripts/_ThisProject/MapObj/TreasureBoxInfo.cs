using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyDictionary/Map/TreasureBoxInfo", fileName = "NewTreasureBox")]
public class TreasureBoxInfo:ScriptableObject
{
    public Sprite sprite;
    public GameObject obj;
    public float probability;
    public TreasureBoxType treasureBoxType;
    [Header("可能出现的品质和该品质的出现概率")]
    public List<ItemQualityAndRatio> itemQualitys = new List<ItemQualityAndRatio>() { new ItemQualityAndRatio(ItemQuality.White, 1) };

}
