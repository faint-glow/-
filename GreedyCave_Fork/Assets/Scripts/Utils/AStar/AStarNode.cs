using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum E_Node_Type
{
    //可行区域
    Walk,
    //不可行区域
    Stop,

    Event
}

/// <summary>
/// A星格子类
/// </summary>
public class AStarNode:IComparable<AStarNode>
{
    //格子对象的坐标
    public int x;
    public int y;

    //寻路消耗
    public float f;
    //距离起点的距离
    public float g;
    //距离终点的距离
    public float h;
    //父对象
    public AStarNode father;
    //格子类型
    public E_Node_Type type;

    public bool isChecked;

    public AStarNode(int x, int y, E_Node_Type type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }

    public int CompareTo(AStarNode other)
    {
        if (this.f > other.f)
            return 1;
        else if (this.f < other.f)
            return -1;
        else
            return 0;
    }
}
