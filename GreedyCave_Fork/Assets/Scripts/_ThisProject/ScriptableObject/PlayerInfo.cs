using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MyDictionary/Warrior/PlayerInfo", fileName = "PlayerInfo")]
public class PlayerInfo:ScriptableObject
{
    //public Parameters parameters;
    [Header("普通攻击伤害在动画中的触发时间")]
    public float normalAttackTriggerTime;
    [Header("BaseValue是1级时的数据，GropValue是每级增加数据")]//[UnityEngine.Serialization.FormerlySerializedAs("propertyFactors")]修改变量名
    public PropertyValue[] propertyFactors;
}