using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class LockTargetInfoProperty
{
    //public LockTargetType lockTargetType;
    //public bool isMustContainUser;//是否必须包含自己，此项不与fixedTargetCount和randomTargetCount关联
    //public bool isCanLockUser;//是否可以锁定自己
    //public bool isAll;
    //public bool isfixedTargetRepeat;
    //public int fixedTargetCount;//可以手动选择的数量
    //public bool isRandomTargetRepeat;
    //public int randomTargetCount;//随机选择的数量
    public SerializedProperty lockTargetType;
    public SerializedProperty isMustContainUser;
    public SerializedProperty isCanLockUser;
    public SerializedProperty isAll;
    public SerializedProperty isfixedTargetRepeat;
    public SerializedProperty fixedTargetCount;
    public SerializedProperty isRandomTargetRepeat;
    public SerializedProperty randomTargetCount;
}
[CustomPropertyDrawer(typeof(LockTargetInfo))]
public class LockTargetInfoEditor : SelfAdaptionPropertyDrawer
{
    private static LockTargetInfoProperty GetLockTargetInfoProperty(SerializedProperty property)
    {
        LockTargetInfoProperty lockTargetInfoProperty = new LockTargetInfoProperty();
        lockTargetInfoProperty.lockTargetType = property.FindPropertyRelative("lockTargetType");
        lockTargetInfoProperty.isMustContainUser = property.FindPropertyRelative("isMustContainUser");
        lockTargetInfoProperty.isCanLockUser = property.FindPropertyRelative("isCanLockUser");
        lockTargetInfoProperty.isAll = property.FindPropertyRelative("isAll");
        lockTargetInfoProperty.isfixedTargetRepeat = property.FindPropertyRelative("isfixedTargetRepeat");
        lockTargetInfoProperty.fixedTargetCount = property.FindPropertyRelative("fixedTargetCount");
        lockTargetInfoProperty.isRandomTargetRepeat = property.FindPropertyRelative("isRandomTargetRepeat");
        lockTargetInfoProperty.randomTargetCount = property.FindPropertyRelative("randomTargetCount");

        return lockTargetInfoProperty;
    }
    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        LockTargetInfoProperty lockTargetInfoProperty = GetLockTargetInfoProperty(property);
        ShowPropertyVertical(lockTargetInfoProperty.lockTargetType);
        if(lockTargetInfoProperty.lockTargetType.enumValueIndex == (int)(LockTargetType.Enemy))
        {
            AddHideProperty(lockTargetInfoProperty.isMustContainUser);
            AddHideProperty(lockTargetInfoProperty.isCanLockUser);
        }
        ShowPropertyHorizontal(lockTargetInfoProperty.isMustContainUser,30);
        if (lockTargetInfoProperty.isMustContainUser.boolValue == true)
            AddHideProperty(lockTargetInfoProperty.isCanLockUser);
        ShowPropertyHorizontal(lockTargetInfoProperty.isCanLockUser,30);
        ShowPropertyVertical(lockTargetInfoProperty.isAll);
        if(lockTargetInfoProperty.isAll.boolValue == true)
        {
            AddHideProperty(lockTargetInfoProperty.isfixedTargetRepeat);
            AddHideProperty(lockTargetInfoProperty.fixedTargetCount);
            AddHideProperty(lockTargetInfoProperty.isRandomTargetRepeat);
            AddHideProperty(lockTargetInfoProperty.randomTargetCount);
        }
        ShowPropertyVertical(lockTargetInfoProperty.isfixedTargetRepeat);
        ShowPropertyVertical(lockTargetInfoProperty.fixedTargetCount);
        ShowPropertyVertical(lockTargetInfoProperty.isRandomTargetRepeat);
        ShowPropertyVertical(lockTargetInfoProperty.randomTargetCount);

    }
}
