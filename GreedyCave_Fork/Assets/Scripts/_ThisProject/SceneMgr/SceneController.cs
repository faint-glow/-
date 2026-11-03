using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public interface ISavePresetObjAble
{
    public bool IsSaveOnChangeScene();
    public string GetSaveTag();
    public string GetSaveContentInfo();
    public void ClearRuntimeData();
    public void LoadSaveInfo(string content);
}
public class SceneController : MonoBehaviour
{
    private static SceneController instance;
    public static SceneController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(SceneController)) as SceneController;
            return instance;//如果场景没有预设好SceneController，那么这是可以返回null的，比如迷宫地图就会返回null
        }
    }
    [SerializeField] private string sceneName = "未命名，请在该场景中寻找SceneController修改";
    private Transform startPos;
    private Tilemap wallTileMap;
    private Tilemap roadTileMap;

    private List<ISavePresetObjAble> saveAbleList;//随玩家保存游戏时保存
    private List<ISavePresetObjAble> saveAbleList_ChangeScene;//切换场景时自动保存

    private Dictionary<string, string> specialObjSaveInfo = new Dictionary<string, string>();//已经被销毁的物体需要在被销毁前自行调用方法保存信息
    private void Awake()
    {
        if (instance == null)//不用UnitySingleton是因为那个DontDestroyOnLoad，此处不需要
            instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
        gameObject.tag = "SceneController";
        startPos = transform.Find("StartPos");
        Transform grid = transform.Find("Grid");
        Transform wall = grid.Find("Wall");
        Transform road = grid.Find("Road");

        wallTileMap = wall.GetComponent<Tilemap>();
        roadTileMap = road.GetComponent<Tilemap>();

        roadTileMap.CompressBounds();//Tilemap的边界不知道为什么会扩大一些没用的地方，压缩哪些地方可以获得正确的边界
        wallTileMap.CompressBounds();//

        //应当使用范围最大的Tilemap来生成寻路地图
        AStarMgr.Instance.InitMapInfo(wallTileMap.cellBounds.size.x, wallTileMap.cellBounds.size.y, wallTileMap);

        SaveInfoCheckOrInitial();
    }
    private void SaveInfoCheckOrInitial()
    {
        if (saveAbleList == null || saveAbleList_ChangeScene == null)
        {
            saveAbleList_ChangeScene = new List<ISavePresetObjAble>();
            saveAbleList = GetAllChildComponent<ISavePresetObjAble>(transform, (saveAble) =>
            {
                if (saveAble.IsSaveOnChangeScene() == true)
                    saveAbleList_ChangeScene.Add(saveAble);
            });
        }
    }
    public static List<T> GetAllChildComponent<T>(Transform trans, Action<T> action = null)
    {
        List<T> components = new List<T>();
        ForeachAllChild(trans, (childTrans) =>
         {
             T component = childTrans.GetComponent<T>();
             if (component != null)
             {
                 components.Add(component);
                 if (action != null)
                     action.Invoke(component);
             }
         });
        return components;
    }
    public string GetSceneName()
    {
        return sceneName;
    }
    public Vector3 GetStartPos()
    {
        return new Vector3((int)startPos.transform.position.x, (int)startPos.transform.position.y, 0);
    }
    public Tilemap GetRoadTilemap()
    {
        return roadTileMap;
    }
    public void SetAstarEventGrid(int x, int y, E_Node_Type nodeType)
    {
        AStarMgr.Instance.SetNodeType(x, y, nodeType);
    }
    public static Dictionary<string, string> GetSaveInfoDic_Base(List<ISavePresetObjAble> saveAbleList)
    {
        Dictionary<string, string> saveInfo = new Dictionary<string, string>();
        if (saveAbleList == null)
            return saveInfo;
        for (int i = 0; i < saveAbleList.Count; i++)
        {
            if (saveAbleList[i] == null || saveAbleList[i].GetSaveTag() == "")
                continue;
            saveInfo[saveAbleList[i].GetSaveTag()] = saveAbleList[i].GetSaveContentInfo();
        }
        return saveInfo;
    }
    public void ClearRunTimeData()
    {
        SaveInfoCheckOrInitial();
        for (int i = 0; i < saveAbleList.Count; i++)
        {
            saveAbleList[i].ClearRuntimeData();
        }
    }
    public static void LoadSaveData_Base(Dictionary<string, string> saveInfo, List<ISavePresetObjAble> saveAbleList)
    {
        if (saveInfo == null || saveInfo.Count <= 0)
            return;

        for (int i = 0; i < saveAbleList.Count; i++)
        {
            string content = saveAbleList[i].GetSaveTag();
            if (saveInfo.ContainsKey(content))
                saveAbleList[i].LoadSaveInfo(saveInfo[saveAbleList[i].GetSaveTag()]);
        }
    }
    public Dictionary<string, string> GetSaveInfoDic()
    {
        SaveInfoCheckOrInitial();
        return GetSaveInfoDic_Base(saveAbleList);
    }
    public void LoadSaveData(Dictionary<string, string> saveInfo)
    {
        SaveInfoCheckOrInitial();
        LoadSaveData_Base(saveInfo, saveAbleList);
    }
    public Dictionary<string, string> GetSaveInfoDic_ChangeScene()
    {
        SaveInfoCheckOrInitial();
        return GetSaveInfoDic_Base(saveAbleList_ChangeScene);
    }
    public void LoadSaveData_ChangeScene(Dictionary<string, string> saveInfo)
    {
        SaveInfoCheckOrInitial();
        LoadSaveData_Base(saveInfo, saveAbleList_ChangeScene);
    }
    public void SaveDestoryObjInfo(ISavePresetObjAble saveAble)
    {
        if (saveAble == null)
            return;
        specialObjSaveInfo[saveAble.GetSaveTag()] = saveAble.GetSaveContentInfo();
        PlayerMgr.Instance.SavePlayerData(false);
    }
    public void LoadSaveDestoryObjInfo(ISavePresetObjAble saveAble)
    {
        if (specialObjSaveInfo.ContainsKey(saveAble.GetSaveTag()))
            saveAble.LoadSaveInfo(specialObjSaveInfo[saveAble.GetSaveTag()]);
    }
    public static void ForeachAllChild(Transform trans, Action<Transform> action)
    {
        if (trans == null || action == null)
            return;
        for (int i = 0; i < trans.childCount; i++)
        {
            Transform child = trans.GetChild(i);
            action.Invoke(child);
            if (child.childCount > 0)
            {
                ForeachAllChild(child, action);
            }
        }
    }
}
