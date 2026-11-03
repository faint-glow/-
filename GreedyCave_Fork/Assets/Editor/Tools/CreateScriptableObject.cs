using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateScriptableObject : EditorWindow
{
    [MenuItem("MenuCommand/Window/CreateScriptableObject")]
    public static void ShowWindow()
    {
        //显示现有窗口实例。如果没有，请创建一个。
        EditorWindow.GetWindow(typeof(CreateScriptableObject));
    }

    void OnGUI()
    {
        GUILayout.Label("选中文件夹。使用这个文件夹下的所有图片创建ScriptableObject");
        GUILayout.Label("至于什么类型的，自己打开脚本修改，并且设置初始值");

        if (GUILayout.Button("创建"))
        {
            CreateEquip();
        }

    }

    private void CreateEquip()
    {
        //UnityEditor.AssetDatabase.CreateAsset使用的路径是直接的相对路径，不需要Application.dataPath
        Texture2D[] texArray = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);
        for (int i = 0; i < texArray.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(texArray[i]);
            string directorPath = Path.GetDirectoryName(path);
            path = path.Substring(0, path.LastIndexOf("."));

            string dirctorName = directorPath.Substring(directorPath.LastIndexOf("\\") + 1);

            //设置Equip的初始值
            EquipInfo equip = ScriptableObject.CreateInstance<EquipInfo>();
            string resourcePath = path.Substring(17);//Assets\Resources\:一共17位
            //Sprite sprite = Resources.Load<Sprite>(resourcePath);//目前还不太清楚怎么动态加载非特殊文件夹的Sprite。动态创建的，方法已结束就没了
            Sprite sprite = Sprite.Create(texArray[i], new Rect(0.0f, 0.0f, texArray[i].width, texArray[i].height), new Vector2(0.5f, 0.5f), 100.0f);//Texture2D转化为Sprite
            sprite.name = "NewSprite";
            GameObject obj = new GameObject();
            obj.AddComponent<SpriteRenderer>().sprite = sprite;

            equip.itemSprite = sprite;
            equip.itemName = "New" + dirctorName;
            equip.equipType = (EquipType)System.Enum.Parse(typeof(EquipType), dirctorName);//字符型转换未枚举类型

            //保存到文件中
            path = path + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(equip, path);
            UnityEditor.AssetDatabase.Refresh();
        }

    }
}
