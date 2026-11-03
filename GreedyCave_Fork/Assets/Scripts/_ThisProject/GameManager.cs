using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Linq;
//public class EqualsTest
//{
//    public int abc;

//    public override bool Equals(object obj)
//    {
//        EqualsTest b = obj as EqualsTest;
//        if (b == null)
//            return false;
//        if (abc == b.abc)
//            return true;

//        return false;
//    }
//}
//[System.Serializable]
public class Person
{
    public Person(string s)
    {
        Debug.Log("Person");
        eeee = s;
    }
    public int age;
    public string eeee;
    public GameObject obj;
    public List<Num> nums = new List<Num>();
    public Num num;
}
public class Student : Person
{
    public Student(string s) : base(s)
    {
        Debug.Log("Student");
        studentName = s;
    }
    public string studentName;
}
[System.Serializable]
public class Num
{
    public Num(int num)
    {
        this.num = num;
    }
    public int num;
}

public class SceneName
{
    public static string Start { get { return "Start"; } }
    public static string MazeScene { get { return "MazeScene"; } }
    public static string MainCity { get { return "MainCity"; } }
    public static string Lounge { get { return "Lounge"; } }
    public static string BossRoom_1 { get { return "BossRoom_1"; } }
}
public enum EventCenterEnum
{
    创建玩家,
    切换场景前,
    切换场景后,
    对话,
    升级,
    击败,
}

public class GameManager : UnitySingleton<GameManager>
{
    //public PlayerData playerData;
    private GameObject tileMapFather;
    private RandomMazeMgr randomMazeMgr = new RandomMazeMgr();
    private Tilemap tileMap;
    private Tilemap smallMapObjMap;
    private Tilemap shadyMap;

    private MapInfo mapInfo;
    //private bool IsFirstOpen = true;
    void Start()
    {
        //playerData = PlayerMgr.Instance.GetPlayerDataMgr().LoadPlayerData();
        Application.runInBackground = true;//让程序支持后台运行
        UIManager.GetInstance().ShowPanelAsync<StartPanel>(StartPanel.prefabsPath);
        InputMgr.GetInstance();//激活输入管理器
        NetController.GetInstance();//激活网络管理
        PlayerMsgMgr.GetInstance();//激活网络玩家管理
        SmallMapMgr.GetInstance();//激活小地图管理
        //NetManager.AddMsgListener("MsgSyneMaze", OnMsgSyneMaze);
        PostProcessingMgr.GetInstance();//激活后处理
        UIManager.GetInstance().ShowPanelAsync<SceneNamePanel>(SceneNamePanel.prefabsPath, PanelLayer.Bot);

        EventCenter.GetInstance().AddEventListener<Scene>(EventCenterEnum.切换场景后.ToString(), (scene) =>
        {
            AudioMgr.Instance.PlayBGM(AudioClipPath.GetBGM_SceneName(scene.name));
        });

        UIManager.GetInstance().ShowPanelAsync<PlayerStatePanelMgr>(PlayerStatePanelMgr.prefabsPath, PanelLayer.Bot, null, (panel) =>
        {
            panel.ShowPlayerState(PlayerMgr.Instance);
        });
        UIManager.GetInstance().ShowPanel<TaskPanel>(TaskPanel.prefabsPath, PanelLayer.Mid_Up);
        CameraMgr.GetInstance();

        //UIManager.GetInstance().ShowPanelAsync<TestButtonsPanel>(TestButtonsPanel.prefabsPath,PanelLayer.Max_Up);
    }
    public void ClearRandomMgr()
    {
        randomMazeMgr = new RandomMazeMgr();
    }
    public RandomMazeMgr GetRandomMaze()
    {
        return randomMazeMgr;
    }
    public void ChangeScene(string sceneName, Action beforeChange = null, Action afterChange = null)
    {
        UIManager.GetInstance().ShowPanelAsync<LoadPanel>(LoadPanel.prefabsPath, PanelLayer.Max_Up);
        if (beforeChange != null)
            beforeChange.Invoke();
        GameManager.Instance.BeforeChangeScene();
        SceneMgr.GetInstance().LoadSceneAsyn(sceneName, () =>
        {
            if (afterChange != null)
                afterChange.Invoke();
            GameManager.Instance.AfterChangeScene();

            UIManager.GetInstance().HidePanel(LoadPanel.prefabsPath);
            PostProcessingMgr.GetInstance().StartOpen((vignette) => { vignette.intensity.value = 1; }, null);
            PlayerMgr.Instance.SavePlayerData();
        });
    }
    public void ChangeSceneAfterVignette(string sceneName, Action beforeChange = null, Action afterChange = null)
    {
        PostProcessingMgr.GetInstance().StartClose(() =>
        {
            ChangeScene(sceneName, beforeChange, afterChange);
        });
    }
    private void BeforeChangeScene()
    {
        ClearRandomMgr();
        Scene currentScene = SceneMgr.GetInstance().GetCurrentScene();
        EventCenter.GetInstance().EventTrigger<Scene>(EventCenterEnum.切换场景前.ToString(), currentScene);
    }
    private void AfterChangeScene()
    {
        Scene currentScene = SceneMgr.GetInstance().GetCurrentScene();
        EventCenter.GetInstance().EventTrigger<Scene>(EventCenterEnum.切换场景后.ToString(), currentScene);
        if (currentScene.name == SceneName.MazeScene || currentScene.name == SceneName.Lounge || currentScene.name == SceneName.BossRoom_1)
            UIManager.GetInstance().ShowPanelAsync<PropShortcutPanel>(PropShortcutPanel.prefabsPath, PanelLayer.Mid_Up);
        else
            UIManager.GetInstance().HidePanel(PropShortcutPanel.prefabsPath);

        if (currentScene.name == SceneName.MazeScene)
            AudioMgr.Instance.PlayBGM(AudioClipPath.RandomCave[UnityEngine.Random.Range(0, AudioClipPath.RandomCave.Length)]);

        UIManager.GetInstance().ShowPanelAsync<OperatePanel>(OperatePanel.prefabsPath, PanelLayer.Bot);

        if (Application.platform == RuntimePlatform.Android)
            UIManager.GetInstance().ShowPanelAsync<MoveControlPanel>(MoveControlPanel.prefabsPath, PanelLayer.Bot);

        InitialPlayerPos(PlayerMgr.Instance);
        if (SceneMgr.GetInstance().GetCurrentScene().name == SceneName.MainCity)
        {
            PlayerMgr.Instance.GetWarrior().RecoverState_Handler();
        }
    }
    public void InitialPlayerPos(IPlayerAble player)
    {
        if (SceneController.Instance != null)
        {
            Vector3 startPos = SceneController.Instance.GetStartPos();
            player.SetPlayerPos(startPos);
        }
    }
    public void NextLayer()
    {
        MapInfo mapInfo = GameManager.Instance.GetMapInfo();

        if (mapInfo == null)
        {
            Debug.Log("缺失地图信息");
            return;
        }
        string scenenName = SceneMgr.GetInstance().GetCurrentScene().name;
        if (mapInfo.LayerCount % 30 == 0)
        {
            if (scenenName == SceneName.MazeScene)
                GameManager.Instance.ChangeSceneAfterVignette(mapInfo.BossSceneName);
            else if (scenenName == SceneName.BossRoom_1)
                GameManager.Instance.ChangeSceneAfterVignette(mapInfo.LoungeSceneName);
            else if (scenenName == SceneName.Lounge)
            {
                GameManager.Instance.ChangeSceneAfterVignette(SceneName.MazeScene, null, () =>
                {
                    RefreshMap(mapInfo.LayerCount + 1);
                });
            }
        }
        else if (mapInfo.LayerCount % 10 == 0)
        {
            if (scenenName == SceneName.MazeScene)
                GameManager.Instance.ChangeSceneAfterVignette(mapInfo.LoungeSceneName);
            else if (scenenName == SceneName.Lounge)
            {
                GameManager.Instance.ChangeSceneAfterVignette(SceneName.MazeScene, null, () =>
                {
                    RefreshMap(mapInfo.LayerCount + 1);
                });
            }
        }
        else
        {
            GameManager.Instance.ChangeSceneAfterVignette(SceneName.MazeScene, null, () =>
            {
                RefreshMap(mapInfo.LayerCount + 1);
            });
        }
    }
    public void RefreshTargetMap(int targetCount)
    {
        RefreshMap(targetCount);

    }
    public void RefreshMap(int layer)
    {
        InitialTileMap();
        if (layer > PlayerMgr.Instance.GetPlayerData().maxLayerCount)
            PlayerMgr.Instance.GetPlayerData().maxLayerCount = layer;
        mapInfo = new MapInfo(layer, GetTileMap(),GetSmallMapObjMap() ,GetShadyMap()/*GetTileMap()*/);
        MazeGrid startPos = randomMazeMgr.CreateMap(mapInfo);//创建地图
        PlayerMgr.Instance.SetPlayerPos(new Vector3(startPos.x, startPos.y, 0));//激活玩家，并且设置初始位置
        ClearShady_Range(startPos.x, startPos.y, 3);
    }

    public void ClearShady_Range(int x, int y, int halfRange = 3)
    {
        if (randomMazeMgr == null)
            return;
        randomMazeMgr.ClearShady_Range(x, y, halfRange);
    }
    public void LoadMapInfo(PlayerData playerData)
    {
        mapInfo = new MapInfo(playerData);
    }
    public void LoadMaze(PlayerData playerData)
    {
        InitialTileMap();
        mapInfo = new MapInfo(playerData.layerCount, GetTileMap(), GetSmallMapObjMap(), GetShadyMap());
        if (randomMazeMgr == null)
            randomMazeMgr = new RandomMazeMgr();

        randomMazeMgr.LoadMaze(mapInfo, playerData.mazsGrids);//创建地图
        PlayerMgr.Instance.SetPlayerPos(new Vector3(playerData.pos_x, playerData.pos_y, playerData.pos_z));//激活玩家，并且设置初始位置
    }
    private void InitialTileMap()
    {
        if (this.GetTileMap() != null)
            this.GetTileMap().ClearAllTiles();
        if (this.GetSmallMapObjMap() != null)
            this.GetSmallMapObjMap().ClearAllTiles();
        if (this.GetShadyMap() != null)
            this.GetShadyMap().ClearAllTiles();
    }

    private static Tilemap GetTilemap_Base(ref GameObject father, ref Tilemap targetTilemap, int sortingOrder = 0, string sortingLayerName = "Default", string layerMaskName = "Default", string tilemapName = "NewTilemap")
    {
        if (father == null)
        {
            father = new GameObject("TilemapFather");
            father.AddComponent<Grid>();
        }
        if (targetTilemap == null)
        {
            GameObject tileMapObj = new GameObject(tilemapName);
            tileMapObj.transform.SetParent(father.transform);
            targetTilemap = tileMapObj.AddComponent<Tilemap>();
            TilemapRenderer sr = tileMapObj.AddComponent<TilemapRenderer>();
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;
            targetTilemap.gameObject.layer = LayerMask.NameToLayer(layerMaskName);
        }
        return targetTilemap;
    }
    public Tilemap GetTileMap()
    {
        return GetTilemap_Base(ref tileMapFather, ref tileMap, -1, "Default", "Default", "BaseTilemap");
    }
    public Tilemap GetSmallMapObjMap()
    {
        return GetTilemap_Base(ref tileMapFather, ref smallMapObjMap, 5, "Default",MazeObj.smallMapLayerMaskName, "SmallMapObject");
    }
    public Tilemap GetShadyMap()
    {
        return GetTilemap_Base(ref tileMapFather, ref shadyMap, 1000, "Max", MazeObj.smallMapLayerMaskName, "ShadyTilemap");
    }
    public MapInfo GetMapInfo()
    {
        return mapInfo;
    }
    public void CreateNormalMapInfo(int layerCount)
    {
        InitialTileMap();
        mapInfo = new MapInfo(layerCount, GetTileMap(), GetSmallMapObjMap(), GetShadyMap());
    }
    private void OnMsgSyneMaze(MsgBase msgBase)
    {
        //MsgSyneMaze msg = (MsgSyneMaze)msgBase;
        //if (msg.id == PlayerMgr.Instance.GetPlayerData().ID)
        //    return;
        //if (SceneMgr.GetInstance().GetCurrentScene().name != SceneName.MazeScnene)
        //{
        //    GameManager.Instance.ChangeScene(SceneName.MazeScnene, null, () =>
        //    {
        //        InitialTileMap();
        //        mapInfo = new MapInfo(msg.mapInfoName, msg.layerCount, tileMap, tileMapFather);
        //        MazeGrid startPos = randomMazeMgr.LoadMaze(mapInfo, msg.mazsGrids);//创建地图
        //        foreach (var item in RoomManager.GetInstance().Players)
        //        {
        //            item.Value.SetPlayerPos(new Vector3(startPos.x, startPos.y, 0));
        //        }
        //    });
        //}
        //else
        //{
        //    InitialTileMap();
        //    mapInfo = new MapInfo(msg.mapInfoName, msg.layerCount, tileMap, tileMapFather);
        //    MazeGrid startPos = randomMazeMgr.LoadMaze(mapInfo, msg.mazsGrids);//创建地图
        //    foreach (var item in RoomManager.GetInstance().Players)
        //    {
        //        item.Value.SetPlayerPos(new Vector3(startPos.x, startPos.y, 0));
        //    }
        //}
    }

}

