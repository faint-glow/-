using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MaskPanel : MapPanel
{
    public static string prefabsPath = "MaskPanel";

    private Button button;
    public override void ShowMeFirst()
    {
        button = ThisPanel.GetUI<Button>();
    }
    public override void ShowMeEveryTime()
    {
        (transform as RectTransform).offsetMax = Vector2.zero;
        (transform as RectTransform).offsetMin = Vector2.zero;
    }
    public void SetClickEvent(Action<MaskPanel> action,params List<Action>[] hideMaskActions)
    {
        if(hideMaskActions != null)
        {
            for (int i = 0; i < hideMaskActions.Length; i++)
            {
                //int index = i;
                List<Action> actions = hideMaskActions[i];
                Action hideAction = null;
                hideAction = () =>
                {
                    HideThisPanel();
                    actions.Remove(hideAction);
                };
                actions.Add(hideAction);
            }
        }

        this.button.onClick.RemoveAllListeners();
        this.button.onClick.AddListener(()=> 
        {
            if(action != null)
                action.Invoke(this);
            if(hideMaskActions != null && hideMaskActions.Length >=0)
                HideThisPanel();
        });
    }

    private void HideThisPanel()
    {
        UIManager.GetInstance().HidePoorPanel(MaskPanel.prefabsPath, this);
    }
}
