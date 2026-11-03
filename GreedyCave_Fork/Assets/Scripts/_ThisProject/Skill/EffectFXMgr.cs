using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OriginalRotate
{
    Right,
    Left,
    Up,
    Forward,
}
public enum FX_FollowType
{
    Static,
    Follow,
}
public enum FX_FirType
{
    Stay,
    Direction,
    MoveToTarget,
}
public enum ExistType
{
    Time,
    Exist_WithHolder,
}
[System.Serializable]
public class EffectFXMgr
{
    public List<FXInfo> fxInfos;

    [System.NonSerialized] private HashSet<GameObject> existObj = new HashSet<GameObject>();
    /// <summary>
    /// 触发特效
    /// </summary>
    /// <param name="applyInfo">施加信息，效果的施加者和被施加者</param>
    /// <param name="useTime">已经使用掉的时间，特效会用预设好的存活时间减去这个值作为存活时间</param>
    public void UseFX(ApplyInfo applyInfo,float useTime = 0)
    {
        if (fxInfos == null)
            return;
        for (int i = 0; i < fxInfos.Count; i++)
        {
            if (fxInfos[i].fxObj == null)
            {
                Debug.Log("存在buff效果含有未填入GameObject的FXInfo");
                continue;
            }
            int index = i;//特别注意，延时执行的方法，在方法开始执行后，i的值已经改变了
            MonoMgr.GetInstance().DelayAction(fxInfos[index].delayTime, ()=>
            {
                FXInfo fxInfo = fxInfos[index];
                GameObject obj = GameObject.Instantiate(fxInfo.fxObj);
                Warrior startWarrior = Warrior.GetApplyTarget(applyInfo, fxInfo.startPosType);
                Warrior targetWarrior = Warrior.GetApplyTarget(applyInfo, fxInfo.targetPosType);
                if (startWarrior == null)
                {
                    return;
                }
                this.OperiateFireType(fxInfo, obj, startWarrior, targetWarrior);
                if (fxInfo.followType == FX_FollowType.Follow)
                {
                    if (startWarrior.GetObj() != null)
                        obj.transform.SetParent(startWarrior.GetObj().transform);
                    else if (obj != null)
                    {
                        GameObject.Destroy(obj);
                        return;
                    }
                }

                AdjustTransform adjustTransform = fxInfos[index].startTransform;
                Vector3 offset = fxInfo.startTransform.offset;
                float originalDir = 1;
                if (fxInfo.originalRotate == OriginalRotate.Left)
                    originalDir = -1;
                if (fxInfo.isAutioOffsetDir)
                    offset.x *= startWarrior.GetDir();
                Vector3 scale = adjustTransform.scale;
                if (fxInfo.isAutioFlipDir)
                    scale.x *= startWarrior.GetDir() * originalDir;
                obj.transform.position += offset;//设置偏移位置
                obj.transform.Rotate(obj.transform.right, adjustTransform.rotation.x);//设置x方向的旋转
                obj.transform.Rotate(obj.transform.up, adjustTransform.rotation.y);//设置y方向的旋转
                obj.transform.Rotate(obj.transform.forward, adjustTransform.rotation.z);//设置z方向的旋转

                obj.transform.localScale = scale;//设置缩放

                if(!existObj.Contains(obj))
                    existObj.Add(obj);
                if (fxInfo.isNeverDie == false)
                {
                    MonoMgr.GetInstance().DelayAction(fxInfo.existTime - useTime, () =>
                    {
                        if (obj != null)
                            GameObject.Destroy(obj);
                        if (existObj.Contains(obj))
                            existObj.Remove(obj);
                    });
                }
            });
        }
    }
    public void DestoryAllFXObj()
    {
        foreach (var item in existObj)
        {
            GameObject obj = item;
            //foreach里不可以修改键和值，所以延时
            MonoMgr.GetInstance().DelayAction(Time.deltaTime, () =>
             {
                 if(obj != null)
                     GameObject.Destroy(obj);
                 if (existObj.Contains(obj))
                     existObj.Remove(obj);
             });
        }
    }
    private void OperiateFireType(FXInfo fxInfo, GameObject obj, Warrior startWarrior, Warrior targetWarrior)
    {
        if (fxInfo.fireType == FX_FirType.Stay)
            OperiateStay(fxInfo, obj, startWarrior, targetWarrior);
        else if (fxInfo.fireType == FX_FirType.Direction)
            OperiateDirection(fxInfo, obj, startWarrior, targetWarrior);
    }
    private void OperiateStay(FXInfo fxInfo, GameObject obj, Warrior startWarrior, Warrior targetWarrior)
    {
        if (startWarrior == null)
            return;
        obj.transform.position = startWarrior.GetWarriorPos();//设置基础位置

    }
    private void OperiateDirection(FXInfo fxInfo, GameObject obj, Warrior startWarrior, Warrior targetWarrior)
    {
        if (startWarrior == null || targetWarrior == null)
            return;
        Vector3 direction = (targetWarrior.GetWarriorPos() - startWarrior.GetWarriorPos()).normalized;
        obj.transform.position = startWarrior.GetWarriorPos();//设置基础位置
        
        if (fxInfo.originalRotate == OriginalRotate.Forward)
            obj.transform.forward = direction;
        else if(fxInfo.originalRotate == OriginalRotate.Right)
            obj.transform.right = direction;
        else if(fxInfo.originalRotate == OriginalRotate.Left)
            obj.transform.right = -direction;
        else if(fxInfo.originalRotate == OriginalRotate.Up)
            obj.transform.up = direction;
    }
}

[System.Serializable]
public class FXInfo:SelfAdaptationClassMgr
{
    public float delayTime = 0;
    public bool isAutioOffsetDir;
    public bool isAutioFlipDir;
    public bool autoFlipX;
    public bool autoFlipY;

    public FX_FollowType followType;
    public FX_FirType fireType;
    public OriginalRotate originalRotate;
    public ApplyTargetType startPosType;
    public ApplyTargetType targetPosType;
    public AdjustTransform startTransform;
    public Vector3 targetOffset;
    public GameObject fxObj;
    public bool isNeverDie;
    public float existTime;//存活时间
}
[System.Serializable]
public class AdjustTransform:SelfAdaptationClassMgr
{
    public Vector3 offset;
    public Vector3 rotation;
    public Vector3 scale = new Vector3(1,1,1);
}