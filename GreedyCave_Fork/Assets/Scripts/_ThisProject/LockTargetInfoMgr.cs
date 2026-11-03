using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LockTargetInfoMgr
{
    public List<LockTargetInfo> lockTargetInfos;

    [System.NonSerialized]
    private Dictionary<LockTargetType, LockTargetInfo> lockTargetInfosDic;
    private Dictionary<LockTargetType, LockTargetInfo> LockTargetInfosDic
    {
        get 
        {
            if (lockTargetInfosDic == null || lockTargetInfosDic.Count <= 0)
            {
                lockTargetInfosDic = new Dictionary<LockTargetType, LockTargetInfo>();

                for (int i = 0; i < lockTargetInfos.Count; i++)
                {
                    LockTargetType type = lockTargetInfos[i].lockTargetType;
                    lockTargetInfosDic[type] = lockTargetInfos[i];
                }
            }
            return lockTargetInfosDic; 
        }
    }
    public int GetFixedTargetCount(LockTargetType lockTargetType)
    {
        if (LockTargetInfosDic.ContainsKey(lockTargetType))
            return LockTargetInfosDic[lockTargetType].fixedTargetCount;
        return 0;
    }
    public List<Warrior> GetOptionalFixedTargets(LockTargetType lockTargetType, List<Warrior> warriors)
    {
        if (LockTargetInfosDic.ContainsKey(lockTargetType))
            return LockTargetInfosDic[lockTargetType].GetRandomLockTargets(warriors);
        return null;
    }
    public List<Warrior> GetRandomTargets(LockTargetType lockTargetType, List<Warrior> warriors)
    {
        if (LockTargetInfosDic.ContainsKey(lockTargetType))
            return LockTargetInfosDic[lockTargetType].GetRandomLockTargets(warriors);
        return null;
    }
}

[System.Serializable]
public class LockTargetInfo:SelfAdaptationClassMgr
{
    public LockTargetType lockTargetType;
    public bool isMustContainUser;//是否必须包含自己，此项不与fixedTargetCount和randomTargetCount关联
    public bool isCanLockUser;//是否可以锁定自己
    public bool isAll;
    public bool isfixedTargetRepeat;
    public int fixedTargetCount;//可以手动选择的数量
    public bool isRandomTargetRepeat;
    public int randomTargetCount;//随机选择的数量

    //注意此方法会对warriors进行修改，因此，传过来的值必须要是一个副本出来的
    //如果是队友类型的，那么传入的warriors的最后一个必须是施法者自身
    //获取可以选中的固定敌人,用于传给UI，让玩家选择
    public List<Warrior> GetOptionalFixedLockTargets(List<Warrior> warriors)
    {
        List<Warrior> returnWarriors = null;
        if (lockTargetType == LockTargetType.Team)
        {
            Warrior user = OperiateUser(warriors, isfixedTargetRepeat);
            if (user != null)
                returnWarriors.Add(user);
        }
        return returnWarriors;
    }

    //注意此方法会对warriors进行修改，因此，传过来的值必须要是一个副本出来的
    //如果是队友类型的，那么传入的warriors的最后一个必须是施法者自身
    public List<Warrior> GetRandomLockTargets(List<Warrior> warriors)
    {
        List<Warrior> returnWarriors = null;
        if (lockTargetType == LockTargetType.Enemy)
        {
            return GetRandomLockTargetAided(warriors);
        }
        else if (lockTargetType == LockTargetType.Team)
        {
            Warrior user = OperiateUser(warriors, isRandomTargetRepeat);
            returnWarriors = GetRandomLockTargetAided(warriors);
            if (user != null)
                returnWarriors.Add(user);
        }
        return returnWarriors;
    }
    private Warrior OperiateUser(List<Warrior> warriors, bool isRepeat)
    {
        Warrior user = null;
        if (isMustContainUser)
        {
            user = warriors[warriors.Count - 1];//最后一个一定是自己
            if (isRepeat == false)
                warriors.RemoveAt(warriors.Count - 1);
        }
        else if (isCanLockUser != false)
            warriors.RemoveAt(warriors.Count - 1);//如果不可以锁定自己，就把最后一个移除，最后一个一定是自己
        return user;
    }

    private List<Warrior> GetRandomLockTargetAided(List<Warrior> warriors)
    {
        List<Warrior> returnWarriors = new List<Warrior>();
        if (isAll == true)
        {
            if (lockTargetType == LockTargetType.Enemy)
                warriors.RemoveAt(warriors.Count - 1);
            return warriors;
        }
        if (isRandomTargetRepeat == false)
            returnWarriors = ToolsMgr.GetRandom_ShuffleCards(warriors, randomTargetCount);
        else
        {
            for (int i = 0; i < randomTargetCount; i++)
            {
                int index = Random.Range(0, warriors.Count);
                returnWarriors.Add(warriors[index]);
            }
        }
        return returnWarriors;
    }

}
