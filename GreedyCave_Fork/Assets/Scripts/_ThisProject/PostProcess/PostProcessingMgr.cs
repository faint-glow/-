using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

//private void AddMainCameraLayer(Scene scene)
//{
//    GameObject obj = CameraMgr.GetInstance().GetCamera().gameObject;
//    if (obj == null)
//        return;
//    PostProcessLayer postprocessLayer = obj.GetComponent<PostProcessLayer>();
//    if (postprocessLayer == null)
//    {
//        PostProcessResources postProcessResources = ResMgr.GetInstance().Load<PostProcessResources>("PostProcessing/PostProcessResources");
//        postprocessLayer = obj.AddComponent<PostProcessLayer>();
//        postprocessLayer.Init(postProcessResources);
//    }
//    postprocessLayer.volumeLayer = 1 << 8;
//}
public class PostProcessingMgr : BaseManager<PostProcessingMgr>
{
    public PostProcessingMgr()
    {
        MonoMgr.GetInstance().AddUpdateListener(Update);
        Volume.profile.TryGetSettings<Vignette>(out vignette);
        Volume.profile.TryGetSettings<ChromaticAberration>(out chromaticAberration);
    }
    private PostProcessVolume volume;
    public PostProcessVolume Volume
    {
        get
        {
            if (volume == null)
            {
                GameObject gameobject = ResMgr.GetInstance().Load<GameObject>("PostProcessing/MainPostProcessing");
                gameobject.transform.SetParent(GameManager.Instance.transform);//设置到主管理下方，切换场景时不删除
                volume = gameobject.GetComponent<PostProcessVolume>();
            }
            return volume;
        }
    }
    private Dictionary<string, float> sceneVignetteValueDic = new Dictionary<string, float>()
    {
        { SceneName.MazeScene,0.35f },
        { SceneName.BossRoom_1,0.35f },
        { SceneName.MainCity,0.2f },
        { SceneName.Lounge,0.25f },
        //{ SceneName.Start,0.35f },
    };
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;

    //private Color dyingVignetteColor = new Color(1, 0, 0, 1);
    private float dyingVignetteSpeed = 2;
    private float currentTime;
    private float lastValue;

    private bool isSmoothOpen = false;
    private bool isSmoothClose = false;

    private Action openEndAction;
    private Action closeEndAction;
    private void Update()
    {
        SmoothOpen();
        SmoothClose();

        AdjustVignette();
        AdjustChromaticAberration();
    }
    private float GetCurrentSceneValue()
    {
        string sceneName = SceneMgr.GetInstance().GetCurrentScene().name;
        if (!sceneVignetteValueDic.ContainsKey(sceneName))
            return sceneVignetteValueDic[SceneName.MainCity];

        return sceneVignetteValueDic[sceneName];
    }
    public void StartOpen(Action<Vignette> openStartAction,Action openEndAction)
    {
        if (isSmoothClose == true)
            return;
        isSmoothOpen = true;
        if (openStartAction != null)
            openStartAction.Invoke(this.vignette);
        this.openEndAction = openEndAction;
    }
    public void StartClose(Action closeEndAction)
    {
        if (isSmoothOpen == true)
            return;
        isSmoothClose = true;
        this.closeEndAction = closeEndAction;
    }
    private void SmoothOpen()
    {
        if (isSmoothOpen == false)
            return;
        else if(vignette.intensity.value <= GetCurrentSceneValue())
        {
            isSmoothOpen = false;
            if (this.openEndAction != null)
                this.openEndAction.Invoke();
            this.openEndAction = null;
            return;
        }
        vignette.intensity.value -= Time.deltaTime * 3;
    }
    private void SmoothClose()
    {
        if (isSmoothClose == false)
            return;
        if (vignette.intensity.value >= 1)
        {
            isSmoothClose = false;
            if (this.closeEndAction != null)
                this.closeEndAction.Invoke();
            this.closeEndAction = null;
            return;
        }
        vignette.intensity.value += Time.deltaTime * 3;
    }
    public void AdjustVignette()
    {
        if (isSmoothClose || isSmoothOpen)
        {
            return;
        }
        //vignette.intensity.value = 1.0f;
        //vignette.intensity.Override(1.0f);
        float hpUpLimit = PlayerMgr.Instance.GetWarrior().GetCurrentProperty(PropertyType.生命值上限);
        float hp = PlayerMgr.Instance.GetWarrior().GetOriginalProperty(PropertyType.生命值);
        float ratio = hp / hpUpLimit;

        if (ratio <= 0.3f)
        {
            currentTime += Time.deltaTime;
            float value = (1 - (ratio * 10.0f / 3.0f));
            float sinValue = Mathf.Sin(currentTime * dyingVignetteSpeed * (1 + 2 * value));
            vignette.color.value.r = (value / 2 + 0.5f) * sinValue;
        }
        else
        {
            vignette.color.value.r = 0;
        }
    }
    public void AdjustChromaticAberration()
    {
        float satiety = PlayerMgr.Instance.GetWarrior().GetOriginalProperty(PropertyType.饱腹度);

        if (satiety <= 50)
            chromaticAberration.intensity.value = Mathf.Clamp(1 - Mathf.Clamp(satiety / 50.0f, 0, 1), 0, 0.8f);
        else
            chromaticAberration.intensity.value = 0;
    }
}
