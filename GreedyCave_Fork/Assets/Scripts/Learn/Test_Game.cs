using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

//public enum OldTestEnum
//{
//    暴击率 = 5,
//    攻击力 = 10,
//    魔力 = 11,
//    魔法值 = 18,
//    生命值 = 20,
//}
//public enum NewTestEnum
//{
//    攻击力 = 0,
//    魔力 = 1,
//    魔法值 = 2,
//    生命值 = 3,
//    暴击率 = 4,
//}

//[AttributeUsage(AttributeTargets.Enum, Inherited = true, AllowMultiple = true)]
//public class EnumEditor : Attribute
//{
//    public Type enumType;
//    public string oldName;
//    public EnumEditor(Type oldEnumType, string oldName)
//    {
        
//        this.enumType = oldEnumType;
//        this.oldName = oldName;
        
//    }
//}

public class Test_Game : MonoBehaviour
{
    //[EnumEditor(typeof(OldTestEnum), "testEnums")]
    //public OldTestEnum testEnum;

    private void Start()
    {
        //transform.Rotate(Vector3.forward, -50);
        //Debug.Log(transform.localEulerAngles);
    }
}
