using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WarriorAnimatorController : MyAnimatorController
{
    protected Warrior warrior;
    public WarriorAnimatorController(Warrior warrior, IAnimtorAble animAble, string objType) : base(animAble, objType)
    {
        this.warrior = warrior;
    }

    protected override float GetBaseSpeed()
    {
        if (warrior == null)
            return base.GetBaseSpeed();
        if (GetCurrentAnimation() == MyAnimatorController.NormalAttackClipName)
            return warrior.GetCurrentProperty(PropertyType.¹¥ËÙ);

        return base.GetBaseSpeed();
    }
}
