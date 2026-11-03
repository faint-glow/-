using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AstarMoveType
{
    Normal,
    MoveOneGrid,
}
public class PlayerBehaviourController
{
    public PlayerBehaviourController(PlayerController playerController)
    {
        this.playerController = playerController;
        this.originalScale = playerController.GetAnimator().gameObject.transform.localScale;
        //this.animObj = animObj;
        EventCenter.GetInstance().AddEventListener<Scene>(EventCenterEnum.切换场景前.ToString(), StartChangeScene);
        EventCenter.GetInstance().AddEventListener<Scene>(EventCenterEnum.切换场景后.ToString(), AfterChangeScene);
    }

    private PlayerController playerController;
    private Vector3 offset = new Vector3(0.5f, 0.1f, 0);//玩家位置偏移
    private Coroutine coroutine;

    private string moveFX_Road = "FX/MoveFX/Move_FX_Road";
    private string moveFX_Obstacle = "FX/MoveFX/Move_FX_Obstacle";
    //方向控制
    private Vector3 originalScale;

    private int direction = Warrior.left;//玩家一开始的方向是朝向左边的

    //移动控制
    private Vector3 lastPos;
    private Vector3 nextPos;//一下次要移动的位置

    private bool isPreMoveOneGriding;
    private bool isAstarMoveEnd;//是否已经结束了当前的移动任务
    private bool isHaveTask;//是否有下一次的移动任务

    private float speed = 4f;
    private bool isNormalMove;
    private bool isChangeScene;

    private bool isShowFX = true;
    private AstarMoveType currentAstartMoveType;
    public void SetPlayerPos(Vector3 targetPos)
    {
        playerController.GetPlayerObject().transform.position = targetPos + offset;
    }
    public Vector3 GetPlayerPos()
    {
        if (playerController.GetPlayerObject() != null)
            return playerController.GetPlayerObject().transform.position;
        Debug.Log("出错");
        return Vector3.zero;
    }
    public int GetDir()
    {
        return direction;
    }
    public void SetDir(int dir)
    {
        direction = dir;
        Warrior.SetDir(playerController.GetWarrior(), originalScale, dir);
    }
    //传入目标位置，自动确定玩家方向
    public void DirControl(Vector3 playerPos, Vector3 targetPos)
    {
        int dir = Warrior.GetDir(playerPos, targetPos);
        if (dir == 0)
            dir = direction;
        SetDir(dir);
    }
    private void DirControl(Vector3 targetPos)
    {
        DirControl(playerController.GetPlayerObject().transform.position, targetPos);
    }
    private void StartChangeScene(Scene scene)
    {
        isChangeScene = true;
    }
    private void AfterChangeScene(Scene scene)
    {
        isChangeScene = false;
    }
    public void SetMoveOneGriding(bool isTrue)
    {
        isPreMoveOneGriding = isTrue;
    }
    public void MoveOneGrid(Vector3 targetPos)
    {
        isShowFX = false;
        currentAstartMoveType = AstarMoveType.MoveOneGrid;
        AstarMoveToTargetAided(playerController.GetWarrior().GetWarriorPos() + targetPos);
    }
    public void AstarMoveToTarget(Vector3 targetPos)
    {
        isShowFX = true;
        currentAstartMoveType = AstarMoveType.Normal;
        AstarMoveToTargetAided(targetPos);
    }
    //移动到目标点。用于A*寻路的目标点
    private void AstarMoveToTargetAided(Vector3 targetPos)
    {
        if (isNormalMove == true || playerController.IsBattleing() == true)
            return;
        if (coroutine != null)
        {
            if (isAstarMoveEnd == true)
            {
                MonoMgr.GetInstance().StopCoroutine(coroutine);
                //coroutine = null;//停止协程不会让coroutine变为null
            }
            else
            {
                isHaveTask = true;
                nextPos = targetPos;
                return;
            }
        }
        Vector3 playerPos = playerController.GetPlayerObject().transform.position;
        Stack<AStarNode> path = AStarMgr.Instance.FindPath(playerPos, targetPos);
        if (path == null)
        {
            IManyGridObj manyGridObj = ToolsMgr.CheckPrestEvent<IManyGridObj>(targetPos);
            if (manyGridObj != null && manyGridObj is IPresetObjAble)
            {
                if (isShowFX == true)
                    AstartMoveFX(targetPos, moveFX_Road);
                targetPos = manyGridObj.GetTriggerPos(playerPos);
                path = AStarMgr.Instance.FindPath(playerPos, targetPos);
            }
            else
            {
                if (isShowFX == true)
                    AstartMoveFX(targetPos, moveFX_Obstacle);
                playerController.StopAnim();//控制动画
                return;
            }
        }
        else
        {
            if (isShowFX == true)
                AstartMoveFX(targetPos, moveFX_Road);
        }

        playerController.WalkAnim();//控制动画-
        #region 协程的一些知识
        //注意：协程不是线程，就算多核(多CUP)系统，只能并发执行，不能并行执行

        //目前推测：协程在正常执行还没有开始等待时，仍然是正常的方法执行顺序，但是yield return以后，就会挂起，此时才会产生协程，开始等待。然后中断

        //如果还没有开始等待，就yield break，就可以让返回的协程为null

        //除此之外，如果还没有开始等待，是不能停止协程（StopCoroutine）的，此时coroutine为null，会报错。等待过一次就不会报错

        //但是如果开始等待，就可以在停止，即便在协程里面也能做到

        //StopCoroutine时，如果协程正在等待期间，则会一直进行等待行为
        //但是，如果协程已经执行完需要等待的代码，那么，后面不需要等待的代码就会继续执行完
        //也就是，从执行StopCoroutine那一刻开始，一直到下一次等待行为之前，代码还是会正常运行
        //但是yield break会直接结束
        #endregion
        isAstarMoveEnd = false;
        coroutine = MonoMgr.GetInstance().StartCoroutine(AstartMove(path, isShowFX));
    }

    private void AstartMoveFX(Vector3 targetPos,string prefabsPath)
    {
        targetPos = new Vector3((int)targetPos.x + 0.5f, (int)targetPos.y + 0.5f, 0);
        PoolMgr.GetInstance().GetObj(prefabsPath, (obj) =>
        {
            obj.transform.position = targetPos;
            MonoMgr.GetInstance().DelayAction(1f, () =>//生成一秒后返回
             {
                 if (obj != null)
                     PoolMgr.GetInstance().PushObj(obj);
             });
        });
    }

    //移动的具体执行
    IEnumerator AstartMove(Stack<AStarNode> path, bool isShowFX)
    {
        isHaveTask = false;
        path.Pop();//第一个是原位置，弹出不要
        Vector3 targetPos = new Vector3();
        int count = path.Count;
        for (int i = 0; i < count; i++)
        {
            if (isChangeScene == true)
                break;

            targetPos.Set(path.Peek().x, path.Peek().y, 0);
            targetPos += offset;

            bool isPresetEventTrigger = PresetEventTrigger(targetPos);
            if (isPresetEventTrigger)//如果触发事件，则不用进行走路
            {
                if (playerController.IsBattleing() == false)//如果正在战斗就不用停
                    playerController.StopAnim();

                if(currentAstartMoveType == AstarMoveType.MoveOneGrid)
                {
                    MoveControlPanel panel = (MoveControlPanel)UIManager.GetInstance().GetExistPanel(MoveControlPanel.prefabsPath);
                    if (panel != null)
                        panel.EndControl();
                }
                isAstarMoveEnd = true;
                yield break;
            }
            GameManager.Instance.ClearShady_Range((int)targetPos.x, (int)targetPos.y);

            //MsgSendNextPos msg = new MsgSendNextPos();
            //msg.x = targetPos.x;
            //msg.y = targetPos.y;
            //msg.z = targetPos.z;
            //NetManager.Send(msg);

            DirControl(targetPos);//传入目标位置，自动确定玩家方向

            yield return new WaitUntil(() =>
            {
                if (isChangeScene == true)
                    return true;
                if (playerController.GetPlayerObject() == null)
                    return true;

                playerController.GetPlayerObject().transform.position = Vector3.MoveTowards(playerController.GetPlayerObject().transform.position, targetPos, Time.deltaTime * speed);
                if (playerController.GetPlayerObject().transform.position == targetPos)
                    return true;
                else
                    return false;
            });
            if (isChangeScene == true)
                break;
            path.Pop();
            if (isHaveTask == true)
            {
                if (currentAstartMoveType == AstarMoveType.Normal || (currentAstartMoveType == AstarMoveType.MoveOneGrid && isPreMoveOneGriding == true))
                {
                    isAstarMoveEnd = true;
                    AstarMoveToTargetAided(nextPos);
                    yield break;
                }
            }
        }

        playerController.StopAnim();//控制动画
        isAstarMoveEnd = true;
    }

    //用于战斗结束，移动到附近格子
    public void NormalMoveToTarget(Vector3 targetPos, Action afterAction = null, Func<bool> breakFunc = null)
    {
        MonoMgr.GetInstance().StartCoroutine(NormalMove(targetPos, afterAction, breakFunc));
    }
    ////用于战斗结束，移动到附近格子
    IEnumerator NormalMove(Vector3 targetPos, Action afterAction = null, Func<bool> breakFunc = null)//听说UnityAction性能没C#自带的Action好
    {
        isNormalMove = true;
        yield return new WaitUntil(() =>
        {
            if (breakFunc != null && breakFunc() == true)
                return true;

            playerController.GetPlayerObject().transform.position = Vector3.MoveTowards(playerController.GetPlayerObject().transform.position, targetPos, Time.deltaTime * speed);
            if (playerController.GetPlayerObject().transform.position == targetPos)
                return true;
            else
                return false;
        });
        isNormalMove = false;
        if (afterAction != null)//移动结束以后需要做什么
            afterAction.Invoke();
    }

    private bool PresetEventTrigger(Vector3 targetPos)
    {
        IPresetObjAble presetObj = ToolsMgr.CheckPrestEvent<IPresetObjAble>(targetPos);
        bool isStop = false;
        if (presetObj != null)
        {
            isStop = presetObj.PresetEventTrigger(playerController.GetPlayer());
        }
        return isStop;
    }

    ////战斗触发器
    //private void StartBallte(MazeGrid grid)
    //{
    //    //if (NetManager.IsLogin)
    //    //{
    //    //    if (playerController.IsHost() == false)//如果是网络玩家，则由网络控制攻击
    //    //        return;

    //    //    MsgStartBattle msg = new MsgStartBattle();
    //    //    msg.behaviourID = PlayerMgr.Instance.GetHostID();
    //    //    msg.x = grid.x;
    //    //    msg.y = grid.y;
    //    //    NetManager.Send(msg);
    //    //    return;//收到服务器的信息以后才能攻击，以确保服务器知道我进行了此攻击行为
    //    //}
    //    playerController.StartBattle(grid.GridObj as Enemy);
    //}
    //private void OpenTreasureBox(MazeGrid grid)
    //{
    //    PlayerMgr.Instance.OpenTreasureBox(grid.GridObj as TreasureBox);
    //    playerController.StopAnim();
    //}
    //private void OpenJar(MazeGrid grid)
    //{
    //    PlayerMgr.Instance.OpenJar(grid.GridObj as Jar);
    //    playerController.StopAnim();
    //}

    //private void NextLayerController()
    //{
    //    playerController.StopAnim();

    //    if (RoomManager.GetInstance().IsMeInRoom == false || RoomManager.GetInstance()._roomInfo.players.Count < 2)
    //    {
    //        GameManager.Instance.NextLayer();
    //    }
    //    else
    //    {
    //        List<string> players = RoomManager.GetInstance()._roomInfo.players;
    //        PlayerMsgMgr.msgNextLayerNum++;

    //        string currentPlayerID = PlayerMgr.Instance.GetPlayerData().ID;

    //        MsgRequest msg = new MsgRequest();
    //        msg.msgID = PlayerMsgMgr.msgNextLayerID;
    //        msg.msgNum = PlayerMsgMgr.msgNextLayerNum;
    //        msg.requestID = currentPlayerID;
    //        msg.content = currentPlayerID + "请求进入下一层";
    //        msg.targetResults = new Dictionary<string, RequestType>();
    //        for (int i = 0; i < players.Count; i++)
    //        {
    //            if (players[i] == currentPlayerID)
    //                continue;
    //            msg.targetResults[players[i]] = RequestType.Wait;
    //        }
    //        msg.isEndRequest = false;

    //        NetManager.Send(msg);
    //    }
    //}


}
