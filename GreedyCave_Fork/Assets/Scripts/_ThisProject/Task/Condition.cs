using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConditionTargetAble
{
    public int GetLevel();
    public DesignItems<Item> GetDesignItems(ItemQuality itemQuality, string itemName);
    public DesignItems<Item> GetDesignItems_Category(ItemQuality itemQuality, ItemCategory itemCategory, EquipType equipType = EquipType.无);
}

public interface IConditionHolderAble
{
    public int GetTargetBeatCount(string targetName);
    public bool GetTargetTalkState(string targetName);
}

public enum ConditionType
{
    等级,
    任务,
    击杀,
    对话,
    收集,
}
public enum SizeType
{
    大于,
    小于,
    等于,
    大于等于,
    小于等于,
}
public class ConditionContainer//注意此类不可以序列化
{
    public ConditionContainer(List<Condition> replaceableCondition)
    {
        this.replaceableCondition = replaceableCondition;
    }
    public List<Condition> replaceableCondition;
}
[System.Serializable]
public class Condition : SelfAdaptationClassMgr
{
    //表示该条件隶属于哪个可选条件集合
    [System.NonSerialized] public ConditionContainer conditionContainter;//此类型不可以序列化，里面是通过代码获取的当前实例的容器

    public ConditionType conditionType;
    public bool isOverrideTip;
    public ConditionTip conditionTip;

    public Condition_Level condition_Level;
    public Condition_Task condition_Task;
    public Condition_Beat condition_Beat;
    public Condition_Talk condition_Talk;
    public Condition_Collect condition_Collection;

    public static bool CheckValue(SizeType sizeType, float targetValue, float targetCheckValue)
    {
        if (sizeType == SizeType.大于)
            return targetCheckValue > targetValue;
        else if (sizeType == SizeType.小于)
            return targetCheckValue < targetValue;
        else if (sizeType == SizeType.大于等于)
            return targetCheckValue >= targetValue;
        else if (sizeType == SizeType.小于等于)
            return targetCheckValue <= targetValue;
        else
            return targetCheckValue == targetValue;
    }

    public bool Check(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        if (conditionType == ConditionType.等级)
            return condition_Level.Check(holderCondition, targetCondition);
        else if (conditionType == ConditionType.任务)
            return condition_Task.Check(holderCondition, targetCondition);
        else if (conditionType == ConditionType.击杀)
            return condition_Beat.Check(holderCondition, targetCondition);
        else if (conditionType == ConditionType.对话)
            return condition_Talk.Check(holderCondition, targetCondition);
        else if (conditionType == ConditionType.收集)
            return condition_Collection.Check(holderCondition, targetCondition);
        return true;
    }

    public string GetTip(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        if (isOverrideTip)
            return conditionTip.content;

        string tip = "";
        if (conditionType == ConditionType.等级)
            tip = condition_Level.GetTip(holderCondition, targetCondition);
        else if (conditionType == ConditionType.任务)
            tip = condition_Task.GetTip(holderCondition, targetCondition);
        else if (conditionType == ConditionType.击杀)
            tip = condition_Beat.GetTip(holderCondition, targetCondition);
        else if (conditionType == ConditionType.对话)
            tip = condition_Talk.GetTip(holderCondition, targetCondition);
        else if (conditionType == ConditionType.收集)
            tip = condition_Collection.GetTip(holderCondition, targetCondition);
        return tip;
    }
}
//public bool Check(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
//{
//    return true;
//}

//public string GetTip(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
//{
//    return "";
//}
[System.Serializable]
public class ConditionTip
{
    [TextArea(2, 5)]
    public string content;
}

[System.Serializable]
public class Condition_Level
{
    public int level;
    public SizeType bigOrSmallType;

    public bool Check(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        int characterLevel = targetCondition.GetLevel();
        //if (bigOrSmallType == SizeType.大于)
        //    return characterLevel > this.level;
        //else if (bigOrSmallType == SizeType.小于)
        //    return characterLevel < this.level;
        //else if (bigOrSmallType == SizeType.大于等于)
        //    return characterLevel >= this.level;
        //else if (bigOrSmallType == SizeType.小于等于)
        //    return characterLevel <= this.level;
        //else
        //    return characterLevel == this.level;

        return Condition.CheckValue(bigOrSmallType, level, characterLevel);
    }


    public string GetTip(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        return "等级" + bigOrSmallType + level + "..." + "(" + targetCondition.GetLevel() + "/" + level + ")";
    }
}
[System.Serializable]
public class Condition_Task
{
    public string taskName;
    public bool Check(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        return TaskMgr.GetInstance().IsFinishTak(taskName);
    }
    public string GetTip(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        return "完成" + taskName + "任务";
    }
}
[System.Serializable]
public class Condition_Beat
{
    public string targetName;
    public int count;
    public bool Check(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        int currentCount = holderCondition.GetTargetBeatCount(targetName);
        return currentCount >= count;
    }

    public string GetTip(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        int currentCount = holderCondition.GetTargetBeatCount(targetName);
        return "击败" + "\"" + targetName + "\"。" + "..." + "(" + Mathf.Clamp(currentCount, 0, count) + "/" + count + ")";
    }
}

[System.Serializable]
public class Condition_Talk
{
    public NPCInfo_TalkInfo talkInfo_Before;
    public NPCInfo_TalkInfo talkInfo_After;

    public bool Check(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        return holderCondition.GetTargetTalkState(talkInfo_Before.NPC_Name);
    }

    public string GetTip(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        return "与" + talkInfo_Before.NPC_Name + "进行对话";
    }
}

public enum CollectionType
{
    指定物品,
    指定类别,
}

[System.Serializable]
public class Condition_Collect : SelfAdaptationClassMgr
{
    public CollectionType collectionType;
    public ItemQuality itemQuality;
    public ItemCategory itemCategory = ItemCategory.无;
    public EquipType equipType;
    public string itemName;
    public int count;

    private DesignItems<Item> designItems;

    public DesignItems<Item> GetDesignItems()
    {
        return designItems;
    }
    public bool Check(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        return GetTargetCount(holderCondition, targetCondition) >= count;
    }

    public string GetTip(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        return "收集" + "\"" + itemName + "\"。" + "..." + "(" + Mathf.Clamp(GetTargetCount(holderCondition, targetCondition), 0, count) + "/" + count + ")";
    }

    private int GetTargetCount(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        int count = 0;
        if (collectionType == CollectionType.指定物品)
        {
            designItems = targetCondition.GetDesignItems(itemQuality, itemName);
            if (designItems != null)
                count = designItems.allCount;
        }
        else if (collectionType == CollectionType.指定类别)
        {
            designItems = targetCondition.GetDesignItems_Category(itemQuality, itemCategory, equipType);
            if (designItems != null)
                count = designItems.allCount;
        }
        return count;
    }
}