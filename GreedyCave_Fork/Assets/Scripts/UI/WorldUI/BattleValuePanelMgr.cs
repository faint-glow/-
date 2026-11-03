using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleValueType
{
    Damage,
    AddState,
}
public class BattleValueInfo
{
    public BattleValueInfo(Warrior warrior, Vector3 offset, float value, Color color, int dir, int textSize = 36)
    {
        this.battleValueType = BattleValueType.Damage;
        this.warrior = warrior;
        this.offset = offset;
        this.value = value;
        this.dir = dir;
        this.color = color;
        this.textSize = textSize;
    }
    public BattleValueInfo(Warrior warrior, Vector3 offset, float value, Color color)
    {
        this.battleValueType = BattleValueType.AddState;
        this.warrior = warrior;
        this.offset = offset;
        this.value = value;
        this.color = color;
    }
    private BattleValueType battleValueType;
    private Warrior warrior;
    private Vector3 offset;
    private float value;
    private int dir;
    private Color color;
    private int textSize;

    public void ShowBattleValue()
    {
        if (battleValueType == BattleValueType.Damage)
            ShowDamageValue();
        else
            ShowAddStateValue();
    }
    private void ShowDamageValue()
    {
        WorldUIManager.GetInstance().ShowWorldPoorPanel<BattleValuePanel>(BattleValuePanel.prefabsPath, null, (panel) =>
        {
            panel.ShowDamageValue(value, dir, color, warrior.GetWarriorPos() + offset, textSize);
        });
    }
    private void ShowAddStateValue()
    {
        WorldUIManager.GetInstance().ShowWorldPoorPanel<BattleValuePanel>(BattleValuePanel.prefabsPath, null, (panel) =>
        {
            panel.ShowAddStateValue(value, color, warrior.GetWarriorPos() + offset);
        });
    }
}
public class BattleValueInfoHandler
{
    public float currentTime = 10f;//一开始就大于间隔时间
    public float CurrentTime
    {
        get { return currentTime; }
        set { currentTime = value; }
    }
    private Queue<BattleValueInfo> battleValueInfos = new Queue<BattleValueInfo>();

    public void AddBattleValueInfo(BattleValueInfo battleInfo)
    {
        battleValueInfos.Enqueue(battleInfo);
    }
    public int GetCount()
    {
        return battleValueInfos.Count;
    }
    public void ShowBattleValue()
    {
        BattleValueInfo battleValueInfo = battleValueInfos.Dequeue();
        battleValueInfo.ShowBattleValue();
    }
}
public class BattleValuePanelMgr : BaseManager<BattleValuePanelMgr>
{
    private float baseIntervalTime = 0.05f;
    private float releaseWarriorTime = 5f;
    public BattleValuePanelMgr()
    {
        MonoMgr.GetInstance().AddUpdateListener(Update);
    }
    private Dictionary<Warrior, BattleValueInfoHandler> warriorBattleInfoDic_Damage = new Dictionary<Warrior, BattleValueInfoHandler>();
    private Dictionary<Warrior, BattleValueInfoHandler> warriorBattleInfoDic_AddState = new Dictionary<Warrior, BattleValueInfoHandler>();

    public void ShowBattleValue_Damage(Warrior warrior, Vector3 offset, float value, Color color, int dir, int textSize = 36)
    {
        BattleValueInfo battleValueInfo = new BattleValueInfo(warrior, offset, value, color, dir, textSize);
        if (!warriorBattleInfoDic_Damage.ContainsKey(warrior))
            warriorBattleInfoDic_Damage[warrior] = new BattleValueInfoHandler();

        warriorBattleInfoDic_Damage[warrior].AddBattleValueInfo(battleValueInfo);
    }
    public void ShowBattleValue_AddState(Warrior warrior, Vector3 offset, float value, Color color)
    {
        BattleValueInfo battleValueInfo = new BattleValueInfo(warrior, offset, value, color);
        if (!warriorBattleInfoDic_AddState.ContainsKey(warrior))
            warriorBattleInfoDic_AddState[warrior] = new BattleValueInfoHandler();

        warriorBattleInfoDic_AddState[warrior].AddBattleValueInfo(battleValueInfo);
    }
    private void Update()
    {
        OperiateWarriorBattleInfoDic(warriorBattleInfoDic_Damage);
        OperiateWarriorBattleInfoDic(warriorBattleInfoDic_AddState);
    }

    private void OperiateWarriorBattleInfoDic(Dictionary<Warrior, BattleValueInfoHandler> warriorBattleInfoDic)
    {
        List<Warrior> targetRemoveWarrior = new List<Warrior>();
        foreach (var item in warriorBattleInfoDic)
        {
            Warrior warrior = item.Key;
            BattleValueInfoHandler battleValueHandler = item.Value;
            battleValueHandler.CurrentTime += Time.deltaTime;
            if (battleValueHandler.GetCount() > 0 && battleValueHandler.CurrentTime >= baseIntervalTime)
            {
                battleValueHandler.CurrentTime = 0;
                battleValueHandler.ShowBattleValue();
            }
            if (battleValueHandler.GetCount() <= 0 && battleValueHandler.CurrentTime >= releaseWarriorTime)
                targetRemoveWarrior.Add(warrior);
        }
        for (int i = 0; i < targetRemoveWarrior.Count; i++)
        {
            warriorBattleInfoDic.Remove(targetRemoveWarrior[i]);
        }
    }

}
