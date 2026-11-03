using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : BattleProp
{
    public Food(FoodPropInfo foodPropInfo, ItemQuality itemQuality, int count) : base(foodPropInfo, itemQuality, count)
    {
        InitialFood(foodPropInfo);
    }
    public Food(Food food, int count) : base(food, count)
    {
        InitialFood(food.itemInfo as FoodPropInfo);
    }
    private void InitialFood(FoodPropInfo foodPropInfo)
    {

    }
    public override ItemCategory GetItemCategory()
    {
        return ItemCategory.Food;
    }
    //暂时方便处理
    protected override void OnUseProp(Warrior target)
    {
        WorldSpritePanelMgr.ShowSprite(PhysiologyMgr.PhysiologySprite_Food,GetWarrior().GetWarriorPos() + new Vector3(0, 1.5f, 0), Vector3.one);
    }
}
