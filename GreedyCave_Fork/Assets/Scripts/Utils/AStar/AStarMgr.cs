//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Tilemaps;

//public class AStarMgr : Singleton<AStarMgr>
//{
//    private int mapW;
//    private int mapH;

//    //地图相关所有格子对象的容器
//    public AStarNode[,] nodes;
//    //开启列表
//    //private List<AStarNode> openList = new List<AStarNode>();
//    private MinQue<AStarNode> openListChange = new MinQue<AStarNode>();
//    //关闭列表
//    private List<AStarNode> closeList = new List<AStarNode>();

//    /// <summary>
//    /// 初始化地图信息
//    /// </summary>
//    /// <param name="w"></param>
//    /// <param name="h"></param>
//    public void InitMapInfo(int w,int h)
//    {
//        //记录宽高
//        this.mapW = w;
//        this.mapH = h;

//        this.nodes = new AStarNode[w, h];

//        for (int i = 0; i < w; i++)
//        {
//            for (int j = 0; j < h; j++)
//            {
//                AStarNode node = new AStarNode(i, j, Random.Range(0, 100) > 20 ? E_Node_Type.Walk : E_Node_Type.Stop);
//                this.nodes[i, j] = node;
//            }
//        }
//    }

//    public void InitMapInfo(int w, int h ,Tilemap wall)
//    {
//        //记录宽高
//        this.mapW = w;
//        this.mapH = h;

//        this.nodes = new AStarNode[w, h];

//        Vector3Int targetPos = new Vector3Int();

//        for (int i = 0; i < w; i++)
//        {
//            for (int j = 0; j < h; j++)
//            {
//                targetPos.Set(i, j, 0);
//                AStarNode node = new AStarNode(i, j, wall.HasTile(targetPos) ? E_Node_Type.Stop: E_Node_Type.Walk);
//                IPresetObjAble obj = ToolsMgr.CheckPrestEvent<IPresetObjAble>(targetPos);
//                if (obj != null)
//                    node.type = E_Node_Type.Event;

//                this.nodes[i, j] = node;
//            }
//        }
//    }

//    public void InitMapInfo(MazeGrid[,]map)
//    {
//        this.mapW = map.GetLength(0);
//        this.mapH = map.GetLength(1);

//        nodes = new AStarNode[mapW, mapH];

//        for (int i = 0; i < mapW; i++)
//        {
//            for (int j = 0; j < mapH; j++)
//            {
//                E_Node_Type type;
//                if (map[i, j].GridType == GridType.Road)
//                    type = E_Node_Type.Walk;
//                else if(map[i, j].GridType == GridType.Wall)
//                    type = E_Node_Type.Stop;
//                else
//                    type = E_Node_Type.Event;

//                AStarNode node = new AStarNode(i, j, type);
//                nodes[i, j] = node;
//            }
//        }
//    }

//    public void SetNodeType(int x, int y, E_Node_Type nodeType)
//    {
//        if (nodes != null && nodes.GetLength(0) > x && nodes.GetLength(1) > y)
//        {
//            nodes[x, y].type = nodeType;
//        }
//    }

//    //public void ReleaseNode(int x ,int y)
//    //{
//    //    if (nodes != null && nodes.GetLength(0) > x && nodes.GetLength(1) > y)
//    //    {
//    //        nodes[x, y].type = E_Node_Type.Walk;
//    //    }
//    //}

//    public Stack<AStarNode> FindPath(Vector2 startPos, Vector2 endPos)
//    {
//        if (
//            startPos.x < 0 || startPos.x >= mapW || startPos.y < 0 || startPos.y >= mapH
//            || endPos.x < 0 || endPos.x >= mapW || endPos.y < 0 || endPos.y >= mapH
//            )
//        {
//            Debug.Log("开始或结束位置在地图外");
//            return null;
//        }
//        //Vector2 startPos和 Vector2 endPos原本应该各除以每个格子的高和宽，变成每个格子的中心点，但是此处默认传进来的就是中心点
//        AStarNode start = nodes[(int)startPos.x, (int)startPos.y];
//        AStarNode end = nodes[(int)endPos.x, (int)endPos.y];

//        if (start.type == E_Node_Type.Stop || end.type == E_Node_Type.Stop)
//        {
//            Debug.Log("开始或结束位置是不可行区域");
//            return null;
//        }

//        //清空上一次寻路的数据
//        closeList.Clear();
//        openListChange.Clear();

//        //把开始点放入关闭列表中
//        start.father = null;
//        start.f = 0;
//        start.g = 0;
//        start.h = 0;
//        closeList.Add(start);

//        while (true)
//        {
//            //FindNearlyNodeToOpenList(start.x - 1, start.y - 1, 1.4f, start, end);
//            FindNearlyNodeToOpenList(start.x, start.y - 1, 1f, start, end);
//            //FindNearlyNodeToOpenList(start.x + 1, start.y - 1, 1.4f, start, end);
//            FindNearlyNodeToOpenList(start.x - 1, start.y, 1f, start, end);
//            FindNearlyNodeToOpenList(start.x + 1, start.y, 1f, start, end);
//            //FindNearlyNodeToOpenList(start.x - 1, start.y + 1, 1.4f, start, end);
//            FindNearlyNodeToOpenList(start.x, start.y + 1, 1f, start, end);
//            //FindNearlyNodeToOpenList(start.x + 1, start.y + 1, 1.4f, start, end);

//            if (openListChange.Count == 0)
//            {
//                Debug.Log("路径为死路，无法到达终点");
//                return null;
//            }

//            openListChange.Peek().isChecked = false;
//            closeList.Add(openListChange.Peek());
//            start = openListChange.Deque();

//            if (start == end)
//            {
//                while (!openListChange.IsEmpty)
//                {
//                    openListChange.Deque().isChecked = false;
//                }

//                Stack<AStarNode> path = new Stack<AStarNode>();
//                path.Push(end);
//                while (end.father != null)
//                {
//                    path.Push(end.father);
//                    end = end.father;
//                }

//                return path;
//            }

//        }
//    }

//    private int SortOpenList(AStarNode a, AStarNode b)
//    {
//        if (a.f > b.f)
//            return 1;
//        else
//            return -1;
//    }

//    /// <summary>
//    /// 把临近的点放入开启列表中
//    /// </summary>
//    /// <param name="x"></param>
//    /// <param name="y"></param>
//    private void FindNearlyNodeToOpenList(int x,int y,float g,AStarNode father,AStarNode end)
//    {
//        if (x < 0 || x >= mapW || y < 0 || y >= mapH)
//            return;

//        AStarNode node = nodes[x, y];
//        if (node.type == E_Node_Type.Event && node != end)//事件类型的格子平时要绕开，但是如果时目标点，就要去触发事件
//            return;

//        if(node == null || node.type == E_Node_Type.Stop || closeList.Contains(node) || node.isChecked)
//            return;

//        //计算f值
//        node.father = father;
//        //我离起点的距离等于我离我父亲的距离加我父亲离起点的距离
//        node.g = father.g + g;
//        node.h = Mathf.Abs(end.x - node.x) + Mathf.Abs(end.y - node.y);
//        node.f = node.g + node.h;

//        node.isChecked = true;

//        openListChange.Enque(node);

//        //openList.Add(node);
//    }

//}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarMgr : Singleton<AStarMgr>
{
    private int mapW;
    private int mapH;

    //地图相关所有格子对象的容器
    public AStarNode[,] nodes;
    //开启列表
    //private List<AStarNode> openList = new List<AStarNode>();
    private MinQue<AStarNode> openListChange = new MinQue<AStarNode>();
    //关闭列表
    private HashSet<AStarNode> closeList = new HashSet<AStarNode>();

    /// <summary>
    /// 初始化地图信息
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    public void InitMapInfo(int w, int h)
    {
        //记录宽高
        this.mapW = w;
        this.mapH = h;

        this.nodes = new AStarNode[w, h];

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                AStarNode node = new AStarNode(i, j, Random.Range(0, 100) > 20 ? E_Node_Type.Walk : E_Node_Type.Stop);
                this.nodes[i, j] = node;
            }
        }
    }

    public void InitMapInfo(int w, int h, Tilemap wall)
    {
        //记录宽高
        this.mapW = w;
        this.mapH = h;

        this.nodes = new AStarNode[w, h];

        Vector3Int targetPos = new Vector3Int();

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                targetPos.Set(i, j, 0);
                AStarNode node = new AStarNode(i, j, wall.HasTile(targetPos) ? E_Node_Type.Stop : E_Node_Type.Walk);
                IPresetObjAble obj = ToolsMgr.CheckPrestEvent<IPresetObjAble>(targetPos);
                if (obj != null)
                    node.type = E_Node_Type.Event;

                this.nodes[i, j] = node;
            }
        }
    }

    public void InitMapInfo(MazeGrid[,] map)
    {
        this.mapW = map.GetLength(0);
        this.mapH = map.GetLength(1);

        nodes = new AStarNode[mapW, mapH];

        for (int i = 0; i < mapW; i++)
        {
            for (int j = 0; j < mapH; j++)
            {
                E_Node_Type type;
                if (map[i, j].GridType == GridType.Road)
                    type = E_Node_Type.Walk;
                else if (map[i, j].GridType == GridType.Wall)
                    type = E_Node_Type.Stop;
                else
                    type = E_Node_Type.Event;

                AStarNode node = new AStarNode(i, j, type);
                nodes[i, j] = node;
            }
        }
    }

    public void SetNodeType(int x, int y, E_Node_Type nodeType)
    {
        if (nodes != null && nodes.GetLength(0) > x && nodes.GetLength(1) > y)
        {
            nodes[x, y].type = nodeType;
        }
    }

    //public void ReleaseNode(int x ,int y)
    //{
    //    if (nodes != null && nodes.GetLength(0) > x && nodes.GetLength(1) > y)
    //    {
    //        nodes[x, y].type = E_Node_Type.Walk;
    //    }
    //}

    public Stack<AStarNode> FindPath(Vector2 startPos, Vector2 endPos)
    {
        if (
            startPos.x < 0 || startPos.x >= mapW || startPos.y < 0 || startPos.y >= mapH
            || endPos.x < 0 || endPos.x >= mapW || endPos.y < 0 || endPos.y >= mapH
            )
        {
            Debug.Log("开始或结束位置在地图外");
            return null;
        }
        //Vector2 startPos和 Vector2 endPos原本应该各除以每个格子的高和宽，变成每个格子的中心点，但是此处默认传进来的就是中心点
        AStarNode start = nodes[(int)startPos.x, (int)startPos.y];
        AStarNode end = nodes[(int)endPos.x, (int)endPos.y];

        if (start.type == E_Node_Type.Stop || end.type == E_Node_Type.Stop)
        {
            Debug.Log("开始或结束位置是不可行区域");
            return null;
        }

        //清空上一次寻路的数据
        closeList.Clear();
        openListChange.Clear();

        //把开始点放入关闭列表中
        start.father = null;
        start.f = 0;
        start.g = 0;
        start.h = 0;
        closeList.Add(start);

        while (true)
        {
            //FindNearlyNodeToOpenList(start.x - 1, start.y - 1, 1.4f, start, end);
            FindNearlyNodeToOpenList(start.x, start.y - 1, 1f, start, end);
            //FindNearlyNodeToOpenList(start.x + 1, start.y - 1, 1.4f, start, end);
            FindNearlyNodeToOpenList(start.x - 1, start.y, 1f, start, end);
            FindNearlyNodeToOpenList(start.x + 1, start.y, 1f, start, end);
            //FindNearlyNodeToOpenList(start.x - 1, start.y + 1, 1.4f, start, end);
            FindNearlyNodeToOpenList(start.x, start.y + 1, 1f, start, end);
            //FindNearlyNodeToOpenList(start.x + 1, start.y + 1, 1.4f, start, end);

            if (openListChange.Count == 0)
            {
                Debug.Log("路径为死路，无法到达终点");
                return null;
            }

            openListChange.Peek().isChecked = false;
            closeList.Add(openListChange.Peek());
            start = openListChange.Deque();

            if (start == end)
            {
                while (!openListChange.IsEmpty)
                {
                    openListChange.Deque().isChecked = false;
                }

                Stack<AStarNode> path = new Stack<AStarNode>();
                path.Push(end);
                while (end.father != null)
                {
                    path.Push(end.father);
                    end = end.father;
                }

                return path;
            }

        }
    }

    private int SortOpenList(AStarNode a, AStarNode b)
    {
        if (a.f > b.f)
            return 1;
        else
            return -1;
    }

    /// <summary>
    /// 把临近的点放入开启列表中
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void FindNearlyNodeToOpenList(int x, int y, float g, AStarNode father, AStarNode end)
    {
        if (x < 0 || x >= mapW || y < 0 || y >= mapH)
            return;

        AStarNode node = nodes[x, y];
        if (node.type == E_Node_Type.Event && node != end)//事件类型的格子平时要绕开，但是如果时目标点，就要去触发事件
            return;

        if (node == null || node.type == E_Node_Type.Stop || closeList.Contains(node) || node.isChecked)
            return;

        //计算f值
        node.father = father;
        //我离起点的距离等于我离我父亲的距离加我父亲离起点的距离
        node.g = father.g + g;
        node.h = Mathf.Abs(end.x - node.x) + Mathf.Abs(end.y - node.y);
        node.f = node.g + node.h;

        node.isChecked = true;

        openListChange.Enque(node);

        //openList.Add(node);
    }

}