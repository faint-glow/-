using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 特别注意，由于内部实现为了节约一点内存
/// 不能直接以StreamingAssets文件夹为ab包文件的根目录，至少要把ab包文件放在StreamingAssets文件夹下至少一个子文件夹下
/// 因为内部字典用Application.StreamingAssets后的路径作为字典的键值来映射AB包
/// 而内部实现必须要知道最后一级文件夹的名字，所以才会产生上述条件
/// </summary>
public class AssetBundleLoadMgr : Singleton<AssetBundleLoadMgr>
{
    //AB包注意事项
    //打包AB包有两种方式
    //1.BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath+ "/AB_Test", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    //这个方式是把Assets文件夹下所有被修改过包名的资源打包。（变体名不改也可以打包）。不同包名会创建一个对应的包

    //2.
    //protected static void Build()
    //{
    //    AssetBundleBuild[] buildMap = new AssetBundleBuild[2];
    //    string[] test2 = new string[1];
    //    buildMap[0].assetBundleName = "gameObj";
    //    test2[0] = "Assets/Sprite/Test/GameObject.prefab";
    //    buildMap[0].assetNames = test2;
    //    BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath + "/NewTest", buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    //    AssetDatabase.Refresh();

    //    buildMap = new AssetBundleBuild[2];
    //    buildMap[0].assetBundleName = "sprite";
    //    string[] test1 = new string[2];
    //    test1[0] = "Assets/Sprite/Test/TestSprite1.png";
    //    test1[1] = "Assets/Sprite/Test/TestSprite2.png";
    //    buildMap[0].assetNames = test1;
    //    BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath + "/NewTest", buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    //    AssetDatabase.Refresh();
    //}

    //一个AssetBundleBuild[]数组的每个元素都是一个包，可以设置其包名（assetBundleName）和资源路径（assetNames），资源路径是数组，数组的每个元素都是一个路径
    //如果资源已经被设置了包名。那么之前设置的跟这次无关。打包时还是用这次代码设置的为准。不过设置包名的界面不会有什么表现
    //一个AssetBundleBuild[]数组进行打包的资源可以互相有依赖关系。（确切来说是一次BuildPipeline.BuildAssetBundles(....)）
    //如果在两个不同的数组打包不会有依赖关系，他依赖的东西会在自己的包里重新创建一个备份。就像上面的代码一样，此时是不会有依赖关系的

    //一次BuildPipeline.BuildAssetBundles有一个确定的路径和一个确定的AssetBundleBuild[]数组。所以有依赖关系的包一定在同一个文件夹下（如果没乱移动文件）
    //而且打包后unity会为这个文件夹创建一个和文件夹名字相同的依赖关系文件。通过加载这个文件。给他一个包的名字，可以返回这个包的依赖包的名字数组
    //注意：他只返回包的名字数组，不返回包的路径数组。还记得有依赖关系的文件必定是在同一个文件夹下吗。我们可以通过这个加载目标包的依赖包

    //如果是用第一种方式，那么就相当于用了一次BuildPipeline.BuildAssetBundles(....)。那么包之间都会有依赖关系。
    //如果第一种方式进行两次。第一次打的包需要依赖第二次打的包。
    //第一次打包后。第一次包会有副本可以直接使用。但是进行第二次打包以后，就没了。就会产生依赖。不能直接使用

    //如果第一次打包的包名为A。第二次打包，把第一次的资源包名设置为其他的或者None。那么他和第二次打包的资源也不会有依赖关系
    //但是第二次设置的包名也会打出一个新包。这第二次打包就会和第一次没什么关系。除了不能换文件夹的名字

    //如果第一次打包，包名为A，第二次打包把第一次和第二次的资源包名设置为B，。那么第一个包不会依赖第二个包，第二个包也不会依赖第一个包

    /// <summary>
    /// 缓存加载的AssetBundle，防止多次加载
    /// </summary>
    private Dictionary<string, AssetBundle> m_abDic = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 依赖项关系映射。每个AssetBundleManifest对应的是从Application.streamingAssetsPath后开始的文件夹路径
    /// </summary>
    private Dictionary<string, AssetBundleManifest> m_manifestDic = new Dictionary<string, AssetBundleManifest>();

    //public string AB_Path = Application.streamingAssetsPath + "/AB_AllTest/";
    //获取依赖项关系列表
    private void GetManifest(string directorPath)
    {
        int index = directorPath.LastIndexOf("\\");
        string manifestName = directorPath.Substring(index + 1);//manifestName正常情况下都是与文件夹的名字一样的，除非干过好事

        string streamingAssetsAbPath = Application.streamingAssetsPath + "/" + directorPath + "/" + manifestName;
        Debug.Log(streamingAssetsAbPath);
        AssetBundle mainPackage = AssetBundle.LoadFromFile(streamingAssetsAbPath);
        m_manifestDic[directorPath] = mainPackage.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        mainPackage.Unload(false);//卸载ab包，true参数会让已经引用ab包实例化后的数据丢失索引。false只卸载包
    }
    /// <summary>
    /// 从ab包中加载数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="allPath">路径从Application.streamingAssetsPath后开始，直到包名，不包含后缀名</param>
    /// <param name="assetName">ab包中的数据名</param>
    /// <returns></returns>
    public T LoadAsset<T>(string allPath, string assetName) where T : UnityEngine.Object
    {
        AssetBundle ab = LoadAssetBundle(allPath);
        if (ab == null)
            return null;
        T t = ab.LoadAsset<T>(assetName);
        return t;
    }

    //注意，此方法暂未处理依赖项
    public void LoadAssetAsync<T>(string allPath, string assetName, Action<T> action) where T : UnityEngine.Object
    {
        MonoMgr.GetInstance().StartCoroutine(LoadAssetAsyncMore(allPath, assetName, action));
    }

    IEnumerator LoadAssetAsyncMore<T>(string allPath, string assetName, Action<T> action) where T : UnityEngine.Object
    {
        AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/" + allPath);
        yield return abcr;
        T res = abcr.assetBundle.LoadAsset<T>(assetName);
        action.Invoke(res);

        //abcr.completed += action;
    }

    public void Recycling()
    {
        //将所有未使用的资源进行回收。这设计到GC机制，非常消耗性能，不要经常调用
        Resources.UnloadUnusedAssets();
    }

    public AssetBundle LoadAB(string allPath)
    {
        AssetBundle ab = LoadAssetBundle(allPath);
        return ab;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">ab包从Application.streamingAssetsPath后开始的路径，不包含后缀名</param>
    /// <returns></returns>
    private AssetBundle LoadAssetBundle(string path)
    {
        AssetBundle ab = null;
        string abResPath = Application.streamingAssetsPath + "/" + path;
        if(File.Exists(abResPath) == false)
        {
            Debug.Log("路径"+ abResPath + "不存在");
            return null;
        }

        if (!m_abDic.ContainsKey(path))
        {
            ab = AssetBundle.LoadFromFile(abResPath);
            m_abDic[path] = ab;
        }
        else
        {
            ab = m_abDic[path];
        }
        //加载依赖包

        int index = path.LastIndexOf("/");
        string directoryPath = path.Substring(0, index);//ab包的文件夹路径
        string abName = path.Substring(index + 1);//ab包的名字

        if (!m_manifestDic.ContainsKey(directoryPath))
        {
            GetManifest(directoryPath);
        }
        string[] dependences = m_manifestDic[directoryPath].GetAllDependencies(abName);
        for (int i = 0; i < dependences.Length; i++)
        {
            string dependenceFullPath = directoryPath + "/" + dependences[i];
            if (!m_abDic.ContainsKey(dependenceFullPath))
            {
                LoadAssetBundle(dependenceFullPath);//方法内部会把ab包索引存入m_abDic中
                //AssetBundle dependenceAb = LoadAssetBundle(dependenceFullPath);
                //m_abDic[dependenceFullPath] = dependenceAb;
            }
        }
        return ab;
    }




    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="abPath"></param>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public IEnumerator IE_LoadABScene(string abPath, string sceneName)
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(abPath));
        yield return request;
        var bundle = request.assetBundle; //这句话一定要加

        AsyncOperation asy = SceneManager.LoadSceneAsync(sceneName); //sceneName不能加后缀,只是场景名称
        yield return asy;
    }

    Dictionary<string, AssetBundleCreateRequest> m_dicObjABRequest = new Dictionary<string, AssetBundleCreateRequest>();
    /// <summary>
    /// 加载异步资源加入AB包缓存
    /// </summary>
    /// <returns></returns>
    public IEnumerator IE_LoadAssetBundleByPathInSceneLoad(List<string> m_listSceneLoadAB)
    {
        if (m_listSceneLoadAB.Count > 0)
        {
            for (int i = 0; i < m_listSceneLoadAB.Count; i++)
            {
                if (!m_dicObjABRequest.ContainsKey(m_listSceneLoadAB[i]))
                {
                    AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(m_listSceneLoadAB[i]));
                    yield return request;
                    m_dicObjABRequest.Add(m_listSceneLoadAB[i], request);
                }
            }
            Debug.Log("加载AB完成");
            m_listSceneLoadAB.Clear();
        }
    }
    /// <summary>
    ///  加载AB包实例化对象
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <param name="p"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public GameObject LoadGameObjectAssetBundleByPathAndName(string path, string name, Vector3 p, Quaternion r)
    {
        GameObject go = null;
        if (m_dicObjABRequest.ContainsKey(path))
        {
            AssetBundle asBundle;
            asBundle = m_dicObjABRequest[path].assetBundle; //对象名字
            GameObject obj = asBundle.LoadAsset<GameObject>(name);
            go = GameObject.Instantiate(obj, p, r);
        }
        return go;
    }

}
