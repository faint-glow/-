using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class TestOfAssetBuild:MonoBehaviour
{
    string testName;
    public void Test()
    {
        //SetAssetBundle("Assets/Sprite/set_denglu_3 1.png", "Test", ".assetbunlde");
        //SetAssetBundle("Assets/Sprite/launcher_icon 1.png", "Test", ".assetbunlde");


        //Build_AB_Package();
    }


    ////设置资源的目标ab包（bundleName）和变体（bundleVariant）一般用于版本控制
    //private void SetAssetBundle(string filePath,string bundleName,string bundleVariant)
    //{
    //    AssetImporter ass = AssetImporter.GetAtPath(filePath);
    //    Debug.Log(ass);
    //    ass.SetAssetBundleNameAndVariant(bundleName, bundleVariant);
    //}

    //[MenuItem("MenuCommand/Build_AB_Package")]
    //private void Build_AB_Package()
    //{
    //    AssetBundleBuild[] buildMap = new AssetBundleBuild[2];
    //    buildMap[0].assetBundleName = "backBundle";
    //    string[] backAssets = new string[2];
    //    backAssets[0] = "Assets/Sprite/set_denglu_3 1.png";
    //    backAssets[1] = "Assets/Sprite/launcher_icon 1.png";
    //    buildMap[0].assetNames = backAssets;

        
    //    BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    //}
}
