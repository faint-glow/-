using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IManyGridObj
{
    //当一个物体占用多个格子，并且玩家点击中间被其他格子包围的区域时，传入玩家当前位置，返回一个让玩家可以触发事件的边缘格子
    public Vector3 GetTriggerPos(Vector3 startPos);
}
public class BossObj : EnemyObj, IManyGridObj
{
    [SerializeField] private EnemyInfo enemyInfo;
    private Boss boss;
    protected void Start()
    {
        boss = new Boss(null, null, enemyInfo, this.gameObject);
        boss.LevelUp(GameManager.Instance.GetMapInfo().LayerCount, 0, true);

        if (SceneController.Instance != null)
            SceneController.Instance.LoadSaveDestoryObjInfo(this);
    }
    public override string GetSaveTag()
    {
        return boss.GetName();
    }
    public void Die()
    {
        this.sceneSpecialObjSaveInfo.isDestory = true;
        if (SceneController.Instance != null)
            SceneController.Instance.SaveDestoryObjInfo(this);
    }
    public Vector3 GetTriggerPos(Vector3 startPos)
    {
        return GetManyGridTriggerPos(startPos, transform.position, CollSize(), CollOffset());
    }
    /// <summary>
    /// 当一个物体占用多个格子，并且是事件类型的物体，同时玩家点击的位置是被包围的中间位置，返回一个可以触发事件的边缘位置
    /// </summary>
    /// <param name="startPos">玩家当前位置</param>
    /// <param name="objPos">占用多个格子的物体的位置</param>
    /// <param name="collSize">占用多个格子的物体的碰撞体大小</param>
    /// <param name="collOffset">占用多个格子的物体的碰撞体偏移</param>
    /// <returns></returns>
    public static Vector3 GetManyGridTriggerPos(Vector3 startPos, Vector3 objPos , Vector2 collSize, Vector3 collOffset)
    {
        //Vector2 collSize = CollSize();
        //Vector3 collOffset = CollOffset();

        Vector3 originalPos = objPos + collOffset;
        int halfWideth = (int)(collSize.x / 2);
        int halfHeight = (int)(collSize.y / 2);

        int xDir = startPos.x > originalPos.x ? 1 : -1;
        int yDir = startPos.y > originalPos.y ? 1 : -1;

        Vector3 targetVector3 = originalPos;
        Vector3 vec3 = originalPos;

        for (int i = 0; i < halfWideth + 1; i++)
        {
            vec3.Set(originalPos.x + i * xDir, originalPos.y + halfHeight * yDir, 0);
            if ((vec3 - startPos).sqrMagnitude < (targetVector3 - startPos).sqrMagnitude)
                targetVector3 = vec3;
        }
        for (int i = 0; i < halfHeight + 1; i++)
        {
            vec3.Set(originalPos.x + halfWideth * xDir, originalPos.y + i * yDir, 0);

            if ((vec3 - startPos).sqrMagnitude < (targetVector3 - startPos).sqrMagnitude)
                targetVector3 = vec3;
        }

        return targetVector3;
    }
    protected override Vector2 CollSize()
    {
        return new Vector2(2.4f, 2.4f);
    }
    protected void OnDestroy()
    {
        Vector3 collOffset = CollOffset();
        Vector3 collSize = CollSize();

        int x = (int)(transform.position.x + collOffset.x);
        int y = (int)(transform.position.y + collOffset.y);

        int halfWideth = (int)(collSize.x / 2);
        int halfHeight = (int)(collSize.y / 2);

        for (int i = -halfWideth; i <= halfWideth; i++)
        {
            for (int j = -halfHeight; j <= halfHeight; j++)
            {
                AStarMgr.Instance.SetNodeType(x + i, y + j, E_Node_Type.Walk);
            }
        }
    }
    //private void OnDestroy()
    //{
    //    int x = (int)transform.position.x;
    //    int y = (int)transform.position.y;
    //    for (int i = -1; i <= 1; i++)
    //    {
    //        for (int j = -1; j <= 1; j++)
    //        {
    //            AStarMgr.Instance.SetNodeType(x + i, y + j,E_Node_Type.Walk);
    //        }
    //    }
    //}
}
