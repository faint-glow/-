using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Condition_Collect))]
public class Condition_CollectEditor : SelfAdaptionPropertyDrawer
{
    public override string GetSpreadName(SerializedProperty property)
    {
        return "收集条件";
    }
    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty collectionType = property.FindPropertyRelative("collectionType");

        SerializedProperty itemCategory = property.FindPropertyRelative("itemCategory");
        SerializedProperty equipType = property.FindPropertyRelative("equipType");
        SerializedProperty itemQuality = property.FindPropertyRelative("itemQuality");

        SerializedProperty itemName = property.FindPropertyRelative("itemName");
        SerializedProperty count = property.FindPropertyRelative("count");

        EditorGUI.PropertyField(GetRectVertical(collectionType), collectionType);
        EditorGUI.PropertyField(GetRectVertical(itemQuality), itemQuality);

        if ((CollectionType)collectionType.enumValueIndex == CollectionType.指定类别)
        {
            EditorGUI.PropertyField(GetRectVertical(itemCategory), itemCategory);
            if((ItemCategory)itemCategory.enumValueIndex == ItemCategory.Equip)
                EditorGUI.PropertyField(GetRectVertical(equipType), equipType);
        }
        else if ((CollectionType)collectionType.enumValueIndex == CollectionType.指定物品)
        {
            EditorGUI.PropertyField(GetRectVertical(itemName), itemName);
        }
        EditorGUI.PropertyField(GetRectVertical(count), count);

    }
}
