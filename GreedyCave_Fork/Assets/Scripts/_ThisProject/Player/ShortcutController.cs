using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public interface IBelongAble
//{
//    //如果是Item，则重写GetHolder      GetWarrio()返回null
//    public IHolderAble GetHolder();
//    //如果是skill，则重写GetWarrio()   GetHolder()返回null
//    public Warrior GetWarrior();
//}
public interface IShortcutAble:IDragAble//, IBelongAble
{
    public Warrior GetWarrior();
    public ShortcutType GetShortcutType();
    public void OnTriggerStart(Warrior warrior);
    public void OnTrigger(Warrior warrior);
    public void OnTriggerEnd(Warrior warrior,Action afterTrigger);
    public void AddCurrentCD(float num);
    public bool IsRunout();
    public float GetAllCD();
    public float GetCurrentCD();
    public int GetCount();
    //技能只需要名字，物品需要名字_背包中的下标
    public string GetSaveData();
}
[System.Serializable]
public class ShortcutSaveData
{
    public Dictionary<ShortcutType, string[]> shortcutsDic;
}
public class ShortcutController
{
    private Dictionary<ShortcutType, IShortcutAble[]> shortcutsDic = new Dictionary<ShortcutType, IShortcutAble[]>();
    public ShortcutController()
    {
        shortcutsDic[ShortcutType.道具] = new IShortcutAble[shortcutCount[ShortcutType.道具]];
        shortcutsDic[ShortcutType.技能] = new IShortcutAble[shortcutCount[ShortcutType.技能]];
    }
    public static Dictionary<ShortcutType, int> shortcutCount = new Dictionary<ShortcutType, int>()
    {
        { ShortcutType.道具,5},
        { ShortcutType.技能,5},
    };
    public void ClearRunTimeData()
    {
        foreach (var item in shortcutsDic)
        {
            if(item.Value != null)
            {
                IShortcutAble[] shortcutAble = item.Value;
                for (int i = 0; i < shortcutAble.Length; i++)
                {
                    shortcutAble[i] = null;
                    EventCenter.GetInstance().EventTrigger<ShortcutInfo>("ChangeShortcut", new ShortcutInfo(null, item.Key, i));
                }
            }
        }
    }
    public ShortcutSaveData GetShortcutSaveData()
    {
        ShortcutSaveData shortcutSaveData = new ShortcutSaveData();
        shortcutSaveData.shortcutsDic = new Dictionary<ShortcutType, string[]>();

        foreach (var item in shortcutsDic)
        {
            ShortcutType shortcutType = item.Key;
            for (int i = 0; i < item.Value.Length; i++)
            {
                IShortcutAble shortcutAble = item.Value[i];
                if (shortcutAble == null)
                    continue;
                if (!shortcutSaveData.shortcutsDic.ContainsKey(shortcutType))
                    shortcutSaveData.shortcutsDic[shortcutType] = new string[shortcutCount[shortcutType]];

                shortcutSaveData.shortcutsDic[shortcutType][i] = shortcutAble.GetSaveData();
            }
        }
        return shortcutSaveData;
    }
    public void LoadShortcutData(PlayerData playerData)
    {
        ShortcutSaveData shortcutSaveData = playerData.shortcutSaveData;
        if (shortcutSaveData == null)
        {
            return;
        }

        foreach (var item in shortcutSaveData.shortcutsDic)
        {
            ShortcutType shortcutType = item.Key;
            for (int i = 0; i < item.Value.Length; i++)
            {
                string shortcutData = item.Value[i];
                if (shortcutData == null || shortcutData == "")
                    continue;

                if (shortcutType == ShortcutType.技能)
                {
                    SkillController skillController = PlayerMgr.Instance.GetSkillController();
                    SkillInfoMgr skillInfoMgr = skillController.GetSkill(shortcutData);
                    if (skillInfoMgr != null)
                        SetShorucut(skillInfoMgr.skill, shortcutType, i);
                }
                else if(shortcutType == ShortcutType.道具)
                {
                    Bag<Item> itemBag = PlayerMgr.Instance.GetBag();
                    int index = int.Parse(shortcutData);
                    Item shortcutItem = itemBag.GetItem_Index(index);

                    if (shortcutItem != null)
                        SetShorucut(shortcutItem as IShortcutAble, shortcutType, i);
                }
            }
        }
    }
    public IShortcutAble[] GetShortcuts(ShortcutType shortCutType)
    {
        if (shortcutsDic.ContainsKey(shortCutType))
            return shortcutsDic[shortCutType];
        return null;
    }
    public void SetShorucut(IShortcutAble shortcutObj, ShortcutType shortcutType, int index)
    {
        if (!shortcutsDic.ContainsKey(shortcutType) || shortcutsDic[shortcutType].Length <= index)
        {
            Debug.Log("索引越界");
            return;
        }
        shortcutsDic[shortcutType][index] = shortcutObj;
        EventCenter.GetInstance().EventTrigger<ShortcutInfo>("ChangeShortcut", new ShortcutInfo(shortcutObj, shortcutType, index));
    }
    public void RemoveShorcut_Type(ShortcutType shortcutType)
    {
        for (int i = 0; i < 5; i++)
        {
            RemoveShortcut(shortcutType,i);
        }
    }
    public void RemoveShortcut(ShortcutType shortcutType, int index)
    {
        shortcutsDic[shortcutType][index] = null;
        EventCenter.GetInstance().EventTrigger<ShortcutInfo>("ChangeShortcut", new ShortcutInfo(null, shortcutType, index));
    }
}
