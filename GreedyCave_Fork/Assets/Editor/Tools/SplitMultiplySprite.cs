using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SplitMultiplySprite : EditorWindow
{
    string path = "";
    string spriteName = "Sprite";
    float distance = 1.5f;

    Vector2 scrollPos;

    /// <summary>
    /// 已弃用。有了更好的切割算法
    /// </summary>
    //[MenuItem("MenuCommand/Window/SplitMultiplySprite")]
    //public static void ShowWindow()
    //{
    //    //显示现有窗口实例。如果没有，请创建一个。
    //    EditorWindow.GetWindow(typeof(SplitMultiplySprite));
    //}

    //FlexibleSpace  自适应间距
    //GUILayout.FlexibleSpace();

    void OnGUI()
    {
        GUILayout.Label("资源路径是从Resources/开始，不包含Resources/,不需要后缀名。");

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("路径");
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.textField, GUILayout.Width(300), GUILayout.Height(100));
        path = EditorGUILayout.TextArea(path);
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndHorizontal();

        spriteName = EditorGUILayout.TextField("分割图片的名字", spriteName);
        distance = float.Parse(EditorGUILayout.TextField("一定范围内，数值越小，图片越大", distance.ToString()));
        if (GUILayout.Button("切割"))
        {
            Split();
        }

    }

    private void Split()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        SplitSpriteNormal(sprites);
    }

    private void SplitSpriteNormal(Sprite[] sprite)
    {
        GameObject obj = new GameObject();

        GameObject cameraObj = new GameObject();
        Camera camera = cameraObj.AddComponent<Camera>();

        obj.name = "SplitSprite";
        obj.transform.position = new Vector3(-10000, 0, 0);
        SpriteRenderer spriteRender = obj.AddComponent<SpriteRenderer>();

        for (int i = 0; i < sprite.Length; i++)
        {
            spriteRender.sprite = sprite[i];

            string savePath = path.Substring(0, path.LastIndexOf("/")); //Application.dataPath + "/Resources/" + basePath + objName + "_" + i + ".png";//保存图片路径
            savePath = Application.dataPath + "/Resources/" + savePath + "/" + spriteName + "_" + i + ".png";

            SplitPhotograph(camera,obj.transform, savePath);
        }
        DestroyImmediate(obj);
        DestroyImmediate(cameraObj);
    }

    public Texture2D SplitPhotograph(Camera camera,Transform targetTrans, string path)
    {
        camera.transform.position = targetTrans.position + new Vector3(0, 0, -distance);
        Texture2D tex = GetTextureAndSave(camera,new Rect(0, 0, 100, 100), path);
        return tex;
    }
    /// <summary>
    /// 获取图片并保存
    /// </summary>
    /// <param name="rect">图片大小设置</param>
    /// <param name="fileName">保存路径</param>
    /// <returns></returns>
    public Texture2D GetTextureAndSave(Camera camera,Rect rect, string fileName)
    {

        RenderTexture render = new RenderTexture((int)rect.width, (int)rect.height, -1);

        camera.targetTexture = render;
        camera.Render();//开启相机渲染

        RenderTexture.active = render;//激活RenderTexture
        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);

        tex.ReadPixels(rect, 0, 0);//复制像素信息
        tex.Apply();//保存像素信息到tex中

        camera.targetTexture = null;
        RenderTexture.active = null;
        //Object.DestroyImmediate(render);//删除RenderTexture;

        //保存图片到文件夹中
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(fileName, bytes);
        AssetDatabase.Refresh();

        return tex;
    }


    #region 窗体事件调用
    //private void OnProjectChange()
    //{
    //    Debug.Log("当场景改变时调用");
    //}

    //private void OnHierarchyChange()
    //{
    //    Debug.Log("当选择对象属性改变时调用");
    //}

    //void OnGetFocus()
    //{
    //    Debug.Log("当窗口得到焦点时调用");
    //}

    //private void OnLostFocus()
    //{
    //    Debug.Log("当窗口失去焦点时调用");
    //}

    //private void OnSelectionChange()
    //{
    //    Debug.Log("当改变选择不同对象时调用");
    //}

    //private void OnInspectorUpdate()
    //{
    //    Debug.Log("监视面板调用");
    //}

    //private void OnDestroy()
    //{
    //    Debug.Log("当窗口关闭时调用");
    //}

    //private void OnFocus()
    //{
    //    Debug.Log("当窗口获取键盘焦点时调用");
    //}
    #endregion

}
