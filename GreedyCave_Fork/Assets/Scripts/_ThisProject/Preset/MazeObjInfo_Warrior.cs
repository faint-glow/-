//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//public class MazeObjInfo_Warrior : Warrior, IMazeObjInfoAble
//{
//    protected MazeGrid grid;//敌人对应的迷宫格子
//    protected MazeObj mazeObj;

//    public MazeObjInfo_Warrior(MazeGrid grid, Transform father, GameObject mazeObjPrefabs, IMazeObjInfo_WarriorInfoAble mazeWarriorInfo) 
//        :base(1,mazeWarriorInfo.GetPropertyValue())
//    {
//        if (grid == null || father == null || mazeObjPrefabs == null)
//        {
//            Debug.Log("有脏东西啊");
//            return;
//        }
//        GameObject obj = GameObject.Instantiate(mazeObjPrefabs, new Vector3(grid.x, grid.y, 0) + GetMazeObjOffset(), Quaternion.identity, father);
//        this.grid = grid;
//        this.mazeObj = obj.GetComponent<MazeObj>();
//        if (this.mazeObj == null)
//            this.mazeObj = obj.AddComponent<MazeObj>();
//    }

//    public Vector3 GetMazeObjOffset()
//    {
//        return Vector3.zero;
//    }

//    public virtual bool IsAstarEvent()
//    {
//        return true;
//    }

//    public virtual bool PresetEventTrigger(IPlayerAble player)
//    {
//        return true;
//    }


//    public override MyAnimatorController GetAnimatorController()
//    {
//        throw new System.NotImplementedException();
//    }

//    public override Booty GetBooty()
//    {
//        throw new System.NotImplementedException();
//    }

//    public override int GetDir()
//    {
//        throw new System.NotImplementedException();
//    }

//    public override Dictionary<WearEquipType, List<Equip>> GetEquips()
//    {
//        throw new System.NotImplementedException();
//    }

//    public override string GetName()
//    {
//        throw new System.NotImplementedException();
//    }

//    public override GameObject GetObj()
//    {
//        throw new System.NotImplementedException();
//    }

//    public override Vector3 GetWarriorPos()
//    {
//        throw new System.NotImplementedException();
//    }

//    public Vector3 GetCollOffset()
//    {
//        throw new System.NotImplementedException();
//    }
//}
