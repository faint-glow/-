using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObj : MazeObj
{
    //protected MazeGrid grid;//敌人对应的迷宫格子
    private Enemy enemy;

    public void Initial(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public override string GetSmallMapSpritePath()
    {
        return "Sprite/SmallMapObj/Enemy";
    }

    protected override Vector2 CollOffset()
    {
        return new Vector2(0, 0.4f);
    }
    public override bool PresetEventTrigger(IPlayerAble player)
    {
        if (enemy != null)
            enemy.PresetEventTrigger(player);
        return true;
    }
}
