using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhysiologyState
{
    Normal,//正常
    Unwell,//不适
    Weak,//乏力
    Hunger,//饥饿
    Dying,//濒死
}

public class PhysiologyMgr : IEffectSourceAble
{
    //private static string physiologySprite_Food = "Sprite/ExtraSprite/饱腹度";
    private static Sprite physiologySprite_Food;
    public static Sprite PhysiologySprite_Food
    {
        get 
        {
            if (physiologySprite_Food == null)
                physiologySprite_Food = ResMgr.GetInstance().Load<Sprite>("Sprite/ExtraSprite/饱腹度");
            return physiologySprite_Food; 
        }
    }



    private Warrior warrior;
    private PhysiologyState currentState;

    private Buff unwell;
    private Buff wakeBuff;
    private Buff hungerBuff;
    private Buff dyingBuff;
    private Dictionary<PhysiologyState, Buff> defaultPhysiologyStateDic = new Dictionary<PhysiologyState, Buff>();

    private bool isStart = true;
    public PhysiologyMgr(Warrior warrior)
    {
        //如果该warrior不包含饱腹度上限，说明该warrior没有饱腹度系统
        if (warrior == null || !warrior.IsContainProperty(PropertyType.饱腹度上限))
        {
            Debug.Log("该warrior不包含饱腹度上限信息，请检查");
            return;
        }
        this.warrior = warrior;
        this.currentState = PhysiologyState.Normal;
        MonoMgr.GetInstance().AddUpdateListener(Update);
    }
    public void Start()
    {
        isStart = true;
    }
    public void End()
    {
        isStart = false;
    }
    public Buff UnwellBuff
    {
        get
        {
            if (unwell == null)
            {
                BuffInfo buffInfo = ResMgr.GetInstance().Load<BuffInfo>("Buff/PhysiologyBuff/1_不适");
                unwell = new Buff(buffInfo, new List<IEffectSourceAble>() { this });
            }
            return unwell;
        }
    }
    public Buff WakeBuff
    {
        get
        {
            if (wakeBuff == null)
            {
                BuffInfo buffInfo = ResMgr.GetInstance().Load<BuffInfo>("Buff/PhysiologyBuff/2_乏力");
                wakeBuff = new Buff(buffInfo, new List<IEffectSourceAble>() { this });
            }
            return wakeBuff;
        }
    }
    public Buff HungerBuff
    {
        get
        {
            if (hungerBuff == null)
            {
                BuffInfo buffInfo = ResMgr.GetInstance().Load<BuffInfo>("Buff/PhysiologyBuff/3_饥饿");
                hungerBuff = new Buff(buffInfo, new List<IEffectSourceAble>() { this });
            }
            return hungerBuff;
        }
    }
    public Buff DyingBuff
    {
        get
        {
            if (dyingBuff == null)
            {
                BuffInfo buffInfo = ResMgr.GetInstance().Load<BuffInfo>("Buff/PhysiologyBuff/4_濒死");
                dyingBuff = new Buff(buffInfo, new List<IEffectSourceAble>() { this });
            }
            return dyingBuff;
        }
    }
    public Dictionary<PhysiologyState, Buff> DefaultPhysiologyStateDic
    {
        get
        {
            if (defaultPhysiologyStateDic == null)
                defaultPhysiologyStateDic = new Dictionary<PhysiologyState, Buff>();

            if (defaultPhysiologyStateDic.Count <= 0)
            {
                defaultPhysiologyStateDic[PhysiologyState.Unwell] = UnwellBuff;
                defaultPhysiologyStateDic[PhysiologyState.Weak] = WakeBuff;
                defaultPhysiologyStateDic[PhysiologyState.Hunger] = HungerBuff;
                defaultPhysiologyStateDic[PhysiologyState.Dying] = DyingBuff;
            }
            return defaultPhysiologyStateDic;
        }
    }

    private void Update()
    {
        if (isStart == false)
        {
            if (DefaultPhysiologyStateDic.ContainsKey(currentState) && DefaultPhysiologyStateDic[currentState] != null)
            {
                warrior.RemoveBuff(DefaultPhysiologyStateDic[currentState]);
                currentState = PhysiologyState.Normal;
            }
            return;
        }
        if (warrior.IsDie())
            return;
        float adjustDeltaTime = GetAdjustDeltaTime(100, 5);//把100秒映射成5分钟
        warrior.ChangerProperty(PropertyType.饱腹度, -adjustDeltaTime);
        //Debug.Log(warrior.GetOriginalProperty(PropertyType.饱腹度));
        if (CheckState(PhysiologyState.Dying, float.MinValue, 0))
            return;
        if (CheckState(PhysiologyState.Hunger, 0, 10))
            return;
        else if (CheckState(PhysiologyState.Weak, 10, 30))
            return;
        else if (CheckState(PhysiologyState.Unwell, 30, 50))
            return;
        else if (CheckState(PhysiologyState.Normal, 50, float.MaxValue))
            return;
    }

    /// <summary>
    /// 获取把秒钟调整成分钟的deltime
    /// </summary>
    /// <param name="allSecond">需要调整的秒钟</param>
    /// <param name="targetMinute">目标调整成为的分钟</param>
    /// <returns></returns>
    private float GetAdjustDeltaTime(float allSecond, float targetMinute)
    {
        if (allSecond <= 0)//分母为0是没有意义的
            return Time.deltaTime;

        float minuteCount = allSecond / 60f;
        float ratio = targetMinute / minuteCount;
        return Time.deltaTime / ratio;
    }

    private bool CheckState(PhysiologyState state, float minLine, float maxLine)
    {
        float value = warrior.GetOriginalProperty(PropertyType.饱腹度);
        if (value > minLine && value <= maxLine && currentState != state)
        {
            if (DefaultPhysiologyStateDic.ContainsKey(currentState) && DefaultPhysiologyStateDic[currentState] != null)
            {
                warrior.RemoveBuff(DefaultPhysiologyStateDic[currentState]);
            }
            if (DefaultPhysiologyStateDic.ContainsKey(state) && DefaultPhysiologyStateDic[state] != null)
            {
                warrior.AddBuff(warrior, DefaultPhysiologyStateDic[state]);
            }
            currentState = state;
            return true;
        }
        return false;
    }

    public string GetName()
    {
        return "生理状态";
    }
}
