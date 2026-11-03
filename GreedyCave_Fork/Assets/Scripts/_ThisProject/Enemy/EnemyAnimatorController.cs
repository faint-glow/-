using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : WarriorAnimatorController
{
    //方向控制
    private Enemy enemy;
    private Vector3 originalScale;
    private int direction = Warrior.left;//一开始的方向是朝向左边的
    private string objType;
    public EnemyAnimatorController(Enemy enemy,string objType):base(enemy,enemy,objType)
    {
        this.enemy = enemy;

        this.objType = objType;
        this.originalScale = enemy.GetAnimator().transform.localScale;
    }

    public void StartBattle()
    {
        if (enemy == null || enemy.GetAnimator() == null)
            return;
        enemy.GetAnimator().SetBool("IsAttack", true);
    }
    public void EndBattle()
    {
        if (enemy == null || enemy.GetAnimator() == null)
            return;
        enemy.GetAnimator().SetBool("IsAttack", false);
        enemy.GetAnimator().Play("Idle");
    }
    //传入目标位置，自动确定方向
    public void DirControl(Vector3 enemyPos, Vector3 playerPos)
    {
        int dir = Warrior.GetDir(enemyPos, playerPos);
        if (dir == 0)
            dir = direction;
        SetDir(dir);
    }

    private void SetDir(int dir)
    {
        direction = dir;
        Warrior.SetDir(enemy, originalScale, dir);
    }

    public int GetDir()
    {
        return direction;
    }
}
