using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ResolveSameName : EditorWindow
{
    ////**********************自建window显示类变量****************************
    //public GameObject targetGameObj;//要显示的物体必须是public
    //private SerializedObject serObj;
    //private SerializedProperty showObj;

    //[MenuItem("MenuCommand/Window/ResolveSameName",true)]
    //static private bool IsProcessAltasToSprites()
    //{
    //    return (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(Selection.activeObject)));//如果选中的物体的路径为空，则不显示按钮（返回false）
    //}


    [MenuItem("MenuCommand/Window/ResolveSameName", false)]
    //解决预制体下重名问题，这是直接对预制体修改，可以永久保存
    public static void Resolve()
    {
        //string path = GetPath(targetGameObj);
        //GameObject root = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);

        GameObject[] roots = Selection.GetFiltered<GameObject>(SelectionMode.DeepAssets);

        //撤销操作Undo：https://blog.csdn.net/qq_36622009/article/details/115615387
        //public static void RecordObject(Object objectToUndo, string name);
        //重要信息：要在 objectToUndo 为预制件实例的情况下正确处理实例
        //必须在 RecordObject 之后调用 PrefabUtility.RecordPrefabInstancePropertyModifications。


        for (int i = 0; i < roots.Length; i++)
        {
            Find(roots[i].transform);

            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(roots[i]))//Assets的资源本体
                UnityEditor.PrefabUtility.SavePrefabAsset(roots[i]);//将未保存的数据写入磁盘

            if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(roots[i]))// 实例化后的资源
                PrefabUtility.ApplyPrefabInstance(roots[i], InteractionMode.UserAction);//对应于ApplyAll按钮。第二个参数是交互模式，这个交互模式可以让我们撤销此操作
        }

        UnityEditor.AssetDatabase.Refresh();//刷新Assets目录
    }


    private string GetPath(GameObject gameObject)
    {
        //只有在Asset文件夹中的Prefabs本体可以成功
        // Project中的Prefab是Asset不是Instance
        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject))
        {
            // 预制体资源就是自身
            return UnityEditor.AssetDatabase.GetAssetPath(gameObject);
        }

        //场景中实例化过的Prefab可以成功，不过此时就算是Asset文件夹中的Prefabs本体也可以成功
        // Scene中的Prefab Instance是Instance不是Asset
        if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            // 获取预制体资源
            var prefabAsset = UnityEditor.PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
            return UnityEditor.AssetDatabase.GetAssetPath(prefabAsset);
        }

        #region 第三种情况
        ////不是上面两种的情况（目前不知道这是什么情况）
        //// PrefabMode中的GameObject既不是Instance也不是Asset
        //var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
        //if (prefabStage != null)
        //{
        //    // 预制体资源：prefabAsset = prefabStage.prefabContentsRoot
        //    return prefabStage.prefabAssetPath;
        //}
        #endregion
        return "";

    }

    private static void Find(Transform father)
    {
        if (father.childCount == 0)
            return;//string虽然是引用传递，但是效果跟值传递一样，所以不需要对path处理即可直接返回

        HashSet<string> childNames = new HashSet<string>();
        for (int i = 0; i < father.childCount; i++)
        {
            Transform objTrans = father.GetChild(i);

            string name = objTrans.gameObject.name;


            int count1 = System.Text.RegularExpressions.Regex.Matches(name, "[(]").Count;
            int count2 = System.Text.RegularExpressions.Regex.Matches(name, "[ ]").Count;

            //对于本身有括号的特殊处理。因为人为操作，不知道()括号会有多少个
            if (!(name.Contains("(TMP)") && count1 < 2))
            {
                int index = name.LastIndexOf("(");
                if (index != -1)
                    name = name.Substring(0, index);
            }

            if (!(name.Contains("(TMP)") && count2 < 2))
            {
                int index = name.LastIndexOf(" ");
                if (index != -1)
                    name = name.Substring(0, index);
            }

            if (childNames.Contains(name))
            {
                int num = 1;
                while (childNames.Contains(name + " " + "(" + num + ")"))
                {
                    num++;
                }
                name = name + " " + "(" + num + ")";
            }
            objTrans.gameObject.name = name;
            childNames.Add(name);

            Find(objTrans);
        }

    }



}
