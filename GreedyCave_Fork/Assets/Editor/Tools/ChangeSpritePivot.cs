using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ChangeSpritePivot : EditorWindow
{
    Vector2 scrollPos;

    ///// <summary>
    ///// 已弃用利用新版切割，已经不需要这个了，但是可以保留，以后可以微改
    ///// </summary>
    //[MenuItem("MenuCommand/ChangeSpritePivot", true)]
    //static private bool IsProcessAltasToSprites()
    //{
    //    return (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(Selection.activeObject)));//如果选中的物体的路径为空，则不显示按钮（返回false）
    //}

    //[MenuItem("MenuCommand/ChangeSpritePivot", false)]
    //public static void ShowWindow()
    //{
    //    //显示现有窗口实例。如果没有，请创建一个。
    //    //EditorWindow.GetWindow(typeof(ChangeSpritePivot));
    //    ChangeSpriteAnchorPoint();
    //}

    public static void ChangeSpriteAnchorPoint()
    {

        Texture2D[] texArray = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);

        //不要第0个，因为那个是All_***
        for (int i = 0; i < texArray.Length; i++)
        {
            string texPath = AssetDatabase.GetAssetPath(texArray[i]);

            TextureImporter ti = AssetImporter.GetAtPath(texPath) as TextureImporter;//路径是从Assets开始
            TextureImporterSettings texSettings = new TextureImporterSettings();
            ti.ReadTextureSettings(texSettings);//把纹理纹理读入TextureImporterSettings
            texSettings.readable = true;//先把可以读写打开
            ti.SetTextureSettings(texSettings);//应用TextureImporterSettings的修改
            ti.SaveAndReimport();//如果资源被标记为脏，则保存资源（估计应该是SetTextureSettings这个步骤里面进行了标记）
            AssetDatabase.Refresh();

            if (texSettings.spriteMode != (int)SpriteImportMode.Single)
                continue;

            Vector2 targetPivot = GetSpritRightDownPos(texArray[i]);

            texSettings.textureType = TextureImporterType.Sprite;
            //texSettings.spriteMode = (int)SpriteImportMode.Single;
            texSettings.spriteAlignment = (int)SpriteAlignment.Custom;
            texSettings.spritePivot = targetPivot;
            ti.SetTextureSettings(texSettings);//应用TextureImporterSettings的修改
            ti.SaveAndReimport();//如果资源被标记为脏，则保存资源（估计应该是SetTextureSettings这个步骤里面进行了标记）
            AssetDatabase.Refresh();
        }
    }



    private static Vector2 GetSpritRightDownPos(Texture2D tex)
    {

        //Texture2D tex = sprite.texture;
        Color[] colors = tex.GetPixels();
        int count = colors.Length;

        float wideth = Mathf.Sqrt((float)count);

        float right = 0;
        float down = wideth;

        int x = 0;//用于描述一位数组的二维坐标的x
        int y = 0;//用于描述一位数组的二维坐标的y

        //注意：GetPixels返回的数组，是从(0,0)左下角开始，向x轴正方向遍历，遍历完x轴，y+1，在继续遍历x轴

        Debug.Log(wideth);
        Debug.Log(Mathf.CeilToInt(wideth));
        for (int i = 0; i < count; i++)
        {
            if (colors[i].a > 0.5)
            {
                if (right < x)
                    right = x;
                if (down > y)
                    down = y;
            }

            x++;
            if (x % Mathf.CeilToInt(wideth) == 0)
            {
                x = 0;
                y++;
            }
        }
        Debug.Log(x + ":"+y);
        return new Vector2(right / wideth + 0.01f, down / wideth);//加0.1f是因为像素位置有偏差这个0.1可以解决
    }
}
