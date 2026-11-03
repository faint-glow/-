using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodFillMaze
{
    public static int _road = 0;
    public static int _wall = 1;

    private static float obstacleRatio = 0.5f;

    public static int GetRoad()
    {
        return _road;
    }
    public static int GetWall()
    {
        return _wall;
    }

    public static int[,] CreateMap(int wideth, int height,out Vector2Int startGrid)
    {
        int[,] map = new int[wideth, height];
        InitialMap(map);//把边缘弄成墙壁
        startGrid = GenerateMap(map);
        return map;
    }
    private static void InitialMap(int[,] map)
    {
        int rowCount = map.GetLength(0);//相当于x轴
        int columnCount = map.GetLength(1);//相当于y轴

        int allCount = rowCount * columnCount;
        if (rowCount != columnCount)//如果长不等于宽
        {
            //map.GetLength(0)参数为0表示一行有多少列（宽度），参数为1表示一列有多少行（高度）
            for (int i = 0; i < rowCount; i++)//把
            {
                map[i, 0] = _wall;
                map[i, columnCount - 1] = _wall;
            }
            for (int i = 0; i < columnCount; i++)
            {
                map[0, i] = _wall;
                map[rowCount - 1, i] = _wall;
            }
        }
        else
        {
            int N = map.GetLength(0);
            for (int i = 0; i < N; i++)
            {
                map[i, 0] = _wall;
                map[i, N - 1] = _wall;
                map[0, i] = _wall;
                map[N - 1, i] = _wall;
            }
        }

    }


    private static Vector2Int GenerateMap(int[,] map)
    {
        int rowCount = map.GetLength(0);//相当于x轴
        int columnCount = map.GetLength(1);//相当于y轴

        List<Vector2Int> allGrid = new List<Vector2Int>();
        for (int i = 1; i < rowCount-1; i++)
        {
            for (int j = 1; j < columnCount-1; j++)
            {
                allGrid.Add(new Vector2Int(i, j));
            }
        }

        int allCount = rowCount * columnCount - rowCount * 2 - columnCount * 2 + 4;//计算总的格子数，因为我们把最外围排除，所以要多做一些处理
        Vector2Int startGrid = allGrid[Random.Range(0, allGrid.Count)];//获取一个随机位置作为初始点，也就是玩家的生成位置
        List<Vector2Int> obstacleGrid = ToolsMgr.GetRandom_ShuffleCards(allGrid, (int)(obstacleRatio * allGrid.Count));
        int currentObstacleCount = 0;
        
        for (int i = 0; i < obstacleGrid.Count; i++)
        {
            Vector2Int grid = obstacleGrid[i];//先把随机到的位置设置为墙壁，后续判断此位置是否会阻断通路
            map[grid.x, grid.y] = _wall;
            currentObstacleCount++;

            bool isAccess = MapIsFullyAccessible(map, startGrid, allCount, currentObstacleCount);
            if (grid == startGrid || isAccess == false)//Vector系列的类型在==和Equials上全都是比较的内部x,y,z在数值上是否全相等
            {
                map[grid.x, grid.y] = _road;
                currentObstacleCount--;
            }
        }

        return startGrid;
    }

    private static bool MapIsFullyAccessible(int[,]map, Vector2Int startGrid,int allCount ,int currentObstacleCount)
    {
        bool[,] mapFlag = new bool[map.GetLength(0), map.GetLength(1)];//已经检测过的格子
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        queue.Enqueue(startGrid);
        mapFlag[startGrid.x,startGrid.y] = true;//初始位置必定是可以行走的，而且是检测过的
        int accessibleCount = 1;//可以行走的数量，因为初始位置一定是可以行走的，所以初始数量为1
        while (queue.Count > 0)
        {
            Vector2Int currentGrid = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if ((x == 0 || y == 0) == false)//如果是是四角，就不执行，我们只检测上下左右
                        continue;
                    int neighborX = currentGrid.x + x;
                    int neighborY = currentGrid.y + y;

                    if (neighborX < 1 || neighborX >= map.GetLength(0) - 1 || neighborY < 1 || neighborY >= map.GetLength(1) - 1)//超越最边缘的墙壁就返回
                        continue;

                    if (mapFlag[neighborX, neighborY] == true || map[neighborX, neighborY] == _wall)//如果当前检测的格子是已经检测过的或者是墙壁，就跳过
                        continue;
                    mapFlag[neighborX, neighborY] = true;//标记为检查过
                    accessibleCount++;//可行走的格子数+1
                    queue.Enqueue(new Vector2Int(neighborX, neighborY));
                }
            }
        }


        int targetAccessibleCount = allCount - currentObstacleCount;//如果按照当前检测的格子是墙壁，其他路完全通畅，那么就有targetAccessibleCount个可以行走的格子
        return accessibleCount == targetAccessibleCount;//返回我们通过洪水填充计算实际情况下可以行走的格子数accessibleCount是否和完全通畅情况下的格子数targetAccessibleCount相等
    }
}
