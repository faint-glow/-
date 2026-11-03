using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TaskStepInfo))]
public class TaskStepInfoEditor : Editor
{
    private string talkInfoName = "NewTalkInfo";

    SerializedProperty receiveTalkInfo_TalkInfo;
    SerializedProperty finishTalkInfo_TalkInfo;

    private void OnEnable()
    {
        receiveTalkInfo_TalkInfo = serializedObject.FindProperty("receiveTalkInfo").FindPropertyRelative("talkInfo");
        finishTalkInfo_TalkInfo = serializedObject.FindProperty("finishTalkInfo").FindPropertyRelative("talkInfo");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        talkInfoName = EditorGUILayout.TextArea(talkInfoName);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        CreateButtonMgr.CreateTalkInfo(receiveTalkInfo_TalkInfo, talkInfoName, "receiveTalkInfo",150);
        CreateButtonMgr.CreateTalkInfo(finishTalkInfo_TalkInfo, talkInfoName , "finishTalkInfo",150);
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
