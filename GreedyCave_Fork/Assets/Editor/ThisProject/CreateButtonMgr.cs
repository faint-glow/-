using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateButtonMgr : MonoBehaviour
{
    public static void CreateItem_List<T>(SerializedProperty valueInfoProperty, string sourceName = "NewItemInfo", bool isNextDirectory = true) where T : ItemInfo
    {
        T itemInfo = CreateButtonMgr.CreateSource<T>(sourceName, isNextDirectory);

        valueInfoProperty.arraySize += 1;
        SerializedProperty itemInfoProperty = valueInfoProperty.GetArrayElementAtIndex(valueInfoProperty.arraySize - 1).FindPropertyRelative("itemInfo");
        itemInfoProperty.objectReferenceValue = itemInfo;
    }

    /// <summary>
    /// 创建一个Button按钮，点击创建一个BuffInfo
    /// </summary>
    /// <param name="buffInfos">实例化对象中的BuffInfo列表(数组)</param>
    /// <param name="sourceName">创建的资源的名字</param>
    /// <param name="isNextDirectory">是否新建下一级文件夹并放入创建的资源</param>
    public static void CreateBuff_List(SerializedProperty buffInfos, string sourceName = "NewBuffInfo",string buttonName = "创建buff" ,bool isNextDirectory = true)
    {
        if (GUILayout.Button(buttonName, new GUILayoutOption[] { GUILayout.Width(150), GUILayout.Height(20) }))
        {
            BuffInfo buffInfo = CreateSource<BuffInfo>(sourceName, isNextDirectory);
            buffInfos.arraySize += 1;
            buffInfos.GetArrayElementAtIndex(buffInfos.arraySize - 1).objectReferenceValue = buffInfo;
        }
    }
    /// <summary>
    /// 创建一个Button按钮，点击创建一个BuffInfo
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="buffInfos">实例化对象中的BuffInfo列表(数组)</param>
    /// <param name="sourceName">创建的资源的名字</param>
    /// <param name="isNextDirectory">是否新建下一级文件夹并放入创建的资源</param>
    public static void CreateBuff_List(Rect rect, SerializedProperty buffInfos, string sourceName = "NewBuffInfo", bool isNextDirectory = true)
    {
        if (GUI.Button(rect, "创建Buff"))
        {
            BuffInfo buffInfo = CreateButtonMgr.CreateSource<BuffInfo>(sourceName, isNextDirectory);
            buffInfos.arraySize += 1;
            buffInfos.GetArrayElementAtIndex(buffInfos.arraySize - 1).objectReferenceValue = buffInfo;
        }
    }

    ///// <summary>
    ///// 创建一个Button按钮，点击创建一个EffectInfo
    ///// </summary>
    ///// <param name="serializedObject">创建这个EffectInfo的实例化对象</param>
    ///// <param name="effectInfos">实例化对象中的EffectInfo列表(数组)</param>
    //public static void CreateEffectsInList(SerializedProperty effectInfos, string sourceName = "NewEffectInfo", bool isNextDirectory = true)
    //{
    //    if (GUILayout.Button("创建Effect", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
    //    {
    //        EffectInfo effectInfo = CreateSource<EffectInfo>(sourceName, isNextDirectory);
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
    //        ValueInfo valueInfo = CreateSource<ValueInfo>(serializedObject, sourceName, isNextDirectory);
    //        valueInfoProperty.objectReferenceValue = valueInfo;
    //    }
    //}

    //public static void CreateCondition_List(SerializedProperty conditionProperty, string sourceName = "NewValueInfo", bool isNextDirectory = true)
    //{
    //    if (GUILayout.Button("创建Condition", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
    //    {
    //        ConditionInfo conditionInfo = CreateSource<ConditionInfo>(sourceName, isNextDirectory);
    //        conditionProperty.arraySize += 1;
    //        conditionProperty.GetArrayElementAtIndex(conditionProperty.arraySize - 1).objectReferenceValue = conditionInfo;
    //    }
    //}

    public static void CreateTalkInfo(SerializedProperty talkInfoProperty, string sourceName = "NewTalkInfo", string buttonName = "Talk", float width = 100, float height = 20, bool isNextDirectory = true)
    {
        if (GUILayout.Button("创建" + buttonName, new GUILayoutOption[] { GUILayout.Width(150), GUILayout.Height(20) }))
        {
            TalkInfo valueInfo = CreateSource<TalkInfo>(sourceName, isNextDirectory);
            talkInfoProperty.objectReferenceValue = valueInfo;
        }
    }
    //public static void CreateTalkInfo_FindChild(SerializedProperty talkInfoProperty,string childName ,string sourceName = "NewTalkInfo", string buttonName = "Talk", float width = 100, float height = 20, bool isNextDirectory = true)
    //{
    //    if (GUILayout.Button("创建" + buttonName, new GUILayoutOption[] { GUILayout.Width(150), GUILayout.Height(20) }))
    //    {
    //        TalkInfo valueInfo = CreateSource<TalkInfo>(sourceName, isNextDirectory);
    //        talkInfoProperty.objectReferenceValue = valueInfo;
    //    }
    //}
    public static void CreateTalkInfo_List_FindChild(SerializedProperty talkInfoListProperty, string targetName, string sourceName = "NewTalkInfo", bool isNextDirectory = true)
    {
        if (GUILayout.Button("创建Talk", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
        {
            TalkInfo talkInfo = CreateSource<TalkInfo>(sourceName, isNextDirectory);
            talkInfoListProperty.arraySize += 1;
            SerializedProperty element = talkInfoListProperty.GetArrayElementAtIndex(talkInfoListProperty.arraySize - 1);
            element.FindPropertyRelative(targetName).objectReferenceValue = talkInfo;
        }
    }
    public static void CreateTaskStepInfo_List(SerializedProperty talkInfoListProperty, string sourceName = "NewTaskStepInfo", bool isNextDirectory = true)
    {
        if (GUILayout.Button("创建TaskStep", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
        {
            TaskStepInfo taskStepInfo = CreateSource<TaskStepInfo>(sourceName, isNextDirectory);
            talkInfoListProperty.arraySize += 1;
            SerializedProperty element = talkInfoListProperty.GetArrayElementAtIndex(talkInfoListProperty.arraySize - 1);
            element.objectReferenceValue = taskStepInfo;
        }
    }

    public static T CreateSource<T>(string sourceName = "NewSources", bool isNextDirectory = true) where T : ScriptableObject
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        //string path = AssetDatabase.GetAssetPath(serializedObject.targetObject);//GetPath(serializedObject.targetObject);
        string directoryPath = Path.GetDirectoryName(path);

        if (isNextDirectory == true)
        {
            directoryPath = directoryPath + @"\" + Selection.activeObject.name;// serializedObject.targetObject.name;
        }

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        int i = 0;
        path = directoryPath + @"\" + sourceName + ".asset";

        while (File.Exists(path))
        {
            i++;
            path = directoryPath + @"\" + sourceName + "_" + i + ".asset";
        }

        T obj = ScriptableObject.CreateInstance<T>();

        UnityEditor.AssetDatabase.CreateAsset(obj, path);
        UnityEditor.AssetDatabase.Refresh();
        return obj;
    }

}
