using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MazeObj : MonoBehaviour, IPresetObjAble,ISavePresetObjAble
{
    public static string smallMapLayerMaskName = "SmallMapObject";
    protected BoxCollider2D coll;
    private Sprite smallMapObjSprite;
    protected GameObject smallMapObj;
    protected SceneSpecialObjSaveInfo sceneSpecialObjSaveInfo = new SceneSpecialObjSaveInfo();
    //protected bool isDestory;

    public void ClearRuntimeData()
    {
        
    }
    private void Awake()
    {
        TryGetComponent<BoxCollider2D>(out coll);
        if (coll == null)
            coll = gameObject.AddComponent<BoxCollider2D>();

        coll.isTrigger = true;

        coll.size = CollSize();
        coll.offset = CollOffset();

        smallMapObj = CreateSmallMapObj(transform,GetSmallMapSprite());
    }

    public virtual string GetSmallMapSpritePath()
    {
        return "";
    }
    protected Sprite GetSmallMapSprite()
    {
        if(smallMapObjSprite == null)
        {
            string path = GetSmallMapSpritePath();
            if (path == "" || path == null)
                return smallMapObjSprite;
            smallMapObjSprite = ResMgr.GetInstance().Load<Sprite>(path);
        }

        return smallMapObjSprite;
    }
    public static GameObject CreateSmallMapObj(Transform trans,Sprite smallMapSprite)
    {
        if (smallMapSprite == null)
            return null;
        GameObject obj = new GameObject("SmallMapObj");
        Vector3Int gridPos = ToolsMgr.GetGirdPos(trans.position);
        obj.transform.SetParent(trans);
        obj.transform.position = gridPos + new Vector3(0.5f, 0.5f, 0);
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        obj.layer = LayerMask.NameToLayer(smallMapLayerMaskName);
        sr.sprite = smallMapSprite;
        sr.sortingOrder = 10;
        return obj;
    }
    public BoxCollider2D GetBoxCollider2D()
    {
        return coll;
    }
    protected virtual Vector2 CollSize()
    {
        return Vector2.one * 0.5f;
    }
    protected virtual Vector2 CollOffset()
    {
        return Vector2.zero;
    }
    public virtual bool IsAstarEvent()
    {
        return true;
    }
    public virtual bool PresetEventTrigger(IPlayerAble player)
    {
        return true;
    }
    public virtual bool IsSaveOnChangeScene()
    {
        return false;
    }
    public virtual string GetSaveTag()
    {
        //Debug.Log(GetType()+":尚未重写保存时的Tag,请注意");
        return GetType().ToString();//treasureBoxType.ToString();
    }
    public virtual string GetSaveContentInfo()
    {
        string content = JsonConvert.SerializeObject(sceneSpecialObjSaveInfo);
        return content;
    }
    public virtual void LoadSaveInfo(string content)
    {
        sceneSpecialObjSaveInfo = (SceneSpecialObjSaveInfo)JsonConvert.DeserializeObject(content, typeof(SceneSpecialObjSaveInfo));

        if (sceneSpecialObjSaveInfo.isDestory == true)
        {
            MazeGridMgr.ReleaseGrid(transform.position);
            GameObject.Destroy(gameObject);
        }
    }

}
