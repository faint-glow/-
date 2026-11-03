using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class TaskInfoProperty
{
    public SerializedProperty taskName;
    public SerializedProperty openConditions;
    public SerializedProperty taskSteps;
}

[CanEditMultipleObjects]
[CustomEditor(typeof(TaskInfo))]
public class TaskInfoEditor : Editor
{
    private ReorderableList conditionArray;

    public SerializedProperty isMainThread;
    public SerializedProperty nextTaskName;
    //public SerializedProperty promoterName;
    public SerializedProperty taskName;
    public SerializedProperty openConditions;
    public SerializedProperty taskSteps;

    private string taskStepName = "TaskStep"; 
    private void InitialPropery()
    {
        taskName = serializedObject.FindProperty("taskName");
        isMainThread = serializedObject.FindProperty("isMainThread");
        nextTaskName = serializedObject.FindProperty("nextTaskName");
        openConditions = serializedObject.FindProperty("openConditions");
        taskSteps = serializedObject.FindProperty("taskSteps");
    }

    private void OnEnable()
    {
        InitialPropery();
        #region ReorderableList对数组或列表进行GUI修改(注：需要在OnInspectorGUI中调用ReorderableList.DoLayoutList();)

        //    conditionArray = new ReorderableList(serializedObject, taskInfoProperty.conditions
        //, true, true, true, true);

        //    //自定义列表名称
        //    conditionArray.drawHeaderCallback = (Rect rect) =>
        //    {
        //        GUI.Label(rect, "Condition Array");
        //    };

        //    //定义元素的高度
        //    conditionArray.elementHeight = 150;//默认为150，但是可以被ReorderableList.elementHeightCallback控制单个元素的高度

        //    #region 绘制样例，包含保留源绘制
        //    //    choiceArray = new ReorderableList(serializedObject, choices
        //    //, true, true, true, true);

        //    //    choiceArray.elementHeight = 60;

        //    //    //保留原来的绘制，但是高度需要自行调整
        //    //    choiceArray.drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
        //    //    {
        //    //        rect.x += 10;
        //    //        SerializedProperty item = choiceArray.serializedProperty.GetArrayElementAtIndex(index);
        //    //        ReorderableList.defaultBehaviours.DrawElement(rect, item, null, selected, focused, true, true);
        //    //        //EditorGUI.PropertyField(rect, item);
        //    //    };

        //    //    //当添加新元素时的回调函数，自定义新元素的值
        //    //    choiceArray.onAddCallback = (ReorderableList list) =>
        //    //    {
        //    //        ReorderableList.defaultBehaviours.DoAddButton(list);
        //    //    };

        //    //    //当删除元素时候的回调函数，实现删除元素时，有提示框跳出
        //    //    choiceArray.onRemoveCallback = (ReorderableList list) =>
        //    //    {
        //    //        ReorderableList.defaultBehaviours.DoRemoveButton(list);
        //    //    };

        //    //    choiceArray.onSelectCallback = (ReorderableList list) =>
        //    //    {
        //    //        SerializedProperty targetElement = choices.GetArrayElementAtIndex(list.index);
        //    //        //list.elementHeight = EditorGUI.GetPropertyHeight(targetElement, GUIContent.none, targetElement.isExpanded);
        //    //    };
        //    #endregion
        //    //自定义绘制列表元素
        //    conditionArray.drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
        //    {
        //        //根据index获取对应元素 
        //        rect.x += 10;
        //        SerializedProperty item = conditionArray.serializedProperty.GetArrayElementAtIndex(index);
        //        EditorGUI.PropertyField(rect, item);//直接这么用的画，需要用PropertyDrawer处理被绘制的类，否则就在这里绘制
        //    };

        //    conditionArray.drawHeaderCallback = (Rect rect) =>
        //    {
        //        string name = "任务接取条件";
        //        EditorGUI.LabelField(rect, name);
        //    };

        //    //可以控制单个元素的高度
        //    conditionArray.elementHeightCallback = (int index) =>
        //    {
        //        var item = conditionArray.serializedProperty.GetArrayElementAtIndex(index);
        //        float h = EditorGUI.GetPropertyHeight(item);
        //        return h;
        //    };

        //    //当添加新元素时的回调函数，自定义新元素的值
        //    conditionArray.onAddCallback = (ReorderableList list) =>
        //    {
        //        if (list.serializedProperty != null)
        //        {
        //            list.serializedProperty.arraySize++;
        //            list.index = list.serializedProperty.arraySize - 1;
        //            SerializedProperty item = list.serializedProperty.GetArrayElementAtIndex(list.index);

        //            //以下代码可以用于初始化新增数据
        //            var condition_TaskName = item.FindPropertyRelative("condition_TaskName");
        //            condition_TaskName.stringValue = "NewCondition_TaskName";
        //        }
        //        else
        //        {
        //            ReorderableList.defaultBehaviours.DoAddButton(list);
        //        }
        //    };

        //    //当删除元素时候的回调函数，实现删除元素时，有提示框跳出
        //    conditionArray.onRemoveCallback = (ReorderableList list) =>
        //    {
        //        //if (EditorUtility.DisplayDialog("Warnning", "Do you want to remove this element?", "Remove", "Cancel"))
        //        //{
        //        //    ReorderableList.defaultBehaviours.DoRemoveButton(list);
        //        //}
        //        ReorderableList.defaultBehaviours.DoRemoveButton(list);
        //    };

        #endregion
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(taskName, new GUIContent("当前任务名"));
        EditorGUILayout.PropertyField(isMainThread,new GUIContent("是否为主线任务"));
        if(isMainThread.boolValue == true)
            EditorGUILayout.PropertyField(nextTaskName, new GUIContent("下一个主线任务的名字"));
        EditorGUILayout.PropertyField(openConditions, new GUIContent("任务开启条件"));
        EditorGUILayout.PropertyField(taskSteps, new GUIContent("任务阶段"));
        GUILayout.Space(10);
        taskStepName = EditorGUILayout.TextArea(taskStepName);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        CreateButtonMgr.CreateTaskStepInfo_List(taskSteps, taskStepName);
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
