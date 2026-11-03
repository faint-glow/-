using System.Collections.Generic;
using UnityEngine;

public enum GridType
{
    Wall,
    Road,
    TreasureBox,
    Jar,
    Sundry,
    Enemy,
    Destination,
    MagicCircle,
}

public enum ShadyType
{
    Normal,
    Shady,
}

[System.Serializable]
public class MazeGrid
{
    public MazeGrid(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int x;//格子的x坐标
    public int y;//格子的y坐标

    public static HashSet<GridType> obstacleType = new HashSet<GridType>()
    {
        GridType.TreasureBox,
        GridType.Jar,
        GridType.Sundry,
        GridType.Destination,
        GridType.MagicCircle,
    };

    public GridType GridType
    {
        get { return gridType; }
        set { gridType = value; }
    }
    public GridType gridType = GridType.Road;

    public EnemyType enemyType = EnemyType.Normal;
    public ShadyType shadyType = ShadyType.Shady;
    public string gridSpecialInfo;//用于网络同步。如果是宝箱，则此为宝箱类型的名字。如果是敌人，则是敌人的名字,罐子则是罐子的开启状态
    public string spriteName;//格子的图片的名字，用于网络同步
}

public class MazeGridMgr
{
    private static string spriteTypeName;//私有变量和静态变量(两个分别都无法被保存)无法被json保存，但是不会报错
    private static Sprite[] sprites;//用于保存一组图片，在进行大地图更换之前，不会一直去内从中加载资源
    public static void SetGridSpecialInfo(MazeGrid grid, string info)
    {
        if (grid == null)
            return;
        grid.gridSpecialInfo = info;
    }
    public static void SetGridSpecialInfo(Vector3 pos,string info)
    {
        SetGridSpecialInfo(GetMazeGird(pos), info);
    }

    public static void ReleaseGrid(MazeGrid grid)
    {
        if (grid == null)
            return;
        grid.GridType = GridType.Road;
        grid.gridSpecialInfo = "";
        AStarMgr.Instance.SetNodeType(grid.x, grid.y,E_Node_Type.Walk);
    }
    public static void ReleaseGrid(Vector3 pos)
    {
        //MazeGrid[,] grids = GameManager.Instance.GetRandomMaze().MapGrids;
        //int x = (int)pos.x;
        //int y = (int)pos.y;
        //if (x >= grids.GetLength(0) || y >= grids.GetLength(1))
        //    return;
        MazeGrid grid = GetMazeGird(pos);
        if(grid == null)
            AStarMgr.Instance.SetNodeType((int)pos.x, (int)pos.y, E_Node_Type.Walk);
        else
            ReleaseGrid(grid);
    }
    public static MazeGrid GetMazeGird(Vector3 pos)
    {
        MazeGrid[,] grids = GameManager.Instance.GetRandomMaze().MapGrids;
        if (grids == null)
            return null;
        int x = (int)pos.x;
        int y = (int)pos.y;
        if (x >= grids.GetLength(0) || y >= grids.GetLength(1))
            return null;
        return grids[x, y];
    }

    //获取Grid中的图片，主要用于和其他主机同步地图图片，实际上不同步图片也不会影响游戏
    public static Sprite GetSprite(MazeGrid grid)
    {
        int num = grid.spriteName.LastIndexOf("_");
        string firstName = grid.spriteName.Substring(0, num);
        string secondName = grid.spriteName.Substring(num + 1);

        if (spriteTypeName == firstName)//如果当前sprites的内容和要访问内容是同一组，就可以不用再从文件中加载
            return sprites[int.Parse(secondName)];

        sprites = ResMgr.GetInstance().LoadAll<Sprite>("Map/MapOne/" + firstName);//这里有很多优化和修改，暂时先这样
        spriteTypeName = firstName;
        return sprites[int.Parse(secondName)];
    }
}
