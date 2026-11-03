using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(EnemyInfo))]
public class EnemyInfoEditor : Editor
{
    SerializedProperty propertyFactors;
    SerializedProperty booty;
    SerializedProperty itemList;

    private void OnEnable()
    {
        booty = serializedObject.FindProperty("booty");
        itemList = booty.FindPropertyRelative("itemList");//FindPropertyRelative在一个属性中查找属性
        propertyFactors = serializedObject.FindProperty("propertyFactors");

        PropertyFactorInfo.InitialPropertys(serializedObject, propertyFactors);
        PropertyFactorInfo.SortPropertys(serializedObject, propertyFactors, PropertyFactorInfo.baseSort);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        GUILayout.Space(30);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Equip", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
        {
            CreateButtonMgr.CreateItem_List<EquipInfo>(itemList, "NewEquipInfo", true);
        }


        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
    //创建新窗口的按钮
    //if (GUILayout.Button("CreateItem", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
    //{
    //    EditorWindow window = EditorWindow.GetWindow(typeof(CreateItemWindow));
    //    (window as CreateItemWindow).Initila(serializedObject, itemList);
    //}

}

public class CreateItemWindow : EditorWindow
{
    SerializedObject serializedObject;
    SerializedProperty itemList;
    public void Initila(SerializedObject serializedObject, SerializedProperty itemList)
    {
        this.serializedObject = serializedObject;
        this.itemList = itemList;
    }

    void OnGUI()
    {
        serializedObject.Update();

        GUILayout.Space(30);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Equip", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
        {
            CreateButtonMgr.CreateItem_List<EquipInfo>(itemList, "NewEquipInfo", true);
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

    }


}
