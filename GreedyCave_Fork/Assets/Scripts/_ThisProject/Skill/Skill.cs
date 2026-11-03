using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public enum SillTriggerType
{
    点按,
}

//有几个SkillEffectMgr，就应该有几个AnimationEventInfo,一个AnimationEventInfo会触发一个SkillEffectMgr管理的效果
//AnimationMgrInfo是该技能所需要的全部动画信息，包含多个AnimationEventInfo和多个AnimationTriggerInfo
//AnimationEventInfo是动画事件的信息，管理动画事件的添加
//AnimationTriggerInfo是动画的触发信息，管理动画的触发

//SkillEffectMgr是该技能触发一次效果所需要的信息，一个skill中可以有多个
//Skill的一次效果也就是一个SkillEffectMgr，将被一个AnimationEventInfo触发，因此，有多少次效果，就应该有多少个AnimationEventInfo

//运行流程是技能在外界被触发，在skill内部启动AnimationTriggerInfo，由AnimationTriggerInfo启动第一次动画事件及该技能所需要的所有动画事件
//AnimationEventInfo会添加动画事件，一次事件被触发将会将会调用对应顺序的一次SkillEffectMgr效果
[System.Serializable]
public class Skill : IGetAble, IDragAble, IShortcutAble, IEffectSourceAble
{
    public string SkillName { get { return skillInfo.skillName; } }//中文名。显示在UI界面上的名字
    public Sprite SkillSprite { get { return skillInfo.skillSprite; } }
    public float SkillCD { get { return skillInfo.skillCD; } }
    public int LearnPoint { get { return skillInfo.learnPoint; } }
    public SkillType _skillType { get { return skillInfo.skillType; } }

    private SkillDamageType _skillDamageType;
    public SkillDamageType skillDamageType { get { return _skillDamageType; } }
    public void SetSkillDamageType(SkillDamageType skillType)//这个理论上讲应该在SkillInfo中预设好，但是这边就直接用代码一键生成了
    {
        this._skillDamageType = skillType;
    }
    private float skillCurrentCD;
    public float SkillCurrentCD
    {
        get { return skillCurrentCD; }
        set { skillCurrentCD = value; }
    }
    protected Warrior warrior;
    protected SkillInfo skillInfo;
    protected LockTargetInfoMgr lockTargetInfoMgr;

    protected EventTriggerEnum currentEventTrigger;
    protected AnimationMgrInfo animationMgrInfo;
    protected Dictionary<string, Dictionary<EventTriggerEnum, int>> needAnimationClips = new Dictionary<string, Dictionary<EventTriggerEnum, int>>();
    protected Dictionary<AnimationTriggerType, List<AnimationTriggerInfo>> animationTriggerTypeListDic = new Dictionary<AnimationTriggerType, List<AnimationTriggerInfo>>();

    protected Dictionary<PropertyType, float> costDic = new Dictionary<PropertyType, float>();
    protected Dictionary<EventTriggerEnum, SkillEffectMgr> skillEffectMgrs = new Dictionary<EventTriggerEnum, SkillEffectMgr>();

    public static List<string> skillInfoPaths = new List<string>()
    {
        "SkillInfo"
    };
    public static Dictionary<string, SkillInfo> skillInfoDic;
    public static Dictionary<string, SkillInfo> AllSkillInfoDic
    {
        get
        {
            if (skillInfoDic == null)
                skillInfoDic = ToolsMgr.LoadAllResourceInfo<SkillInfo>(skillInfoPaths, (skillInfo) => { return skillInfo.skillExist == SkillExistType.Exist; });
            return skillInfoDic;
        }
    }
    public static SkillInfo GetSkillInfo(string skillName)
    {
        if (AllSkillInfoDic.ContainsKey(skillName))
            return AllSkillInfoDic[skillName];

        Debug.Log("缺失资源：" + skillName + "。请确认路径");
        return null;
    }
    //public static Skill CreateSkill(Warrior warrior, AnimationMgrInfo animationMgrInfo, SkillInfo skillInfo)
    //{
    //    if (warrior == null || animationMgrInfo == null || skillInfo == null)
    //        return null;
    //    return new Skill(warrior, animationMgrInfo, skillInfo);
    //}
    public Skill(Warrior warrior, AnimationMgrInfo animationMgrInfo, SkillInfo skillInfo)
    {
        this.warrior = warrior;
        this.skillInfo = skillInfo;
        this.lockTargetInfoMgr = skillInfo.lockTargetInfoMgr;
        this.animationMgrInfo = animationMgrInfo;
        ForeachAllEventInfo((AnimationEventInfo animationEventInfo) =>
        {
            if (!needAnimationClips.ContainsKey(animationEventInfo.clipName))
                needAnimationClips[animationEventInfo.clipName] = new Dictionary<EventTriggerEnum, int>();
            //needAnimationClips.Add(animationEventInfo.clipName);
        });
        ForeachAllTriggerInfo((AnimationTriggerInfo animationTriggerInfo) =>
        {
            AnimationTriggerType type = animationTriggerInfo.animationTriggerType;
            if (!animationTriggerTypeListDic.ContainsKey(type))
                animationTriggerTypeListDic[type] = new List<AnimationTriggerInfo>();
            animationTriggerTypeListDic[type].Add(animationTriggerInfo);
        });
        for (int i = 0; i < skillInfo.skillEffectMgrs.Count; i++)
        {
            SkillEffectMgr skillEffectMgr = skillInfo.skillEffectMgrs[i];
            skillEffectMgrs[skillEffectMgr.triggerEnum] = skillEffectMgr;
            skillEffectMgr.ForeachAllEffects((effect) =>
            {
                effect.SetEffectSource(new List<IEffectSourceAble>() { this });
            });
        }
        for (int i = 0; i < skillInfo.costValue.Length; i++)
        {
            PropertyValue propertyValue = skillInfo.costValue[i];
            costDic[propertyValue.propertyType] = propertyValue.baseValue;
        }

    }

    public void TriggerSkillBuff()
    {
        for (int i = 0; i < skillInfo.talentBuffInfo.Count; i++)
        {
            if (skillInfo.talentBuffInfo[i] == null)
                continue;
            Buff buff = new Buff(skillInfo.talentBuffInfo[i], new List<IEffectSourceAble>() { this });
            this.warrior.AddBuff(this.warrior, buff);
        }
    }
    public void AddNormalAttackEvent()
    {
        if (skillInfo.skillType == SkillType.普攻)
        {
            RemoveNeedClipEvent();//其实不用移除，重复添加方法时，也会把之前的移除
            AddNeedClipEvent(null, this.warrior.GetAnimatorController(), new Dictionary<string, int>());
        }
    }

    protected void ForeachAllEventInfo(Action<AnimationEventInfo> actions)
    {
        if (actions == null)
            return;
        if (animationMgrInfo == null)
            return;
        for (int i = 0; i < animationMgrInfo.animationEventInfos.Count; i++)
        {
            AnimationEventInfo animationEventInfo = animationMgrInfo.animationEventInfos[i];
            actions.Invoke(animationEventInfo);
        }
    }
    protected void ForeachAllTriggerInfo(Action<AnimationTriggerInfo> actions)
    {
        if (actions == null)
            return;
        if (animationMgrInfo == null)
            return;
        for (int i = 0; i < animationMgrInfo.animationTriggerInfos.Count; i++)
        {
            AnimationTriggerInfo animationTriggerInfo = animationMgrInfo.animationTriggerInfos[i];
            actions.Invoke(animationTriggerInfo);
        }
    }
    protected void TriggerAnimation(AnimationTriggerType type)
    {
        if (!animationTriggerTypeListDic.ContainsKey(type))
            return;

        List<AnimationTriggerInfo> list = animationTriggerTypeListDic[type];
        for (int i = 0; i < list.Count; i++)
        {
            AnimationTriggerInfo animationTriggerInfo = list[i];
            MonoMgr.GetInstance().DelayAction(animationTriggerInfo.delayTime, () =>
            {
                TriggerAnimationParameterInfos(animationTriggerInfo);
            });
        }
    }
    private void TriggerAnimationParameterInfos(AnimationTriggerInfo animationTriggerInfo)
    {
        List<AnimationParameterInfo> animationParameterInfos = animationTriggerInfo.animationParameterInfos;
        for (int i = 0; i < animationParameterInfos.Count; i++)
        {
            AnimationParameterInfo parameterInfo = animationParameterInfos[i];
            MyAnimatorController animController = warrior.GetAnimatorController();
            if (parameterInfo.animationParameterType == AnimationParameterType.Trigger)
                animController.SetTrigger(parameterInfo.parametersName);
            else if (parameterInfo.animationParameterType == AnimationParameterType.Bool)
                animController.SetBool(parameterInfo.parametersName, parameterInfo.parameterBool);
            else if (parameterInfo.animationParameterType == AnimationParameterType.Float)
                animController.SetFloat(parameterInfo.parametersName, parameterInfo.parameterFloat);
        }
    }
    public string GetSkillInfoText()
    {
        if (skillInfo.skillInfoText != "")
            return skillInfo.skillInfoText;
        string content = GetChargeTextInfo();
        if (content != null && content != "")
            content += "\n";
        content += GetAutoSkillInfo();
        return content;
    }
    public string GetChargeTextInfo()
    {
        string content = "";
        string lastElement = "";
        foreach (var item in costDic)
        {
            if (lastElement != "")
                content += "\n";
            lastElement = "消耗" + item.Key + ":" + Mathf.Abs(item.Value);
            content += lastElement;
        }
        return content;
    }
    private string GetAutoSkillInfo()
    {
        string content = "";
        if (skillInfo.talentBuffInfo.Count > 0)
        {
            content += "被动：";
            for (int i = 0; i < skillInfo.talentBuffInfo.Count; i++)
            {
                content += skillInfo.talentBuffInfo[i].GetBuffInfoText();
            }
            content += "\n";
        }

        int count = 0;
        foreach (var item in skillEffectMgrs)
        {
            var skillEffectMgr = item.Value;
            string currentContent = Effect.GetEffectDicInfoText(skillEffectMgr.GetTargetEffectDic(), "");
            if (skillEffectMgrs.Count - count > 1)
            {
                if (currentContent == "")
                    count++;
                else
                {
                    int num = (int)item.Key;
                    content += "第" + (num + 1 - count) + "段：";
                }
            }
            content += currentContent;
            if (content != "")
                content += "\n";
        }
        return content;
    }

    public void EffectTrigger(TriggerType type, Warrior target)
    {
        if (!skillEffectMgrs.ContainsKey(currentEventTrigger))
            return;
        ApplyInfo applyInfoToTarget = new ApplyInfo(warrior, target);
        TriggerEffect(type, applyInfoToTarget, skillEffectMgrs[currentEventTrigger].GetTargetEffectDic());
    }
    private void TriggerEffect(TriggerType type, ApplyInfo applyInfo, Dictionary<TriggerType, List<Effect>> effectDic)
    {
        if (applyInfo == null || effectDic == null)
            return;
        if (effectDic.ContainsKey(type))
        {
            List<Effect> effects = effectDic[type];
            if (effects == null)
                return;
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].EffectTrigger(applyInfo);
            }
        }
    }

    protected List<PropertyType> CheckCost(Dictionary<PropertyType, float> costDic)
    {
        List<PropertyType> lackPropertys = new List<PropertyType>();
        foreach (var item in costDic)
        {
            PropertyType lackType = warrior.CheckPropertyEnough(item.Key, item.Value);
            if (lackType != PropertyType.无)
                lackPropertys.Add(lackType);
        }
        return lackPropertys;
    }
    protected void UseCost(Warrior warrior)
    {
        foreach (var item in costDic)
        {
            warrior.ChangerProperty(item.Key, item.Value);
        }
    }

    //比如这个技能时按钮触发。这是按钮按下触发
    public virtual void OnTriggerStart(Warrior warrior)
    {
        TriggerAnimation(AnimationTriggerType.TriggerStart);
    }
    //比如这个技能时按钮触发。这是按钮按下后为放开持续触发
    public virtual void OnTrigger(Warrior warrior)
    {
        TriggerAnimation(AnimationTriggerType.Trigger);
    }
    //比如这个技能时按钮触发。这是按钮按下后抬起触发
    public virtual void OnTriggerEnd(Warrior warrior, Action afterTrigger)
    {
        if (this.warrior == null)
        {
            Debug.Log("battleInfo为空");
            return;
        }
        string currentClip = this.warrior.GetCurrentAnimation();
        float animSpeed = this.warrior.GetCurrentAnimationSpeed();

        if (currentClip != "Attack" || animSpeed <= 0)//这里只是做简单的判断，如果是其他复杂的游戏，要更加细节地判断现在的状态是否适合释放技能,还有被控制的状态
        {
            UIManager.GetInstance().ShowTips("当前状态不可释放技能");
            return;
        }
        if (IsSuccessCharge() == false)
            return;

        this.TriggerSkillController(warrior, afterTrigger);
        //this.warrior.GetRandomTargetByInfo(this.lockTargetInfoMgr, LockTargetType.Team)
        //TriggerSkill(, controller, afterTrigger);
    }
    private void TriggerSkillController(Warrior warrior, Action afterTrigger)
    {
        MyAnimatorController controller = this.warrior.GetAnimatorController();
        //把warrior或者battleinfo和Trigger传给UI，让玩家选择目标后调用方法
        if (this.warrior.IsCanSelect(this.lockTargetInfoMgr))
        {

        }
        else
        {
            //List<Warrior> targetWarriors = this.warrior.GetAllRandomTargetByInfo(this.lockTargetInfoMgr);
            TriggerSkill(null, controller, afterTrigger);
        }
    }
    public virtual void TriggerSkill(List<Warrior> fixedTargets, MyAnimatorController controller, Action afterTrigger)
    {
        RemoveNeedClipEvent();//移除当前技能需要的所有动画的事件，使用当前动画的不一定只有该技能，所以来的时候还要清除一次

        bool isStartSkill = false;
        MonoMgr.GetInstance().WaitUntil(() =>
        {
            if (isStartSkill == false && this.needAnimationClips.ContainsKey(this.warrior.GetCurrentAnimation()))
                isStartSkill = true;

            //如果动画已经执行到当前技能不需要的动画阶段了，就可以移除该技能需要的动画事件，使用当前动画的不一定只有该技能，所以走的时候还要清除一次
            if (isStartSkill == true && !this.needAnimationClips.ContainsKey(this.warrior.GetCurrentAnimation()))
                return true;

            return false;
        }, RemoveNeedClipEvent);

        SkillCurrentCD = SkillCD;
        Dictionary<string, int> clipEventDic = new Dictionary<string, int>();
        foreach (var item in this.needAnimationClips)//对该技能需要的所有动画全部添加一个初始事件
        {
            string clipName = item.Key;
            if (!clipEventDic.ContainsKey(clipName))
                clipEventDic[clipName] = 0;
            EventTriggerEnum eventTrigger = (EventTriggerEnum)clipEventDic[clipName];

            Action action = null;
            action = () =>
            {
                if (needAnimationClips[clipName].ContainsKey(eventTrigger))
                    needAnimationClips[clipName][eventTrigger] += 1;
                else
                    needAnimationClips[clipName] = new Dictionary<EventTriggerEnum, int>() { { eventTrigger, 0 } };

                if (animationTriggerTypeListDic.ContainsKey(AnimationTriggerType.AfterClip))
                {
                    List<AnimationTriggerInfo> animationTriggerInfos = animationTriggerTypeListDic[AnimationTriggerType.AfterClip];
                    for (int i = 0; i < animationTriggerInfos.Count; i++)
                    {
                        if (animationTriggerInfos[i].clipName != clipName)
                            continue;
                        if (needAnimationClips[clipName][eventTrigger] >= 1)
                            continue;

                        int index = i;
                        MonoMgr.GetInstance().DelayAction(animationTriggerInfos[i].delayTime, () =>
                        {
                            TriggerAnimationParameterInfos(animationTriggerInfos[index]);
                        });
                        //controller.RemoveAnimationEvent(clipName, eventTrigger, action);//魔法技能吟唱会让同一个动画播放多次，所以删除触发过一次的事件
                    }
                }
            };
            controller.AddAnimationEvent(clipName, eventTrigger, 0, action);
            clipEventDic[clipName]++;
        }
        AddNeedClipEvent(fixedTargets, controller, clipEventDic);

        TriggerAnimation(AnimationTriggerType.TriggerEnd);
        if (afterTrigger != null)
            afterTrigger.Invoke();
    }

    private void AddNeedClipEvent(List<Warrior> fixedTargets, MyAnimatorController controller, Dictionary<string, int> clipEventDic)
    {
        if (controller == null || clipEventDic == null)
            return;

        if (_skillType == SkillType.技能)
            this.warrior.EffectTrigger(TriggerType.释放技能时触发, this.warrior);//释放技能时，没有锁定目标，只能是自己

        ForeachAllEventInfo((AnimationEventInfo animationEventInfo) =>
        {
            if (animationEventInfo.haveEvent == false)//没有事件，就不添加
                return;
            string clipName = animationEventInfo.clipName;
            if (!clipEventDic.ContainsKey(clipName))
                clipEventDic[clipName] = 0;
            EventTriggerEnum eventTrigger = (EventTriggerEnum)clipEventDic[clipName];
            controller.AddAnimationEvent(animationEventInfo.clipName, eventTrigger, animationEventInfo.skilltriggerTime, () =>
            {
                if (this.warrior == null)
                {
                    Debug.Log("battleInfo为null");
                    return;
                }
 
                this.currentEventTrigger = animationEventInfo.eventTriggerEnum;
                for (int i = 0; i < animationEventInfo.triggerTypes.Count; i++)
                {
                    TriggerType triggerType = animationEventInfo.triggerTypes[i];
                    if (triggerType != TriggerType.释放技能对所有目标触发)
                    {
                        this.EffectTrigger(triggerType, this.warrior);//其他类型再这里触发
                    }
                    else
                    {
                        List<Warrior> allTargetWarrior = new List<Warrior>();
                        List<Warrior> randomTargets = this.warrior.GetAllRandomTargetByInfo(this.lockTargetInfoMgr);

                        if (fixedTargets != null)
                            allTargetWarrior = allTargetWarrior.Concat(fixedTargets).ToList<Warrior>();
                        if (randomTargets != null)
                            allTargetWarrior = allTargetWarrior.Concat(randomTargets).ToList<Warrior>();

                        this.warrior.StartWarriorBehaviour();
                        if (allTargetWarrior.Count <= 0)
                            allTargetWarrior.Add(this.warrior.GetRandomEnemyTarget());//只针对这个游戏，我希望敌人死后，技能释放时，仍然可以释放特效
                        for (int j = 0; j < allTargetWarrior.Count; j++)
                        {
                            Warrior targetWarrior = allTargetWarrior[j];
                            this.EffectTrigger(TriggerType.释放技能对所有目标触发, targetWarrior);
                            this.warrior.EffectTrigger(TriggerType.释放技能对所有目标触发, targetWarrior);

                            //此方法可以根据不同游戏类型决定触发时机
                            if (_skillType == SkillType.技能)
                            {
                                this.EffectTrigger(TriggerType.技能命中敌人触发, targetWarrior);//内部进行命中判断,如果是其他游戏类型，比如指向性技能，那么技能的特效打中后，调用即可
                                this.warrior.EffectTrigger(TriggerType.技能命中敌人触发, targetWarrior);
                            }
                            else if (_skillType == SkillType.普攻)
                            {
                                this.EffectTrigger(TriggerType.普攻命中敌人触发, targetWarrior);//内部进行命中判断,如果是其他游戏类型，比如指向性技能，那么技能的特效打中后，调用即可
                                this.warrior.EffectTrigger(TriggerType.普攻命中敌人触发, targetWarrior);
                            }
                        }
                    }

                    //TriggerType triggerType = animationEventInfo.triggerTypes[i];
                    //if (triggerType == TriggerType.释放技能对所有目标触发)//释放技能是特殊的，需要warrior做一些初始化处理，再调用EffectTrigger
                    //{
                    //    this.warrior.StartWarriorBehaviour();
                    //    Warrior targetWarrior = this.warrior.GetRandomEnemyTarget();
                    //    this.EffectTrigger(TriggerType.释放技能对所有目标触发, targetWarrior);
                    //    //此方法可以根据不同游戏类型决定触发时机
                    //    if(_skillType == SkillType.技能)
                    //    {
                    //        this.EffectTrigger(TriggerType.技能命中敌人触发, targetWarrior);//内部进行命中判断,如果是其他游戏类型，比如指向性技能，那么技能的特效打中后，调用即可
                    //    }
                    //    else if(_skillType == SkillType.普攻)
                    //    {
                    //        this.EffectTrigger(TriggerType.普攻命中敌人触发, targetWarrior);//内部进行命中判断,如果是其他游戏类型，比如指向性技能，那么技能的特效打中后，调用即可
                    //    }
                    //}
                    //else
                    //    this.EffectTrigger(triggerType, this.warrior.GetRandomEnemyTarget());//其他类型再这里触发
                }
            });
            clipEventDic[clipName]++;
        });
    }

    private void RemoveNeedClipEvent()
    {
        foreach (var item in needAnimationClips)
        {
            warrior.GetAnimatorController().RemoveClipAnimationEvent(item.Key, new HashSet<EventTriggerEnum>() { EventTriggerEnum.EventTriggertStart, EventTriggerEnum.EventTriggerEnd });
            item.Value.Clear();
        }
    }
    private bool IsSuccessCharge()
    {
        List<PropertyType> lackTypes = CheckCost(costDic);
        if (lackTypes.Count <= 0)
        {
            this.UseCost(this.warrior);
        }
        else
        {
            if (this.warrior == PlayerMgr.Instance.GetWarrior())
            {
                for (int i = 0; i < lackTypes.Count; i++)
                    UIManager.GetInstance().ShowTips(lackTypes[i].ToString() + "不足");
            }
            return false;
        }
        return true;
    }

    public Sprite GetSprite_Drag()
    {
        return SkillSprite;
    }
    public bool IsRunout()
    {
        return false;
    }
    public float GetAllCD()
    {
        return SkillCD;
    }
    public float GetCurrentCD()
    {
        return SkillCurrentCD;
    }
    public void AddCurrentCD(float num)
    {
        SkillCurrentCD += num;
    }
    public IHolderAble GetHolder()
    {
        return null;
    }
    public Warrior GetWarrior()
    {
        return warrior;
    }
    public string GetName()
    {
        return SkillName;
    }

    public ShortcutType GetShortcutType()
    {
        return ShortcutType.技能;
    }

    public int GetCount()
    {
        return 1;
    }

    public string GetSaveData()
    {
        return SkillName;
    }

}




