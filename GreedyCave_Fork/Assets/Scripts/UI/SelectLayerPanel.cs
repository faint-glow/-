using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectLayerPanel : ContainerPanel<SelectLayerButton, SelectLayerButtonInfo>
{
    public static string prefabsPath = "SelectLayerPanel";

    private int maxLayer = 20;
    private Transform buttonFather;
    public override void ShowMeFirst()
    {
        Button clickButton = ThisPanel.GetUI<Button>();
        clickButton.onClick.AddListener(()=> 
        {
            HidePanel();
            AudioMgr.Instance.PlayEffect(AudioClipPath.close);
        });
        buttonFather = GetPanel("Scroll View/Viewport/Content").transform;
    }
    public override void ShowMeEveryTime()
    {
        Refresh();
    }
    public void HidePanel()
    {
        UIManager.GetInstance().HidePanel(SelectLayerPanel.prefabsPath);
    }
    private void Refresh()
    {
        List<SelectLayerButtonInfo> infos = new List<SelectLayerButtonInfo>();
        SelectLayerButtonInfo firstInfo = new SelectLayerButtonInfo();
        firstInfo.targetLayer = 1;
        infos.Add(firstInfo);
        int count = maxLayer / 10;
        for (int i = 0; i < count; i++)
        {
            SelectLayerButtonInfo info = new SelectLayerButtonInfo();
            info.targetLayer = (i+1) * 10 + 1;

            infos.Add(info);
        }
        RefreshContainer(infos);
    }

    public override string GetCellPath()
    {
        return SelectLayerButton.prefabsPath;
    }

    public override Transform GetContainterTrans()
    {
        return buttonFather;
    }

    public override IHolderAble GetHolder()
    {
        return null;
    }

    public override ScrollRect GetScrollRect()
    {
        return null;
    }

}
