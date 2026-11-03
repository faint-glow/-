using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyDictionary/Map/MapInfo", fileName = "MapInfoResource")]
public class MapInfoResource : ScriptableObject
{
    public string mapName = "未命名";
    public string loungeSceneName;
    public string bossSceneName;
    public RuleTile roadTile;
    public RuleTile wallTile;
    public GameObject jarPrefab;
    public GameObject entryPrefab;
    public GameObject magicCirclePrefab;

    public EnemyInfo[] enemyFirst;//第一级怪物
    public EnemyInfo[] enemySecond;//第二级怪物
    public EnemyInfo[] enemyThird;//第三级怪物

    public LoungeSupply[] loungeSupply;

}

[System.Serializable]
public class LoungeSupply
{
    public ItemInfo itemInfo;
    public int count;
}
