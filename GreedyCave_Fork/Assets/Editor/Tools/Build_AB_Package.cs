using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Build_AB_Package : EditorWindow
{
    string sourcePath = "";
    string targetPath = "";
    string bundleName = "";
    //string bundleVariant = "assetBundle";
    Vector2 scrollPosOne;
    Vector2 scrollPosTwo;


    //https://blog.csdn.net/linxinfa/article/details/100143530

    //动态设置包名和变体
    //AssetImporter.GetAtPath(filePath).SetAssetBundleNameAndVariant(bundleName, bundleVariant);
    [MenuItem("MenuCommand/Window/Build_AB_Package")]
    public static void ShowWindow()
    {
        //显示现有窗口实例。如果没有，请创建一个。
        EditorWindow.GetWindow(typeof(Build_AB_Package));
    }

    void OnGUI()
    {
        GUILayout.Label("一个文件夹下的所有物体同时打包");

        //EditorGUILayout.BeginHorizontal();

        GUILayout.Label("资源路径，一个目标文件夹路径，从Assets/后开始");
        scrollPosOne = EditorGUILayout.BeginScrollView(scrollPosOne, EditorStyles.textField, GUILayout.Width(300), GUILayout.Height(100));
        sourcePath = EditorGUILayout.TextArea(sourcePath);
        EditorGUILayout.EndScrollView();

        GUILayout.Label("打包路径，一个目标文件夹路径，从Assets/StreamingAssets/AB_Package/后开始");
        scrollPosTwo = EditorGUILayout.BeginScrollView(scrollPosTwo, EditorStyles.textField, GUILayout.Width(300), GUILayout.Height(100));
        targetPath = EditorGUILayout.TextArea(targetPath);
        EditorGUILayout.EndScrollView();

        //EditorGUILayout.EndHorizontal();

        bundleName = EditorGUILayout.TextField("包名", bundleName);

        if (GUILayout.Button("创建"))
        {
            //BuildAllTest();
            //BuildDirector();
            BulidSingleTest();
        }

    }

    private void BuildDirector()
    {
        List<string> fileList = new List<string>();
        GetFilePath(Path.Combine(Application.dataPath,sourcePath), fileList, false);
        string[] fileArray = fileList.ToArray();


        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
        buildMap[0].assetBundleName = this.bundleName;
        buildMap[0].assetNames = fileArray;

        int index = fileArray[0].IndexOf("Assets");
        for (int i = 0; i < fileArray.Length; i++)
        {
            fileArray[i] = fileArray[i].Substring(index);//资源路径需要在从Assest
        }

        string allPath = Path.Combine(Application.streamingAssetsPath, "AB_Package", targetPath);
        if (!Directory.Exists(allPath))
            Directory.CreateDirectory(allPath);

        BuildPipeline.BuildAssetBundles(allPath, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
    }


    /// <summary>
    ///获取所有文件和子文件夹的文件路径
    /// </summary>
    /// <param name="path">文件夹的路径</param>
    /// <param name="fileList">传入一个接收路径的列表</param>
    /// <param name="isGetChild">是否获取子文件夹的文件路径</param>
    public static void GetFilePath(string path, List<string> fileList,bool isGetChild = false)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] fil = dir.GetFiles();
        
        foreach (FileInfo f in fil)
        {
            if (f.FullName.Substring(f.FullName.LastIndexOf(".")) == ".meta")//不要后缀名为.meta的文件
                continue;

            fileList.Add(f.FullName);//添加文件路径到列表中  
        }

        if (isGetChild)
        {
            //获取子文件夹内的文件列表，递归遍历  
            DirectoryInfo[] dii = dir.GetDirectories();
            foreach (DirectoryInfo d in dii)
            {
                GetFilePath(d.FullName, fileList, isGetChild);
            }
        }

    }



    private void BuildAllTest()
    {
        //路径类型Assets/......*.png
        //AssetImporter.GetAtPath("Assets/Sprite/Test/TestSprite2.png").SetAssetBundleNameAndVariant("Test", "assetBundle");

        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath + "/AB_AllTest", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
    }

    private void BulidSingleTest()
    {
        AssetBundleBuild[] buildMap = new AssetBundleBuild[2];

        string[] test2 = new string[1];
        buildMap[0].assetBundleName = "Test";
        test2[0] = "Assets/Sprite/Test/GameObject.prefab";
        buildMap[0].assetNames = test2;

        //BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath + "/AB_Package", buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        //AssetDatabase.Refresh();
        //buildMap = new AssetBundleBuild[2];

        buildMap[1].assetBundleName = "Test1";
        string[] test1 = new string[2];
        test1[0] = "Assets/Sprite/Test/TestSprite1.png";
        test1[1] = "Assets/Sprite/Test/TestSprite2.png";
        buildMap[1].assetNames = test1;

        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath + "/AB_Package1", buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 打包普通资源
    /// </summary>
    [MenuItem("Bundle/BuildMyBundle")]
    public static void BuildMyBundle(string outAbPath)
    {

        //当在硬盘目录结构里不存在该路径时,创建文件夹
        if (!Directory.Exists(outAbPath))
        {
            Directory.CreateDirectory(outAbPath);
        }
        BuildPipeline.BuildAssetBundles(outAbPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
    /// <summary>
    /// 打包场景
    /// </summary>
    [MenuItem("Bundle/BuildMySceneBundle")]
    public static void CreateSceneAssetBundle_BuildPlayer(string outAbPath)
    {
        if (!Directory.Exists(outAbPath))
        {
            Directory.CreateDirectory(outAbPath);
        }
        string[] path = { "Assets/Scene/PlayMusic.unity" };
        BuildPipeline.BuildPlayer(path, outAbPath, BuildTarget.StandaloneWindows, BuildOptions.BuildAdditionalStreamedScenes);
    }
}
