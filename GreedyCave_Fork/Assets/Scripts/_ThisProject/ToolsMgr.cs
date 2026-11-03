using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ToolsMgr
{
    public static T GetRandomTarget<T>(List<T> list)
    {
        if (list == null || list.Count <= 0)
            return default(T);
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    /// <summary>
    /// 获取不重复随机值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">目标List</param>
    /// <param name="targetCount">需要多少个随机值</param>
    /// <param name="isShuffle">是否允许打乱传入的list内容顺序</param>
    /// <returns></returns>
    public static List<T> GetRandom_ShuffleCards<T>(List<T> list, int targetCount,bool isShuffle = true)
    {
        //if (list == null || list.Count <= 0)
        //    return default(List<T>);
        //List<T> targetList = new List<T>();
        //List<T> useList = new List<T>();
        //if (isShuffle == false)//如果不洗牌的话，创建副本，不能打乱原来的顺序
        //    useList = new List<T>(list);//复制一个副本，为了不影响原来的数据结构
        //else
        //    useList = list;

        //int count = Mathf.Min(useList.Count, targetCount);
        //for (int i = 0; i < count; i++)//获取不重复随机值，并且将其放在targetList里面
        //{
        //    int allCount = useList.Count;
        //    int num = UnityEngine.Random.Range(0, allCount);
        //    Swap<T>(useList, num, allCount - 1);
        //    targetList.Add(useList[allCount - 1]);
        //    useList.RemoveAt(allCount - 1);
        //}
        //return targetList;


        if (list == null || list.Count <= 0)
            return default(List<T>);

        List<T> targetList = new List<T>();
        List<T> useList = new List<T>();
        if (isShuffle == false)//如果不洗牌的话，创建副本，不能打乱原来的顺序
            useList = new List<T>(list);//复制一个副本，为了不影响原来的数据结构
        else
            useList = list;

        int count = Mathf.Min(useList.Count, targetCount);
        int randomRange = useList.Count;
        for (int i = 0; i < count; i++)//获取不重复随机值，并且将其放在targetList里面
        {
            int num = UnityEngine.Random.Range(0, randomRange);
            Swap<T>(useList, num, randomRange - 1);
            targetList.Add(useList[randomRange - 1]);
            randomRange--;
        }
        return targetList;
    }


    public static void Swap<T>(List<T> list, int num1, int num2)
    {
        if (num1 > list.Count || num2 > list.Count)
        {
            Debug.Log("索引越界");
            return;
        }
        T temp = list[num1];
        list[num1] = list[num2];
        list[num2] = temp;
    }

    #region 查找一个类的所有字段名
    public static List<string> GetFieldsHandler<T>(T t)
    {
        List<string> listStr = new List<string>();
        GetFields<T>(t, listStr);
        return listStr;
    }
    private static List<string> GetFields<T>(T t, List<string> listStr)
    {
        if (t == null)
        {
            return listStr;
        }
        System.Reflection.FieldInfo[] fields = t.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (fields.Length <= 0)
        {
            return listStr;
        }
        foreach (System.Reflection.FieldInfo item in fields)
        {
            string name = item.Name; //名称
            object value = item.GetValue(t);  //值

            if (item.FieldType.IsValueType || item.FieldType.Name.StartsWith("String"))
            {
                listStr.Add(name);
            }
            else
            {
                GetFields(value, listStr);
            }
        }
        return listStr;
    }
    public List<string> GetPropertiesHandler<T>(T t)
    {
        List<string> listStr = new List<string>();
        GetProperties<T>(t, listStr);
        return listStr;
    }
    private List<string> GetProperties<T>(T t, List<string> listStr)
    {
        if (t == null)
        {
            return listStr;
        }
        System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        if (properties.Length <= 0)
        {
            return listStr;
        }
        foreach (System.Reflection.PropertyInfo item in properties)
        {
            string name = item.Name; //名称
            object value = item.GetValue(t, null);  //值

            if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
            {
                listStr.Add(name);
            }
            else
            {
                GetProperties(value, listStr);
            }
        }
        return listStr;
    }
    #endregion


    public static string white = "#ffffffff";
    public static string green = "#00ff00ff";
    public static string red = "#ff0000ff";
    public static string bleack = "#000000ff";

    public static string GetColorText(string content, string richColor = "#ffffffff")
    {
        return "<color=" + richColor + ">" + content + "</color>";
    }

    public static Vector3Int GetGirdPos(Vector3 pos)
    {
        return new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
    }

    public static T DeepCopyByBinary<T>(T obj)//不能复制不可序列化的对象，比如GameObject
    {
        object retval;
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            retval = bf.Deserialize(ms);
            ms.Close();
        }
        return (T)retval;
    }

    public static T DeepCopyByJson<T>(T obj)//不能复制不可序列化的对象，比如GameObject
    {
        string s = JsonConvert.SerializeObject(obj);
        T copyObj = (T)JsonConvert.DeserializeObject(s, obj.GetType());
        return copyObj;
    }


    //检测Layer
    public static Collider2D CheckLayer(Vector3 targetPos, LayerMask mask)
    {
        targetPos = new Vector3((int)targetPos.x + 0.5f, (int)targetPos.y + 0.5f, 0);
        Collider2D coll = Physics2D.OverlapBox(targetPos, Vector2.one * 0.5f, 0, mask);
        return coll;
    }
    //检测Layer
    public static Collider2D[] CheckLayers(Vector3 targetPos, LayerMask mask)
    {
        targetPos = new Vector3((int)targetPos.x + 0.5f, (int)targetPos.y + 0.5f, 0);
        Collider2D[] colls = Physics2D.OverlapBoxAll(targetPos, Vector2.one * 0.5f, 0, mask);
        return colls;
    }
    //检测目标位置第一个预设事件
    public static T CheckPrestEvent<T>(Vector3 targetPos)
    {
        List<T> presetObjs = CheckPrestEvents<T>(targetPos);
        if (presetObjs != null && presetObjs.Count > 0)
            return presetObjs[0];
        else
            return default(T);
    }
    //检测目标位置的所有预设事件
    public static List<T> CheckPrestEvents<T>(Vector3 targetPos)
    {
        targetPos = new Vector3((int)targetPos.x + 0.5f, (int)targetPos.y + 0.5f, 0);
        Collider2D[] colls = Physics2D.OverlapBoxAll(targetPos, Vector2.one * 0.5f,0);
        List<T> presetObjs = new List<T>();
        if(colls != null)
        {
            for (int i = 0; i < colls.Length; i++)
            {
                T presetObj = colls[i].gameObject.GetComponent<T>();
                if (presetObj != null)
                    presetObjs.Add(presetObj);
            }
        }
        return presetObjs;
    }

    /// <summary>
    /// 加载以paths的所有元素作为路径的T类型资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="paths">路径列表</param>
    /// <param name="func">改资源是否满足加入条件，满足返回true，不满足返回false</param>
    /// <returns></returns>
    public static Dictionary<string, T> LoadAllResourceInfo<T>(List<string> paths,System.Func<T,bool> func = null) where T:Object, INameAble
    {
        Dictionary<string, T> allResourcesDic = new Dictionary<string, T>();

        for (int i = 0; i < paths.Count; i++)
        {
            T[] resources = ResMgr.GetInstance().LoadAll<T>(paths[i]);
            for (int j = 0; j < resources.Length; j++)
            {
                if (func != null && func(resources[i]) == false)
                    continue;
                AddAllItemInfo(allResourcesDic, resources[j]);
            }
        }
        return allResourcesDic;
    }
    private static void AddAllItemInfo<T>(Dictionary<string, T> allResourcesDic, T resources) where T:INameAble
    {
        if (allResourcesDic.ContainsKey(resources.GetName()))
            Debug.Log(resources.GetName() + "物品重名");
        allResourcesDic[resources.GetName()] = resources;
    }

    public static void InvokeAllActions(List<System.Action> actions)
    {
        if (actions == null)
            return;
        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i] != null)
                actions[i].Invoke();
        }
    }
}

public interface INameAble
{
    public string GetName();
}