using UnityEngine;


[System.Serializable]
[CreateAssetMenu(menuName = "MyDictionary/Skill/Skill_Anim_Info", fileName = "Skill_Anim_Info")]
public class SkillInfoAndAnimInfo:ScriptableObject
{
    public AnimationMgrInfo animationMgrInfo;
    public SkillInfo skillInfo;
}
