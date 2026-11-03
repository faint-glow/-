using UnityEngine;
using UnityEditor;
using System.IO;

public static class AltasSplit
{
    private const string defaultPlatform = "DefaultTexturePlatform";
#if UNITY_IOS
    private static string platform = "iPhone";
#elif UNITY_ANDROID
    private static string platform = "Android";
#else
    private static string platform = "DefaultTexturePlatform";
#endif

    [MenuItem("MenuCommand/Split Atlas", true)]//参数为true是，这个方发的返回值决定了和这个方法的参数一相同且参数二为false的方法，是否可以点击
    static private bool IsProcessAltasToSprites()
    {
        return (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(Selection.activeObject)));//如果选中的物体的路径为空，则不显示按钮（返回false）
    }

    //代码来源：https://blog.csdn.net/yupu56/article/details/108442656

    //MenuItem的详解：https://blog.csdn.net/qq_33337811/article/details/72852342
    [MenuItem("MenuCommand/Split Atlas", false, 11)]//false点击后才会调用
    static void ProcessAltasToSprites()
    {
        //newSpritePath  图集 导出路径   默认为 Assets同级目录下。AltasToSprites文件夹+图片在Assets中的路径
        string newSpritePath = Path.Combine(Application.dataPath, "AltasToSprites");
        UnityEngine.Debug.Log("newSpritePath---->" + newSpritePath);
        newSpritePath = newSpritePath.Replace("\\", "/");

        if (!Directory.Exists(newSpritePath))
            Directory.CreateDirectory(newSpritePath);
        Texture2D[] altases = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);//获取选中的：一个文件夹下（包含所有子文件夹）或者单个文件的全部Texture
        int index = 0;

        if (altases.Length <= 0)//如果选中空文件夹/没有图片的文件夹会无法跳出update，且持续报错
        {
            Debug.Log("选中项不含图片");
            return;
        }

        EditorApplication.update = () =>
        {
            string altasPath = AssetDatabase.GetAssetPath(altases[index]);//获取下标为index的Texture在Assets/下的文件路径
            UnityEngine.Debug.Log(altasPath);

            //显示和更新含有取消按钮的进度条：EditorUtility.DisplayCancelableProgressBar
            bool isCancel = EditorUtility.DisplayCancelableProgressBar("图集切割中~~", altasPath, (float)index / altases.Length);
            try
            {
                //TextureImporter.spritesheet应该就是表示被切割的每个单张图片的数组
                TextureImporter texImp = AssetImporter.GetAtPath(altasPath) as TextureImporter;//获取一张Texture的TextureImporter
                if (texImp.textureType == TextureImporterType.Sprite && texImp.spritesheet.Length > 0)
                    Process(texImp, altases[index], newSpritePath, altasPath);//切割Texture2D
                else
                    UnityEngine.Debug.Log(string.Format("{0}-->格式不对 或者没有需要拆分的资源", altasPath));
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("图集切割错误---->" + altasPath);
                UnityEngine.Debug.LogError(e.Message);
            }
            index++;
            if (isCancel || index >= altases.Length)
            {
                EditorUtility.ClearProgressBar();//清除含有取消按钮的进度条
                EditorApplication.update = null;//释放EditorApplication.update资源
                Debug.Log("图集切割完成----->" + index.ToString());
                index = 0;
                System.Diagnostics.Process.Start(newSpritePath);//使用newSpritePath这个路径打开文件管理
            }
        };
    }


    private static void Process(TextureImporter texImp, Texture2D texture, string rootPath, string altasPath)
    {
        //altasPath = altasPath.Substring(altasPath.IndexOf("/") + 1);//去掉Assets/，取后半部分的路径
        //altasPath = altasPath.Replace(Path.GetExtension(altasPath), "");//Path.GetExtension返回指定路径字符串的扩展名。此操作为去除扩展名。因为要用这个名字作为文件夹的名字
        //string filesDirectory = Path.Combine(rootPath, altasPath);//设置目标路径。为rootPath/altasPath
        altasPath = altasPath.Replace(Path.GetExtension(altasPath), "");//Path.GetExtension返回指定路径字符串的扩展名。此操作为去除扩展名。因为要用这个名字作为文件夹的名字
        string filesDirectory = altasPath;//设置目标路径。为rootPath/altasPath


        bool isRead = texImp.isReadable;//文件是否可读
        bool isRGB32 = texImp.GetAutomaticFormat(platform) == TextureImporterFormat.RGBA32;//GetAutomaticFormat系统为提供的平台选择的格式
        System.Action callBack = null;
        if (!isRead)
        {
            texImp.isReadable = true;//如果文件不可读，就让他可读
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texImp));//保存
            callBack += () => texImp.isReadable = isRead;//结束以后让文件的可读性变回原来的样子。相当于恢复
        }
        if (!isRGB32)
        {
            TextureImporterPlatformSettings textureImporterPlatformSettings = GetOldTextureImporterPlatformSettings(texImp);
            SetTexturePlatformSettings(!isRGB32, texImp, TextureImporterFormat.RGBA32);
            callBack += () => texImp.SetPlatformTextureSettings(textureImporterPlatformSettings);
        }

        if (!isRead || !isRGB32)
            callBack += () => AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texImp));//保存恢复效果

        if (Directory.Exists(filesDirectory))//以当前切割的图片名字为目标文件夹的名字。如果已经存在，就删掉
            Directory.Delete(filesDirectory, true);
        Directory.CreateDirectory(filesDirectory);

        foreach (SpriteMetaData metaData in texImp.spritesheet)//遍历被切割过的每张图片
        {
            Texture2D myimage = new Texture2D((int)metaData.rect.width, (int)metaData.rect.height);//创建一个新的Texture2D
            for (int y = (int)metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++)
            {
                for (int x = (int)metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
                    myimage.SetPixel(x - (int)metaData.rect.x, y - (int)metaData.rect.y, texture.GetPixel(x, y));//为新的Texture2D的每一个像素复制颜色信息
            }
            byte[] pngData = myimage.EncodeToPNG();//转化为PNG格式数据，用于保存
            string fileName = Path.Combine(filesDirectory, metaData.name + ".PNG");//整理保存路径
            fileName = fileName.Replace("\\", "/");
            File.WriteAllBytes(fileName, pngData);//保存图片
        }
        if (callBack != null)
            callBack();//数据恢复，让原本的图片数据和没切割时一样
        callBack = null;//释放委托
    }

    private static void SetTexturePlatformSettings(bool isNeedSet, TextureImporter texImp, TextureImporterFormat textureImporterFormat = TextureImporterFormat.RGBA32)
    {
        if (!isNeedSet) return;//如果时RGB32就直接返回
        UnityEditor.TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings
        {
            name = platform,
            overridden = true,
            maxTextureSize = 2048,
            format = textureImporterFormat,
            compressionQuality = (int)TextureCompressionQuality.Normal,
            allowsAlphaSplitting = false,
            textureCompression = TextureImporterCompression.Compressed,
            crunchedCompression = false
        };
        texImp.SetPlatformTextureSettings(settings);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texImp));
    }


    private static TextureImporterPlatformSettings GetOldTextureImporterPlatformSettings(TextureImporter texImp)
    {
        int maxTextureSize;
        TextureImporterFormat textureFormat;
        bool etc1AlphaSplitEnabled;
        int compressionQuality;

        //返回是否发现平台覆盖
        bool overridden = texImp.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat, out compressionQuality, out etc1AlphaSplitEnabled);
        if (platform != defaultPlatform)//如果不是默认平台
        {
            if (!overridden)//如果没有发生平台覆盖
            {
                platform = defaultPlatform;
                overridden = texImp.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat, out compressionQuality, out etc1AlphaSplitEnabled);
            }
        }
        return new TextureImporterPlatformSettings()
        {
            name = platform,
            overridden = overridden,
            maxTextureSize = 2048,
            format = textureFormat,
            compressionQuality = (int)TextureCompressionQuality.Normal,
            allowsAlphaSplitting = false,
            textureCompression = TextureImporterCompression.Compressed,
            crunchedCompression = false
        };
    }
}
