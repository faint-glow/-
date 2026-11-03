using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IShowSpriteAble
{
    public Sprite GetSprite();
    public Vector3 CenterPosOffset();
    public Vector3 GetShowScale();
}
public class WorldSpritePanelMgr
{
    public static float ShowItems<T>(List<T> targets, Vector3 startPos,float intervalTime = 0.2f, Transform father = null, float existTime = 3f) where T: IShowSpriteAble
    {
        float delayTime = 0;
        if (targets == null)
            return delayTime;
        for (int i = 0; i < targets.Count; i++)
        {
            T target = targets[i];
            if (target == null)
                continue;
            MonoMgr.GetInstance().DelayAction(delayTime, ()=> 
            {
                ShowItem(target, startPos, father, existTime);
            });
            delayTime += intervalTime;
        }

        return delayTime;
    }
    public static void ShowItem<T>(T target, Vector3 startPos, Transform father = null, float existTime = 3f) where T : IShowSpriteAble
    {
        ShowSprite(target.GetSprite(), startPos + target.CenterPosOffset(), target.GetShowScale(), father, existTime);
    }

    public static void ShowSprite(Sprite sprite,Vector3 startPos,Vector3 scale,Transform father = null,float existTime = 3f)
    {
        if (sprite == null)
            return;
        GameObject obj = new GameObject(sprite.name);
        if (father != null)
            obj.transform.SetParent(father);
        obj.transform.localPosition = startPos;

        obj.transform.localScale = scale;
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerName = "Max";
        sr.sortingOrder = 1000;
        MonoMgr.GetInstance().StartCoroutine(Anim(obj, startPos + new Vector3(0,2,0)));
        MonoMgr.GetInstance().DelayAction(existTime,()=> 
        {
            if (obj != null)
                GameObject.Destroy(obj);
        });
    }

    static IEnumerator Anim(GameObject obj,Vector3 targetPos ,float speed = 3)
    {
        if (obj == null)
            yield break;
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = obj.AddComponent<SpriteRenderer>();

        Color targetColor = sr.color;
        float distance = (targetPos - obj.transform.position).magnitude;
        yield return new WaitUntil(()=> 
        {
            if (obj == null)
                return true;
            if (sr == null)
                return true;
            if (obj.transform.position == targetPos)
                return true;
            targetColor.a -= Time.deltaTime / distance * speed;
            sr.color = targetColor;
            BattleValuePanel.MoveToWorld(obj.transform, targetPos, speed);

            return false;
        });
        
    }
}
