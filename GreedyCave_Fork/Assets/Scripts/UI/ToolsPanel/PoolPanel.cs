using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PoolPanel : MapPanel
{
    Dictionary<string, List<MapPanel>> panelListDic = new Dictionary<string, List<MapPanel>>();

    public void ShowPoorPanelAsync<T>(string panelName, Transform father, UnityAction<T> firstTimeCallback = null, UnityAction<T> everyTimeCallBack = null) where T : MapPanel
    {
        if (panelListDic.ContainsKey(panelName) && panelListDic[panelName].Count > 0)
        {
            List<MapPanel> panelList = panelListDic[panelName];
            T panel = panelList[panelList.Count - 1] as T;
            while (panel == null && panelList.Count > 0)
            {
                panelList.RemoveAt(panelList.Count - 1);
                if(panelList.Count > 0)
                    panel = panelList[panelList.Count - 1] as T;
                else
                {
                    ShowPoorPanelAsync<T>(panelName, father, firstTimeCallback, everyTimeCallBack);
                    return;
                }
            }
            RectTransform rectTrans = panel.GetComponent<RectTransform>();
            panel.transform.SetParent(father);
            panel.transform.localScale = Vector3.one;

            Show(ShowType.Always, panel, everyTimeCallBack);
            panelList.RemoveAt(panelList.Count - 1);
        }
        else
        {
            ResMgr.GetInstance().LoadAsync<GameObject>("UI/" + panelName, (obj) =>
            {
                //InitialPanl(obj, father);//初始化UI信息
                obj.transform.SetParent(father);
                obj.transform.localScale = Vector3.one;

                T panel = obj.GetComponent<T>();//得到预设体身上的脚本（继承自MapPanel）
                if (!panelListDic.ContainsKey(panelName))
                    panelListDic[panelName] = new List<MapPanel>();//创建一个池。但是不添加内容。只有物体被隐藏才会回到池中被直接利用

                Show(ShowType.First, panel, firstTimeCallback);
                Show(ShowType.Always, panel, everyTimeCallBack);

            });
        }
    }

    //隐藏面板
    public void HidePanel(string panelName, MapPanel mapPanel)//此方法会让panel回到池中
    {
        if (panelListDic.ContainsKey(panelName))
        {
            panelListDic[panelName].Add(mapPanel);
            mapPanel.HideMeHandler();
            mapPanel.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("不存在" + panelName + "面板");
        }
    }

    //隐藏面板
    public void DestoryPanel(string panelName, MapPanel mapPanel)//此方法会直接删除该物体
    {
        if (!panelListDic.ContainsKey(panelName))
            return;

        List<MapPanel> panelList = panelListDic[panelName];
        panelList.Remove(mapPanel);//如果先被隐藏再被删除，就需要移除列表中的索引

        mapPanel.HideMeHandler();
        GameObject.Destroy(mapPanel.gameObject);
    }

}
