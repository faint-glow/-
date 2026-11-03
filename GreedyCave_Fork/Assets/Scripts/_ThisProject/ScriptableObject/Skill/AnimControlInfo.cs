using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimControlInfoType
{
    Change_Name,//直接通过动画名字瞬间切换
    Change_Parameter,//通过控制动画控制器的参数进行条件切换
}
[CreateAssetMenu(menuName = "MyDictionary/Skill/AnimControlInfo", fileName = "NewAnimControlInfo")]
public class AnimControlInfo: ControlInfo
{
    public string stateName;
    public float speed = 1;
    //public AnimationParameterInfo animationParameterInfo;
    public override Control GetControl()
    {
        return new AnimControl(this);
    }
}
