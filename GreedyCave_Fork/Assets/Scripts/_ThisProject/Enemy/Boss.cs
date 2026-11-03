using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public Boss(MazeGrid grid, Transform father, EnemyInfo enemyInfo,GameObject enemyObj = null):base(grid, father, enemyInfo,enemyObj)
    {
        SetEnemyType(EnemyType.Boss);
    }
    protected override void OnDie()
    {
        BossObj bossObj = this.mazeObj as BossObj;
        if (bossObj != null)
            bossObj.Die();
    }
    //这将成为1的倍数，用于控制X方向的偏移
    protected override int GetBattlePosX()
    {
        return 2;
    }
    protected override float GetBattlePosYOne()
    {
        return -1f;
    }
    protected override float GetBattlePosYTwo()
    {
        return 0;
    }
}
