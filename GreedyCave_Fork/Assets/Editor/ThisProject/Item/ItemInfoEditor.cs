using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemProperty
{
    public SerializedProperty itemName;
    public SerializedProperty itemSprite;
    public SerializedProperty isNeverDrop;
    public SerializedProperty basePrice;
    public SerializedProperty itemQualitys;
    //[TextArea(2, 5)]//最小显示两行，字多了以后最多显示5行，5行之后变成滚动条
    //public SerializedProperty itemDescription;
    public SerializedProperty itemBaseInfo;
    public SerializedProperty itemExtraInfo;
}

[CanEditMultipleObjects]
[CustomEditor(typeof(ItemInfo))]
public class ItemInfoEditor : Editor
{
    public static void InitialItemProperty(ItemProperty itemProperty,SerializedObject serializedObject)
    {
        itemProperty.itemName = serializedObject.FindProperty("itemName");
        itemProperty.itemSprite = serializedObject.FindProperty("itemSprite");
        itemProperty.isNeverDrop = serializedObject.FindProperty("isNeverDrop");
        itemProperty.basePrice = serializedObject.FindProperty("basePrice");

        itemProperty.itemQualitys = serializedObject.FindProperty("itemQualitys");
        itemProperty.itemBaseInfo = serializedObject.FindProperty("itemBaseInfo");
        itemProperty.itemExtraInfo = serializedObject.FindProperty("itemExtraInfo");

    }
    public static void ShowItemProperty(ItemProperty itemProperty, SerializedObject serializedObject)
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(itemProperty.itemName);
        EditorGUILayout.PropertyField(itemProperty.itemSprite);
        EditorGUILayout.PropertyField(itemProperty.isNeverDrop);
        EditorGUILayout.PropertyField(itemProperty.basePrice);
        EditorGUILayout.PropertyField(itemProperty.itemQualitys);
        EditorGUILayout.PropertyField(itemProperty.itemBaseInfo);
        EditorGUILayout.PropertyField(itemProperty.itemExtraInfo);

        serializedObject.ApplyModifiedProperties();
    }
}
