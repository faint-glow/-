using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//UI层级枚举
public enum PanelLayer
{
    Bot_Down,
    Bot,
    Bot_Up,

    Mid_Down,
    Mid,
    Mid_Up,

    Top_Down,
    Top,
    Top_Up,

    Max_Down,
    Max,
    Max_Up,
}


//UI管理器（管理面板）
//管理所有显示的面板
//提供给外部 显示和隐藏
public class UIManager : BaseManager<UIManager>
{
    private BasePanel basePanel;
    private PoolPanel poorPanel;

    //这是几个UI面板
    private Transform bot_Down;
    private Transform bot;
    private Transform bot_Up;

    private Transform mid_Down;
    private Transform mid;
    private Transform mid_Up;

    private Transform top_Down;
    private Transform top;
    private Transform top_Up;

    private Transform max_Down;
    private Transform max;
    private Transform max_Up;

    public UIManager()
    {
        //去找Canvas（做成了预设体在Resources/UI下面）
        GameObject obj = ResMgr.GetInstance().Load<GameObject>("UI/Canvas");
        basePanel = obj.AddComponent<BasePanel>();
        basePanel.InitialForUIManager();
        poorPanel = obj.AddComponent<PoolPanel>();

        Transform canvas = obj.transform;
        //创建Canvas，让其过场景的时候不被移除
        GameObject.DontDestroyOnLoad(canvas);

        //找到各层
        bot_Down = basePanel.GetPanel("Bot_Down").transform;
        bot = basePanel.GetPanel("Bot").transform;
        bot_Up = basePanel.GetPanel("Bot_Up").transform;

        mid_Down = basePanel.GetPanel("Mid_Down").transform;
        mid = basePanel.GetPanel("Mid").transform;
        mid_Up = basePanel.GetPanel("Mid_Up").transform;

        top_Down = basePanel.GetPanel("Top_Down").transform;
        top = basePanel.GetPanel("Top").transform;
        top_Up = basePanel.GetPanel("Top_Up").transform;

        max_Down = basePanel.GetPanel("Max_Down").transform;
        max = basePanel.GetPanel("Max").transform;
        max_Up = basePanel.GetPanel("Max_Up").transform;

        obj = GameObject.Find("EventSystem");
        if (obj == null)
        {
            //加载EventSystem，有了它，按钮等组件才能响应
            obj = ResMgr.GetInstance().Load<GameObject>("UI/EventSystem");
        }

        //创建Canvas，让其过场景的时候不被移除
        GameObject.DontDestroyOnLoad(obj);
    }
    public MapPanel GetExistPanel(string name)
    {
        return basePanel.GetExistPanel(name);
    }
    //受上一条发出的GameTip影响，会间隔一定的时间发出下一条GameTip
    public void ShowTips(string content)
    {
        GameTipMgr.GetInstance().AddTipsToQue(content);
    }
    //立即发出GameTip
    public void ShowTip_Instant(string content)
    {
        Transform father = GetFather(PanelLayer.Max);
        poorPanel.ShowPoorPanelAsync<GameTip>(GameTip.prefabsPath, father, null, (panel) => { panel.SetContent(content); });
    }
    public bool IsShow(string name)
    {
        return basePanel.IsShow(name);
    }
    public MapPanel GetShowPanel(string name)
    {
        return basePanel.GetShowPanel(name);
    }
    //ShowPoorPanelAsync与ShowPanelAsync的区别在于这个可以展示名字相同的面板，
    public void ShowPoorPanelAsync<T>(string panelName, PanelLayer layer = PanelLayer.Top, UnityAction<T> firstTimeCallback = null, UnityAction<T> everyTimeCallBack = null) where T : MapPanel
    {
        Transform father = GetFather(layer);
        poorPanel.ShowPoorPanelAsync<T>(panelName, father, firstTimeCallback, everyTimeCallBack);
    }
    //隐藏面板
    public void HidePoorPanel(string panelName, MapPanel mapPanel)//此方法会让panel回到池中
    {
        poorPanel.HidePanel(panelName, mapPanel);
    }
    public void DestroyPoorPanel(string panelName, MapPanel mapPanel)
    {
        poorPanel.DestoryPanel(panelName, mapPanel);
    }
    public void HideTips(MapPanel mapPanel)
    {
        poorPanel.HidePanel(GameTip.prefabsPath, mapPanel);
    }

    public void ShowPanel<T>(string panelName, PanelLayer layer = PanelLayer.Top, UnityAction<T> firstTimeCallback = null, UnityAction<T> everyTimeCallBack = null) where T : MapPanel
    {
        Transform father = GetFather(layer);
        basePanel.ShowPanel(panelName, father, firstTimeCallback, everyTimeCallBack);
    }

    public void ShowPanelAsync<T>(string panelName, PanelLayer layer = PanelLayer.Top, UnityAction<T> firstTimeCallback = null, UnityAction<T> everyTimeCallBack = null) where T : MapPanel
    {
        Transform father = GetFather(layer);
        basePanel.ShowPanelAsync(panelName, father, firstTimeCallback, everyTimeCallBack);
    }
    public void HideAllBasePanel(HashSet<string> breakPanel = null)
    {
        basePanel.HideAllBasePanel(breakPanel);
    }
    private Transform GetFather(PanelLayer layer)
    {
        Transform father = bot;
        switch (layer)
        {
            case PanelLayer.Bot_Down:
                father = bot_Down;
                break;
            case PanelLayer.Bot:
                father = bot;
                break;
            case PanelLayer.Bot_Up:
                father = bot_Up;
                break;

            case PanelLayer.Mid_Down:
                father = mid_Down;
                break;
            case PanelLayer.Mid:
                father = mid;
                break;
            case PanelLayer.Mid_Up:
                father = mid_Up;
                break;

            case PanelLayer.Top_Down:
                father = top_Down;
                break;
            case PanelLayer.Top:
                father = top;
                break;
            case PanelLayer.Top_Up:
                father = top_Up;
                break;

            case PanelLayer.Max_Down:
                father = max_Down;
                break;
            case PanelLayer.Max:
                father = max;
                break;
            case PanelLayer.Max_Up:
                father = max_Up;
                break;

            default:
                break;
        }
        return father;
    }

    /// <summary>
    /// 仅调用面板HideMe方法，不删除
    /// </summary>
    /// <param name="panelName"></param>
    public void HidePanel(string panelName)
    {
        basePanel.HidePanel(panelName);
    }
    //隐藏面板
    public void DestroyPanel(string panelName)
    {
        basePanel.DestroyPanel(panelName);
    }

    public static void InitialPropertyImage(Image image)
    {
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;
        image.fillOrigin = 0;
    }
    public static void InitialAbandonButton(MapPanel mapPanel,string buttonPath)
    {
        Button targetButton = mapPanel.GetPanel(buttonPath).GetUI<Button>();
        targetButton.onClick.RemoveAllListeners();
        targetButton.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            UIManager.GetInstance().ShowTips("暂未开放");
        });
    }
    public static void SetPropertyPanel(Warrior warrior, PropertyType valueMaxType, PropertyType valueCurrentType, Image targetImage, TMP_Text targetText, bool ratio = false)
    {
        float valueMax = warrior.GetCurrentProperty(valueMaxType,null);
        float valueCurrent = warrior.GetCurrentProperty(valueCurrentType,null);

        targetImage.fillAmount = valueCurrent / valueMax;

        if(ratio == false)
            targetText.text = ((int)valueCurrent).ToString();
        else
            targetText.text = ((int)valueMax).ToString() + "/" + ((int)valueCurrent).ToString();

    }
}
