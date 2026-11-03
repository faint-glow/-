using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DecorateGrid : MonoBehaviour
{
    private static Sprite whiteGridSprite;
    public static Sprite WhiteGridSprite
    {
        get
        {
            if (whiteGridSprite == null)
                whiteGridSprite = ResMgr.GetInstance().Load<Sprite>("Decorate/White");
            return whiteGridSprite;
        }
    }
    private static Sprite blueGridSprite;
    public static Sprite BlueGridSprite
    {
        get
        {
            if (blueGridSprite == null)
                blueGridSprite = ResMgr.GetInstance().Load<Sprite>("Decorate/Blue");
            return blueGridSprite;
        }
    }
    private static Sprite redGridSprite;
    public static Sprite RedGridSprite
    {
        get
        {
            if (redGridSprite == null)
                redGridSprite = ResMgr.GetInstance().Load<Sprite>("Decorate/Red");
            return redGridSprite;
        }
    }

    SpriteRenderer sr;
    BoxCollider2D boxColl;
    private void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = gameObject.AddComponent<SpriteRenderer>();

        boxColl = gameObject.GetComponent<BoxCollider2D>();
        if (boxColl == null)
            boxColl = gameObject.AddComponent<BoxCollider2D>();

        gameObject.layer = LayerMask.NameToLayer("DecorateGrid");
        boxColl.isTrigger = true;
        boxColl.size = Vector2.one * 0.8f;

        sr.sortingOrder = 1;
    }

    private void OnEnable()//每次显示都要检查格子情况
    {
        IPresetObjAble presetObj = ToolsMgr.CheckPrestEvent<IPresetObjAble>(transform.position);
        if (presetObj == null && PlayerMgr.Instance.GetGirdPos() != ToolsMgr.GetGirdPos(transform.position))
            sr.sprite = WhiteGridSprite;
        else
            sr.sprite = RedGridSprite;
    }



    public bool IsAccessible()
    {
        if (sr.sprite == RedGridSprite)
            return false;
        else
            return true;
    }
    public void SetSprite_White()
    {
        sr.sprite = WhiteGridSprite;
    }
    public void SetSprite_Blue()
    {
        sr.sprite = BlueGridSprite;
    }
    public void SetSprite_Red()
    {
        sr.sprite = RedGridSprite;
    }
}
