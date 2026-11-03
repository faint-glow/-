using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EquipType
{
    Sword,//剑
    Cane,//杖
    Helmet,//头盔
    Armour,//护甲
    HandArmour,//手甲
    FootArmour,//足夹
    Necklace,//项链
    Ring,//戒指

    无,
}

public class Equip : Item,IPropertyAble,IEffectSourceAble
{
    public Equip(EquipInfo equipInfo, ItemQuality itemQuality,int level) 
        :base(equipInfo, itemQuality, level)
    {
        this.InitialEquip(equipInfo,level);

        if (Random.Range(0, 1f) < 0.3f)
            this.equipRandomBuff.Add(GetOneRandomEquipBuff(new List<IEffectSourceAble>() { this }));

        //this.equipRandomBuff.Clear();
        //for (int i = 0; i < allEquipRandomBuff.Length; i++)
        //{
        //    equipRandomBuff.Add(new Buff(allEquipRandomBuff[i], new List<IEffectSourceAble>() { this }));
        //}
    }
    public Equip(Equip equip,int count)
        : base(equip, count)
    {
        this.InitialEquip(equip.itemInfo as EquipInfo,equip.level);

        this.strengthenProperty = equip.strengthenProperty;
        this.extraProperty = equip.extraProperty;
        this.equipRandomBuff = equip.equipRandomBuff;
        this.strengthenLevel = equip.strengthenLevel;
    }

    private void InitialEquip(EquipInfo equipInfo, int level)
    {
        if (equipInfo == null)
        {
            Debug.Log("equipInfo为空");
            return;
        }
        this.equipType = equipInfo.equipType;
        this.baseProperty = new Property(this, level, equipInfo.propertyFactors);
        for (int i = 0; i < equipInfo.buffInfo.Count; i++)
        {
            if (equipInfo.buffInfo[i] == null)
                return;
            this.equipBaseBuff.Add(new Buff(equipInfo.buffInfo[i], new List<IEffectSourceAble>() { this }));
        }
    }

    public EquipType equipType;
    private Property baseProperty;
    private Dictionary<PropertyType, float> strengthenProperty = new Dictionary<PropertyType, float>();
    private Dictionary<PropertyType, float> randomProperty = new Dictionary<PropertyType, float>();
    private Dictionary<PropertyType, float> extraProperty = new Dictionary<PropertyType, float>();
    private List<Buff> equipBaseBuff = new List<Buff>();//对装备该武器的Warrior提供的效果
    private List<Buff> equipRandomBuff = new List<Buff>();//对装备该武器的Warrior提供的效果
    private List<Buff> equipAllBuff;//

    private Warrior warrior;
    private int strengthenLevel = 0;

    //private static List<Buff> allEquipRandomBuff = new List<Buff>();
    private static BuffInfo[] allEquipRandomBuff;


    public override ItemSaveData GetItemSaveData()
    {
        ItemSaveData itemSaveData = base.GetItemSaveData();
        itemSaveData.strengthenProperty = strengthenProperty;
        itemSaveData.extraProperty = extraProperty;
        itemSaveData.equipRandomBuff = new List<string>();
        for (int i = 0; i < equipRandomBuff.Count; i++)
            itemSaveData.equipRandomBuff.Add(equipRandomBuff[i].GetName());
        itemSaveData.strengthenLevel = strengthenLevel; 
        return itemSaveData;
    }
    public override void LoadItemSaveData(ItemSaveData itemSaveData)
    {
        base.LoadItemSaveData(itemSaveData);
        this.strengthenProperty = itemSaveData.strengthenProperty;
        this.extraProperty = itemSaveData.extraProperty;
        this.equipRandomBuff = new List<Buff>();
        for (int i = 0; i < itemSaveData.equipRandomBuff.Count; i++)
        {
            string buffName = itemSaveData.equipRandomBuff[i];
            if (Buff.AllBuffInfoDic.ContainsKey(buffName))
                this.equipRandomBuff.Add(new Buff(Buff.AllBuffInfoDic[buffName], new List<IEffectSourceAble>() { this }));
            else
                Debug.Log("Buff.AllBuffInfoDic缺失：" + buffName + "。请检查路径");
        }
        this.strengthenLevel = itemSaveData.strengthenLevel;
    }
    public override int GetPrice()
    {
        return base.GetPrice() * (1 + strengthenLevel * strengthenLevel / 10);
    }
    protected override int CreatePrice()
    {
        int price = base.CreatePrice();
        return (int)(price * UnityEngine.Random.Range(0.9f,1.1f));
    }
    public override ItemType GetItemType()
    {
        return ItemType.CannotCombine;
    }
    public static Buff GetOneRandomEquipBuff(List<IEffectSourceAble> effectSource)
    {
        if (allEquipRandomBuff == null)
        {
            allEquipRandomBuff = ResMgr.GetInstance().LoadAll<BuffInfo>("Buff/EquipRandomBuff");
        }
        if (allEquipRandomBuff.Length <= 0)
            return null;
        int index = Random.Range(0, allEquipRandomBuff.Length);

        return new Buff(allEquipRandomBuff[index], effectSource);
    }

    //装备身上没有装备这个说法，额，也许可能由宝石加持？
    public float GetExtraProperty_Equip(PropertyType propertyType)//获取装备加持过的对应属性
    {
        return 0;
    }
    public float GetExtraProperty_Buff(PropertyType propertyType, IEffectSourceAble effectSource)//获取buff加持后的对应属性
    {
        return 0;
    }
    public Dictionary<PropertyType, float> GetOriginalPropertys()
    {
        return baseProperty.CurrentPropertys;
    }
    public Dictionary<PropertyType, float> GetStrengthenPropertys()
    {
        return strengthenProperty;
    }
    //获取武器额外数值（附魔）
    public Dictionary<PropertyType, float> GetRandomPropertys()
    {
        return randomProperty;
    }
    //获取武器额外数值（附魔）
    public Dictionary<PropertyType, float> GetExtraPropertys()
    {
        return extraProperty;
    }
    public List<Buff> GetBaseEquipBuff()
    {
        return equipBaseBuff; 
    }
    public List<Buff> GetEquipRandomBuff()
    {
        return equipRandomBuff;
    }
    public List<Buff> GetAllEquipBuff()
    {
        if(equipAllBuff == null || equipAllBuff.Count != (equipBaseBuff.Count + equipRandomBuff.Count))
            equipAllBuff = equipBaseBuff.Concat(equipRandomBuff).ToList<Buff>();//合并两个列表
        return equipAllBuff;
    }
    
    //获取武器原始数值
    public float GetOriginalProperty(PropertyType propertyType)
    {
        return baseProperty.GetOriginalProperty(propertyType);
    }
    //获取武器基础数值，包含原始数值+强化数值
    public float GetBaseProperty(PropertyType propertyType)
    {
        float originalProperty = GetOriginalProperty(propertyType);
        if (strengthenProperty.ContainsKey(propertyType))
            return originalProperty + strengthenProperty[propertyType];
        return originalProperty;
    }
    //获取武器生成是随机获得的属性
    public float GetRandomProperty(PropertyType propertyType)
    {
        if (randomProperty.ContainsKey(propertyType))
            return randomProperty[propertyType];
        return 0;
    }
    //获取武器额外数值（附魔）
    public float GetExtraProperty(PropertyType propertyType)
    {
        if (extraProperty.ContainsKey(propertyType))
            return extraProperty[propertyType];
        return 0;
    }
    //获取所有效果加成后的值
    public float GetCurrentProperty(PropertyType propertyType)
    {
        return GetBaseProperty(propertyType) + GetExtraProperty(propertyType);
    }

    //修改武器强化数值
    public void ChangeStrengthenProperty(PropertyType propertyType, float value)
    {
        if (!strengthenProperty.ContainsKey(propertyType))
            strengthenProperty[propertyType] = 0;
        strengthenProperty[propertyType] += value;
    }
    //修改武器额外数值（附魔）
    public void ChangeExtraProperty(PropertyType propertyType, float value)
    {
        if (!extraProperty.ContainsKey(propertyType))
            extraProperty[propertyType] = 0;
        extraProperty[propertyType] += value;
    }

    public void UseBuffToWarrior(Warrior warrior)
    {

        this.warrior = warrior;
        if (GetAllEquipBuff() == null)
            return;

        for (int i = 0; i < GetAllEquipBuff().Count; i++)
        {
            Buff buff = GetAllEquipBuff()[i];
            warrior.AddBuff(warrior, buff);
            //buff.UseBuff(new ApplyInfo(warrior,warrior),new List<IEffectSourceAble>() {this });
        }
    }
    public void RemoveBuffFromWarrior()
    {
        if (this.warrior == null || GetAllEquipBuff() == null)
            return;
        for (int i = 0; i < GetAllEquipBuff().Count; i++)
        {
            Buff buff = GetAllEquipBuff()[i];
            warrior.RemoveBuff(buff);
        }
    }

    public void TakeOffEquip()
    {
        this.warrior.TakeOffEquip();
    }

    public int GetStrengthenLevel()
    {
        return strengthenLevel;
    }
    public void levelUpOne_Strengthen()
    {
        LevelUp_Strengthen(++strengthenLevel);
    }
    public void LevelUp_Strengthen(int level)
    {
        strengthenLevel = level;
    }

    public override Dictionary<DetailButtonType, DetailButton> DetailButtons
    {
        get 
        {
            SetDetailButtons();
            return detailButtons;
        }
    }
    public int GetScore()//装备评分。待实现
    {
        return 0;
    }

    public override void SetDetailButtons()
    {
        int index;
        if (!PlayerMgr.Instance.IsWearEquip(this,out index))
        {
            base.SetDetailButtons();
            detailButtons[DetailButtonType.Middle] = null;
            detailButtons[DetailButtonType.Down] = new DetailButton("装备", Color.white, () =>
            {
                PlayerMgr.Instance.WearEquip(this);
                AudioMgr.Instance.PlayEffect(AudioClipPath.equip);
                UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
            });
        }
        else
        {
            detailButtons[DetailButtonType.Up] = null;
            detailButtons[DetailButtonType.Middle] = null;
            detailButtons[DetailButtonType.Down] = new DetailButton("卸下", Color.white, () =>
            {
                PlayerMgr.Instance.TakeOffEquip(this,index);
                AudioMgr.Instance.PlayEffect(AudioClipPath.equip);
                UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
            });
        }
    }

    public override ItemCategory GetItemCategory()
    {
        return ItemCategory.Equip;
    }

    protected override int CompareToSameCatagory(Item item)
    {
        Equip equip = item as Equip;
        if (equip == null)
            return base.CompareToSameCatagory(item);

        if ((int)this.equipType < (int)equip.equipType)
            return 1;
        else
            return -1;
    }

    public override Vector3 CenterPosOffset()
    {
        if(equipType == EquipType.Cane || equipType == EquipType.Sword)
            return new Vector3(0.5f, -0.5f, 0);
        return base.CenterPosOffset();
    }
    
}
