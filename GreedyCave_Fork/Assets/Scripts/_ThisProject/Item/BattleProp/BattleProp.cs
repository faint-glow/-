using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleProp : ShortcutItem, IEffectSourceAble, IShortcutAble
{
    public BattleProp(BattlePropInfo battlePropInfo, ItemQuality itemQuality, int count)
    : base(battlePropInfo, itemQuality, count)
    {
        InitialBattleProp(battlePropInfo);
    }
    public BattleProp(BattleProp battleProp, int count)
    : base(battleProp, count)
    {
        InitialBattleProp(battleProp.itemInfo as BattlePropInfo);
    }
    private void InitialBattleProp(BattlePropInfo battlePropInfo)
    {
        if (battlePropInfo != null && battlePropInfo.effects != null)
        {
            for (int i = 0; i < battlePropInfo.effects.Count; i++)
            {
                Effect effect = battlePropInfo.effects[i];
                Effect.AddEffect(effectDic, effect);
                effect.SetEffectSource(new List<IEffectSourceAble>() { this });
            }
        }
        this.itemBaseInfo = Effect.GetEffectDicInfoText(effectDic,"");
    }
    protected Warrior holderWarrior;
    protected Dictionary<TriggerType, List<Effect>> effectDic = new Dictionary<TriggerType, List<Effect>>();

    public override void SetDetailButtons()
    {
        base.SetDetailButtons();
        detailButtons[DetailButtonType.Middle] = null;
        detailButtons[DetailButtonType.Down] = new DetailButton("使用", Color.white, () =>
        {
            IPlayerAble player = holder as IPlayerAble;
            if (player == null)
                return;

            OnTriggerEnd(player.GetWarrior(), () =>
            {
                if (player is PlayerMgr)
                    (player as PlayerMgr).AddShortcutCD(this);
            });
            UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
        });
    }
    protected override bool OnTriggerEndAided(Warrior warrior)
    {
        if (PlayerStateCheck() == false)
            return false;
        if (!effectDic.ContainsKey(TriggerType.使用道具触发))
            return false;
        GetWarrior().StartWarriorBehaviour();
        EffectTrigger(TriggerType.使用道具触发, GetWarrior().GetRandomEnemyTarget());

        return true;
    }
    public void EffectTrigger(TriggerType type, Warrior target)
    {
        UseProp(() =>
        {
            List<Effect> newEffects = effectDic[type];
            for (int i = 0; i < newEffects.Count; i++)
            {
                newEffects[i].SetEffectSource(new List<IEffectSourceAble>() { this });
                newEffects[i].EffectTrigger(new ApplyInfo(GetWarrior(), target));
            }
            PlayerMgr.Instance.RemoveItem(this, 1);

            OnUseProp(target);
        });
    }

    protected virtual void OnUseProp(Warrior target)
    {
        
    }
}
