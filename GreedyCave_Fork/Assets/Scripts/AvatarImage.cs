using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class AvatarImage
{
    public static Vector3 cameraPos = new Vector3(1000, 0, 0);
    private static Camera staticCamera;
    private static Camera StaticCamera
    {
        get
        {
            if (staticCamera == null)
            {
                GameObject obj = new GameObject();
                obj.name = "StaticImageGetCamera";
                staticCamera = obj.AddComponent<Camera>();
                staticCamera.depth = -100;

                staticCamera.transform.position = cameraPos;
                cameraPos.y -= 10;
            }
            return staticCamera;
        }
        set { staticCamera = value; }
    }

    private Camera dynamicCamera;
    private Camera DynamicCamera
    {
        get
        {
            if (dynamicCamera == null)
            {
                GameObject obj = new GameObject();
                obj.name = "DynamicImageGetCamera";
                dynamicCamera = obj.AddComponent<Camera>();
                dynamicCamera.depth = -100;

                dynamicCamera.transform.position = cameraPos;
                cameraPos.y -= 10;
            }
            return dynamicCamera;
        }
        set { dynamicCamera = value; }
    }
    private GameObject cloneObj;
    private RenderTexture dynaimcTex;
    public GameObject GetCloneObj()
    {
        return cloneObj;
    }
    /// <returns></returns>
    public static Texture2D GetStaticTex(GameObject targetObj,Vector3 offsetPos, int wideth = 512, int height = 512)
    {
        StaticCamera.gameObject.SetActive(true);

        GameObject obj = GameObject.Instantiate(targetObj);
        Vector3 newScale = obj.transform.localScale;//控制朝向
        newScale.x = Mathf.Abs(newScale.x);
        obj.transform.localScale = newScale;

        obj.transform.position = StaticCamera.transform.position - offsetPos;
        Texture2D tex = GetTexture(new Rect(0, 0, wideth, height));
        obj.transform.position += new Vector3(0, -10, 0);//据推测Destroy这个方法是协程/线程做的。在一帧内多次使用该方法，上一个物体还没删除，就进行拍照，就会把上一个物体拍进去
        GameObject.Destroy(obj);
        StaticCamera.gameObject.SetActive(false);
        return tex;
    }

    private void SetCloneObj(GameObject targetObj, Vector3 offsetPos)
    {
        GameObject newObj = GameObject.Instantiate(targetObj);
        if (cloneObj != null)
            GameObject.Destroy(cloneObj);

        this.cloneObj = newObj;
        Vector3 newScale = cloneObj.transform.localScale;//控制朝向
        newScale.x = Mathf.Abs(newScale.x);
        cloneObj.transform.localScale = newScale;
        cloneObj.transform.position = DynamicCamera.transform.position - offsetPos;
    }

    public RenderTexture GetDynamicTex(GameObject targetObj, Vector3 offsetPos, int wideth = 512, int height = 512)
    {
        if (dynaimcTex == null)
            dynaimcTex = new RenderTexture(wideth, height, -1);

        SetCloneObj(targetObj, offsetPos);
        cloneObj.SetActive(true);//让当前拍摄物体显示
        DynamicCamera.gameObject.SetActive(true);

        DynamicCamera.targetTexture = dynaimcTex;
        return dynaimcTex;
    }

    public void UsingEnd()
    {
        if (cloneObj != null)
            GameObject.Destroy(cloneObj.gameObject);
        if (DynamicCamera != null)
        {
            GameObject.Destroy(DynamicCamera.gameObject);
            DynamicCamera = null;
        }
    }

    /// <summary>
    /// 获取图片
    /// </summary>
    /// <param name="rect">图片大小设置</param>
    /// <returns></returns>
    private static Texture2D GetTexture(Rect rect,Camera targetCamera = null)
    {
        RenderTexture render = new RenderTexture((int)rect.width, (int)rect.height, -1);

        if (targetCamera == null)
            targetCamera = staticCamera;
        targetCamera.targetTexture = render;
        targetCamera.Render();//开启相机渲染

        RenderTexture.active = render;//激活RenderTexture
        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);

        tex.ReadPixels(rect, 0, 0);//复制像素信息,复制 RenderTexture.active的信息到Texture2D中
        tex.Apply();//保存像素信息到tex中

        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Object.Destroy(render);//删除RenderTexture;
        
        return tex;
    }

    /// <summary>
    /// 获取图片并保存
    /// </summary>
    /// <param name="rect">图片大小设置</param>
    /// <param name="fileName">保存路径</param>
    /// <returns></returns>
    private Texture2D GetTextureAndSave(Rect rect, string fileName)
    {
        RenderTexture render = new RenderTexture((int)rect.width, (int)rect.height, -1);

        DynamicCamera.targetTexture = render;
        DynamicCamera.Render();//开启相机渲染

        RenderTexture.active = render;//激活RenderTexture
        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);

        tex.ReadPixels(rect, 0, 0);//复制像素信息
        tex.Apply();//保存像素信息到tex中

        DynamicCamera.targetTexture = null;
        RenderTexture.active = null;
        Object.Destroy(render);//删除RenderTexture;

        //保存图片到文件夹中
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(fileName, bytes);

        return tex;
    }

}
