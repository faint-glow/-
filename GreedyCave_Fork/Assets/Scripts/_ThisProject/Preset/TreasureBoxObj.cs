using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TreasureBoxType
{
    Normal,//普通
    Excellent,//优质
    Valuable,//贵重
    King,//国王
}
public class TreasureBoxObj : MazeObj, IBootyHolderAble
{
    public float probability;
    public TreasureBoxType treasureBoxType;
    [Header("可能出现的品质和该品质的出现概率")]
    public List<ItemQualityAndRatio> itemQualitys = new List<ItemQualityAndRatio>() { new ItemQualityAndRatio(ItemQuality.White, 1) };
    private Vector3 openPosition;
    private void Start()
    {
        if (SceneController.Instance != null)
            SceneController.Instance.LoadSaveDestoryObjInfo(this);
    }
    public override string GetSaveTag()
    {
        return treasureBoxType.ToString();
    }
    public override string GetSmallMapSpritePath()
    {
        return "Sprite/SmallMapObj/TreasureBox";
    }
    protected override Vector2 CollOffset()
    {
        return new Vector2(0, 0.4f);
    }
    public override bool PresetEventTrigger(IPlayerAble player)
    {
        player.OpenTreasureBox(this);
        return base.PresetEventTrigger(player);
    }
    public static int GetEquipLevel()
    {
        int level = 1;
        MapInfo mapInfo = GameManager.Instance.GetMapInfo();
        if (mapInfo != null)
            return mapInfo.LayerCount;
        level = PlayerMgr.Instance.GetPlayerData().maxLayerCount;

        level += Random.Range(-2, 3);
        level = Mathf.Clamp(level, 1, int.MaxValue);

        return level;
    }

    public Equip OpenTreasureBox()
    {
        this.openPosition = transform.position;

        int count = itemQualitys.Count;
        Dictionary<ItemQuality, float> itemQualityRatio = new Dictionary<ItemQuality, float>();
        for (int i = 0; i < count; i++)
        {
            ItemQuality qualityType = itemQualitys[i].itemQuality;
            itemQualityRatio[qualityType] = itemQualitys[i].fallRatio;
        }
        ItemQuality targetQuality = RewardLibrary.GetRandomType<ItemQuality>(itemQualityRatio);
        MapInfo mapInfo = GameManager.Instance.GetMapInfo();
        RewardLibrary library = mapInfo.GetLibrary();
        Item item = null;
        int cycleCount = 0;
        while (item == null)
        {
            item = library.GetRandomItem(targetQuality, "", GetEquipLevel());
            targetQuality = (ItemQuality)((int)targetQuality - 1);
            cycleCount++;
            if (cycleCount > 10)//避免因为预设没处理好导致无限循环
                break;
        }

        GameObject obj = ResMgr.GetInstance().Load<GameObject>("FX/OpenTreasureBox");
        obj.transform.position = this.transform.position;

        sceneSpecialObjSaveInfo.isDestory = true;
        if (SceneController.Instance != null)
            SceneController.Instance.SaveDestoryObjInfo(this);

        MazeGridMgr.ReleaseGrid(transform.position);
        GameObject.Destroy(gameObject);
        MonoMgr.GetInstance().DelayAction(1f, () =>
        {
            if (obj != null)
                GameObject.Destroy(obj);
        });
        AudioMgr.Instance.PlayEffect(AudioClipPath.openTreasureBox);

        Equip equip = item as Equip;
        if (equip == null)
            return null;

        return equip;
    }
    public static Equip OpenExcellentTreasureBox()
    {
        List<ItemQualityAndRatio> itemQualitys = new List<ItemQualityAndRatio>()
        {
            new ItemQualityAndRatio(ItemQuality.Blue, 0.75f),
            new ItemQualityAndRatio(ItemQuality.Purple, 0.22f),
            new ItemQualityAndRatio(ItemQuality.Orange, 0.03f),
        };
        return Open(itemQualitys);
    }
    public static Equip OpenValuableTreasureBox()
    {
        List<ItemQualityAndRatio> itemQualitys = new List<ItemQualityAndRatio>()
        {
            new ItemQualityAndRatio(ItemQuality.Purple, 0.85f),
            new ItemQualityAndRatio(ItemQuality.Orange, 0.14f),
            new ItemQualityAndRatio(ItemQuality.Red, 0.01f),
        };
        return Open(itemQualitys);
    }
    public static Equip OpenKingTreasureBox()
    {
        List<ItemQualityAndRatio> itemQualitys = new List<ItemQualityAndRatio>()
        {
            new ItemQualityAndRatio(ItemQuality.Purple, 0.75f),
            new ItemQualityAndRatio(ItemQuality.Orange, 0.22f),
            new ItemQualityAndRatio(ItemQuality.Red, 0.03f),
        };
        return Open(itemQualitys);
    }

    public static Equip Open(List<ItemQualityAndRatio> itemQualitys)
    {
        int count = itemQualitys.Count;
        Dictionary<ItemQuality, float> itemQualityRatio = new Dictionary<ItemQuality, float>();
        for (int i = 0; i < count; i++)
        {
            ItemQuality qualityType = itemQualitys[i].itemQuality;
            itemQualityRatio[qualityType] = itemQualitys[i].fallRatio;
        }
        ItemQuality targetQuality = RewardLibrary.GetRandomType<ItemQuality>(itemQualityRatio);

        MapInfo mapInfo = GameManager.Instance.GetMapInfo();
        RewardLibrary library = null;
        if (mapInfo != null)
            library = mapInfo.GetLibrary();
        else
            library = new RewardLibrary("Item/Equip/MapOne");

        Item item = null;
        int cycleCount = 0;
        while (item == null)
        {
            item = library.GetRandomItem(targetQuality, "", GetEquipLevel());
            targetQuality = (ItemQuality)((int)targetQuality - 1);
            cycleCount++;
            if (cycleCount > 10)//避免因为预设没处理好导致无限循环
                break;
        }

        AudioMgr.Instance.PlayEffect(AudioClipPath.openTreasureBox);
        return item as Equip;
    }
    public Vector3 GetPosition()
    {
        return openPosition;
    }
    public float AdjustReward_Property(float targetValue, ref PropertyType targetType)
    {
        return targetValue;
    }

}

[System.Serializable]
public class SceneSpecialObjSaveInfo
{
    public bool isDestory;
}
