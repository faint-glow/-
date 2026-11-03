using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateSkill : EditorWindow
{
    //public Transform parentTrans;//要显示的物体必须是public
    private SerializedObject serObj;

    public PropertyType propertyType;
    private SerializedProperty propertyTypeShow;
    public Buff buff;
    public Skill skill;
    private SerializedProperty typeShow; 


    [MenuItem("MenuCommand/CreateSkill", false)]
    public static void ShowWindow()
    {
        //显示现有窗口实例。如果没有，请创建一个。
        EditorWindow.GetWindow(typeof(CreateSkill));
    }

    private void OnEnable()
    {
        ////**********************自建window显示类变量****************************
        serObj = new SerializedObject(this);
        //showObj = serObj.FindProperty("parentTrans");
        propertyTypeShow = serObj.FindProperty("propertyType");
    }

    void OnGUI()
    {
        //EditorGUILayout.LabelField("");

        //**********************自建window显示类变量****************************
        EditorGUILayout.PropertyField(propertyTypeShow, true);


        serObj.ApplyModifiedProperties();//应用更改

        if (propertyType == PropertyType.攻击力)
        {
            typeShow = serObj.FindProperty("buff");
        }
        else
        {
            typeShow = serObj.FindProperty("skill");
        }
        EditorGUILayout.PropertyField(typeShow, true);

    }

}
