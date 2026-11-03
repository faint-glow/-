//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomPropertyDrawer(typeof(EnumEditor))]
//public class EnumEditorPropertyDrawer : PropertyDrawer
//{
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        Debug.Log(attribute.GetType());

//        //EnumEditor enumEditor = attribute as EnumEditor;
//        //Dictionary<string, int> oldEnumDic = new Dictionary<string, int>();
//        //for (int i = 0; i < enumEditor.enumType.GetEnumValues().Length; i++)
//        //{
//        //    object enumElement = enumEditor.enumType.GetEnumValues().GetValue(i);
//        //    oldEnumDic[enumElement.ToString()] = (int)enumElement;
//        //}
//        //Dictionary<string, int> newEnumDic = new Dictionary<string, int>();
//        //if (property.propertyType == SerializedPropertyType.Enum)
//        //{
//        //    string currentEnumName = property.enumNames[property.enumValueIndex];

//        //    for (int i = 0; i < length; i++)
//        //    {

//        //    }
//        //}
//        EditorGUI.PropertyField(position, property);
//    }
//}
