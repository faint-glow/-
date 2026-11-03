using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillExistType
{
    Exist,
    Null,
}
public enum SkillType
{
    技能,
    普攻,
    被动,
}
[CreateAssetMenu(menuName = "MyDictionary/Skill/SkillInfo", fileName = "NewSkill")]
public class SkillInfo : ScriptableObject,INameAble
{
    public SkillExistType skillExist = SkillExistType.Exist;
    public SillTriggerType skillTriggerType = SillTriggerType.点按;
    public SkillType skillType;
    //public SkillDamageType SkillDamageType;
    public string skillName;
    public float skillCD = 2;
    public int learnPoint = 1;
    public Sprite skillSprite;
    [TextArea(2, 5)] public string skillInfoText;
    public LockTargetInfoMgr lockTargetInfoMgr;
    public PropertyValue[] costValue = new PropertyValue[1] { new PropertyValue(PropertyType.魔法值, -5) };
    public List<BuffInfo> talentBuffInfo;
    public List<SkillEffectMgr> skillEffectMgrs;
    public EffectFXMgr fxMgr;
    public string GetName()
    {
        return skillName;
    }
}

[System.Serializable]
public class SkillEffectMgr
{
    public EventTriggerEnum triggerEnum;
    //public List<Effect> targetEffects;
    [UnityEngine.Serialization.FormerlySerializedAs("targetEffects")]
    public List<Effect> effects;
    [System.NonSerialized]
    private Dictionary<TriggerType, List<Effect>> targetEffectDic = new Dictionary<TriggerType, List<Effect>>();
    
    public void ForeachAllEffects(Action<Effect> action)
    {
        if (action == null)
            return;
        for (int i = 0; i < effects.Count; i++)
            action.Invoke(effects[i]);
    }

    public Dictionary<TriggerType, List<Effect>> GetTargetEffectDic()
    {
        if (targetEffectDic == null || targetEffectDic.Count <= 0)
            InitialEffectDic(targetEffectDic, effects);
        return targetEffectDic;
    }
    public List<Effect> GetTargetEffectTypeList(TriggerType triggerType)
    {
        return GetEffectList_Type(triggerType, targetEffectDic, effects);
    }
    private List<Effect> GetEffectList_Type(TriggerType triggerType, Dictionary<TriggerType, List<Effect>> effectListDic, List<Effect> allList)
    {
        if (effectListDic == null || effectListDic.Count <= 0)
            InitialEffectDic(effectListDic, allList);

        if (effectListDic.ContainsKey(triggerType))
            return effectListDic[triggerType];

        return null;
    }
    private void InitialEffectDic(Dictionary<TriggerType, List<Effect>> effectListDic, List<Effect> allList)
    {
        for (int i = 0; i < allList.Count; i++)
        {
            Effect effect = allList[i];
            Effect.AddEffect(effectListDic, effect);
        }
    }
}