using System;
using System.Collections.Generic;
using UnityEngine;
//[System.Serializable]
//public class FixedConditions
//{
//    public Condition condition;
//}
[System.Serializable]
public class FixedConditions
{
    public Condition condition;
}
[System.Serializable]
public class ReplaceableConditions : IGetAble
{
    /// <summary>
    /// 注意此处的一个List<Condition>内部元素之间是或的关系
    /// </summary>
    public List<Condition> conditions;
    public void InitialConditions()
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            //ConditionContainter是不可以序列化的，所以不可以用ReplaceableConditions代替
            conditions[i].conditionContainter = new ConditionContainer(conditions);
        }
    }
}

[System.Serializable]
public class ConditionMgr
{
    [Header("必须完成的条件")]
    [SerializeField] private List<FixedConditions> fixedConditions;//这里直接使用List<Condition>的话，Condition的内部类展开时，下面的字段都会被隐藏，不知道为什么
    [Header("每一个元素之间是且的关系，每一个元素内部的是或的关系")]
    [SerializeField] private List<ReplaceableConditions> replaceableConditions;

    public List<FixedConditions> FixedConditions
    {
        get { return fixedConditions; }
    }
    public List<ReplaceableConditions> ReplaceableConditions
    {
        get
        {
            //为了节约性能，判定第一个元素如果没有容器索引，则后面的都没有
            if (replaceableConditions.Count > 0 && replaceableConditions[0].conditions.Count > 0)
            {
                if (replaceableConditions[0].conditions[0].conditionContainter == null)
                {
                    for (int i = 0; i < replaceableConditions.Count; i++)
                    {
                        replaceableConditions[i].InitialConditions();//主要是让replaceableConditions的每一个或关系的条件获取其他共同作用于同一个bool值的"队友"列表
                    }
                }
            }
            return replaceableConditions;
        }
    }

    public static Dictionary<bool, List<Condition>> GetConditionsFinishInfo(IConditionHolderAble holderConditionAble, IConditionTargetAble targetConditionAble, List<FixedConditions> fixedConditions)
    {
        Dictionary<bool, List<Condition>> checkConditionsDic = new Dictionary<bool, List<Condition>>();
        if (fixedConditions == null || fixedConditions.Count <= 0)
            return checkConditionsDic;

        for (int i = 0; i < fixedConditions.Count; i++)
        {
            bool isFinish = fixedConditions[i].condition.Check(holderConditionAble, targetConditionAble);
            if (!checkConditionsDic.ContainsKey(isFinish))
                checkConditionsDic[isFinish] = new List<Condition>();

            checkConditionsDic[isFinish].Add(fixedConditions[i].condition);
        }

        return checkConditionsDic;
    }
    public static Dictionary<bool, List<Condition>> GetConditionsFinishInfo(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition, List<Condition> conditions)
    {
        Dictionary<bool, List<Condition>> checkConditionsDic = new Dictionary<bool, List<Condition>>();
        if (conditions == null || conditions.Count <= 0)
            return checkConditionsDic;

        for (int i = 0; i < conditions.Count; i++)
        {
            bool isFinish = conditions[i].Check(holderCondition, targetCondition);
            if (!checkConditionsDic.ContainsKey(isFinish))
                checkConditionsDic[isFinish] = new List<Condition>();

            checkConditionsDic[isFinish].Add(conditions[i]);
        }

        return checkConditionsDic;
    }

    private bool CheckConditions_And(Dictionary<bool, List<Condition>> fixedConditionsCheck)
    {
        if (fixedConditionsCheck.ContainsKey(false))
            return false;
        return true;
    }
    private bool CheckConditions_Or(Dictionary<bool, List<Condition>> orConditionsCheck)
    {
        if (orConditionsCheck.ContainsKey(true))
            return true;
        return false;
    }
    private bool CheckCondistion_Fixed_Replaceable(Dictionary<bool, List<Condition>> fixedConditionsCheck, Dictionary<bool, List<Dictionary<bool, List<Condition>>>> replaceableConditionsCheck)
    {
        if (fixedConditionsCheck.ContainsKey(false) || replaceableConditionsCheck.ContainsKey(false))
            return false;

        return true;
    }
    /// <summary>
    /// 返回ConditionMgr是否满足条件，包含了“且条件”和“或条件”的处理，返回最终结果
    /// </summary>
    /// <param name="targetConditionAble"></param>
    /// <returns></returns>
    public ConditionMgrState GetConditionMgrState(IConditionHolderAble holderConditionAble, IConditionTargetAble targetConditionAble)
    {
        //“并且”关系的一个List中的是否已完成对应Condition
        Dictionary<bool, List<Condition>> fixedConditionsCheck = ConditionMgr.GetConditionsFinishInfo(holderConditionAble, targetConditionAble, FixedConditions);
        //把最内部的Dictionary<bool, List<Condition>视作一个Condition，因为他的内部是或关系的Condition，其他理解与上面相同
        Dictionary<bool, List<Dictionary<bool, List<Condition>>>> replaceableConditionsCheck = new Dictionary<bool, List<Dictionary<bool, List<Condition>>>>();

        for (int i = 0; i < ReplaceableConditions.Count; i++)
        {
            Dictionary<bool, List<Condition>> orConditionsCheck = ConditionMgr.GetConditionsFinishInfo(holderConditionAble, targetConditionAble, ReplaceableConditions[i].conditions);
            if (orConditionsCheck.Count <= 0)//内容为空，就不要这个
                continue;

            bool isFinish = CheckConditions_Or(orConditionsCheck);
            if (!replaceableConditionsCheck.ContainsKey(isFinish))
                replaceableConditionsCheck[isFinish] = new List<Dictionary<bool, List<Condition>>>();

            replaceableConditionsCheck[isFinish].Add(orConditionsCheck);
        }
        bool isSatisfied = CheckCondistion_Fixed_Replaceable(fixedConditionsCheck, replaceableConditionsCheck);

        ConditionMgrState conditionMgrState = new ConditionMgrState(isSatisfied, fixedConditionsCheck, replaceableConditionsCheck);
        return conditionMgrState;
    }

    public void ForeachAdllCondition(Action<Condition> action)
    {
        if (FixedConditions != null)
        {
            for (int i = 0; i < FixedConditions.Count; i++)
            {
                Condition condition = FixedConditions[i].condition;
                action.Invoke(condition);
            }
        }
        if (ReplaceableConditions != null)
        {
            for (int i = 0; i < ReplaceableConditions.Count; i++)
            {
                ReplaceableConditions replaceableCondition = ReplaceableConditions[i];
                if (replaceableCondition != null)
                {
                    for (int j = 0; j < replaceableCondition.conditions.Count; j++)
                    {
                        Condition condition = replaceableCondition.conditions[j];
                        action.Invoke(condition);
                    }
                }
            }
        }
    }

    public Dictionary<ConditionType, List<Condition>> GetAllConditionTypes()
    {
        //List<ConditionType> targetConditions = new List<ConditionType>();
        Dictionary<ConditionType, List<Condition>> targetConditions = new Dictionary<ConditionType, List<Condition>>();

        ForeachAdllCondition((Condition condition) =>
        {
            if (!targetConditions.ContainsKey(condition.conditionType))
                targetConditions[condition.conditionType] = new List<Condition>();

            targetConditions[condition.conditionType].Add(condition);
        });

        return targetConditions;
    }
}

public class ConditionMgrState
{
    public ConditionMgrState(bool isSatisfied, Dictionary<bool, List<Condition>> fixedConditionsCheck, Dictionary<bool, List<Dictionary<bool, List<Condition>>>> replaceableConditionsCheck)
    {
        this.isSatisfied = isSatisfied;
        this.fixedConditionsCheck = fixedConditionsCheck;
        this.replaceableConditionsCheck = replaceableConditionsCheck;
    }
    private bool isSatisfied;
    private Dictionary<bool, List<Condition>> fixedConditionsCheck;

    //最内层的Dictionary<bool, List<Condition>可以看作是一个Condition，因为他内部是“或关系”的Condition，其他和fixedConditionsCheck一样
    //整体上看Dictionary<bool, List<Condition>是一个与其他同级元素为并且关系的Condition。独立来看，他是内部元素为或关系的列表字典
    //Dictionary<bool, List<Condition>如果包含了Key：true，说明Dictionary<bool, List<Condition>作为一个Condition是返回true的
    //Dictionary<bool, List<Dictionary<bool, List<Condition>>>>如果是Key：false，说明List<Dictionary<bool, List<Condition>>>中的Dictionary<bool, List<Condition>>不包含key：true。其内部的所有Condition全部为fasle
    //Dictionary<bool, List<Dictionary<bool, List<Condition>>>>如果是Key：true，说明List<Dictionary<bool, List<Condition>>>中的Dictionary<bool, List<Condition>>包含Key：ture。key为true的List内部的Condition全为true，反之全为fasle
    private Dictionary<bool, List<Dictionary<bool, List<Condition>>>> replaceableConditionsCheck;

    public bool IsSatisfied()
    {
        return isSatisfied;
    }
    public string GetTip(IConditionHolderAble holderCondition, IConditionTargetAble targetCondition)
    {
        string tip = "";
        if (fixedConditionsCheck != null && fixedConditionsCheck.ContainsKey(false))
        {
            List<Condition> notFinishedFixedConditions = fixedConditionsCheck[false];
            if (notFinishedFixedConditions.Count > 0)
            {
                tip += "[未完成的条件]" + "\n";
            }
            for (int i = 0; i < notFinishedFixedConditions.Count; i++)
            {
                tip += notFinishedFixedConditions[i].GetTip(holderCondition, targetCondition) + "\n";
            }
        }
        if (replaceableConditionsCheck != null && replaceableConditionsCheck.ContainsKey(false))
        {
            List<Dictionary<bool, List<Condition>>> notFinishedReplaceableConditions = replaceableConditionsCheck[false];
            if (notFinishedReplaceableConditions.Count > 0)
            {
                tip += "[未完成的条件]" + "\n";
            }
            for (int i = 0; i < notFinishedReplaceableConditions.Count; i++)
            {
                Dictionary<bool, List<Condition>> element = notFinishedReplaceableConditions[i];
                if (element.ContainsKey(false))
                {
                    List<Condition> or_Not_Conditions = element[false];//这里只有为key：false的情况，不可能含有key：true
                    for (int j = 0; j < or_Not_Conditions.Count; j++)
                    {
                        tip += "[组" + (i + 1) + "]：" + or_Not_Conditions[j].GetTip(holderCondition, targetCondition) + "\n";
                    }
                }
            }
        }
        if (replaceableConditionsCheck != null && replaceableConditionsCheck.ContainsKey(true))
        {
            List<Dictionary<bool, List<Condition>>> finishedReplaceableConditions = replaceableConditionsCheck[true];
            if (finishedReplaceableConditions.Count > 0)
            {
                tip += ToolsMgr.GetColorText("[已完成的条件]", ToolsMgr.green) + "\n";
            }

            for (int i = 0; i < finishedReplaceableConditions.Count; i++)
            {
                Dictionary<bool, List<Condition>> element = finishedReplaceableConditions[i];
                if (element.ContainsKey(true))
                {
                    List<Condition> or_Not_Conditions = element[true];//这里只有为key：false的情况，不可能含有key：true
                    for (int j = 0; j < or_Not_Conditions.Count; j++)
                    {
                        tip += ToolsMgr.GetColorText("[组" + (i + 1) + "]：" + or_Not_Conditions[j].GetTip(holderCondition, targetCondition), ToolsMgr.green) + "\n";
                    }
                }
                if (element.ContainsKey(false))
                {
                    List<Condition> or_Not_Conditions = element[false];//这里只有为key：false的情况，不可能含有key：true
                    for (int j = 0; j < or_Not_Conditions.Count; j++)
                    {
                        tip += "[组" + (i + 1) + "]：" + or_Not_Conditions[j].GetTip(holderCondition, targetCondition) + "\n";
                    }
                }
            }
        }
        if (fixedConditionsCheck != null && fixedConditionsCheck.ContainsKey(true))
        {
            List<Condition> notFinishedFixedConditions = fixedConditionsCheck[true];
            if (notFinishedFixedConditions.Count > 0)
            {
                tip += ToolsMgr.GetColorText("[已完成的条件]", ToolsMgr.green) + "\n";
            }

            for (int i = 0; i < notFinishedFixedConditions.Count; i++)
            {
                tip += ToolsMgr.GetColorText(notFinishedFixedConditions[i].GetTip(holderCondition, targetCondition), ToolsMgr.green) + "\n";
            }
        }

        return tip;
    }
}
