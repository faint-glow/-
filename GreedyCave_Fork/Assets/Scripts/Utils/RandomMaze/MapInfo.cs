using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapInfo
{
    public MapInfo(int layerCount,Tilemap tileMap, Tilemap smallMapObjMap,Tilemap shadyMap)
    {
        this.wideth = Random.Range(20, 41);
        this.height = Random.Range(20, 41);

        this.layerCount = layerCount;
        this.tilemap = tileMap;
        this.smallMapObjMap = smallMapObjMap;
        this.shadyMap = shadyMap;

        this.mapInfoName = GetMapInfoName(layerCount);
        this.resource = ResMgr.GetInstance().Load<MapInfoResource>("Map/" + mapInfoName);
        //设置怪物类型
        this.SetEnemyType();
        //设置宝箱类型
        this.SetTreasureBoxType();
        //设置宝箱，罐子，杂物，怪物的数量
        this.SetCount();
        this.SetLibrary();
    }

    public MapInfo(PlayerData playerData)
    {
        this.layerCount = playerData.layerCount;
        this.resource = ResMgr.GetInstance().Load<MapInfoResource>("Map/" + GetMapInfoName(playerData.layerCount));
        this.SetLibrary();
    }
    private string GetMapInfoName(int targetLayer)
    {
        return "MapOne";
    }

    private string mapInfoName;//这是Resources/Map下的地图名字，资源加载需要经过这个名字
    private int layerCount;//层数（这是第几层的洞窟）

    private int wideth;
    private int height;

    //对应物体的数量
    private int treasureBoxCount;
    private int enemyCount;
    private int jarCount;
    private int sundryCount;

    private MapInfoResource resource;
    private Tilemap tilemap;
    private Tilemap smallMapObjMap;
    private Tilemap shadyMap;
    private RewardLibrary rewordLibrary;
    public string MapInfoName
    {
        get { return mapInfoName; }
    }
    public string MapName//这是Resources/Map下的地图名字，资源加载需要经过这个名字
    {
        get { return resource.mapName; }
    }
    public string LoungeSceneName
    {
        get { return resource.loungeSceneName; }
    }
    public string BossSceneName
    {
        get { return resource.bossSceneName; }
    }
    public int LayerCount//层数（这是第几层的洞窟）
    {
        get { return layerCount; }
    }
    public int Wideth
    {
        get { return wideth; }
    }
    public int Height
    {
        get { return height; }
    }
    public int TreasureBoxCount
    {
        get { return treasureBoxCount; }
    }
    public int EnemyCount
    {
        get { return enemyCount; }
    }
    public int JarCount
    {
        get { return jarCount; }
    }
    public int SundryCount
    {
        get { return sundryCount; }
    }

    public LoungeSupply[] _loungeSupplys
    {
        get { return resource.loungeSupply;}
    }

    public Tilemap Tilemap//当前场景的Tilemap地图
    {
        get { return tilemap; }
    }
    public Tilemap SmallMapObjMap //当前场景的Tilemap地图
    {
        get { return smallMapObjMap; }
    }
    public Tilemap ShadyMap //当前场景的Tilemap地图
    {
        get { return shadyMap; }
    }
   
    public RuleTile WallTile
    {
        get { return resource.wallTile; }
    }
    public RuleTile RoadTile
    {
        get { return resource.roadTile; }
    }
    public GameObject JarPrefab
    {
        get { return resource.jarPrefab; }
    }

    public GameObject EntryPrefab
    {
        get { return resource.entryPrefab; }
    }
    
    public GameObject MagicCirclePrefab
    {
        get { return resource.magicCirclePrefab; }
    }

    //怪物分为三种级别
    //1到10层有第一级别和第二级别的怪物
    //11到20层有三种级别的怪物
    //21到30层有第二级别和第三级别的怪物
    //int代表等级
    //注意，这只相当于图鉴，是没有实例化的。每个列表的内容物是对应等级的怪物类型
    private Dictionary<int, List<EnemyInfo>> enemyDic = new Dictionary<int, List<EnemyInfo>>();
    public Dictionary<int, List<EnemyInfo>> EnemyDic
    {
        get { return enemyDic; }
    }
    private Dictionary<string, EnemyInfo> enemyTypeDic = new Dictionary<string, EnemyInfo>();
    public Dictionary<string, EnemyInfo> EnemyTypeDic
    {
        get { return enemyTypeDic; }
    }

    private Dictionary<TreasureBoxType, TreasureBoxObj> treasureBoxPrefabDic = new Dictionary<TreasureBoxType, TreasureBoxObj>();
    public Dictionary<TreasureBoxType, TreasureBoxObj> TreasureBoxPrefabDic
    {
        get { return treasureBoxPrefabDic; }
    }

    private void SetMap()
    {
        tilemap = GameObject.FindWithTag("BaseMap").GetComponent<Tilemap>();
    }

    //设置怪物类型。
    private void SetEnemyType()
    {
        enemyDic[1] = new List<EnemyInfo>(resource.enemyFirst);
        enemyDic[2] = new List<EnemyInfo>(resource.enemySecond);
        enemyDic[3] = new List<EnemyInfo>(resource.enemyThird);

        foreach (var item in enemyDic)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                EnemyInfo info = item.Value[i];
                enemyTypeDic[info.enemyName] = item.Value[i];
            } 
        }
    }

    private void SetTreasureBoxType()
    {
        //TreasureBoxInfo[] treasureBoxs = ResMgr.GetInstance().LoadAll<TreasureBoxInfo>("Map/TreasureBox");
        //for (int i = 0; i < treasureBoxs.Length; i++)
        //{
        //    treasureBoxDic[treasureBoxs[i].treasureBoxType] = treasureBoxs[i];
        //}
        TreasureBoxObj[] treasureBoxs = ResMgr.GetInstance().LoadAll<TreasureBoxObj>("Map/TreasureBox");
        for (int i = 0; i < treasureBoxs.Length; i++)
        {
            if (treasureBoxs[i] == null)
                continue;
            treasureBoxPrefabDic[treasureBoxs[i].treasureBoxType] = treasureBoxs[i];
        }
    }

    //private void SetJarType()
    //{
    //    jarInfo = ResMgr.GetInstance().Load<JarInfo>("Map/Jar/NormalJar");
    //}

    //设置宝箱，罐子，杂物，怪物的数量
    private void SetCount()
    {
        int baseNum = (Wideth * Height) / 100;//目前情况下宝箱数最低为4
        baseNum = Random.Range(baseNum , baseNum + 3);//随机宝箱数
        treasureBoxCount = baseNum < 1 ? 1 : baseNum;//限制宝箱数最低为1。不用Clamp是因为不需要确定最大限定值
        jarCount = TreasureBoxCount + Random.Range(-3, 4);//罐子数一般比宝箱多，-1是为了有可能罐子数更少
        sundryCount = TreasureBoxCount + Random.Range(-3, 4);//杂物数
        enemyCount = TreasureBoxCount + JarCount + SundryCount + Random.Range(3, 11);//敌人数
    }

    private void SetLibrary()
    {
        rewordLibrary = new RewardLibrary("Item/Equip/MapOne");
    }

    public RewardLibrary GetLibrary()
    {
        return rewordLibrary;
    }

}
