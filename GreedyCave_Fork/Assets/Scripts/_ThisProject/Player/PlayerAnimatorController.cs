//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public enum PlayerAnimState
//{
//    Idle,
//    Walk,
//    Attack,
//    Skill_Physics_1,
//    Skill_Magic_1,
//    Skill_Magic_2_1,
//    Skill_Magic_2_2,
//    Skill_Magic_2_3,
//    BackCity,
//    UseProp,
//    Die,
//}

//public class PlayerAnimatorController : MyAnimatorController
//{
//    PlayerBattleController playerBattleController;
//    Dictionary<PlayerAnimState, float> animationClipSpeed = new Dictionary<PlayerAnimState, float>();

//    private PlayerAnimState playerState;
//    public PlayerAnimatorController(PlayerBattleController playerBattleController) : base(playerBattleController, "Player")
//    {
//        this.playerBattleController = playerBattleController;

//        EventCenter.GetInstance().AddEventListener<GameObject>(EventCenterEnum.创建玩家.ToString(), (GameObject Obj) =>
//        {
//            CreateAnimController("Player");
//            SetAnimationEvent();
//        });
//    }

//    public void SetAnimationEvent()
//    {
//        AnimationClip[] clips = GetAnimator().runtimeAnimatorController.animationClips;
//        for (int i = 0; i < clips.Length; i++)
//        {
//            AddAnimationEvent(clips[i].name,TimeType.Start, AnimationClipStart);
//        }
//        //AddAnimationEvent("Attack", EventTriggerEnum.EventTriggerOne, 0.39f, Attack);
//        AddAnimationEvent("BackCity", EventTriggerEnum.EventTriggerOne, 1.37f, FlashFX);
//    }

//    public static float delayTime = 0.0005f;
//    public void AnimationClipStart()
//    {
//        AnimatorClipInfo[] clips = GetAnimator().GetCurrentAnimatorClipInfo(0);
//        string clipName = clips[0].clip.name;
//        //极限卡刀，此前用于迄今为止遇见的最难以处理的bug
//        //if (clipName == "Attack")
//        //{
//        //    MonoMgr.GetInstance().StartCoroutine(Delay(0.65f, TriggerSkill));
//        //}

//        //if (clipName == "Attack")
//        //{
//        //    GetAnimator().ResetTrigger("Skill_Magic_1");
//        //    Debug.Log(delayTime);
//        //    MonoMgr.GetInstance().DelayAction(delayTime, TriggerSkill);
//        //}

//        SetState(clipName);
//    }
//    private void TriggerSkill()
//    {
//        SkillController skillController = PlayerMgr.Instance.GetSkillController();
//        Skill skill = skillController.GetSkill("寒冰射线").skill;
//        skill.OnTriggerEnd(PlayerMgr.Instance.GetWarrior(),null);
//    }

//    public void SetState(string clipName)
//    {
//        PlayerAnimState state = (PlayerAnimState)Enum.Parse(typeof(PlayerAnimState), clipName);
//        if (!animationClipSpeed.ContainsKey(state))
//            animationClipSpeed[state] = 1;

//        SetAnimatorSpeed(animationClipSpeed[state]);
//        SetState(state);
//    }
//    public void TestSetTime()
//    {
//        GetAnimator().Play("Attack", 0, 0.8f);//可用于强行控制播放时间到达第几秒
//    }
//    public PlayerAnimState GetPlayerAnimState()
//    {
//        return this.playerState;
//    }
//    public void SetState(PlayerAnimState state)
//    {
//        this.playerState = state;
//    }
//    public void SetAnimatorSpeed(PlayerAnimState state, float value)
//    {
//        if (!animationClipSpeed.ContainsKey(state))
//            animationClipSpeed[state] = 1;

//        animationClipSpeed[state] = value;

//        //如果当前动画就是被修改了速度的动画，就直接进行实际修改
//        AnimatorClipInfo[] clips = GetAnimator().GetCurrentAnimatorClipInfo(0);
//        string clipName = clips[0].clip.name;
//        if (clipName == state.ToString())
//            SetAnimatorSpeed(value);
//    }


//    public void Test()
//    {
//        //RemoveAnimationEvent("Attack", EventTriggerEnum.EventTriggerOne, Attack);
//    }

//    public void Attack()
//    {
//        //playerBattleController.NormalAttackController();
//    }

//    private void FlashFX()
//    {
//        InputMgr.GetInstance().AddForbidPlayerInput_Time("BackCity", 0.5f);
//        PoolMgr.GetInstance().GetObj("FX/Flash", (obj) =>
//        {
//            if(obj != null)
//                obj.transform.position = GetAnimator().transform.position;

//            MonoMgr.GetInstance().DelayAction(1f, () =>//生成的特效1秒后返回
//            {
//                if(obj != null)//切换场景以后物体会不存在，那样直接跳过
//                    PoolMgr.GetInstance().PushObj(obj);
//            });
//        });
//    }

//    public void IdleAnim()
//    {
//        if (GetAnimator() == null)
//            return;
//        GetAnimator().SetBool("IsAttack", false);
//        GetAnimator().SetBool("IsWalk", false);
//    }

//    public void WalkAnim()
//    {
//        if (GetAnimator() == null)
//            return;
//        GetAnimator().SetBool("IsWalk", true);
//        GetAnimator().SetBool("IsAttack", false);
//    }

//    public void AttckAnim()
//    {
//        if (GetAnimator() == null)
//            return;
//        GetAnimator().SetBool("IsWalk", false);
//        GetAnimator().SetBool("IsAttack", true);
//    }

//    public void SetBackCityAnim(bool isTrue)
//    {
//        if (GetAnimator() == null)
//            return;
//        GetAnimator().SetBool("BackCity", isTrue);
//    }
//}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimState
{
    Idle,
    Walk,
    Attack,
    Skill_Physics_1,
    Skill_Magic_1,
    Skill_Magic_2_1,
    Skill_Magic_2_2,
    Skill_Magic_2_3,
    BackCity,
    UseProp,
    Die,
}
public class PlayerAnimatorController : WarriorAnimatorController
{
    private PlayerBattleController playerBattleController;
    public PlayerAnimatorController(PlayerBattleController playerBattleController) : base(playerBattleController,playerBattleController, "Player")
    {
        this.playerBattleController = playerBattleController;

        EventCenter.GetInstance().AddEventListener<GameObject>(EventCenterEnum.创建玩家.ToString(), (GameObject Obj) =>
        {
            CreateAnimController("Player");
            SetAnimationEvent();
        });
    }
    protected override void SetAnimationEvent()
    {
        base.SetAnimationEvent();
        AddAnimationEvent("BackCity", EventTriggerEnum.EventTriggerOne, 1.37f, FlashFX);
    }
    private void TriggerSkill()
    {
        SkillController skillController = PlayerMgr.Instance.GetSkillController();
        Skill skill = skillController.GetSkill("寒冰射线").skill;
        skill.OnTriggerEnd(PlayerMgr.Instance.GetWarrior(), null);
    }

    public PlayerAnimState GetPlayerAnimState()
    {
        return (PlayerAnimState)System.Enum.Parse(typeof(PlayerAnimState), GetCurrentAnimation());//(PlayerAnimState)GetCurrentAnimation();
    }

    public void TestSetTime()
    {
        GetAnimator().Play("Attack", 0, 0.8f);//可用于强行控制播放时间到达第几秒
    }


    private void FlashFX()
    {
        InputMgr.GetInstance().AddForbidPlayerInput_Time("BackCity", 0.5f);
        PoolMgr.GetInstance().GetObj("FX/Flash", (obj) =>
        {
            if (obj != null)
                obj.transform.position = GetAnimator().transform.position;

            MonoMgr.GetInstance().DelayAction(1f, () =>//生成的特效1秒后返回
            {
                if (obj != null)//切换场景以后物体会不存在，那样直接跳过
                    PoolMgr.GetInstance().PushObj(obj);
            });
        });
    }

    public void IdleAnim()
    {
        if (GetAnimator() == null)
            return;
        GetAnimator().SetBool("IsAttack", false);
        GetAnimator().SetBool("IsWalk", false);
    }

    public void WalkAnim()
    {
        if (GetAnimator() == null)
            return;
        GetAnimator().SetBool("IsWalk", true);
        GetAnimator().SetBool("IsAttack", false);
    }

    public void AttckAnim()
    {
        if (GetAnimator() == null)
            return;
        GetAnimator().SetBool("IsWalk", false);
        GetAnimator().SetBool("IsAttack", true);
    }

    public void SetBackCityAnim(bool isTrue)
    {
        if (GetAnimator() == null)
            return;
        GetAnimator().SetBool("BackCity", isTrue);
    }
}
