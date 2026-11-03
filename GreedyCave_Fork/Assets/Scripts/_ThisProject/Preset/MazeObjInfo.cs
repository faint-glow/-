//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public interface IMazeObjInfoAble
//{
//    public Vector3 GetMazeObjOffset();
//    public Vector3 GetCollOffset();
//    public bool IsAstarEvent();

//    public bool PresetEventTrigger(IPlayerAble player);
//}
//public class MazeObjInfo: IMazeObjInfoAble
//{
//    protected MazeGrid grid;//敌人对应的迷宫格子
//    protected MazeObj mazeObj;

//    public MazeObjInfo(MazeGrid grid,Transform father, GameObject mazeObjPrefabs)
//    {
//        if (grid == null || father == null || mazeObjPrefabs == null)
//        {
//            Debug.Log("有脏东西啊");
//            return;
//        }
//        GameObject obj = GameObject.Instantiate(mazeObjPrefabs, new Vector3(grid.x + 0.5f, grid.y + 0.1f, 0) + GetMazeObjOffset(), Quaternion.identity, father);
//        this.grid = grid;
//        this.mazeObj = obj.GetComponent<MazeObj>();
//        if (this.mazeObj == null)
//            this.mazeObj = obj.AddComponent<MazeObj>();
//        //this.mazeObj.SetMazeObjInfo(this);
//    }
//    public virtual bool IsAstarEvent()
//    {
//        return true;
//    }
//    public virtual Vector3 GetMazeObjOffset()
//    {
//        return Vector3.zero;
//    }
//    public Vector3 GetCollOffset()
//    {
//        return new Vector3(0, 0.4f, 0);
//    }
//    public virtual bool PresetEventTrigger(IPlayerAble player)
//    {
//        return true;
//    }
//}
