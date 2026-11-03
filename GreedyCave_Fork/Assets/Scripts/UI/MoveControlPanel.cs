using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveControlPanel : MapPanel
{
    public static string prefabsPath = "MoveControlPanel";//主要背包

    private Image range;
    private Image back;
    private Image center;

    private Vector3 originalLocalCenter;
    private Vector3 originalCenter;

    private float breakRadius = 60;//当前距离小于breakRadius时不响应
    private float radius = 180;
    private float currentDistance;

    private Vector3 currentDir;
    private Vector3 currentDir_Four;

    private Vector3 mousePosition;
    public override void ShowMeFirst()
    {
        range = GetPanel("Range").GetUI<Image>();
        back = GetPanel("Range/Main/Back").GetUI<Image>();
        center = GetPanel("Range/Main/Back/Center").GetUI<Image>();

        originalCenter = center.transform.position;
        originalLocalCenter = center.transform.localPosition;

        UITools.AddEvent(range, EventTriggerType.PointerDown, PointerDown);
        UITools.AddEvent(range, EventTriggerType.Drag, Drag);
        UITools.AddEvent(range, EventTriggerType.PointerUp, PointerUp);

        EventCenter.GetInstance().AddEventListener<Scene>(EventCenterEnum.切换场景前.ToString(), StartChangeScene);
        EventCenter.GetInstance().AddEventListener<Scene>(EventCenterEnum.切换场景后.ToString(), AfterChangeScene);
    }
    private bool isStartPointer = false;
    private bool isLastControll = false;
    //private bool isOpenControl = false;

    private void StartChangeScene(Scene scene)
    {
        //isOpenControl = false;
        EndControl();
    }
    private void AfterChangeScene(Scene scene)
    {
        //isOpenControl = true;
        //isLastControll = false;
    }

    private void Update()
    {
        if (isStartPointer == false)
            return;
        RectTransform rect = InputMgr.IsPointerOnTarget<RectTransform>(Input.mousePosition);
        if (rect != range.rectTransform && rect != null)
        {
            EndControl();
            return;
        }
        Vector3 targetDir_Four = GetDir_Four();
        if (targetDir_Four != Vector3.zero && GetDistance() > breakRadius)
        {
            PlayerMgr.Instance.MoveOneGrid(targetDir_Four);
            PlayerMgr.Instance.SetMoveOneGriding(true);
            //isLastControll = true;
        }
        //else
        //{
        //    if (isLastControll == true)//停止控制时，结束上一次的预设前进位置
        //        PlayerMgr.Instance.SetMoveOneGriding(false);
        //    isLastControll = false;
        //}
    }
    
    private void OnEndControl()
    {
        PlayerMgr.Instance.SetMoveOneGriding(false);
    }
    
    private void PointerDown(BaseEventData data)
    {
        isStartPointer = true;
        Control(data);
    }
    private void Drag(BaseEventData data)
    {
        if (isStartPointer == true)
            Control(data);
    }
    private void PointerUp(BaseEventData data)
    {
        EndControl();
    }
    private void Control(BaseEventData data)
    {
        Vector3 mousPosition = (data as PointerEventData).position;
        Vector3 dirVector3 = (mousPosition - originalCenter);
        currentDistance = dirVector3.magnitude;
        currentDistance = Mathf.Clamp(currentDistance, 0, radius);
        currentDir = dirVector3.normalized;
        center.transform.localPosition = originalLocalCenter + currentDir * currentDistance;
    }
    public void EndControl()
    {
        isStartPointer = false;
        isLastControll = false;
        currentDistance = 0;
        currentDir = Vector3.zero;
        center.transform.localPosition = originalLocalCenter;

        OnEndControl();//主要是触发外部对结束控制时的方法，这里图个方便，内部直接就处理玩家的结束控制
    }
    public float GetDistance()
    {
        return currentDistance;
    }
    //获取上下左右四个方向的遥感方向
    public Vector3 GetDir_Four()
    {
        //范围是x轴正向0，负向180或-180，（0开始逆时针为正，顺时针为负）
        float dir = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;//Mathf.Atan2(currentDir.y, currentDir.x)//正切函数获取弧度制角度 * Mathf.Rad2Deg(大概等于57.2958)转化为角度制

        if (Mathf.Abs(dir) <= 45/* dir >= -45 && dir < 45*/)
            currentDir_Four.Set(1, 0, 0);
        else if (dir >= 45 && dir <= 135)
            currentDir_Four.Set(0, 1, 0);
        else if (Mathf.Abs(dir) >= 135 /* dir >= 135 && dir <= 180 || (dir < -135 && dir <= -180)*/)
            currentDir_Four.Set(-1, 0, 0);
        else if (dir >= -135 && dir < -45)
            currentDir_Four.Set(0, -1, 0);

        if (currentDistance == 0)
            currentDir_Four.Set(0, 0, 0);
        return currentDir_Four;
    }
}
