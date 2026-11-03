using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuffProperty
{
    public SerializedProperty buffName;//buff的名字
    public SerializedProperty liveTime;//存活时间
    public SerializedProperty existType;//是否没有倒计时
    public SerializedProperty buffCategoryList;
    public SerializedProperty overlayType;//可叠加层数
    public SerializedProperty effects;
    //public SerializedProperty fxInfoList;
    public SerializedProperty fxMgr;

}

[CanEditMultipleObjects]
[CustomEditor(typeof(BuffInfo))]
public class BuffInfoEditor : Editor
{
    BuffProperty buffProperty;
    private void OnEnable()
    {
        buffProperty = GetBuffProperty(serializedObject);
    }

    public override void OnInspectorGUI()
    {
        ShowBuff(buffProperty, serializedObject);
    }
    
    public static BuffProperty GetBuffProperty(SerializedObject serializedObject)
    {
        BuffProperty buffProperty = new BuffProperty();
        buffProperty.buffName = serializedObject.FindProperty("buffName");
        buffProperty.liveTime = serializedObject.FindProperty("liveTime");
        buffProperty.existType = serializedObject.FindProperty("existType");
        buffProperty.buffCategoryList = serializedObject.FindProperty("buffCategoryList");

        buffProperty.overlayType = serializedObject.FindProperty("overlayType");
        buffProperty.effects = serializedObject.FindProperty("effects");

        //buffProperty.fxInfoList = serializedObject.FindProperty("fxInfoList");
        buffProperty.fxMgr = serializedObject.FindProperty("fxMgr");

        return buffProperty;
    }

    public static void ShowBuff(BuffProperty buffProperty, SerializedObject serializedObject)
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(buffProperty.buffName);
        EditorGUILayout.PropertyField(buffProperty.liveTime);
        EditorGUILayout.PropertyField(buffProperty.existType);
        EditorGUILayout.PropertyField(buffProperty.buffCategoryList);

        EditorGUILayout.PropertyField(buffProperty.overlayType);
        EditorGUILayout.PropertyField(buffProperty.effects);

        //GUILayout.Space(30);
        //EditorGUILayout.BeginHorizontal();
        //GUILayout.FlexibleSpace();
        //CreateButtonMgr.CreateEffectsInList(buffProperty.effectInfos, "Buff_Effect");
        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.PropertyField(buffProperty.fxInfoList);
        EditorGUILayout.PropertyField(buffProperty.fxMgr);

        serializedObject.ApplyModifiedProperties();
    }


}
