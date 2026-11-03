using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PropertyType
{
    //其实不想用中文，不方便代码提示，但是这在随机属性时直接应用到UI中
    生命值上限 = 0,
    魔法值上限 = 1,
    攻击力 = 2,
    魔力 = 3,
    物理防御 = 4,
    魔法防御 = 5,

    生命值 = 6,
    魔法值 = 7,

    物理附加伤害 = 8,
    魔法附加伤害 = 9,
    真实附加伤害 = 10,

    每秒回血 = 11,
    每秒回蓝 = 12,

    护盾 = 13,
    免伤率 = 14,

    经验值上限 = 15,
    经验值 = 16,
    无 = 17,

    攻速 = 18,
    金币 = 19,
    钻石 = 20,

    物理穿透 = 21,
    魔法穿透 = 22,

    暴击率 = 23,
    抗爆率 = 24,
    暴击效果 = 25,

    命中率 = 26,
    闪避率 = 27,

    饱腹度上限 = 28,
    饱腹度 = 29,
}

public interface IPropertyAble
{
    public float GetExtraProperty_Equip(PropertyType propertyType);//获取装备加持过的对应属性
    public float GetExtraProperty_Buff(PropertyType propertyType, IEffectSourceAble effectSource);//获取buff加持后的对应属性
}
public class Property
{
    private IPropertyAble propertyAble;
    private Warrior warrior;
    private PropertyValue[] propertyFactors;
    private int level;
    public Property(IPropertyAble propertyAble, int level, PropertyValue[] propertyFactors)//传入等级和各属性的成长系数
    {
        if (propertyFactors == null)
            return;
        this.propertyAble = propertyAble;
        this.propertyFactors = propertyFactors;
        this.CreateProperty(level, propertyFactors);
    }
    private void CreateProperty(int level, PropertyValue[] propertyFactors)
    {
        LevelUp(level, propertyFactors, 0);
        if (CurrentPropertys.ContainsKey(PropertyType.经验值上限))
            CurrentPropertys[PropertyType.经验值] = 0;

        RecoverState();
    }
    public bool IsContainProperty(PropertyType propertyType)
    {
        return CurrentPropertys.ContainsKey(propertyType);
    }
    //物体最原始的属性
    public float GetOriginalProperty(PropertyType propertyType)
    {
        float value = 0;
        if (CurrentPropertys.ContainsKey(propertyType))
            value += CurrentPropertys[propertyType];
        return value;
    }
    //获取装备加持过的属性
    public float GetBaseProperty(PropertyType propertyType)
    {
        float value = 0;
        if (CurrentPropertys.ContainsKey(propertyType))
            value += CurrentPropertys[propertyType];
        if (propertyAble != null)
            value += propertyAble.GetExtraProperty_Equip(propertyType);
        return value;
    }
    //获取装备，buff加持过的属性
    public float GetCurrentProperty(PropertyType propertyType, IEffectSourceAble effectSource = null)
    {
        float value = 0;
        if (CurrentPropertys.ContainsKey(propertyType))
            value += CurrentPropertys[propertyType];
        if (propertyAble != null)
        {
            value += propertyAble.GetExtraProperty_Equip(propertyType);
            value += propertyAble.GetExtraProperty_Buff(propertyType, effectSource);
        }
        return value;
    }

    public int GetLevel()
    {
        return level;
    }

    /// <summary>
    /// 升1级
    /// </summary>
    /// <param name="overflowExperience">溢出经验值</param>
    public void LevelUpOne(float overflowExperience, bool isFull = false)
    {
        LevelUp(this.level + 1, overflowExperience, isFull);
    }
    /// <summary>
    /// 升级
    /// </summary>
    /// <param name="level">目标等级</param>
    /// <param name="overflowExperience">溢出经验值</param>
    public void LevelUp(int level, float overflowExperience, bool isFull = false)
    {
        LevelUp(level, this.propertyFactors, overflowExperience);
        if (isFull)
            RecoverState();
    }
    /// <summary>
    /// 升级
    /// </summary>
    /// <param name="level">目标等级</param>
    /// <param name="propertyFactors">成长参数</param>
    /// <param name="overflowExperience">溢出经验值</param>
    private void LevelUp(int level, PropertyValue[] propertyFactors, float overflowExperience)
    {
        this.level = level;

        if (propertyFactors != null)
        {
            for (int i = 0; i < propertyFactors.Length; i++)
            {
                PropertyType type = propertyFactors[i].propertyType;
                if (LeveUpBreakProperty.Contains(type) && CurrentPropertys.ContainsKey(type))//如果是特殊类型，比如金币和钻石，除了第一次，后面都不需要再次计算
                    continue;
                float value = propertyFactors[i].baseValue + propertyFactors[i].growValue * (level - 1);//减一是因为一级的时候不用这个成长值
                CurrentPropertys[type] = value;
            }
        }

        if (CurrentPropertys.ContainsKey(PropertyType.经验值上限))
        {
            CurrentPropertys[PropertyType.经验值] = 0;//升完级把经验值清空
            //对一次性获得大量经验连升多级的处理
            float newOverflowExperience = ChangerProperty(PropertyType.经验值, overflowExperience);
            if (IsExperienceFull())
                LevelUpOne(newOverflowExperience);
        }
    }
    public void RecoverState()
    {
        if ((propertyAble is Warrior) == false)
            return;

        if (CurrentPropertys.ContainsKey(PropertyType.生命值上限))
            CurrentPropertys[PropertyType.生命值] = GetCurrentProperty(PropertyType.生命值上限);
        if (CurrentPropertys.ContainsKey(PropertyType.魔法值上限))
            CurrentPropertys[PropertyType.魔法值] = GetCurrentProperty(PropertyType.魔法值上限);
        if (CurrentPropertys.ContainsKey(PropertyType.饱腹度上限))
            CurrentPropertys[PropertyType.饱腹度] = GetCurrentProperty(PropertyType.饱腹度上限);
    }
    //当前属性值，用于动态改变
    private Dictionary<PropertyType, float> currentPropertys = new Dictionary<PropertyType, float>();
    public Dictionary<PropertyType, float> CurrentPropertys
    {
        get { return currentPropertys; }
        set { currentPropertys = value; }
    }

    public static string GetAttackTypeText(PropertyType type)
    {
        string content = "";
        if (type == PropertyType.攻击力)
            content = "物理";
        else if (type == PropertyType.魔力)
            content = "魔法";
        return content;
    }
    //不同类型的攻击对应的防御映射
    public static Dictionary<PropertyType, PropertyType> attackDefenceMap = new Dictionary<PropertyType, PropertyType>()
    {
        {PropertyType.攻击力,PropertyType.物理防御 },
        {PropertyType.魔力,PropertyType.魔法防御 },
    };
    //不同类型的攻击对应的穿透类型
    public static Dictionary<PropertyType, PropertyType> attackPenetrateMap = new Dictionary<PropertyType, PropertyType>()
    {
        {PropertyType.攻击力,PropertyType.物理穿透 },
        {PropertyType.魔力,PropertyType.魔法穿透 },
    };
    //不同类型的攻击对应的穿透类型
    public static Dictionary<PropertyType, PropertyType> penetrateDefenceMap = new Dictionary<PropertyType, PropertyType>()
    {
        {PropertyType.物理穿透,PropertyType.物理防御 },
        {PropertyType.魔法穿透,PropertyType.魔法防御 },
    };

    public static Dictionary<PropertyType, PropertyType> UpLimitMap = new Dictionary<PropertyType, PropertyType>()
    {
        { PropertyType.生命值,PropertyType.生命值上限 },
        { PropertyType.魔法值,PropertyType.魔法值上限 },
        { PropertyType.经验值,PropertyType.经验值上限 },
    };

    public static HashSet<PropertyType> LeveUpBreakProperty = new HashSet<PropertyType>()
    {
        PropertyType.金币,
        PropertyType.钻石,
    };

    //以下为比率值
    public static HashSet<PropertyType> RatioValueDic = new HashSet<PropertyType>()
    {
        { PropertyType.物理穿透 },
        { PropertyType.魔法穿透 },
        { PropertyType.暴击率 },
        { PropertyType.抗爆率 },
        { PropertyType.暴击效果 },
        { PropertyType.命中率 },
        { PropertyType.闪避率 },
        { PropertyType.攻速 },
    };
    //public static Dictionary<PropertyType, PropertyType> LossPropertyDic = new Dictionary<PropertyType, PropertyType>()
    //{
    //    { PropertyType.生命值,PropertyType.生命值上限},
    //    { PropertyType.魔法值,PropertyType.魔法值上限},
    //};
    public static PropertyType AttackDefenceMap(PropertyType propertyType)
    {
        if (!attackDefenceMap.ContainsKey(propertyType))
            return PropertyType.无;

        return attackDefenceMap[propertyType];
    }
    public static PropertyType AttackPenetrateMap(PropertyType propertyType)
    {
        if (!attackPenetrateMap.ContainsKey(propertyType))
            return PropertyType.无;

        return attackPenetrateMap[propertyType];
    }
    public static PropertyType PenetrateDefenceMap(PropertyType propertyType)
    {
        if (!penetrateDefenceMap.ContainsKey(propertyType))
            return PropertyType.无;

        return penetrateDefenceMap[propertyType];
    }

    public bool PropertyCheck()
    {
        if (!CurrentPropertys.ContainsKey(PropertyType.攻击力))
        {
            Debug.Log("攻击属性缺失");
            return false;
        }
        if (!CurrentPropertys.ContainsKey(PropertyType.魔力))
        {
            Debug.Log("魔力属性缺失");
            return false;
        }
        if (!CurrentPropertys.ContainsKey(PropertyType.物理防御))
        {
            Debug.Log("物理防御属性缺失");
            return false;
        }
        if (!CurrentPropertys.ContainsKey(PropertyType.魔法防御))
        {
            Debug.Log("魔法防御属性缺失");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 修改当前属性值，并返回溢出值，返回溢出值仅对有上限值的属性有效
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public float ChangerProperty(PropertyType propertyType, float value)
    {
        if (!CurrentPropertys.ContainsKey(propertyType))
        {
            Debug.Log("信息文件缺少" + propertyType);
            return 0;
        }
        CurrentPropertys[propertyType] += value;

        float overflow = CheckClamp(propertyType);
        return overflow;
    }

    public void SetProperty(PropertyType propertyType, float value)
    {
        if (!CurrentPropertys.ContainsKey(propertyType))
        {
            Debug.Log("信息文件缺少" + propertyType);
            return;
        }
        CurrentPropertys[propertyType] = value;
        CheckClamp(propertyType);
    }
    public void CheckAllClamp()
    {
        foreach (var item in UpLimitMap)
            CheckClamp(item.Key);
    }
    private float CheckClamp(PropertyType propertyType)
    {
        float overflow = 0;
        if (UpLimitMap.ContainsKey(propertyType))
        {
            if (GetCurrentProperty(propertyType) > GetCurrentProperty(UpLimitMap[propertyType]))
                overflow = GetCurrentProperty(propertyType) - GetCurrentProperty(UpLimitMap[propertyType]);

            CurrentPropertys[propertyType] = Mathf.Clamp(GetCurrentProperty(propertyType), 0, GetCurrentProperty(UpLimitMap[propertyType]));
        }
        else
            CurrentPropertys[propertyType] = Mathf.Clamp(GetCurrentProperty(propertyType), 0, float.MaxValue);

        return overflow;
    }

    //查阅某个属性值是否足够
    public PropertyType CheckPropertyEnough(PropertyType propertyType, float value)
    {
        if (!CurrentPropertys.ContainsKey(propertyType))
        {
            Debug.Log("物体不包含改属性，请确认并修改");
            return PropertyType.无;
        }

        if (value < 0 && GetCurrentProperty(propertyType) < Mathf.Abs(value))
            return propertyType;

        return PropertyType.无;
    }

    //经验值是否已经满了
    public bool IsExperienceFull()
    {
        if (!CurrentPropertys.ContainsKey(PropertyType.经验值) || !CurrentPropertys.ContainsKey(PropertyType.经验值上限))
            return false;

        return GetCurrentProperty(PropertyType.经验值) == GetCurrentProperty(PropertyType.经验值上限);
    }

    public bool IsDie()
    {
        return GetCurrentProperty(PropertyType.生命值) <= 0;
    }
}
