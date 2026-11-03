using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController:IAnimtorAble
{
    private IPlayerAble player;
    //private PlayerAnimatorController animatorController;
    private PlayerBehaviourController behaviourController;
    private PlayerBattleController battleController;
    private PlayerInfo playerInfo;
    public PlayerInfo _playerInfo
    {
        get
        {
            if (playerInfo == null)
                playerInfo = ResMgr.GetInstance().Load<PlayerInfo>("Player/PlayerInfo");
            return playerInfo;
        }
    }
    public PlayerController(IPlayerAble player)
    {
        this.player = player;
        //animatorController = new PlayerAnimatorController(this);
        behaviourController = new PlayerBehaviourController(this);
        battleController = new PlayerBattleController(this);

        player.GetStaticAvatar(true);
        RefreshAvata();
        EventCenter.GetInstance().AddEventListener<GameObject>(EventCenterEnum.创建玩家.ToString(),(obj)=> 
        {
            player.GetStaticAvatar(true);
            RefreshAvata();
        });
    }

    public IPlayerAble GetPlayer()
    {
        return player;
    }

    public GameObject GetPlayerObject()
    {
        return GetPlayer().GetPlayerObject();
    }

    public Animator GetAnimator()
    {
        return GetPlayer().GetAnimator();
    }

    #region IPlayerAble
    public bool IsHost()//当前玩家是否是主机玩家
    {
        return PlayerMgr.Instance.IsHost(player);
    }
    private PlayerInfoPanel GetPlayerInfoPanel()
    {
        PlayerStatePanelMgr mgrPanel = (PlayerStatePanelMgr)UIManager.GetInstance().GetExistPanel(PlayerStatePanelMgr.prefabsPath);
        if (mgrPanel != null)
            mgrPanel.RefreshPanel(player);
        BagPanel bagPanel = (BagPanel)UIManager.GetInstance().GetExistPanel(BagPanel.prefabsPath);
        if (bagPanel == null)
            return null;
        PlayerInfoPanel playerInfoPanel = (PlayerInfoPanel)bagPanel.GetExistPanel(PlayerInfoPanel.prefabsPath);
        if (playerInfoPanel == null)
            return null;
        return playerInfoPanel;
    }
    public void RefreshAvata()
    {
        PlayerInfoPanel playerInfoPanel = GetPlayerInfoPanel();

        if (playerInfoPanel != null)
            playerInfoPanel.RefreshAvata();
    }
    public void RefreshStatePanel()
    {
        PlayerInfoPanel playerInfoPanel = GetPlayerInfoPanel();
        if(playerInfoPanel != null)
            playerInfoPanel.RefreshState();
    }
    public Dictionary<WearEquipType, List<Equip>>  GetEquips()
    {
        return player.GetEquips();
    }
    #endregion

    #region DetailInfo

    #endregion

    #region PlayerBehaviourController

    //设置玩家位置，用于初始化
    public int GetDir()
    {
        return behaviourController.GetDir();
    }
    public void SetPlayerPos(Vector3 targetPos)
    {
        behaviourController.SetPlayerPos(targetPos);
    }
    public Vector3 GetPlayerPos()
    {
        return behaviourController.GetPlayerPos();
    }
    //传入目标位置自动寻路
    public void AstarMoveToTarget(Vector3 targetPos)
    {
        behaviourController.AstarMoveToTarget(targetPos); 
    }
    public void MoveOneGrid(Vector3 targetPos)
    {
        behaviourController.MoveOneGrid(targetPos); 
    }
    public void SetMoveOneGriding(bool isTrue)
    {
        behaviourController.SetMoveOneGriding(isTrue);
    }

    public void NormalMoveToTarget(Vector3 targetPos ,Action afterAction = null,Func<bool> breakFunc = null)
    {
        behaviourController.NormalMoveToTarget(targetPos, afterAction, breakFunc);
    }
    public void SetDir(int dir)
    {
        behaviourController.SetDir(dir);
    }
    //目标位置自动控制玩家朝向
    public void DirControl(Vector3 playerPos, Vector3 targetPos)
    {
        behaviourController.DirControl(playerPos, targetPos);
    }
    #endregion
    //*************************************************************
    #region PlayerAnimatorController

    public void StopAnim()
    {
        battleController.GetPlayerAnimatorController().IdleAnim();
    }

    public void WalkAnim()
    {
        battleController.GetPlayerAnimatorController().WalkAnim();
    }

    public void BattleAnim()
    {
        battleController.GetPlayerAnimatorController().AttckAnim();
    }
    #endregion
    //*************************************************************
    #region PlayerBattleController

    public PlayerAnimState GetPlayerAnimState()
    {
        return battleController.GetPlayerAnimatorController().GetPlayerAnimState();
    }
    public Warrior GetWarrior()
    {
        return battleController;
    }
    public void StartBattle(Enemy enemy)
    {
        battleController.StartBattleHandler(enemy);
    }
    public bool IsBattleing()
    {
        return battleController.IsBattleing();
    }

    //public void NormalAttackController()
    //{
    //    //battleController.NormalAttackController();
    //}
    public float GetRunProcess()
    {
        return battleController.GetRunProcess();
    }
    public void StartRun()
    {
        battleController.StartRunAway();
    }
    #endregion


    //public void GetDressed(Equip equip)
    //{
    //    if (equip != null)
    //    {
    //        weaponSr.sprite = equip.itemSprite;
    //        Photograph();
    //        SetCloneObj();
    //    }
    //    else
    //    {
    //        Undress();
    //    }
    //}


}


