using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShortcutItem : Item, IShortcutAble
{
    public ShortcutItem(ShortcutItemInfo shortcutItemInfo,ItemQuality itemQuality, int count): base(shortcutItemInfo, itemQuality, count)
    {
        InitialShortcutItem(shortcutItemInfo);
    }
    public ShortcutItem(ShortcutItem shortcutItem, int count) : base(shortcutItem, count)
    {
        InitialShortcutItem(shortcutItem.itemInfo as ShortcutItemInfo);
    }

    private void InitialShortcutItem(ShortcutItemInfo shortcutItemInfo)
    {
        this.allCD = shortcutItemInfo.allCD;
    }

    protected float allCD = 0;
    protected float currentCD = 0;


    public virtual void OnTriggerStart(Warrior warrior)
    {
        
    }

    public virtual void OnTrigger(Warrior warrior)
    {

    }

    public void OnTriggerEnd(Warrior warrior, Action afterTrigger)
    {
        if(OnTriggerEndAided(warrior))
        {
            currentCD = allCD;
            if (afterTrigger != null)
                afterTrigger.Invoke();
        }
    }
    
    protected virtual bool OnTriggerEndAided(Warrior warrior)
    {
        return false;
    }

    public virtual bool IsRunout()
    {
        return GetCount() <= 0;
    }
    public virtual float GetAllCD()
    {
        return allCD;
    }

    public virtual float GetCurrentCD()
    {
        return currentCD;
    }

    public virtual void AddCurrentCD(float num)
    {
        currentCD += num;
    }

    public virtual IHolderAble GetHolder()
    {
        return holder;
    }

    public virtual Warrior GetWarrior()
    {
        Warrior warrior = null;
        IPlayerAble player = this.holder as IPlayerAble;
        if (player != null)
            warrior = player.GetWarrior();
        return warrior;
    }

    public virtual ShortcutType GetShortcutType()
    {
        return ShortcutType.道具;
    }

    public virtual string GetSaveData()
    {
        if (holder == null)
        {
            Debug.Log("有问题");
            return "";
        }
        string saveData = holder.GetBag().GetIndex_Item(this).ToString();
        return saveData;
    }


    //public virtual override ItemCategory GetItemCategory()
    //{
    //    throw new System.NotImplementedException();
    //}
}
