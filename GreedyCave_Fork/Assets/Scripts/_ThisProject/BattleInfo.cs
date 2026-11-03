using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleInfo
{
    public BattleInfo(Warrior user, Warrior target)
    {
        this.user = user;
        //SetBattleInfo(user, EnemyTargets);
    }
    //public Warrior GetRandomEnemyTarget()
    //{
    //    return ToolsMgr.GetRandomTarget<Warrior>(EnemyTargets);
    //}
    public Warrior GetRandomEnemyTarget()
    {

        return ToolsMgr.GetRandomTarget<Warrior>(GetOptionalTargets(LockTargetType.Enemy));
    }
    public List<Warrior> GetRandomTargets(int count)
    {
        return ToolsMgr.GetRandom_ShuffleCards<Warrior>(GetOptionalTargets(LockTargetType.Enemy), count);
    }

    //public int GetTargetCount()
    //{
    //    return EnemyTargets.Count;
    //}

    private Warrior user;//使用者
    private Warrior lockTarget;//锁定目标
    //private List<Warrior> hitTarget = new List<Warrior>();//命中目标
    //private List<Warrior> enemyTargets = new List<Warrior>();//可作用目标
    //private List<Warrior> beatEnemyWarrior = new List<Warrior>();//战胜的目标
    //private List<Warrior> teamTargets = new List<Warrior>();//队友
    //private List<Warrior> dieTeamWarrior = new List<Warrior>();//阵亡队友

    private Dictionary<LockTargetType, List<Warrior>> optionalTargetDic = new Dictionary<LockTargetType, List<Warrior>>();//可以选中的队友和对手
    private Dictionary<LockTargetType, List<Warrior>> hitTargetDic = new Dictionary<LockTargetType, List<Warrior>>();//命中的队友和对手
    private Dictionary<LockTargetType, List<Warrior>> dieTargetDic = new Dictionary<LockTargetType, List<Warrior>>();//阵亡的队友和对手
    public int GetFixedTargetCount(LockTargetInfoMgr lockTargetInfoMgr, LockTargetType lockTargetType)
    {
        if (lockTargetInfoMgr == null)
            return 0;
        if (optionalTargetDic.Count <= 0)
            return 0;
        int count = 0;
        if (optionalTargetDic.ContainsKey(lockTargetType))
            count = lockTargetInfoMgr.GetFixedTargetCount(lockTargetType);
        return count;
    }
    public int GetAllFixedTargetCount(LockTargetInfoMgr lockTargetInfoMgr)
    {
        int count = 0;
        foreach (var item in optionalTargetDic)
        {
            count += GetFixedTargetCount(lockTargetInfoMgr, item.Key);
        }
        return count;
    }
    public bool IsCanSelect(LockTargetInfoMgr lockTargetInfoMgr)
    {
        if (lockTargetInfoMgr == null)
            return false;
        foreach (var item in optionalTargetDic)
        {
            LockTargetType lockTargetType = item.Key;
            if (GetFixedTargetCount(lockTargetInfoMgr, lockTargetType) > 0)
            {
                if (optionalTargetDic.ContainsKey(lockTargetType) && (optionalTargetDic[lockTargetType].Count > 0))
                    return true;//只要有一个满足，就可以进行选择
            }
        }
        return false;
    }
    public List<Warrior> GetFixedOptionalTargetByInfo(LockTargetInfoMgr lockTargetInfoMgr,LockTargetType lockTargetType)
    {
        if (lockTargetInfoMgr == null)
            return null;
        List<Warrior> optionalTargets = new List<Warrior>(GetOptionalTargets(lockTargetType));
        optionalTargets.Add(user);
        return lockTargetInfoMgr.GetOptionalFixedTargets(lockTargetType, optionalTargets);
    }
    public List<Warrior> GetAllRandomTargetByInfo(LockTargetInfoMgr lockTargetInfoMgr)
    {
        if (lockTargetInfoMgr == null)
            return null;
        List<Warrior> warriors = new List<Warrior>();
        foreach (var item in optionalTargetDic)
        {
            LockTargetType lockTargetType = item.Key;
            List<Warrior> randomrWarriors = GetRandomTargetByInfo(lockTargetInfoMgr, lockTargetType);
            if (randomrWarriors != null && randomrWarriors.Count > 0)
            {
                warriors = warriors.Concat(randomrWarriors).ToList<Warrior>();
            }
        }
        return warriors;
    }
    public List<Warrior> GetRandomTargetByInfo(LockTargetInfoMgr lockTargetInfoMgr, LockTargetType lockTargetType)
    {
        if (lockTargetInfoMgr == null)
            return null;
        List<Warrior> optionalTargets = new List<Warrior>(GetOptionalTargets(lockTargetType));
        optionalTargets.Add(user);
        return lockTargetInfoMgr.GetRandomTargets(lockTargetType, optionalTargets);
    }
    public void StartBehaviour()
    {
        hitTargetDic.Clear();
        damageValueDic.Clear();
    }
    public void ClearBattleInfo()
    {
        lockTarget = null;
        optionalTargetDic.Clear();
        hitTargetDic.Clear();
        dieTargetDic.Clear();
        damageValueDic.Clear();
    }

    private void ForeachAllTarget(Dictionary<LockTargetType, List<Warrior>> targetDic, LockTargetType lockTargetType,Action<Warrior> action)
    {
        if (!targetDic.ContainsKey(lockTargetType) || action == null)
            return;
        if (targetDic[lockTargetType] == null)
            return;
        for (int i = 0; i < targetDic[lockTargetType].Count; i++)
        {
            Warrior warrior = targetDic[lockTargetType][i];
            if (warrior == null)
                continue;
            action.Invoke(warrior);
        }
    }
    public void ForeachAllOptionalTarget(LockTargetType lockTargetType, Action<Warrior> action)
    {
        ForeachAllTarget(optionalTargetDic, lockTargetType, action);
    }
    public void ForeachAllHitTarget(LockTargetType lockTargetType, Action<Warrior> action)
    {
        ForeachAllTarget(hitTargetDic, lockTargetType, action);
    }
    public void ForeachAllDieTarget(LockTargetType lockTargetType, Action<Warrior> action)
    {
        ForeachAllTarget(dieTargetDic, lockTargetType, action);
    }

    private void AddTarget(Dictionary<LockTargetType, List<Warrior>> targetDic, LockTargetType lockTargetType, Warrior warrior)
    {
        if (warrior == null)
            return;
        if (!targetDic.ContainsKey(lockTargetType))
            targetDic[lockTargetType] = new List<Warrior>();
        targetDic[lockTargetType].Add(warrior);
    }
    public void AddOptionalTarget(LockTargetType lockTargetType, Warrior warrior)
    {
        AddTarget(optionalTargetDic, lockTargetType, warrior);
    }
    public void AddHitTarget(LockTargetType lockTargetType, Warrior warrior)
    {
        AddTarget(hitTargetDic, lockTargetType, warrior);
    }
    public void AddDieTarget(LockTargetType lockTargetType, Warrior warrior)
    {
        AddTarget(dieTargetDic, lockTargetType, warrior);
    }
    private void RemoveTarget(Dictionary<LockTargetType, List<Warrior>> targetDic, LockTargetType lockTargetType, Warrior warrior)
    {
        if (warrior == null)
            return;
        if (!targetDic.ContainsKey(lockTargetType))
            return;
        if (targetDic[lockTargetType] == null)
            return;
        targetDic[lockTargetType].Remove(warrior);
    }
    public void RemoveOptionalTarget(LockTargetType lockTargetType, Warrior warrior)
    {
        RemoveTarget(optionalTargetDic, lockTargetType, warrior);
    }
    public void RemoveHitTarget(LockTargetType lockTargetType, Warrior warrior)
    {
        RemoveTarget(hitTargetDic, lockTargetType, warrior);
    }
    public void RemoveDieTarget(LockTargetType lockTargetType, Warrior warrior)
    {
        RemoveTarget(dieTargetDic, lockTargetType, warrior);
    }
    private List<Warrior> GetTargets(Dictionary<LockTargetType, List<Warrior>> targetDic, LockTargetType lockTargetType)
    {
        if (!targetDic.ContainsKey(lockTargetType))
            return null;
        if (targetDic[lockTargetType] == null)
            return null;
        return targetDic[lockTargetType];
    }

    public List<Warrior> GetOptionalTargets(LockTargetType lockTargetType)
    {
        return GetTargets(optionalTargetDic, lockTargetType);
    }
    public List<Warrior> GetHitTargets(LockTargetType lockTargetType)
    {
        return GetTargets(hitTargetDic, lockTargetType);
    }
    public List<Warrior> GetDieTargets(LockTargetType lockTargetType)
    {
        return GetTargets(dieTargetDic, lockTargetType);
    }
    private int GetTargetCount(Dictionary<LockTargetType, List<Warrior>> targetDic, LockTargetType lockTargetType)
    {
        if (!targetDic.ContainsKey(lockTargetType))
            return 0;
        if (targetDic[lockTargetType] == null)
            return 0;
        return targetDic[lockTargetType].Count;
    }
    private int GetAllTargetCount(Dictionary<LockTargetType, List<Warrior>> targetDic)
    {
        if (targetDic == null)
            return 0;
        int count = 0;
        foreach (var item in targetDic)
        {
            if(item.Value != null)
                count += item.Value.Count;
        }
        return count;
    }
    public int GetAllOptionalTargetCount()
    {
        return GetAllTargetCount(optionalTargetDic);
    }
    public int GetOptionalTargetCount(LockTargetType lockTargetType)
    {
        return GetTargetCount(optionalTargetDic, lockTargetType);
    }
    public int GetAllHitTargetCount()
    {
        return GetAllTargetCount(hitTargetDic);
    }
    public int GetHitTargetCount(LockTargetType lockTargetType)
    {
        return GetTargetCount(hitTargetDic, lockTargetType);
    }
    public int GetAllDieTargetCount()
    {
        return GetAllTargetCount(dieTargetDic);
    }
    public int GetDieTargetCount(LockTargetType lockTargetType)
    {
        return GetTargetCount(dieTargetDic, lockTargetType);
    }
    
    private Warrior GetLastTarget(Dictionary<LockTargetType, List<Warrior>> targetDic, LockTargetType lockTargetType)
    {
        if (!targetDic.ContainsKey(lockTargetType))
            return null;

        if (targetDic[lockTargetType] == null)
            return null;
        List<Warrior> warriors = targetDic[lockTargetType];
        if (warriors != null && warriors.Count > 0)
            return warriors[warriors.Count - 1];
        return null;
    }
    public Warrior GetLastDieTarget(LockTargetType lockTargetType)
    {
        return GetLastTarget(dieTargetDic, lockTargetType);
    }
    public Warrior User
    {
        get { return user; }
        set { user = value; }
    }
    public Warrior LockTarget
    {
        get
        {
            if (lockTarget == null)
                lockTarget = GetRandomEnemyTarget();
            return lockTarget;
        }
        set { lockTarget = value; }
    }
    private Dictionary<PropertyType, float> damageValueDic = new Dictionary<PropertyType, float>();
    public Dictionary<PropertyType, float> DamageValueDic
    {
        get { return damageValueDic; }
        set { damageValueDic = value; }
    }
    private Dictionary<PropertyType, float> injuredValueDic = new Dictionary<PropertyType, float>();
    public Dictionary<PropertyType, float> InjuredValueDic
    {
        get { return injuredValueDic; }
        set { injuredValueDic = value; }
    }
    //public Dictionary<PropertyType, float> GetDamageValueDic()
    //{
    //    return damageValueDic;
    //}
    public static void ClearDicValue(Dictionary<PropertyType, float> damageValueDic)
    {
        damageValueDic.Clear();
    }
    public static void AddDicValue(Dictionary<PropertyType, float> damageValueDic, PropertyType damageType, float value)
    {
        if (!damageValueDic.ContainsKey(damageType))
            damageValueDic[damageType] = 0;
        damageValueDic[damageType] += value;
    }
    public static float GetDicValue(Dictionary<PropertyType, float> damageValueDic, PropertyType damageType)
    {
        if (!damageValueDic.ContainsKey(damageType))
            return 0;
        return damageValueDic[damageType];
    }
    public void ClearDamageValue()
    {
        damageValueDic.Clear();
    }
    public void ClearInjuredValue()
    {
        injuredValueDic.Clear();
    }

    //public void AddDamageValue(PropertyType damageType, float value)
    //{
    //    if (!damageValueDic.ContainsKey(damageType))
    //        damageValueDic[damageType] = 0;
    //    damageValueDic[damageType] += value;
    //    AddDicValue(damageValueDic, damageType, value);
    //}
    public float GetDamageValue(PropertyType damageType)
    {
        //if (!damageValueDic.ContainsKey(damageType))
        //    return 0;
        //return damageValueDic[damageType];
        return GetDicValue(damageValueDic, damageType);
    }
    public float GetInjuredValue(PropertyType damageType)
    {
        return GetDicValue(injuredValueDic, damageType);
    }
}


