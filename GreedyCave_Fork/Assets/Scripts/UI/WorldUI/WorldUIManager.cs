using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldUIManager : BaseManager<WorldUIManager>
{
    private PoolPanel poorPanel;
    public PoolPanel _poorPanel
    {
        get
        {
            if(poorPanel == null)
            {
                GameObject obj = ResMgr.GetInstance().Load<GameObject>("UI/WorldCanvas");
                poorPanel = obj.AddComponent<PoolPanel>();
            }
            return poorPanel;
        }
    }


    public void ShowWorldPoorPanel<T>(string panelName, UnityAction<T> firstTimeCallback = null, UnityAction<T> everyTimeCallBack = null) where T:MapPanel
    {
        _poorPanel.ShowPoorPanelAsync(panelName, _poorPanel.transform, firstTimeCallback, everyTimeCallBack);
    }

    public void HideWorldPoorPanel(string panelName, MapPanel mapPanel)
    {
        _poorPanel.HidePanel(panelName, mapPanel);
    }
}
