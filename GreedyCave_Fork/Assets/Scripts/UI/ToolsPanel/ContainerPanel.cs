using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//被赋值给某个Panel
public interface IGetAble//空接口，用于表明哪些东西是可以赋值给某个panel"。如Item，Magic
{

}
//赋值给某个Panel
public interface ISetAble<T> where T : IGetAble
{
    public int Index
    {
        get;
        set;
    }
    public MapPanel FatherPanel
    {
        get;
        set;
    }
    void SetItem_Container(T obj);
}
public interface IHolderAble
{
    public string GetBagName();
    public BagMgr<Item> GetBagMgr();
    public Bag<Item> GetBag();
    public void RefreshBag();
    public HolderType GetHolderType();
    public Item AddItem(Item item);
    //移除以后，返回的物体和移除之前的物体不是同一个物体，例如数量只有1的物体移除后，传入的物体数量为0，返回的物体数量为1
    public Item RemoveItem(Item item, int count = 1);
    public int GetCoin();
    public void ChangeCoin(int count);

    public int GetItemPrice(Item item);

    //获取禁止获得的物品的名字和即将获得该物品时取消获得的提示信息（主要应用于回城卷轴不可以加入魔法球之中）
    public Dictionary<string, string> GetForbidItemDic();
}

/// <summary>
/// 拥有背包一样的能力，可以对某个子物体动态添加多个重复面板
/// </summary>
/// <typeparam name="Cell">加载的单元，比如ItemBag中的每个格子</typeparam>
/// <typeparam name="CellInfo">每个格子可以装入的数据类型</typeparam>
public abstract class ContainerPanel<Cell, CellInfo> : MapPanel
    where Cell : MapPanel, ISetAble<IGetAble>
    where CellInfo : IGetAble
{
    protected List<Cell> cellList = new List<Cell>();//单元列表
    /// <summary>
    /// //单元在Resources中的路径，用于动态加载资源
    /// </summary>
    /// <returns></returns>
    public abstract string GetCellPath();
    public abstract ScrollRect GetScrollRect();

    /// <summary>
    /// 动态加载的单元挂载的目标父物体
    /// </summary>
    /// <returns></returns>
    public abstract Transform GetContainterTrans();//单元的父物体
    public override void ShowMeEveryTimesHandler()
    {
        base.ShowMeEveryTimesHandler();
        for (int i = 0; i < cellList.Count; i++)
        {
            cellList[i].ShowMeEveryTimesHandler();
        }
    }

    public override void HideMeHandler()
    {
        base.HideMeHandler();
        for (int i = 0; i < cellList.Count; i++)
        {
            cellList[i].HideMeHandler();
        }
    }

    public override void DestroyHandler()
    {
        base.DestroyHandler();
        for (int i = 0; i < cellList.Count; i++)
        {
            cellList[i].DestroyHandler();
        }
    }


    protected void ForeachChild(Action<Cell> action)
    {
        for (int i = 0; i < cellList.Count; i++)
        {
            if (cellList[i] != null)
                action.Invoke(cellList[i]);
        }
    }

    protected Cell GetCell(int index)
    {
        if (cellList != null && cellList.Count > index)
            return cellList[index];
        return null;
    }

    /// <summary>
    /// 刷新背包的UI列表
    /// </summary>
    /// <param name="cellInfoList">玩家的背包</param>
    /// <param name="firstTimeCallBack">对新增的格子的处理</param>
    protected void RefreshContainer(List<CellInfo> cellInfoList, UnityAction<Cell> firstTimeCallBack = null, UnityAction<Cell> everyTimeCallBack = null)
    {
        if (cellInfoList == null)
        {
            RefreshCell(new List<CellInfo>());//清空所有子物体
            AfterRefresh();
            return;
        }
        if (everyTimeCallBack != null)
        {
            for (int i = 0; i < cellList.Count; i++)
            {
                Show(ShowType.Always, cellList[i].GetComponent<Cell>(), everyTimeCallBack);
            }
        }
        if (cellList.Count >= cellInfoList.Count)
        {
            RefreshCell(cellInfoList);
            AfterRefresh();
            return;
        }
        ResMgr.GetInstance().LoadAsync<GameObject>("UI/" + GetCellPath(), (resObj) =>
        {
            int count = cellList.Count;
            for (int i = 0; i < cellInfoList.Count - count; i++)
            {
                //初始化每个cell单元
                GameObject obj = Instantiate(resObj);
                obj.transform.SetParent(GetContainterTrans());
                obj.transform.localScale = Vector3.one;

                Cell panel = obj.GetComponent<Cell>();//得到预设体身上的脚本
                panel.FatherPanel = this;
                Show(ShowType.First, panel, firstTimeCallBack);
                Show(ShowType.Always, panel, everyTimeCallBack);
                cellList.Add(panel);
            }
            RefreshCell(cellInfoList);
            AfterRefresh();
        }, false);
    }


    protected virtual void AfterRefresh()
    {

    }

    private void RefreshCell(List<CellInfo> cellInfoList)
    {
        for (int i = 0; i < cellList.Count; i++)
        {
            if (i >= cellInfoList.Count)
                break;
            cellList[i].Index = i;
            cellList[i].SetItem_Container(cellInfoList[i]);
        }
        int count = cellList.Count - cellInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            int index = cellList.Count - 1;
            DestroyPanel(index);
            cellList.RemoveAt(index);
        }
    }


    //清除所有Cell单元
    protected void ClearCells()
    {
        for (int i = cellList.Count; i > 0; i--)
        {
            Destroy(cellList[i].gameObject);
        }
    }

    //销毁面板
    public void DestroyPanel(int index)
    {
        cellList[index].HideMeHandler();
        GameObject.Destroy(cellList[index].gameObject);
    }

    //public void SetHolder(IHolderAble holder)
    //{
    //    this.holder = holder;
    //}
    public abstract IHolderAble GetHolder();
    //{
    //    return holder;
    //}
}
