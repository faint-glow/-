using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class RandomMazeMgr
{
    private enum SetEnemyType
    {
        宝箱守护怪,
        守门员,
        无,
    }
    private MapInfo mapInfo;

    private GameObject mazeObjFather;
    private static Tile shadyTile;
    public static Tile ShadyTile
    {
        get
        {
            if (shadyTile == null)
                shadyTile = ResMgr.GetInstance().Load<Tile>("Map/NormalTile/Black");
            return shadyTile;
        }
    }
    private static Tile roadSmallMapTile;
    public static Tile RoadSmallMapTile
    {
        get
        {
            if (roadSmallMapTile == null)
                roadSmallMapTile = ResMgr.GetInstance().Load<Tile>("Map/NormalTile/Road");
            return roadSmallMapTile;
        }
    }
    private static Tile wallSmallMapTile;
    public static Tile WallSmallMapTile
    {
        get
        {
            if (wallSmallMapTile == null)
                wallSmallMapTile = ResMgr.GetInstance().Load<Tile>("Map/NormalTile/Wall");
            return wallSmallMapTile;
        }
    }

    private MazeGrid[,] mapGrids;
    public MazeGrid[,] MapGrids
    {
        get { return mapGrids; }
    }
    //1.            
    //*   * 
    //* * * 
    List<MazeGrid> cornerOne;//满足特定角落的格子
    //2
    //* * *     * *     * * *.......
    //*         *       *
    //*         *       
    List<MazeGrid> cornerTwo;//满足特定角落的格子
    //3.
    //* * *
    //List<MazeGrid> lineOne;//满足特定线条的格子
    List<MazeGrid> emptyGrid;//剩余可以放置物品的格子
    public int GetLayerCount()
    {
        return mapInfo.LayerCount;
    }
    private int GetWall()
    {
        return FloodFillMaze.GetWall();
    }
    public static int GetRoad()
    {
        return FloodFillMaze.GetRoad();
    }
    public MazeGrid LoadMaze(MapInfo mapInfo, MazeGrid[,] maze)
    {
        if (this.mazeObjFather != null)
            GameObject.Destroy(this.mazeObjFather);
        this.mazeObjFather = new GameObject();
        this.mapInfo = mapInfo;
        this.mapGrids = maze;
        if (this.emptyGrid == null)
            this.emptyGrid = new List<MazeGrid>();
        this.emptyGrid.Clear();

        Vector3Int pos = new Vector3Int();
        for (int i = 0; i < mapGrids.GetLength(0); i++)//mapGrid.GetLength(0)参数为0是宽度。表示横坐标
        {
            for (int j = 0; j < mapGrids.GetLength(1); j++)//mapGrid.GetLength(0)参数为1是高度。表示纵坐标
            {
                pos.Set(i, j, 0);

                MazeGrid grid = mapGrids[i, j];
                Tile tile = ScriptableObject.CreateInstance<Tile>();//设置瓦片
                tile.sprite = MazeGridMgr.GetSprite(grid);
                mapInfo.Tilemap.SetTile(pos, tile);

                if (grid.shadyType == ShadyType.Normal)
                    ClearShady(grid.x, grid.y);//设置黑幕
                else if (grid.shadyType == ShadyType.Shady)
                    SetShady(grid.x, grid.y);//设置黑幕

                if (grid.gridType == GridType.Wall)
                    SetWallTile_SmallMap(grid.x, grid.y);
                else
                    SetRoadTile_SmallMap(grid.x, grid.y);

                if (grid.gridSpecialInfo == null)
                    grid.gridSpecialInfo = "";

                if (grid.GridType == GridType.Road)
                {
                    emptyGrid.Add(grid);
                }
                else if (grid.GridType == GridType.Enemy)
                {
                    if (grid.gridSpecialInfo == null)
                        grid.gridSpecialInfo = "";
                    if (mapInfo.EnemyTypeDic.ContainsKey(grid.gridSpecialInfo))
                    {
                        EnemyInfo enemyInfo = mapInfo.EnemyTypeDic[grid.gridSpecialInfo];
                        SetEnemy(grid, enemyInfo);
                    }
                    else
                    {
                        Debug.Log("出错,可能是同一个地图中的敌人信息中的名字设为相同");
                    }
                }
                else if (grid.GridType == GridType.TreasureBox)
                {
                    TreasureBoxType type = (TreasureBoxType)System.Enum.Parse(typeof(TreasureBoxType), grid.gridSpecialInfo);
                    SetTreasureBox(grid, type);
                }
                else if (grid.GridType == GridType.Jar)
                {
                    SetJar(grid);
                }
                else if (grid.GridType == GridType.Destination)
                {
                    SetDestination(grid);
                }
                else if (grid.GridType == GridType.MagicCircle)
                {
                    SetMagicCircle(grid);
                }
            }
        }
        AStarMgr.Instance.InitMapInfo(mapGrids);//创建寻路地图
        return emptyGrid[0];
    }
    //创建地图，并且返回玩家的初始位置
    public MazeGrid CreateMap(MapInfo mapInfo)
    {
        this.mapInfo = mapInfo;
        MazeGrid startGrid = CreateMazeMap(mapInfo, mapInfo.Wideth, mapInfo.Height);//创建迷宫地图
        AStarMgr.Instance.InitMapInfo(mapGrids);//创建寻路地图
        return startGrid;
    }
    //private void CreateShadyMap(MapInfo mapInfo)
    //{
    //    //if(shadyTilemap != null)
    //    //{
    //    //    if (shadyTilemap.gameObject != null)
    //    //        GameObject.Destroy(shadyTilemap.gameObject);
    //    //}
    //    //Debug.Log("Create");
    //    //GameObject tileMapObj = new GameObject("ShadyTilemap");
    //    //tileMapObj.transform.SetParent(mapInfo.TilemapFather.transform);
    //    //Tilemap shadyTilemap = tileMapObj.AddComponent<Tilemap>();
    //    //TilemapRenderer sr = tileMapObj.AddComponent<TilemapRenderer>();
    //    //sr.sortingOrder = 10;
    //    //this.shadyTilemap = shadyTilemap;
    //}

    private void SetTileMap(Tilemap tilemap, Tile tile, int x, int y, System.Action<MazeGrid> action = null)
    {
        if (mapInfo == null)
            return;
        if (x >= mapGrids.GetLength(0) || x < 0 || y >= mapGrids.GetLength(1) || y < 0)
            return;

        ///mapGrids[x, y].shadyType = ShadyType.Shady;
        tilemap.SetTile(new Vector3Int(x, y, 0), tile);//设置黑幕图片
        if (action != null)
            action.Invoke(mapGrids[x, y]);
    }
    private void ClearTile_Range(int x, int y, int halfRange, System.Action<int, int> clearAction)
    {
        if (clearAction == null)
            return;

        if (mapInfo == null)
            return;
        if (mapGrids == null)
            return;
        if (x >= mapGrids.GetLength(0) || x < 0 || y >= mapGrids.GetLength(1) || y < 0)
            return;
        for (int i = -halfRange; i < halfRange + 1; i++)
        {
            for (int j = -halfRange; j < halfRange + 1; j++)
            {
                int gridX = x + i;
                int gridY = y + j;
                if (gridX >= mapGrids.GetLength(0) || gridX < 0 || gridY >= mapGrids.GetLength(1) || gridY < 0)
                    continue;
                clearAction.Invoke(gridX, gridY);
            }
        }
    }
    private void ClearTilemap(Tilemap tilemap, int x, int y, System.Action<MazeGrid> action = null)
    {
        if (x >= mapGrids.GetLength(0) || x < 0 || y >= mapGrids.GetLength(1) || y < 0)
            return;
        if (tilemap == null)
            return;
        tilemap.SetTile(new Vector3Int(x, y, 0), null);//设置黑幕图片
        if (action != null)
            action.Invoke(mapGrids[x, y]);
    }
    public void SetShady(int x, int y)
    {
        if (mapInfo == null || mapInfo.ShadyMap == null)
            return;
        SetTileMap(mapInfo.ShadyMap, ShadyTile, x, y, (grid) => { grid.shadyType = ShadyType.Shady; });
    }
    public void ClearShady_Range(int x, int y, int halfRange)
    {
        ClearTile_Range(x, y, halfRange, ClearShady);
    }
    public void ClearShady(int x, int y)
    {
        if (mapInfo == null || mapInfo.ShadyMap == null)
            return;
        ClearTilemap(mapInfo.ShadyMap, x, y, (grid) => { grid.shadyType = ShadyType.Normal; });
    }
    public void SetWallTile_SmallMap(int x, int y)
    {
        if (mapInfo == null || mapInfo.SmallMapObjMap == null)
            return;
        SetTileMap(mapInfo.SmallMapObjMap, WallSmallMapTile, x, y);
    }
    public void SetRoadTile_SmallMap(int x, int y)
    {
        if (mapInfo == null || mapInfo.SmallMapObjMap == null)
            return;
        SetTileMap(mapInfo.SmallMapObjMap, RoadSmallMapTile, x, y);
    }

    //public MazeGrid CreateTestMap(MapInfo mapInfo)
    //{
    //    this.mapInfo = mapInfo;
    //    InitialVirable(4, 4);
    //    int[,] map = new int[,]
    //    {
    //        {0,1,1,1 },
    //        { 1,0,0,0},
    //        { 1,0,0,1},
    //        {0,1,1,0},
    //    };
    //    Vector3Int pos = new Vector3Int();
    //    //设置地图的road和wall图片，并且添加到相应的mapGrid和emptyGrid
    //    for (int i = 0; i < 4; i++)
    //    {
    //        for (int j = 0; j < 4; j++)
    //        {
    //            mapGrids[i, j] = new MazeGrid(i, j);
    //            pos.Set(i, j, 0);
    //            //不同算法适合不同的wall和road。所以可以直接用常量代替。但是更换算法，可能会忘记更改，所以用变量
    //            if (map[i, j] == GetWall())
    //            {
    //                mapInfo.TileMap.SetTile(pos, mapInfo.WallTile);
    //                mapGrids[i, j].GridType = GridType.Wall;
    //            }
    //            else
    //            {
    //                mapInfo.TileMap.SetTile(pos, mapInfo.RoadTile);
    //                mapGrids[i, j].GridType = GridType.Road;
    //                emptyGrid.Add(mapGrids[i, j]);//剩余可用的格子
    //            }
    //        }
    //    }
    //    //检查特殊格子，并且添加到相应的列表中
    //    CheckGrid(mapInfo, map);
    //    //本次生成的地图不太行重新生成一次，环形地图，一直环可以一个角落都没，这种地图没有合适的位置生成宝箱和下一层
    //    if (cornerOne.Count <= 0 && cornerTwo.Count <= 0)
    //        return CreateMap(mapInfo);
    //    MazeGrid startGrid = null;// mapGrids[startPos.x, startPos.y];
    //    List<MazeGrid> firstKindGrid = ToolsMgr.GetRandom_ShuffleCards(cornerOne, mapInfo.TreasureBoxCount);
    //    List<MazeGrid> secondKindGrid = ToolsMgr.GetRandom_ShuffleCards(cornerTwo, mapInfo.JarCount);
    //    CheckGround(mapInfo, startGrid, firstKindGrid, GridType.TreasureBox, GridType.Road, GridType.Enemy, 1f); //放置宝箱
    //    CheckGround(mapInfo, startGrid, secondKindGrid, GridType.Jar, GridType.Road, GridType.Enemy, 1f);//放置罐子
    //    List<MazeGrid> nextLayerGrids = GetEmptySpecialGrid(firstKindGrid, secondKindGrid, startGrid);
    //    CheckGround(mapInfo, startGrid, nextLayerGrids, GridType.Destination, GridType.Road, GridType.Enemy, 1);//检测nextLayerGrids类型的格子，设置成下一层入口，如果这个格子周围有路，就把路设置成敌人，比率为1就是一定有敌人
    //    List<MazeGrid> magicCircleGrids = GetEmptySpecialGrid(firstKindGrid, secondKindGrid, startGrid);
    //    CheckGround(mapInfo, startGrid, magicCircleGrids, GridType.MagicCircle, GridType.Road, GridType.Enemy, 1);
    //    startGrid = mapGrids[0, 0];
    //    return startGrid;
    //}

    public MazeGrid CreateMazeMap(MapInfo mapInfo, int wideth, int height)
    {
        InitialVirable(wideth, height);

        Vector2Int startPos;
        int[,] map = FloodFillMaze.CreateMap(wideth, height, out startPos);//C#二维数组前面是列坐标，后面是行坐标,C++与之相反（目前推测）
        Vector3Int pos = new Vector3Int();
        //设置地图的road和wall图片，并且添加到相应的mapGrid和emptyGrid
        for (int i = 0; i < wideth; i++)
        {
            for (int j = 0; j < height; j++)
            {
                mapGrids[i, j] = new MazeGrid(i, j);

                pos.Set(i, j, 0);
                //不同算法适合不同的wall和road。所以可以直接用常量代替。但是更换算法，可能会忘记更改，所以用变量
                if (map[i, j] == GetWall())
                {
                    mapInfo.Tilemap.SetTile(pos, mapInfo.WallTile);
                    SetWallTile_SmallMap(pos.x, pos.y);
                    mapGrids[i, j].GridType = GridType.Wall;
                }
                else
                {
                    mapInfo.Tilemap.SetTile(pos, mapInfo.RoadTile);
                    SetRoadTile_SmallMap(pos.x, pos.y);
                    mapGrids[i, j].GridType = GridType.Road;
                    emptyGrid.Add(mapGrids[i, j]);//剩余可用的格子
                }
                SetShady(i, j);
            }
        }
        MazeGrid startGrid = mapGrids[startPos.x, startPos.y];

        //检查特殊格子，并且添加到相应的列表中
        CheckGrid(mapInfo, map, startGrid);
        //本次生成的地图不太行重新生成一次，环形地图，一直环可以一个角落都没，这种地图没有合适的位置生成宝箱和下一层
        if (cornerOne.Count + cornerTwo.Count < 2)
            return CreateMap(mapInfo);

        PlaceObject(mapInfo, startGrid);
        return startGrid;
    }

    public void InitialVirable(int wideth, int height)
    {
        if (mazeObjFather != null)
            GameObject.Destroy(mazeObjFather);
        mapInfo.Tilemap.ClearAllTiles();
        mazeObjFather = new GameObject();
        mapGrids = new MazeGrid[wideth, height];
        cornerOne = new List<MazeGrid>();
        cornerTwo = new List<MazeGrid>();
        emptyGrid = new List<MazeGrid>();
    }
    //对格子分类
    private void CheckGrid(MapInfo mapInfo, int[,] map, MazeGrid startGrid)
    {
        int wideth = map.GetLength(0);
        int height = map.GetLength(1);

        int wall = GetWall();
        int road = GetRoad();

        Vector3Int pos = new Vector3Int();
        for (int x = 0; x < wideth; x++)
        {
            for (int y = 0; y < height; y++)
            {
                pos.Set(x, y, 0);
                MapGrids[x, y].spriteName = mapInfo.Tilemap.GetSprite(pos).name;
                if (map[x, y] == wall)
                    continue;
                //防止越界，不过由于特殊性，其实直接用上面的墙壁检测就行
                if (x <= 0 || x >= wideth - 1 || y <= 0 || y >= height - 1)
                    continue;
                bool isDiagonal = false;
                int cornerCount = 0;
                if (map[x - 1, y] == wall && map[x, y + 1] == wall)//左上
                {
                    cornerCount++;
                    if (map[x + 1, y - 1] == wall)//如果满足一个角的情况下，对角也有墙，那么这个就不是第二种特殊格子，在这种地方放置宝箱会阻断通路
                        isDiagonal = true;
                }
                if (map[x + 1, y] == wall && map[x, y + 1] == wall)//右上
                {
                    cornerCount++;
                    if (map[x - 1, y - 1] == wall)
                        isDiagonal = true;
                }
                if (map[x - 1, y] == wall && map[x, y - 1] == wall)//左下
                {
                    cornerCount++;
                    if (map[x + 1, y + 1] == wall)
                        isDiagonal = true;
                }
                if (map[x + 1, y] == wall && map[x, y - 1] == wall)//右下
                {
                    cornerCount++;
                    if (map[x - 1, y + 1] == wall)
                        isDiagonal = true;
                }
                if (cornerCount > 2)//这种情况只会出现下在一个格子四周都是墙的情况
                    Debug.Log("这。。这不对吧");
                if (cornerCount == 2 && mapGrids[x, y] != startGrid)
                {
                    cornerOne.Add(mapGrids[x, y]);
                    continue;
                }
                if (cornerCount == 1 && isDiagonal == false && mapGrids[x, y] != startGrid)
                {
                    cornerTwo.Add(mapGrids[x, y]);
                    continue;
                }
            }
        }
    }

    private void PlaceObject(MapInfo mapInfo, MazeGrid startGrid)
    {
        List<MazeGrid> firstKindGrid = ToolsMgr.GetRandom_ShuffleCards(cornerOne, mapInfo.TreasureBoxCount);
        List<MazeGrid> secondKindGrid = ToolsMgr.GetRandom_ShuffleCards(cornerTwo, mapInfo.JarCount);
        CheckGround(mapInfo, startGrid, firstKindGrid, GridType.TreasureBox, GridType.Road, GridType.Enemy, 0.95f); //放置宝箱
        CheckGround(mapInfo, startGrid, secondKindGrid, GridType.Jar, GridType.Road, GridType.Enemy, 0.6f);//放置罐子

        List<MazeGrid> nextLayerGrids = GetEmptySpecialGrid(firstKindGrid, secondKindGrid, startGrid);
        CheckGround(mapInfo, startGrid, nextLayerGrids, GridType.Destination, GridType.Road, GridType.Enemy, 1f);//检测nextLayerGrids类型的格子，设置成下一层入口，如果这个格子周围有路，就把路设置成敌人，比率为1就是一定有敌人

        int count = mapInfo.LayerCount % 10;
        count = count == 0 ? 10 : count;//如果是第10层的倍数则返回值为0，重新设为10 
        float ratio = (count) / 25.0f;
        if (Random.Range(0, 1.0f) < ratio)
        {
            List<MazeGrid> magicCircleGrids = GetEmptySpecialGrid(firstKindGrid, secondKindGrid, startGrid);
            CheckGround(mapInfo, startGrid, magicCircleGrids, GridType.MagicCircle, GridType.Road, GridType.Enemy, 1f);
        }
    }
    //虽然是list，但是只获取一个，用于魔法阵和下一层入口
    private List<MazeGrid> GetEmptySpecialGrid(List<MazeGrid> firstKindGrid, List<MazeGrid> secondKindGrid, MazeGrid startGrid)
    {
        List<MazeGrid> specialGrids = null;
        int num = Random.Range(0, 2);
        if (num == 0)
        {
            if (cornerOne.Count > 0)
                specialGrids = ToolsMgr.GetRandom_ShuffleCards(cornerOne, 1);
            else if (cornerTwo.Count > 0)
                specialGrids = ToolsMgr.GetRandom_ShuffleCards(cornerTwo, 1);
        }
        else
        {
            if (cornerTwo.Count > 0)
                specialGrids = ToolsMgr.GetRandom_ShuffleCards(cornerTwo, 1);
            else if (cornerOne.Count > 0)
                specialGrids = ToolsMgr.GetRandom_ShuffleCards(cornerOne, 1);
        }
        if (specialGrids != null && specialGrids.Count > 0 && specialGrids[0] == startGrid)
            specialGrids = null;

        if (specialGrids == null)//如果角落类型的格子都没了，或者位置是玩家的出生位置，就强占宝箱或者罐子的格子
        {
            specialGrids = new List<MazeGrid>();
            if (num == 0 && firstKindGrid != null && firstKindGrid.Count > 0)
            {
                specialGrids = ToolsMgr.GetRandom_ShuffleCards(firstKindGrid, 1);
                specialGrids.Add(firstKindGrid[0]);
            }
            else if (secondKindGrid != null && secondKindGrid.Count > 0)
            {
                specialGrids = ToolsMgr.GetRandom_ShuffleCards(secondKindGrid, 1);
                specialGrids.Add(secondKindGrid[0]);
            }
        }
        //MazeObj obj = ToolsMgr.CheckPrestEvent<MazeObj>(new Vector3(specialGrids[0].x, specialGrids[0].y, 0));
        //if (obj != null)
        //    GameObject.Destroy(obj.gameObject);
        //specialGrids[0].GridType = GridType.Road;

        ReleaseGrid(specialGrids[0]);
        return specialGrids;
    }

    private void ReleaseGrid(MazeGrid mazeGrid)
    {
        MazeObj obj = ToolsMgr.CheckPrestEvent<MazeObj>(new Vector3(mazeGrid.x, mazeGrid.y, 0));
        if (obj != null)
            GameObject.Destroy(obj.gameObject);

        mazeGrid.GridType = GridType.Road;
    }

    ///// <summary>
    ///// 检查周围的格子，根据情况设置当前格子和周围格子
    ///// </summary>
    ///// <param name="list">需要设置的列表</param>
    ///// <param name="targetType">当前格子应该设置为什么类型</param>
    ///// <param name="checkType">需要检查周围格子的类型</param>
    ///// <param name="setCheckType">检查到周围的格子为对于类型时，应该设置成什么</param>
    ///// <param name="setRatio">对检测到的格子设置类型时的设置的比率范围为0到1</param>
    //private void CheckGround
    //    (
    //    MapInfo mapInfo, MazeGrid startGrid, List<MazeGrid> list, GridType targetType, GridType checkType, GridType setCheckType, float setRatio
    //    )
    //{
    //    MazeGrid grid;
    //    if (list == null)
    //        return;
    //    for (int i = 0; i < list.Count; i++)//遍历随机选出来的适合放置对应物品的格子
    //    {
    //        grid = list[i];

    //        if (grid.GridType != GridType.Road || grid == startGrid)//如果是玩家生成位置或者已经有东西存在，就不用进行此次操作
    //            continue;

    //        bool isBreak = false;
    //        for (int j = -1; j < 2; j++)
    //        {
    //            if (isBreak == true)
    //                break;
    //            for (int k = -1; k < 2; k++)
    //            {
    //                int x = grid.x + j;
    //                int y = grid.y + k;

    //                //如果周围有和自己要设置的类型相同的格子，那就把这个位置设置成敌人，并且不能再将该格子设置目标类型
    //                if (mapGrids[x, y].GridType == targetType)
    //                {
    //                    if (grid != startGrid)//这个位置也不可以是初始位置
    //                        SetObject(mapInfo, grid, GridType.Enemy);
    //                    isBreak = true;
    //                    break;
    //                }
    //                if ((j == 0 || k == 0) && !(j == 0 && k == 0))//四周
    //                {
    //                    if (mapGrids[x, y] == startGrid)//如果周围有玩家的初始位置，那么这个周围位置就不能设置东西
    //                        continue;

    //                    if (mapGrids[x, y].GridType == checkType)//设置守护怪
    //                    {
    //                        float num = Random.Range(0, 1f);
    //                        if (num < setRatio)
    //                        {
    //                            SetEnemyType setEnemyType = SetEnemyType.无;
    //                            if (targetType == GridType.TreasureBox)
    //                                setEnemyType = SetEnemyType.宝箱守护怪;
    //                            else if (targetType == GridType.Destination)
    //                                setEnemyType = SetEnemyType.守门员;
    //                            SetObject(mapInfo, mapGrids[x, y], setCheckType, setEnemyType);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        if (isBreak == false && grid != startGrid)
    //            SetObject(mapInfo, grid, targetType);
    //    }
    //}

    //放置宝箱，罐子，杂物，怪物的

    /// <summary>
    /// 检查周围的格子，根据情况设置当前格子和周围格子
    /// </summary>
    /// <param name="list">需要设置的列表</param>
    /// <param name="targetType">当前格子应该设置为什么类型</param>
    /// <param name="checkType">需要检查周围格子的类型</param>
    /// <param name="setCheckType">检查到周围的格子为对于类型时，应该设置成什么</param>
    /// <param name="setRatio">对检测到的格子设置类型时的设置的比率范围为0到1</param>
    private void CheckGround
        (
        MapInfo mapInfo, MazeGrid startGrid, List<MazeGrid> list, GridType targetType, GridType checkType, GridType setCheckType, float setRatio
        )
    {
        MazeGrid grid;
        if (list == null)
            return;
        for (int i = 0; i < list.Count; i++)//遍历随机选出来的适合放置对应物品的格子
        {
            grid = list[i];
            if (grid.GridType != GridType.Road || grid == startGrid)//如果是玩家生成位置或者已经有东西存在，就不用进行此次操作
                continue;

            for (int j = -1; j < 2; j++)
            {
                for (int k = -1; k < 2; k++)
                {
                    int x = grid.x + j;
                    int y = grid.y + k;

                    //如果周围有障碍物，就把障碍物移除
                    if (MazeGrid.obstacleType.Contains(mapGrids[x, y].GridType))
                    {
                        ReleaseGrid(mapGrids[x, y]);
                    }
                    if ((j == 0 || k == 0) && !(j == 0 && k == 0))//四周
                    {
                        if (mapGrids[x, y] == startGrid)//如果周围有玩家的初始位置，那么这个周围位置就不能设置东西
                            continue;

                        if (mapGrids[x, y].GridType == checkType)//设置守护怪
                        {
                            float num = Random.Range(0, 1f);
                            if (num < setRatio)
                            {
                                SetEnemyType setEnemyType = SetEnemyType.无;
                                if (targetType == GridType.TreasureBox)
                                    setEnemyType = SetEnemyType.宝箱守护怪;
                                else if (targetType == GridType.Destination)
                                    setEnemyType = SetEnemyType.守门员;
                                SetObject(mapInfo, mapGrids[x, y], setCheckType, setEnemyType);
                            }
                        }
                    }
                }
            }
            if (grid != startGrid)
                SetObject(mapInfo, grid, targetType);
        }
    }

    private void SetObject(MapInfo mapInfo, MazeGrid grid, GridType targetGridType, SetEnemyType setEnemyType = SetEnemyType.无)
    {
        Vector3Int vector3 = new Vector3Int(grid.x, grid.y, 0);
        grid.GridType = targetGridType;
        emptyGrid.Remove(grid);//可优化
        if (targetGridType == GridType.Enemy)
        {
            SetEnemyHandler(mapInfo, grid, setEnemyType);
        }
        else if (targetGridType == GridType.TreasureBox)
        {
            SetTreasureBoxHandler(grid);
        }
        else if (targetGridType == GridType.Jar)
        {
            SetJarHandler(grid);
        }
        else if (targetGridType == GridType.Destination)
        {
            SetDestination(grid);
        }
        else if (targetGridType == GridType.MagicCircle)
        {
            SetMagicCircle(grid);
        }
    }

    //设置敌人
    private void SetEnemyHandler(MapInfo mapInfo, MazeGrid grid, SetEnemyType setEnemyType = SetEnemyType.无)
    {
        int level;
        int layerCount = mapInfo.LayerCount % 30;
        layerCount = layerCount == 0 ? 30 : layerCount;//第三十层也应该第二和第三级别的怪物，但是会变成0层。虽然下面的算法不需要这一步，但还是这样

        if (layerCount >= 1 && layerCount <= 10)
            level = Random.Range(1, 11) > 8 ? 2 : 1;//注意Random.Range取下不取上
        else if (layerCount >= 11 && layerCount <= 20)
        {
            level = Random.Range(1, 11);
            if (level <= 7)
                level = 2;
            else if (level > 7 && level <= 9)
                level = 1;
            else
                level = 3;
        }
        else
        {
            level = Random.Range(1, 11) > 8 ? 2 : 3;
        }
        if (!mapInfo.EnemyDic.ContainsKey(level))
        {
            Debug.Log("缺少对应的键");
            return;
        }

        List<EnemyInfo> enemyInfoList = mapInfo.EnemyDic[level];
        if (enemyInfoList.Count <= 0)
        {
            Debug.Log("缺少对应级别的怪物");
            return;
        }
        EnemyInfo enemyInfo = enemyInfoList[Random.Range(0, enemyInfoList.Count)];
        int num = Random.Range(0, 100);
        if (num > 90)
            grid.enemyType = EnemyType.Eltie;
        if (layerCount == 30 && setEnemyType == SetEnemyType.守门员)//上面已经做了取模处理，所有30的整数倍都会变成30
            grid.enemyType = EnemyType.Goalkeeper;//用于同步网络信息

        SetEnemy(grid, enemyInfo);
    }

    //设置宝箱
    private void SetTreasureBoxHandler(MazeGrid grid)
    {
        Dictionary<float, TreasureBoxType> edges = new Dictionary<float, TreasureBoxType>();
        float num = 0;
        System.Array array = System.Enum.GetValues(typeof(TreasureBoxType));
        foreach (KeyValuePair<TreasureBoxType, TreasureBoxObj> item in mapInfo.TreasureBoxPrefabDic)
        {
            num += item.Value.probability * 100;
            edges[num] = item.Key;
        }
        float randomNum = Random.Range(0, 100f);
        TreasureBoxType type = TreasureBoxType.Normal;//预防下面的算法有问题
        //遍历所有临界值，随机数第一次小于临界值时的TreasureBoxType就是对应宝箱
        for (int i = 0; i < edges.Count; i++)
        {
            if (randomNum < edges.ElementAt(i).Key)
            {
                type = edges.ElementAt(i).Value;
                break;
            }
        }
        SetTreasureBox(grid, type);
    }
    private void SetEnemy(MazeGrid grid, EnemyInfo enemyInfo)
    {
        Enemy enemy = new Enemy(grid, mazeObjFather.transform, enemyInfo);//初始化敌人
        enemy.LevelUp(mapInfo.LayerCount, 0, true);
        enemy.SetEnemyType(grid.enemyType);
        grid.gridSpecialInfo = enemyInfo.enemyName;//用于同步网络信息

    }
    private void SetJarHandler(MazeGrid grid)
    {
        SetJar(grid);
    }

    private void SetTreasureBox(MazeGrid grid, TreasureBoxType type)
    {
        if (mapInfo == null || mapInfo.TreasureBoxPrefabDic == null || !mapInfo.TreasureBoxPrefabDic.ContainsKey(type))
            return;
        GameObject.Instantiate(mapInfo.TreasureBoxPrefabDic[type].gameObject, new Vector3(grid.x + 0.5f, grid.y + 0.1f, 0), Quaternion.identity, mazeObjFather.transform);
        grid.gridSpecialInfo = type.ToString();//用于同步网络信息
    }
    private void SetJar(MazeGrid grid)
    {
        if (mapInfo == null || mapInfo.JarPrefab == null)
            return;
        GameObject.Instantiate(mapInfo.JarPrefab, new Vector3(grid.x + 0.5f, grid.y, 0), Quaternion.identity, mazeObjFather.transform);
    }
    private void SetDestination(MazeGrid grid)
    {
        if (mapInfo == null || mapInfo.EntryPrefab == null)
            return;
        GameObject.Instantiate(mapInfo.EntryPrefab, new Vector3(grid.x + 0.5f, grid.y + 0.5f, 0), Quaternion.identity, mazeObjFather.transform);
    }
    //其实这里只生成，没有效果处理，因为魔法阵基础了预设物体脚本，所以魔法阵的效果由预设物体的触发方式触发
    private void SetMagicCircle(MazeGrid grid)
    {
        if (mapInfo == null || mapInfo.MagicCirclePrefab == null)
            return;
        GameObject.Instantiate(mapInfo.MagicCirclePrefab, new Vector3(grid.x + 0.5f, grid.y + 0.5f, 0), Quaternion.identity, mazeObjFather.transform);
    }
}
