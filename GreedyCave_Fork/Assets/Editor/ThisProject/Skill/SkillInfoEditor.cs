using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SkillProperty
{
    public SerializedProperty skillExist;
    public SerializedProperty skillTriggerType;
    public SerializedProperty skillType;

    public SerializedProperty skillCD;
    public SerializedProperty skillName;
    public SerializedProperty skillSprite;
    public SerializedProperty skillInfoText;
    public SerializedProperty lockTargetInfoMgr;

    public SerializedProperty costValue;
    public SerializedProperty talentBuffInfo;
    public SerializedProperty skillEffectMgrs;
}

[CanEditMultipleObjects]
[CustomEditor(typeof(SkillInfo))]
public class SkillInfoEditor : Editor
{
     SkillProperty skillProperty = new SkillProperty();

    private void OnEnable()
    {
        InitialSkillProperty(skillProperty, serializedObject);
    }
    public override void OnInspectorGUI()
    {
        ShowSkill(skillProperty, serializedObject);
    }
    public static void InitialSkillProperty(SkillProperty skillProperty, SerializedObject serializedObject)
    {
        skillProperty.skillExist = serializedObject.FindProperty("skillExist");
        skillProperty.skillTriggerType = serializedObject.FindProperty("skillTriggerType");
        skillProperty.skillType = serializedObject.FindProperty("skillType");

        skillProperty.skillName = serializedObject.FindProperty("skillName");
        skillProperty.skillSprite = serializedObject.FindProperty("skillSprite");
        skillProperty.skillInfoText = serializedObject.FindProperty("skillInfoText");
        skillProperty.lockTargetInfoMgr = serializedObject.FindProperty("lockTargetInfoMgr");

        skillProperty.skillCD = serializedObject.FindProperty("skillCD");
        skillProperty.costValue = serializedObject.FindProperty("costValue");
        skillProperty.talentBuffInfo = serializedObject.FindProperty("talentBuffInfo");

        skillProperty.skillEffectMgrs = serializedObject.FindProperty("skillEffectMgrs");
    }
    public static void ShowSkill(SkillProperty skillProperty, SerializedObject serializedObject)
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(skillProperty.skillExist);
        if (skillProperty.skillExist.enumValueIndex == (int)SkillExistType.Exist)
        {
            EditorGUILayout.PropertyField(skillProperty.skillTriggerType);
            EditorGUILayout.PropertyField(skillProperty.skillType);

            EditorGUILayout.PropertyField(skillProperty.skillName);
            EditorGUILayout.PropertyField(skillProperty.skillCD);
            EditorGUILayout.PropertyField(skillProperty.skillSprite);
            EditorGUILayout.PropertyField(skillProperty.skillInfoText);
            EditorGUILayout.PropertyField(skillProperty.lockTargetInfoMgr);
            EditorGUILayout.PropertyField(skillProperty.costValue);

            EditorGUILayout.PropertyField(skillProperty.talentBuffInfo);
            EditorGUILayout.PropertyField(skillProperty.skillEffectMgrs, new GUIContent("技能效果"));
            if(skillProperty.skillEffectMgrs.isArray)
            {
                for (int i = 0; i < skillProperty.skillEffectMgrs.arraySize; i++)
                {
                    SerializedProperty triggerEnum = skillProperty.skillEffectMgrs.GetArrayElementAtIndex(i).FindPropertyRelative("triggerEnum");
                    triggerEnum.enumValueIndex = i;
                }
            }
        }

        GUILayout.Space(30);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        CreateButtonMgr.CreateBuff_List(skillProperty.talentBuffInfo,"NewBuffINfo","创建天赋buff");
        //CreateButtonMgr.CreateValue(serializedObject, skillProperty.valueInfo, "Skill_Value", true);
        //CreateButtonMgr.CreateEffectsInList(skillProperty.effectInfos, "Skill_Effect");
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    ///// <summary>
    ///// 创建一个Button按钮，点击创建一个BuffInfo
    ///// </summary>
    ///// <param name="serializedObject">创建这个BuffInfo的实例化对象</param>
    ///// <param name="buffInfos">实例化对象中的BuffInfo列表(数组)</param>
    //public static void CreateBuff(SerializedObject serializedObject, SerializedProperty buffInfos, string sourceName = "NewBuffInfo", bool isNextDirectory = true)
    //{
    //    if (GUILayout.Button("创建Buff", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
    //    {
    //        BuffInfo buffInfo = SkillInfoEditor.CreateSource<BuffInfo>(serializedObject, sourceName, isNextDirectory);
    //        buffInfos.arraySize += 1;
    //        buffInfos.GetArrayElementAtIndex(buffInfos.arraySize - 1).objectReferenceValue = buffInfo;
    //    }
    //}
    ///// <summary>
    ///// 创建一个Button按钮，点击创建一个EffectInfo
    ///// </summary>
    ///// <param name="serializedObject">创建这个EffectInfo的实例化对象</param>
    ///// <param name="effectInfos">实例化对象中的EffectInfo列表(数组)</param>
    //public static void CreateEffects(SerializedObject serializedObject, SerializedProperty effectInfos, string sourceName = "NewEffectInfo", bool isNextDirectory = true)
    //{
    //    if (GUILayout.Button("创建Effect", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
    //    {
    //        EffectInfo effectInfo = SkillInfoEditor.CreateSource<EffectInfo>(serializedObject, sourceName, isNextDirectory);
    //        effectInfos.arraySize += 1;
    //        effectInfos.GetArrayElementAtIndex(effectInfos.arraySize - 1).objectReferenceValue = effectInfo;
    //    }
    //}
    ///// <summary>
    ///// 创建一个Button按钮，点击创建一个ValueInfo
    ///// </summary>
    ///// <param name="serializedObject">创建这个Value的实例化对象</param>
    ///// <param name="valueInfoProperty">实例化对象中的ValueInfo字段</param>
    //public static void CreateValue(SerializedObject serializedObject, SerializedProperty valueInfoProperty, string sourceName = "NewValueInfo", bool isNextDirectory = false)
    //{
    //    if (GUILayout.Button("创建Value", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
    //    {
    //        ValueInfo valueInfo = SkillInfoEditor.CreateSource<ValueInfo>(serializedObject, sourceName, isNextDirectory);
    //        valueInfoProperty.objectReferenceValue = valueInfo;
    //    }
    //}

    //public static T CreateSource<T>(SerializedObject serializedObject, string sourceName = "NewSources", bool isNextDirectory = true) where T : ScriptableObject
    //{
    //    string path = AssetDatabase.GetAssetPath(serializedObject.targetObject);//GetPath(serializedObject.targetObject);
    //    string directoryPath = Path.GetDirectoryName(path);

    //    if (isNextDirectory == true)
    //    {
    //        directoryPath = directoryPath + @"\" + serializedObject.targetObject.name;
    //    }

    //    if (!Directory.Exists(directoryPath))
    //        Directory.CreateDirectory(directoryPath);

    //    int i = 1;
    //    path = directoryPath + @"\" + sourceName + "_" + i + ".asset";

    //    while (File.Exists(path))
    //    {
    //        i++;
    //        path = directoryPath + @"\" + sourceName + "_" + i + ".asset";
    //    }

    //    T obj = ScriptableObject.CreateInstance<T>();

    //    UnityEditor.AssetDatabase.CreateAsset(obj, path);
    //    UnityEditor.AssetDatabase.Refresh();
    //    return obj;
    //}

    ////传入当前serializedObject的所在文件夹路径，根据需要决定目标文件夹
    //private static string ChoiceDirectory<T>(SerializedObject serializedObject, string directoryPath) where T : ScriptableObject
    //{
    //    if (typeof(T) != typeof(ValueInfo))
    //    {
    //        directoryPath = directoryPath + @"\" + serializedObject.targetObject.name;
    //    }
    //    return directoryPath;
    //}
}
