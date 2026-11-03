using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SmallMapMgr : BaseManager<SmallMapMgr>
{
    private Camera camera;
    private Camera _Camera
    {
        get
        {
            if (camera == null)
            {
                GameObject cameraObject = new GameObject("SmallMapCamera");
                camera = cameraObject.AddComponent<Camera>();
                camera.cullingMask = (1 << 7);//只渲染第7层的小地图物体
                camera.orthographic = true;
                camera.orthographicSize = 5;
                camera.transform.position = new Vector3(0, 0, -10);
            }
            return camera;
        }
    }
    public SmallMapMgr()
    {
        EventCenter.GetInstance().AddEventListener<Scene>(EventCenterEnum.切换场景后.ToString(), (scene)=> 
        {
            if (scene.name == SceneName.MazeScene)
                ShowFolowSmallMap(PlayerMgr.Instance.GetPlayerObject());
            else
                HideFolowSmallMap();
        });
    }

    public void ShowFolowSmallMap(GameObject gameObject, int viewWideth  = 10, int viewheight = 10)
    {
        if (gameObject == null)
            return;
        HideGlobalSmallMap();
        UIManager.GetInstance().ShowPanelAsync<SmallMapPanel_Follow>(SmallMapPanel_Follow.prefabsPath, PanelLayer.Bot, null,
        (panel) =>
        {
            panel.SetTexture(GetFolowTexture(gameObject.transform, viewWideth, viewheight));
        });
    }
    public void HideFolowSmallMap()
    {
        UIManager.GetInstance().HidePanel(SmallMapPanel_Follow.prefabsPath);
    }

    public void ShowGlobalSmallMap(int viewWideth, int viewheight)
    {
        HideFolowSmallMap();
        UIManager.GetInstance().ShowPanelAsync<SmallMapPanel_Global>(SmallMapPanel_Global.prefabsPath, PanelLayer.Max_Down, null,
        (panel) =>
        {
            panel.SetTexture(GetGlobalTexture(viewWideth, viewheight));
        });
    }
    public void HideGlobalSmallMap()
    {
        UIManager.GetInstance().HidePanel(SmallMapPanel_Global.prefabsPath);
    }
    public RenderTexture GetFolowTexture(Transform transform, int viewWideth, int viewheight)
    {
        if (transform == null)
            return null;
        SetCameraSize(viewWideth, viewheight);
        RenderTexture renderTexture = new RenderTexture(512, 512, -1);

        _Camera.transform.SetParent(transform);
        _Camera.transform.localPosition = new Vector3(0,1f, _Camera.transform.localPosition.z);

        _Camera.targetTexture = renderTexture;
        return renderTexture;
    }
    /// <summary>
    /// 传入地图宽高，获取地图全景的Texture，注意，这里为了方便没有选择位置，以地图中心位中点
    /// </summary>
    /// <param name="mapWideth"></param>
    /// <param name="mapHeight"></param>
    /// <returns></returns>
    public RenderTexture GetGlobalTexture(int mapWideth, int mapHeight)
    {
        SetCameraSize(mapWideth, mapHeight);
        RenderTexture renderTexture = new RenderTexture(1024, 1024, -1);
        _Camera.transform.SetParent(null);
        _Camera.transform.position = new Vector3(mapWideth / 2 + 0.5f, mapHeight / 2 + 0.5f, _Camera.transform.position.z);
        _Camera.targetTexture = renderTexture;
        return renderTexture;
    }
    public void SetCameraSize(int mapWideth, int mapHeight,int textureWideth = 512, int textureHeight = 512)
    {
        if (mapHeight >= mapWideth)
        {
            _Camera.orthographicSize = mapHeight / 2f;
        }
        else
        {
            float textureRatio = (float)textureHeight / textureWideth;
            _Camera.orthographicSize = (float)mapWideth * textureRatio / 2f;
        }
    }
}
