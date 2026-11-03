using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


[CanEditMultipleObjects]
[CustomEditor(typeof(TalkInfo))]
public class TalkInfoEditor : Editor
{
    private SerializedProperty choices;
    private ReorderableList choiceArray;

    private void OnEnable()
    {
        choices = serializedObject.FindProperty("choices");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        //choiceArray.DoLayoutList();

        GUILayout.Space(30);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        CreateButtonMgr.CreateTalkInfo_List_FindChild(choices,"talk","Ñ¡Ôñ");
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
