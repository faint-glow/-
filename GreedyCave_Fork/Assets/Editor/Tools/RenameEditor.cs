using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RenameEditor : EditorWindow
{
    string targetName;

    [MenuItem("MenuCommand/Window/RenameEditor",true)]
    static private bool IsProcessAltasToSprites()
    {
        return (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(Selection.activeObject)));//如果选中的物体的路径为空，则不显示按钮（返回false）
    }

    [MenuItem("MenuCommand/Window/RenameEditor",false)]
    public static void ShowWindow()
    {
        //显示现有窗口实例。如果没有，请创建一个。
        EditorWindow.GetWindow(typeof(RenameEditor));
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("选中项包含其子文件夹的文件，名字全部变为设定的名字");
        targetName = EditorGUILayout.TextField("目标名字", targetName);

        if (GUILayout.Button("重命名"))
        {
            Rename();
        }

    }

    public void Rename()
    {
        Object[] objects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        //Texture2D[] texArray = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);

        for (int i = 0; i < objects.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(objects[i]);
            AssetDatabase.RenameAsset(path, targetName+"_" + i);
        }
    }
}
