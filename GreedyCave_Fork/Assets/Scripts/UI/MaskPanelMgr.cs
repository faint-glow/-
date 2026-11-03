using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskPanelMgr:BaseManager<MaskPanelMgr>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="transform">可以遮挡该面板的物体</param>
    /// <param name="action">点击遮罩后执行的方法</param>
    /// <param name="index"></param>
    public void ShowMask(Transform transform,Action<MaskPanel> action,params List<Action>[] hideMaskAction)
    {
        UIManager.GetInstance().ShowPoorPanelAsync<MaskPanel>(MaskPanel.prefabsPath,PanelLayer.Max_Up,null,(panel)=> 
        {
            panel.SetClickEvent(action, hideMaskAction);
            if (transform.parent != null)
            {
                panel.transform.SetParent(transform.parent);
                panel.transform.SetSiblingIndex(transform.GetSiblingIndex());
            }
            else
            {
                Debug.Log("目标父物体为空");
            }
        });
    }

}
