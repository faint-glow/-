using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WearEquipType//处理特殊的Weapon等，其他名字必须与EquipType中的名字一样，否则会转换失败
{
    Weapon,//武器
    Helmet,//头盔
    Armour,//护甲
    HandArmour,//手甲
    FootArmour,//足夹
    Necklace,//项链
    Ring,//戒指
}
public class EquipController
{

}
public class PlayerEquipController : EquipController
{
    //使用List是为了做出像伊洛纳一样可以有副武器和两个戒指的效果
    //不过此处不需要
    private Dictionary<WearEquipType, List<Equip>> equipDic = new Dictionary<WearEquipType, List<Equip>>();
    public Dictionary<WearEquipType, List<Equip>> EquipDic
    {
        get { return equipDic; }
    }
    public PlayerEquipController()
    {
        ClearRunTimeData();
    }
    public void ClearRunTimeData()
    {
        foreach (WearEquipType type in Enum.GetValues(typeof(WearEquipType)))
        {
            int capacity = 1;
            equipDic[type] = new List<Equip>(capacity) { null };//如果有副武器可以加if判断修改Capacity的值
        }
    }

    public bool IsWearEquip(Equip equip, out int index)
    {
        WearEquipType type = EquipTypeConversion(equip.equipType);
        index = 0;
        for (int i = 0; i < EquipDic[type].Count; i++)
        {
            if (EquipDic[type][i] == equip)
            {
                index = i;
                return true;
            }
        }
        return false;
    }
    //对于副武器的添加处理
    //优先替换空位置，如果没有就替换评分更低的。伊洛纳中还有双手武器占两个格子。
    //可以增加对应的枚举类型，增加判断。这里就不处理了
    //通过点击装备按钮穿戴装备
    public void WearEquip(Warrior warrior, Equip newEquip)
    {
        WearEquipType type = EquipTypeConversion(newEquip.equipType);

        int score = -1;
        int index = 0;
        for (int i = 0; i < equipDic[type].Count; i++)
        {
            if (equipDic[type][i] == null)
            {
                WearEquipHandler(warrior, type, newEquip, i);
                return;
            }
            int equipScore = equipDic[type][i].GetScore();
            if (score < equipScore)
            {
                score = equipScore;
                index = i;
            }
        }

        WearEquipHandler(warrior, type, newEquip, index);
    }
    //通过拖拽穿戴装备
    public void WearEquip(Warrior warrior, Equip newEquip, int index)
    {
        WearEquipType type = EquipTypeConversion(newEquip.equipType);
        WearEquipHandler(warrior, type, newEquip, index);
    }
    public Equip GetEquip(WearEquipType equipType)
    {
        if (EquipDic.ContainsKey(equipType) && EquipDic[equipType].Count >= 1)
            return EquipDic[equipType][0];
        return null;
    }
    private void WearEquipHandler(Warrior warrior, WearEquipType type, Equip newEquip, int index)
    {
        Equip oldEquip = equipDic[type][index];//之前装备的装备，用于返回背包
        newEquip = (Equip)PlayerMgr.Instance.RemoveItem(newEquip);//先把装备从玩家身上移除，注意，此时传入的参数的newEquip物品数量为1，已经没用了，要用返回的equip
        PlayerMgr.Instance.AddItem(oldEquip);//此位置原来有装备，要把该装备返回背包

        WearEquip(warrior, type, oldEquip, newEquip, index);
    }
    private void WearEquip(Warrior warrior, WearEquipType type, Equip oldEquip, Equip newEquip, int index)
    {
        equipDic[type][index] = newEquip;

        if (oldEquip != null)//处理武器buff
            oldEquip.RemoveBuffFromWarrior();
        if (newEquip != null)
            newEquip.UseBuffToWarrior(warrior);
        warrior.WearEquip();
        PlayerMgr.Instance.WearClothes(type);
    }
    public void LoadEquip(Warrior warrior, Dictionary<WearEquipType, List<ItemSaveData>> equipDic)
    {
        if (equipDic == null)
            return;
        foreach (var item in equipDic)
        {
            WearEquipType type = item.Key;
            for (int i = 0; i < item.Value.Count; i++)
            {
                ItemSaveData itemData = item.Value[i];
                if (itemData != null)
                {
                    Equip equip = itemData.GetItem() as Equip;
                    if (equip != null)
                    {
                        WearEquip(warrior, type, null, equip, i);
                        equip.holder = PlayerMgr.Instance;
                    }
                }
            }
        }
    }
    public void TakeOffEquip(Equip oldEquip, int index)
    {
        if (PlayerMgr.Instance.IsBagFull() == true)
        {
            UIManager.GetInstance().ShowTips("背包已满，无法脱下该装备");
            return;
        }
        WearEquipType type = EquipTypeConversion(oldEquip.equipType);
        if (!equipDic.ContainsKey(type))
            return;
        equipDic[type][index] = null;
        if (oldEquip != null)//处理武器buff
        {
            PlayerMgr.Instance.AddItem(oldEquip);
            oldEquip.RemoveBuffFromWarrior();
            oldEquip.TakeOffEquip();
        }

        PlayerMgr.Instance.Undress(type);
    }
    /// <summary>
    /// 脱下所有满足条件的装备
    /// </summary>
    /// <param name="func">满足条件返回true</param>
    public void TakeOffEquips(Func<Equip, bool> func)
    {
        foreach (KeyValuePair<WearEquipType, List<Equip>> item in EquipDic)
        {
            List<Equip> equips = item.Value;
            for (int i = 0; i < equips.Count; i++)
            {
                if (equips != null && func(equips[i]) == true)
                    TakeOffEquip(equips[i], i);
            }
        }
    }
    public static WearEquipType EquipTypeConversion(EquipType type)
    {
        WearEquipType playerEquipType;
        if (type == EquipType.Sword || type == EquipType.Cane)
        {
            playerEquipType = WearEquipType.Weapon;
            return playerEquipType;
        }
        playerEquipType = (WearEquipType)Enum.Parse(typeof(WearEquipType), type.ToString());
        return playerEquipType;
    }

}
