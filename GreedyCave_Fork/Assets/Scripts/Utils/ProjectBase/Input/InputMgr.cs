using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using System.Reflection;
using UnityEditor;
using System;

//上下顺序决定优先级，同时存在的话，最下面的拥有控制权
public enum InputType
{
    PlayerMouseInput,
    SmallMapMouseInput,
    DecorateMouseInput,
}
//按键管理
public class InputMgr : BaseManager<InputMgr>
{
    //是否开启按键检测
    private bool isStart = false;
    private bool isPointerRectTrans = false;//是否点击到UI组件

    private SortedList mouseInput = new SortedList();
    private List<string> targetRelease = new List<string>();
    private Dictionary<string, Func<bool>> forbidMouseInputRelease = new Dictionary<string, Func<bool>>();
    private Dictionary<string, HashSet<InputType>> forbidMouseInputTypes = new Dictionary<string, HashSet<InputType>>();

    //手机版手指控制
    //缩放
    //private Vector2 oldPos1;
    //private Vector2 oldPos2;
    private bool isStartScale = false;
    private float lastDistance = 0;
    private Action onEndScale;

    //构造方法中，添加Update监听
    public InputMgr()
    {
        //开启按键检测
        StartOrEndCheck(true);
        //添加按键检测事件
        MonoMgr.GetInstance().AddUpdateListener(MyUpdate);
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);

        AddMouseInput(InputType.PlayerMouseInput, PlayerMouseInput);
    }
    //是否需要开启输入检测
    public void StartOrEndCheck(bool isOpen)
    {
        isStart = isOpen;
    }
    //按键检测
    private void MyUpdate()
    {
        //没有开启输入检测，就不去检测
        if (!isStart)
            return;
        Test();
    }

    public bool IsStartScale()
    {
        return isStartScale;
    }
    //获取手指缩放值
    public float GetTouchScale()
    {
        float delta = 0;
        if (Input.touchCount == 2)
        {
            isStartScale = true;
            if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                Vector2 newPos1 = Input.GetTouch(0).position;
                Vector2 newPos2 = Input.GetTouch(1).position;

                float distance = (newPos2 - newPos1).magnitude;
                if (lastDistance != 0)
                    delta = distance - lastDistance;

                lastDistance = distance;
            }
        }
        else
        {
            if (isStartScale == true)
                OnEndScale();
            isStartScale = false;
            lastDistance = 0;
        }
        return delta;
    }
    private void OnEndScale()
    {
        if (onEndScale != null)
            onEndScale.Invoke();
    }
    public void AddOnEndScaleListener(Action action)
    {
        if (onEndScale == null)
            onEndScale = action;
        onEndScale += action;
    }
    public void RemoveOnEndScaleListener(Action action)
    {
        if (onEndScale == null)
            return;
        onEndScale -= action;
    }

    public void AddMouseInput(InputType mouseInputType, Action action)
    {
        mouseInput[mouseInputType] = action;
    }
    public void RemoveMouseInput(InputType mouseInputType)
    {
        if (mouseInput.ContainsKey(mouseInputType))
            mouseInput.Remove(mouseInputType);
    }

    private void PlayerMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //按下的时候检测是因为，有些面板会被按钮点击隐藏，但是按钮的的调用速度更优先，相当于没有点击到面板
            isPointerRectTrans = IsPointerOnTarget<RectTransform>(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (isPointerRectTrans == true)//放开的时候都检测一次
            {
                isPointerRectTrans = false;
                return;
            }
            //松开始再检测一次是因为如果寻路过程，鼠标一直按下不松开，等事件触发以后再松开，玩家就可以在面板打开时移动，这会引发许多问题
            isPointerRectTrans = IsPointerOnTarget<RectTransform>(Input.mousePosition);
            if (isPointerRectTrans == true)
            {
                isPointerRectTrans = false;
                return;
            }
            Vector3 mousScreenPos = Input.mousePosition;
            Vector3 mousWorldPos = CameraMgr.GetInstance().GetCamera().ScreenToWorldPoint(mousScreenPos);
            mousWorldPos.z = 0;
            PlayerMgr.Instance.MoveToTarget(mousWorldPos);
        }
        //if (Input.GetMouseButtonUp(1))
        //{
        //    Vector3 mousScreenPos = Input.mousePosition;
        //    Vector3 mousWorldPos = CameraMgr.GetInstance().GetCamera().ScreenToWorldPoint(mousScreenPos);
        //    mousWorldPos.z = 0;
        //    PlayerMgr.Instance.SetPlayerPos(new Vector3((int)mousWorldPos.x, (int)mousWorldPos.y, 0));
        //}
    }
    /// <summary>
    /// 将玩家输入禁用一段时间
    /// </summary>
    /// <param name="forbidName">自定义的名字，不可以和其他禁用名字相同</param>
    /// <param name="time">需要禁用的时间</param>
    /// <param name="forbidTypes">禁用类型,默认为null，也就是全部禁用</param>
    public void AddForbidPlayerInput_Time(string forbidName, float time, HashSet<InputType> forbidTypes = null)
    {
        float currentTime = 0;
        AddForbidPlayerInput(forbidName, () =>
        {
            currentTime += Time.deltaTime;
            if (currentTime >= time)
                return true;

            return false;
        }, forbidTypes);
    }
    /// <summary>
    /// 添加玩家输入的禁用
    /// </summary>
    /// <param name="forbidName">自定义的名字，不可以和其他禁用名字相同</param>
    /// <param name="releaseFunc">释放禁用的方法，返回true解除禁用</param>
    /// <param name="forbidTypes">禁用类型,默认为null，也就是全部禁用</param>
    public void AddForbidPlayerInput(string forbidName, Func<bool> releaseFunc, HashSet<InputType> forbidTypes = null)
    {
        if (forbidTypes == null)
        {
            forbidTypes = new HashSet<InputType>();
            foreach (InputType mouseInputType in Enum.GetValues(typeof(InputType)))
            {
                forbidTypes.Add(mouseInputType);
            }
        }
        forbidMouseInputRelease[forbidName] = releaseFunc;
        forbidMouseInputTypes[forbidName] = forbidTypes;
    }

    public void RemoveForbidPlayerInput(string forbidName)
    {
        if (forbidMouseInputRelease.ContainsKey(forbidName))
            forbidMouseInputRelease.Remove(forbidName);
        if (forbidMouseInputTypes.ContainsKey(forbidName))
            forbidMouseInputTypes.Remove(forbidName);
    }

    private void PlayerInputHandler()
    {
        targetRelease.Clear();
        if (forbidMouseInputRelease.Count > 0)
        {
            foreach (KeyValuePair<string, Func<bool>> item in forbidMouseInputRelease)//检查所有禁用事件是否停止禁用
            {
                Func<bool> func = item.Value;
                if (func.Invoke() == true)
                    targetRelease.Add(item.Key);
            }
        }
        if (targetRelease.Count > 0)//将停止禁用的的类型移除
        {
            for (int i = 0; i < targetRelease.Count; i++)
            {
                RemoveForbidPlayerInput(targetRelease[i]);
            }
        }
        if (mouseInput.Count > 0)
        {
            InputType inputType = (InputType)mouseInput.GetKey(mouseInput.Count - 1);
            bool isForbid = false;
            if (forbidMouseInputTypes != null)
            {
                foreach (KeyValuePair<string, HashSet<InputType>> item in forbidMouseInputTypes)
                {
                    if (item.Value.Contains(inputType))
                    {
                        isForbid = true;
                        break;
                    }
                }
            }
            if (isForbid == false)
            {
                if (mouseInput.Contains(inputType))
                {
                    Action action = (Action)mouseInput[inputType];//.GetByIndex(mouseInput.Count - 1);
                    if (action != null)
                        action.Invoke();
                }
            }
        }
    }
    public void Test()
    {
        PlayerInputHandler();
    }
    //清除Console窗口
    private void ToClearConsole()
    {
        //Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        //System.Type type = assembly.GetType("UnityEditor.LogEntries");
        //MethodInfo method = type.GetMethod("Clear");
        //method.Invoke(null, null);
    }
    //按键触发事件
    private void CheckKeyCode(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            //事件中心模块，分发按下抬起事件（把哪个按键也发送出去）
            EventCenter.GetInstance().EventTrigger("KeyIsDown", key);
        }
        if (Input.GetKeyUp(key))
        {
            EventCenter.GetInstance().EventTrigger("KeyIsUp", key);
        }
    }

    public static T IsPointerOnTarget<T>(Vector3 touchPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touchPosition;
        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, results);

        foreach (var item in results)
        {
            T target = item.gameObject.GetComponent<T>();
            if (target != null)
                return target;
        }
        return default(T);
    }

    public static List<T> IsPointerOnTargets<T>(Vector3 touchPosition)
    {
        List<T> targetElements = new List<T>();

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touchPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var item in results)
        {
            T target = item.gameObject.GetComponent<T>();
            if (target != null)
                targetElements.Add(target);
        }
        return targetElements;
    }

    public void ConnectForMe(string id, string pw)
    {
        NetManager.Connect("127.0.0.1", 9999);
        MonoMgr.GetInstance().StartCoroutine(ConnetCoroutineForMe(id, pw));
    }

    public IEnumerator ConnetCoroutineForMe(string id, string pw)
    {
        while (NetManager.IsConnect() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        MsgLogin msg = new MsgLogin();
        msg.id = id;
        msg.pw = pw;
        NetManager.Send(msg);
    }

    public void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msg = (MsgLogin)msgBase;
        if (msg.result == 0)
        {
            NetManager.IsLogin = true;
            UIManager.GetInstance().ShowTips("登录成功");


            UIManager.GetInstance().ShowPanelAsync<PlayerStatePanelMgr>(PlayerStatePanelMgr.prefabsPath, PanelLayer.Bot, null, (panel) =>
            {
                PlayerMgr.Instance.SetPlayerData(msg.playerData);
                GameManager.Instance.ChangeSceneAfterVignette(SceneName.MainCity);

                panel.RemoveAllPlayerStates();
                panel.ShowPlayerState(PlayerMgr.Instance);
            });
        }
        else
            UIManager.GetInstance().ShowTips("登录失败");
    }
}
