using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public interface IDragAble//用于表明哪些东西是可以"被拖拽
{
    Sprite GetSprite_Drag();
}

//用于拖拽，只能拖拽IGetAble
public interface IDragDownAble<T> where T : IDragAble
{
    T GetItem_Drag();
}
//可以接收Drag结束的物品
public interface IDragUpAble<T> where T : IDragAble
{
    //返回原来的物品
    T SetItem_Drag(T obj);
}

public abstract class DragPanel : MapPanel, IDragDownAble<IDragAble>, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public abstract ScrollRect GetScrollRect();
    public abstract IDragAble GetItem_Drag();
    /// <summary>
    /// 作为物品开始拖拽的面板，对拖拽结束后溢出的物品的处理
    /// </summary>
    /// <param name="drageAble"></param>
    public abstract void OperiateOverflowItem_Drag(IDragAble drageAble);

    private DragImagePanel dragImagePanel;

    private bool isDragScrollRect = true;
    private bool isEndDrag = false;
    protected Vector2 startPos;

    public void OnPointerDown(PointerEventData eventData)
    {
        startPos = eventData.position;

        if (IsCanDrag() == false)
            return;

        isDragScrollRect = true;
        isEndDrag = false;
        ScrollRect scrollRect = GetScrollRect();
        if (scrollRect == null)
            isDragScrollRect = false;

        StartCoroutine(CheckDragType(0.25f, eventData.position));
    }

    //必须要实现ScrollRect.OnBeginDrag(eventData);才可以实现ScrollRect.OnDrag(eventData)
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsCanDrag() == false && GetScrollRect() != null)
        {
            GetScrollRect().OnBeginDrag(eventData);
            return;
        }

        if (isDragScrollRect && GetScrollRect() != null)
        {
            GetScrollRect().OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsCanDrag() == false && GetScrollRect() != null)
        {
            GetScrollRect().OnDrag(eventData);
            return;
        }

        if (isDragScrollRect && GetScrollRect() != null)
        {
            GetScrollRect().OnDrag(eventData);
        }
        else
        {
            if (dragImagePanel == null)
                return;
            dragImagePanel.transform.position = eventData.position;
        }
    }

    //根据设置，可以回弹
    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsCanDrag() == false && GetScrollRect() != null)
        {
            GetScrollRect().OnEndDrag(eventData);
            return;
        }

        if (isDragScrollRect && GetScrollRect() != null)
        {
            GetScrollRect().OnEndDrag(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)//不同子类实现方式不一样
    {
        if (IsCanDrag() == false)
            return;

        isEndDrag = true;
        if (isDragScrollRect)
            return;

        IDragUpAble<IDragAble> panel = InputMgr.IsPointerOnTarget<IDragUpAble<IDragAble>>(eventData.position);
        PointerUpOperates(panel, dragImagePanel);

        if (dragImagePanel != null)
        {
            UIManager.GetInstance().HidePanel(DragImagePanel.prefabsPath);
            dragImagePanel = null;
        }
        
    }
    //比如装备拖拽到一个空地方可以脱下该装备。所以子类可以自行处理
    protected virtual void PointerUpOperates(IDragUpAble<IDragAble> panel, DragImagePanel dragImagePanel)
    {
        if (panel != null && dragImagePanel != null)
        {
            IDragAble drageAble = panel.SetItem_Drag(dragImagePanel.Obj);//每一个含有
            OperiateOverflowItem_Drag(drageAble);
        }
    }
    protected virtual bool IsCanDrag()
    {
        return true;
    }
    IEnumerator CheckDragType(float time, Vector3 originalPos)
    {
        Vector2 vector = new Vector2();
        yield return new WaitUntil(() =>
        {
            if (isEndDrag)
                return true;

            time -= Time.deltaTime;
            vector = Input.mousePosition - originalPos;
            if (vector.sqrMagnitude > 2)
            {
                isDragScrollRect = true;
                return true;
            }
            if (time <= 0)
            {
                isDragScrollRect = false;
                UIManager.GetInstance().ShowPanelAsync<DragImagePanel>(DragImagePanel.prefabsPath, PanelLayer.Max,null,(panel) =>
                  {
                      IDragAble obj = GetItem_Drag();
                      if (obj == null)
                      {
                          UIManager.GetInstance().HidePanel(DragImagePanel.prefabsPath);
                          dragImagePanel = null;
                          return;
                      }

                      panel.SetObj(obj);
                      this.dragImagePanel = panel;
                      this.dragImagePanel.transform.position = Input.mousePosition;

                      //if (isEndDrag)//中断机制可能会让这些点击出现问题
                      //{
                      //    UIManager.GetInstance().HidePanel(DragImagePanel.prefabsPath);
                      //    dragImagePanel = null;
                      //    Debug.Log("协程");
                      //}
                  });
                return true;
            }
            return false;
        });
    }

}
