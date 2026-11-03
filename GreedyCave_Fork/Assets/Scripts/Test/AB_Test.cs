using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEngine.Networking;


public class AB_Test : MonoBehaviour
{
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        //LoadAssetFirst(Application.streamingAssetsPath + "/AB_Package/test");

        ////StartCoroutine(UnityWebRequestLearn(Application.streamingAssetsPath + "/AB_Test/newtest"));


        //GameObject obj = AssetBundleLoadMgr.Instance.LoadAsset<GameObject>("AB_Package/test", "GameObject");
        //GameObject.Instantiate(obj);

        
        //Sprite sprite = AssetBundleLoadMgr.Instance.LoadAsset<Sprite>("AB_Package/test1", "TestSprite1");
        //GameObject obj = new GameObject();
        //sr = obj.AddComponent<SpriteRenderer>();
        //sr.sprite = sprite;

        AssetBundleLoadMgr.Instance.LoadAssetAsync<Sprite>("AB_Package/test1", "TestSprite1",(sprite)=>
        {
            GameObject obj = new GameObject();
            sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
        });


        //GameObject.Instantiate(obj);

        //AssetBundle bundle = AssetBundleLoadMgr.Instance.LoadAB("AB_Package/Sword/test1");
        //Sprite[] sprites = bundle.LoadAllAssets<Sprite>();
        //Debug.Log(sprites.Length);
        //Debug.Log(sprites[1].name);
        //sr.sprite = sprites[1];
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B");
            Sprite sprite = AssetBundleLoadMgr.Instance.LoadAsset<Sprite>("AB_Package/test1", "TestSprite2");
            sr.sprite = sprite;
        }
    }

    private void Depend()
    {
        //加载生成ab包时，自动生成的ab依赖关系的ab包，这个包的文件名，一般和最后一级文件夹路径一样
        AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AB_Package/AB_Package");
        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");//这里的这个名字好像时固定的

        //获取名为test的ab的所有依赖包的名字
        string[] dependencies = manifest.GetAllDependencies("test"); //Pass the name of the bundle you want the dependencies for.

        for (int i = 0; i < dependencies.Length; i++)
        {
            //加载这些ab包
            AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AB_Package/" + dependencies[i]);
        }
    }


    //首先我们用到了System.Uri这个类，这个类可以帮助我们更好的构造uri，特别是在请求本地的file文件时。在请求本地文件时，不同的平台使用的Uri略有不同。
    //如果不使用Uri类时，各个平台所需要的Uri的字符串示例如下：
    //Windows平台 file:///D:/DATA/StreamingAssets/data.json
    //WebGl平台 http://localhost/StreamingAssets/data.json
    //Android平台 jar:file:///data/app/xxx!/assets/data.json
    //使用Uri类之后，就可以自动帮我们拼接好这些Uri的字符串，一定要记得：
    //var uri = new System.Uri(Path.Combine(Application.streamingAssetsPath, "data.json"));
    IEnumerator UnityWebRequestLearn(string path)
    {
        System.Uri uri = new System.Uri(Path.Combine(path));

        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri, 0);
        yield return request.SendWebRequest();

        AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);
        //Sprite sprite = assetBundle.LoadAsset<Sprite>("set_denglu_3 1");
        //GetComponent<SpriteRenderer>().sprite = sprite;

        GameObject obj = assetBundle.LoadAsset<GameObject>("GameObject");
        GameObject.Instantiate(obj);
    }

    public void LoadAssetFirst(string path)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(path);//内存中创建较小的SerializedFile。而后续的AssetBundle.Load需要通过IO从磁盘中获取

        Depend();


        GameObject obj = bundle.LoadAsset<GameObject>("GameObject");
        GameObject.Instantiate(obj);

        //AssetBundle bundle2 = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AB_Test/test");
        //Sprite sprite = bundle2.LoadAsset<Sprite>("TestSprite1");
        //obj.GetComponent<SpriteRenderer>().sprite = sprite;
    }


    //IEnumerator LoadAssetTwo(string path)
    //{
    //    WWW data = new WWW("file://" + path); //完成后会在内存中创建较大的WebStream。因此后续的AssetBundle.LoadAsset可以直接在内存中进行。
    //    yield return data;

    //    Sprite sprite = data.assetBundle.LoadAsset<Sprite>("set_denglu_3 1");
    //    GetComponent<SpriteRenderer>().sprite = sprite;

    //}

    //IEnumerator LoadAssetThird(string path)
    //{
    //    WWW data = new WWW("file://" + path);
    //    yield return data;

    //    AssetBundle bundle = AssetBundle.LoadFromMemory(data.bytes);//在内存中创建较大的webStream（含异步版本）

    //    Sprite sprite = bundle.LoadAsset<Sprite>("set_denglu_3 1");
    //    GetComponent<SpriteRenderer>().sprite = sprite;

    //    bundle.Unload(false);
    //}

    //资源卸载部分的变化不大，使用的仍然是Unload方法。

    //该方法会卸载运行时内存中包含在bundle中的所有资源。

    //当传入的参数为true，则不仅仅内存中的AssetBundle对象包含的资源会被销毁。根据这些资源实例化而来的游戏内的对象也会销毁。

    //当传入的参数为false，则仅仅销毁内存中的AssetBundle对象包含的资源。

    //IEnumerator LoadAssetForth(string path)
    //{

    //    WWW data = WWW.LoadFromCacheOrDownload("file://" + path, new Hash128(1, 1, 1, 1));
    //    yield return data;
    //    Sprite sprite = data.assetBundle.LoadAsset<Sprite>("set_denglu_3 1");
    //    GetComponent<SpriteRenderer>().sprite = sprite;
    //}



    //前者的优势
    //● 后续的Load操作在内存中进行，相比后者的IO操作开销更小；

    //● 不形成缓存文件，而后者则需要额外的磁盘空间存放缓存；

    //● 能通过WWW.texture，WWW.bytes，WWW.audioClip等接口直接加载外部资源，而后者只能用于加载AssetBundle；

    //前者的劣势

    //● 每次加载都涉及到解压操作，而后者在第二次加载时就省去了解压的开销；

    //● 在内存中会有较大的WebStream，而后者在内存中只有通常较小的SerializedFile。（此项为一般情况，但并不绝对，对于序列化信息较多的Prefab，很可能出现SerializedFile比WebStream更大的情况）
}
