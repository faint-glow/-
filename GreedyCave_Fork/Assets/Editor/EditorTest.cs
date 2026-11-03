using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//https://zhuanlan.zhihu.com/p/109328430

[CustomEditor(typeof(NewTest))]
public class EditorTest : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Click Me"))
        {
            //法一
            //NewTest test = target as NewTest;
            //test.testName = "eee";
            //EditorUtility.SetDirty(test);//应用更改，此时Hierarchy界面会出现为保存的小星星，此时Ctrl+S才可以真正做到保存

            //法二（未测试：据说可以撤销，但是法一不行）
            serializedObject.FindProperty("testName").stringValue = "123";
            serializedObject.ApplyModifiedProperties();//
        }
    }

}


public class MenuCommand
{
    //[MenuItem("MenuCommand/SwapGameObject")]
    protected static void SwapGameObject()
    {
        //只有两个物体才能交换
        if (Selection.gameObjects.Length == 2)
        {
            Vector3 tmpPos = Selection.gameObjects[0].transform.position;
            Selection.gameObjects[0].transform.position = Selection.gameObjects[1].transform.position;
            Selection.gameObjects[1].transform.position = tmpPos;

            //处理两个以上的场景物体可以使用MarkSceneDirty
            //功能暂时未知，应该是类似于应用更改一样的东西
            //UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
    }

    //动态设置包名和变体
    //AssetImporter.GetAtPath(filePath).SetAssetBundleNameAndVariant(bundleName, bundleVariant);
    //[MenuItem("MenuCommand/Build_AB_Package")]
    protected static void Build_AB_Package()
    {
        AssetBundleBuild[] buildMap = new AssetBundleBuild[2];
        buildMap[0].assetBundleName = "backBundle";
        string[] backAssets = new string[2];
        backAssets[0] = "Assets/Sprite/set_denglu_3 1.png";
        backAssets[1] = "Assets/Sprite/launcher_icon 1.png";
        buildMap[0].assetNames = backAssets;


        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/Test", buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
    }
}

//开启一个新窗口
public class MyWindow : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    // 将名为"My Window"的菜单项添加到 Window 菜单
    [MenuItem("Window/My Window")]
    public static void ShowWindow()
    {
        //显示现有窗口实例。如果没有，请创建一个。
        EditorWindow.GetWindow(typeof(MyWindow));
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
    }
}

//开启一个新窗口
public class WizardCreateLight : ScriptableWizard
{
    public float range = 500;
    public Color color = Color.red;

    [MenuItem("GameObject/Create Light Wizard")]
    static void CreateWizard()
    {
        //标题名字，创建按钮的名字，其他按钮的名字
        ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create", "Apply");
        //If you don't want to use the secondary button simply leave it out:
        //ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
    }
    //创建按钮的调用函数
    void OnWizardCreate()
    {
        GameObject go = new GameObject("New Light");
        Light lt = go.AddComponent<Light>();
        lt.range = range;
        lt.color = color;
    }

    void OnWizardUpdate()
    {
        helpString = "Please set the color of the light!";
    }

    //其他按钮的调用函数
    // When the user presses the "Apply" button OnWizardOtherButton is called.
    void OnWizardOtherButton()
    {
        if (Selection.activeTransform != null)
        {
            Light lt = Selection.activeTransform.GetComponent<Light>();

            if (lt != null)
            {
                lt.color = Color.red;
            }
        }
    }
}



