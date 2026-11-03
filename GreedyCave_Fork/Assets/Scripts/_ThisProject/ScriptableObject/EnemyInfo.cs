using UnityEngine;

public interface IMazeObjInfo_WarriorInfoAble
{
    public PropertyValue[] GetPropertyValue();
}
[System.Serializable]
[CreateAssetMenu(menuName = "MyDictionary/Warrior/EnemyInfo", fileName = "EnemyInfo")]
public class EnemyInfo:ScriptableObject, IMazeObjInfo_WarriorInfoAble
{
    //这里的obj是加载出来的未实例化的物体，使用前需要实例化
    public GameObject obj;
    public Sprite avataImage;
    public string enemyName = "暂未命名";
    //public Parameters parameters;
    [Header("普通攻击伤害在动画中的触发时间")]
    public float normalAttackTriggerTime;
    [Header("为空时，利用上面的攻击时间使用默认攻击")]
    public SkillInfoAndAnimInfo NormalAttackInfo;
    [Header("该敌人每次释放完技能以后会获取以下范围的随机值来决定下次技能什么时候释放")]
    public int minSkillAttackCount = 2;
    public int maxSkillAttackCount = 7;
    public SkillInfoAndAnimInfo[] skillInfoAndAnimInfo;
    [Header("上面是基础是，下面是每级成长系数")]//[UnityEngine.Serialization.FormerlySerializedAs("propertyFactors")]修改变量名
    public PropertyValue[] propertyFactors;
    public Booty booty;

    public PropertyValue[] GetPropertyValue()
    {
        return propertyFactors;
    }
}
