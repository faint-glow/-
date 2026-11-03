using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//需要在PropertyDrawer自定义绘制的时候自适应高度的话，就继承这个
//注意：继承这个，以后，属性都不能使用特性[Header("")]
public class SelfAdaptationClassMgr
{
    public bool isInitial;
    public bool isSpread;//是否展开
    public float allControlsHeight;//其他控件
    public List<SerializedPropertyHeightInfo> serializedPropertyHeightInfos;//属性
}
[System.Serializable]
public class SerializedPropertyHeightInfo
{
    public string serializedPropertyName;
    public float y;
}