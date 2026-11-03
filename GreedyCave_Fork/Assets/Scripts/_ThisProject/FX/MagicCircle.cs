using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MagicCircle : MazeObj, IPresetObjAble
{
    private Animator anim;
    private IPlayerAble player;

    protected void Start()
    {
        anim = GetComponent<Animator>();
    }
    //private void Start()
    //{
    //    anim = GetComponent<Animator>();
    //}
    public override string GetSmallMapSpritePath()
    {
        return "Sprite/SmallMapObj/MagicCircle";
    }
    public override bool PresetEventTrigger(IPlayerAble player)
    {
        this.player = player;
        this.anim.SetBool("BackFX", true);
        Vector3Int myGridPos = ToolsMgr.GetGirdPos(transform.position);
        bool isStop = false;
        player.GetPlayerAnimContoller().SetBackCityAnim(true);
        MonoMgr.GetInstance().CountdownTimer(2.1f, 
        (CountdownTimerInfo countdownTimeInfo) => 
        { 
            countdownTimeInfo.isInfluenceEndAction = true;
        }, 
        (CountdownTimerInfo countdownTimeInfo) =>
        {
            if (player.GetGirdPos() == myGridPos && isStop == false)//触发PresetEventTrigger时，玩家还在上一个格子，所以要做一点手段
                isStop = true;

            if (player.GetGirdPos() != myGridPos && isStop == true)
            {
                anim.SetBool("BackFX", false);
                player.GetPlayerAnimContoller().SetBackCityAnim(false);
                countdownTimeInfo.isBreak = true;
            }
          },
        BackCity
        );

        return false;
    }

    private void BackCity(CountdownTimerInfo countdownTimerInfo)
    {
        if (player.GetGirdPos() != ToolsMgr.GetGirdPos(transform.position))
            return;

        GameManager.Instance.ChangeSceneAfterVignette(SceneName.MainCity);
        PlayerMgr.Instance.TakeOffEquips((equip) =>
        {
            if (equip == null)
                return false;
            if (equip.isNeverDrop == true)
                return false;
            if ((int)equip.itemQuality >= (int)ItemQuality.Purple)
                return false;
            return true;
        });
        PlayerMgr.Instance.GetBag().RemoveBagItems((item)=> 
        {
            if (item == null)
                return false;
            if (item.isNeverDrop == true)
                return false;
            if ((int)item.itemQuality >= (int)ItemQuality.Purple)
                return false;
            return true;
        });
    }
}
