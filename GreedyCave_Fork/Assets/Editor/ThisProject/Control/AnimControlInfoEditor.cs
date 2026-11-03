using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimControlInfoProperty
{
    public SerializedProperty controlName;
    public SerializedProperty isNeverEnd;
    public SerializedProperty controlTime;//¿ØÖÆÊ±¼ä
    public SerializedProperty controlLevel;
    public SerializedProperty effectFXMgr;
    public SerializedProperty stateName;
    public SerializedProperty speed;
}
[CustomEditor(typeof(AnimControlInfo))]
public class AnimControlInfoEditor : Editor
{
    public AnimControlInfoProperty animControlInfoProperty = new AnimControlInfoProperty();

    private void OnEnable()
    {
        animControlInfoProperty.controlName = serializedObject.FindProperty("controlName");
        animControlInfoProperty.isNeverEnd = serializedObject.FindProperty("isNeverEnd");
        animControlInfoProperty.controlTime = serializedObject.FindProperty("controlTime");
        animControlInfoProperty.controlLevel = serializedObject.FindProperty("controlLevel");
        animControlInfoProperty.effectFXMgr = serializedObject.FindProperty("effectFXMgr");

        animControlInfoProperty.stateName = serializedObject.FindProperty("stateName");
        animControlInfoProperty.speed = serializedObject.FindProperty("speed");

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(animControlInfoProperty.controlName);
        EditorGUILayout.PropertyField(animControlInfoProperty.isNeverEnd);
        if (animControlInfoProperty.isNeverEnd.boolValue == false)
            EditorGUILayout.PropertyField(animControlInfoProperty.controlTime);
        EditorGUILayout.PropertyField(animControlInfoProperty.controlLevel);
        EditorGUILayout.PropertyField(animControlInfoProperty.effectFXMgr);
        EditorGUILayout.PropertyField(animControlInfoProperty.stateName);
        EditorGUILayout.PropertyField(animControlInfoProperty.speed);

        serializedObject.ApplyModifiedProperties();
    }
}
