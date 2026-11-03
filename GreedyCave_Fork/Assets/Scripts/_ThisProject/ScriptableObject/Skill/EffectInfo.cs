//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(menuName = "MyDictionary/EffectInfo", fileName = "NewEffectInfo")]
//public class EffectInfo : ScriptableObject
//{
//    [Header("效果中的ApplyInfo的user是施加效果的warrior，target是指拥有该效果的warrior")]
//    public EffectType effectType;
//    public float triggerRatio = 1;//触发概率

//    public float intervalTime = 1000;
//    public PropertyType targetProperty;

//    //public ValueInfo valueInfo;
//    public Value value;
//    public List<TriggerType> triggerTypes = new List<TriggerType>();//什么情况下会触发该效果

//    [Header("效果中的buff，施加条件不管设置成什么，都至于效果触发条件一致")]
//    public List<BuffInfo> buffInfos;

//    [HideInInspector]
//    public bool isValueFold;
//}
