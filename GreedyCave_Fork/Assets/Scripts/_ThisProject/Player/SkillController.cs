using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillDamageType
{
    物理,
    魔法,
    辅助,
}
public class SkillInfoMgr : ICollectAble<SkillInfoMgr>, IGetAble
{
    public int learnIndex;
    public Skill skill;
    private SkillDamageType skillDamageType;
    public void SetSkillDamageType(SkillDamageType skillDamageType)
    {
        this.skillDamageType = skillDamageType;
    }
    public int Compare(SkillInfoMgr item)
    {
        return 0;
    }
    public SkillInfoMgr DecreaseRules(int count)
    {
        return null;
    }
    public HashSet<string> GetCategory()
    {
        if (skill == null)
            return new HashSet<string>() { skillDamageType.ToString() };
        return new HashSet<string>() { skill.skillDamageType.ToString() };
    }
    public int GetCount()
    {
        return 1;
    }
    public string GetName()
    {
        if (skill == null)
            return skillDamageType.ToString() + ":" +"空" + "("+learnIndex+")";
        return skill.SkillName;
    }
    public bool IsCanCombine()
    {
        return false;
    }
    public bool IsSame(SkillInfoMgr item)
    {
        return false;
    }
    public void ReverseSpltRules(SkillInfoMgr item)
    {

    }
    public List<SkillInfoMgr> SplitRules(int countUpperLimit)
    {
        return null;
    }
    public SkillInfoMgr StackRules(SkillInfoMgr item, int countUpperLimit)
    {
        return null;
    }
    public void UseGrid()
    {

    }
}
[System.Serializable]
public class SkillSaveData
{
    public int baseSkillPoint = 0;//总共的技能点数
    public int extarSkillPoint = 0;//其他行为获得的技能点数
    public int useSkillPoint = 0;//使用掉的技能点

    public HashSet<string> learnedSkill;
    public Dictionary<SkillDamageType, int> skillTypeLearnIndex;
}

public class SkillController
{
    public SkillController(Warrior warrior)
    {
        Initial(warrior);
        EventCenter.GetInstance().AddEventListener(EventCenterEnum.升级.ToString(), (int level) =>
        {
            SetBaseSkillPoint(level / 3);
        });
    }
    public void ClearRunTimeData(Warrior warrior)
    {
        baseSkillPoint = 0;
        extarSkillPoint = 0;
        useSkillPoint = 0;
        skillBag = new Bag<SkillInfoMgr>();
        learnedSkill = new HashSet<string>();
        skillTypeLearnIndex = new Dictionary<SkillDamageType, int>();

        Initial(warrior);
    }
    private int baseSkillPoint = 0;//总共的技能点数
    private int extarSkillPoint = 0;//其他行为获得的技能点数
    private int useSkillPoint = 0;//使用掉的技能点
    public int CurrentSkillPoint//剩余的技能点数
    {
        get { return baseSkillPoint + extarSkillPoint - useSkillPoint; }
    }

    private Bag<SkillInfoMgr> skillBag = new Bag<SkillInfoMgr>();
    private HashSet<string> learnedSkill = new HashSet<string>();
    private Dictionary<SkillDamageType, int> skillTypeLearnIndex = new Dictionary<SkillDamageType, int>();

    private static string physicsSkillPath = "SkillInfo/Physics";
    private static string magicSkillPath = "SkillInfo/Magic";
    private static string aidedSkillPath = "SkillInfo/Aided";
    
    private static Material greyMaterial;
    public static Material GreyMaterial
    {
        get 
        {
            if (greyMaterial == null)
                greyMaterial = ResMgr.GetInstance().Load<Material>("Material/Grey");
            return greyMaterial; 
        }
    }

    public SkillSaveData GetSkillSaveData()
    {
        SkillSaveData saveData = new SkillSaveData();
        saveData.baseSkillPoint = this.baseSkillPoint;
        saveData.extarSkillPoint = this.extarSkillPoint;
        saveData.useSkillPoint = this.useSkillPoint;
        saveData.learnedSkill = this.learnedSkill;
        saveData.skillTypeLearnIndex = this.skillTypeLearnIndex;
        return saveData;
    }
    public void LoadSkillSaveData(PlayerData playerData)
    {
        SkillSaveData skillSaveData = playerData.skillSaveData;
        if (skillSaveData == null)
            return;
        this.baseSkillPoint = skillSaveData.baseSkillPoint;
        this.extarSkillPoint = skillSaveData.extarSkillPoint;
        this.useSkillPoint = skillSaveData.useSkillPoint;
        this.learnedSkill = skillSaveData.learnedSkill;
        this.skillTypeLearnIndex = skillSaveData.skillTypeLearnIndex;

        foreach (var item in learnedSkill)
        {
            SkillInfoMgr skillInfoMgr = GetSkill(item);
            if (skillInfoMgr != null || skillInfoMgr.skill != null)
                skillInfoMgr.skill.TriggerSkillBuff();
        }
    }
    private static AnimationMgrInfo physicsAnimMgrInfo;
    public static AnimationMgrInfo PhysicsAnimMgrInfo
    {
        get
        {
            if (physicsAnimMgrInfo == null)
                physicsAnimMgrInfo = ResMgr.GetInstance().Load<AnimationMgrInfo>("Player/Skill_Physics_1");
            return physicsAnimMgrInfo;
        }
    }
    private static AnimationMgrInfo magicAnimMgrInfo;
    public static AnimationMgrInfo MagicAnimMgrInfo
    {
        get
        {
            if (magicAnimMgrInfo == null)
                magicAnimMgrInfo = ResMgr.GetInstance().Load<AnimationMgrInfo>("Player/Skill_Magic_2");
            return magicAnimMgrInfo;
        }
    }
    public void ForgetSkill()
    {
        useSkillPoint = 0;
        learnedSkill.Clear();
        skillTypeLearnIndex.Clear();
        PlayerMgr.Instance.GetShortcutController().RemoveShorcut_Type(ShortcutType.技能);
        PlayerMgr.Instance.GetWarrior().RemoveBuff_Category(BuffCategory.技能.ToString());
    }
    public void LearnSkill(SkillInfoMgr skillInfoMgr)
    {
        if (UseSkillPoint(skillInfoMgr.skill.LearnPoint) == false)
            return;
        if(!skillBag.IsContainItem(skillInfoMgr))
        {
            Debug.Log("没有该技能");
            return;
        }
        if (!learnedSkill.Contains(skillInfoMgr.skill.SkillName))
            learnedSkill.Add(skillInfoMgr.skill.SkillName);
        if (!skillTypeLearnIndex.ContainsKey(skillInfoMgr.skill.skillDamageType))
            skillTypeLearnIndex[skillInfoMgr.skill.skillDamageType] = 0;

        skillTypeLearnIndex[skillInfoMgr.skill.skillDamageType]++;
        skillInfoMgr.skill.TriggerSkillBuff();
        EventCenter.GetInstance().EventTrigger<SkillInfoMgr>("学习技能", (skillInfoMgr));
    }
    public int GetLearnIndex(SkillDamageType skillType)
    {
        if (!skillTypeLearnIndex.ContainsKey(skillType))
            skillTypeLearnIndex[skillType] = 0;
        return skillTypeLearnIndex[skillType];
    }
    public SkillInfoMgr GetLearnedSkill(string skillName)
    {
        if (learnedSkill.Contains(skillName))
            return skillBag.GetOneNameBagItem(skillName);
        return null;
    }
    public SkillInfoMgr GetSkill(string skillName)
    {
        return skillBag.GetOneNameBagItem(skillName);
    }
    public bool IsLearn(SkillInfoMgr skillInfoMgr)
    {
        return learnedSkill.Contains(skillInfoMgr.skill.SkillName);
    }
    private void SetBaseSkillPoint(int count)
    {
        this.baseSkillPoint = count;
    }
    public bool IsSatisfied(int count)
    {
        return CurrentSkillPoint >= count;
    }
    //返回是否使用成功
    public bool UseSkillPoint(int count = 1)
    {
        if(IsSatisfied(count) == false)
        {
            UIManager.GetInstance().ShowTips("贷款和零元购是不行的哟");
            return false;
        }
        useSkillPoint += count;
        return true;
    }
    private void Initial(Warrior warrior)
    {
        List<Skill> physicsSkills = GetSkillGroups(SkillDamageType.物理, warrior, ResMgr.GetInstance().LoadAll<SkillInfoAndAnimInfo>(physicsSkillPath));
        List<Skill> magicSkills = GetSkillGroups(SkillDamageType.魔法, warrior, ResMgr.GetInstance().LoadAll<SkillInfoAndAnimInfo>(magicSkillPath));
        List<Skill> aidedSkills = GetSkillGroups(SkillDamageType.辅助, warrior, ResMgr.GetInstance().LoadAll<SkillInfoAndAnimInfo>(aidedSkillPath));

        InitialSkills(physicsSkills, SkillDamageType.物理);
        InitialSkills(magicSkills, SkillDamageType.魔法);
        InitialSkills(aidedSkills, SkillDamageType.辅助);
    }
    private void InitialSkills(List<Skill> skills, SkillDamageType skillDamageType)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            SkillInfoMgr skillInfoMgr = new SkillInfoMgr();
            skillInfoMgr.learnIndex = i / 2;
            skillInfoMgr.skill = skills[i];
            skillInfoMgr.SetSkillDamageType(skillDamageType);
            skillBag.AddItem(skillInfoMgr);
        }
    }
    private List<Skill> GetSkillGroups (SkillDamageType skillType, Warrior warrior, SkillInfoAndAnimInfo[] skillAnimAnimInfos)
    {
        List<Skill> skills = new List<Skill>();
        for (int i = 0; i < skillAnimAnimInfos.Length; i++)
        {
            Skill skill = null;
            SkillInfo skillInfo = skillAnimAnimInfos[i].skillInfo;
            if (skillInfo != null && skillInfo.skillExist == SkillExistType.Exist)
            {
                AnimationMgrInfo animInfo = skillAnimAnimInfos[i].animationMgrInfo;

                skill = new Skill(warrior, animInfo, skillInfo);
                skill.SetSkillDamageType(skillType);
            }
            skills.Add(skill);
        }
        return skills;
    }
    private AnimationMgrInfo GetDefaultAnimationInfo(SkillDamageType skillType)
    {
        if (skillType == SkillDamageType.物理)
            return PhysicsAnimMgrInfo;
        if(skillType == SkillDamageType.魔法)
            return MagicAnimMgrInfo;
        return null;
    }
    public List<SkillInfoMgr> GetSkills(SkillDamageType skillType)
    {
        return skillBag.GetCategoryBagItems(skillType.ToString());
    }
}
