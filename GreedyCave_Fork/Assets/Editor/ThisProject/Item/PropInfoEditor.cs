//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//public class PropProperty : ItemProperty
//{
//    public SerializedProperty propType;
//    public SerializedProperty itemType;
//    public SerializedProperty medicineProperty;
//    public SerializedProperty materialPropertys;
//    public SerializedProperty containterPropertys;

//}

//[CanEditMultipleObjects]
//[CustomEditor(typeof(PropInfo))]
//public class PropInfoEditor : Editor
//{
//    PropProperty property = new PropProperty();

//    private void OnEnable()
//    {
//        ItemInfoEditor.InitialItemProperty(property, serializedObject);
//        InitialPropProperty(property, serializedObject);
//    }

//    public override void OnInspectorGUI()
//    {
//        ItemInfoEditor.ShowItemProperty(property, serializedObject);
//        ShowPropInfo(property, serializedObject);
//    }

//    public void InitialPropProperty(PropProperty property, SerializedObject serializedObject)
//    {
//        property.propType = serializedObject.FindProperty("propType");
//        property.itemType = serializedObject.FindProperty("itemType");

//        property.medicineProperty = serializedObject.FindProperty("medicineProperty");
//        property.materialPropertys = serializedObject.FindProperty("materialPropertys");
//        property.containterPropertys = serializedObject.FindProperty("containterPropertys");

//    }

//    public void ShowPropInfo(PropProperty property, SerializedObject serializedObject)
//    {
//        serializedObject.Update();

//        EditorGUILayout.PropertyField(property.propType);
//        EditorGUILayout.PropertyField(property.itemType);
//        if ((PropType)property.propType.enumValueIndex == PropType.Medicine)
//        {
//            EditorGUILayout.PropertyField(property.medicineProperty);
//        }
//        else if ((PropType)property.propType.enumValueIndex == PropType.Material)
//        {
//            EditorGUILayout.PropertyField(property.materialPropertys);
//        }
//        else if ((PropType)property.propType.enumValueIndex == PropType.Containter)
//        {
//            EditorGUILayout.PropertyField(property.containterPropertys);
//        }

//        //GUILayout.Space(30);
//        //EditorGUILayout.BeginHorizontal();
//        //GUILayout.FlexibleSpace();

//        ////if ((PropType)property.propType.enumValueIndex != PropType.Material)
//        ////{
//        ////    CreateButtonMgr.CreateEffectsInList(property.effectInfos);
//        ////}

//        //EditorGUILayout.EndHorizontal();

//        serializedObject.ApplyModifiedProperties();
//    }


//}
