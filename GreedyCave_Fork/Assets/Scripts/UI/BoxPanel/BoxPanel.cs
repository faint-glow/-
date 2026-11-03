using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxPanel : CurrentBagPanel
{
    public static string prefabsPath = "BoxPanel";
    private Transform leftPanelFather;
    private Transform rightPanelFather;
    private ItemBagPanel itemBagPanel;
    private ContainerBagPanel boxBagPanel;
    public override void ShowMeFirst()
    {
        Button closeButton = GetPanel("ClosePanel").GetUI<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.close);
            UIManager.GetInstance().HidePanel(BoxPanel.prefabsPath);
        });
    }
    protected override BagPanelType GetBagPanelType()
    {
        return BagPanelType.BoxPanel;
    }
    public void SetBox(ContainerProp box)
    {
        rightPanelFather = GetPanel("Right/Main").transform;
        leftPanelFather = GetPanel("Left/Main").transform;

        ShowPanel<ContainerBagPanel>(ContainerBagPanel.prefabsPath_Bag, leftPanelFather);//先显示这个面板，那么这个面板会先被隐藏
        ShowPanel<ItemBagPanel>(ItemBagPanel.prefabsPath_Box, rightPanelFather,null,(panel) => { this.itemBagPanel = panel; });//等待物品背包被显示后，刷新过玩家背包，再显示其他背包进行移除魔法球类型的物品的操作
        ShowPanel<ContainerBagPanel>(ContainerBagPanel.prefabsPath_Bag, leftPanelFather, null, (panel) => { this.boxBagPanel = panel; panel.SetHolder(box); });
    }
    public override Dictionary<DetailButtonType, DetailButton> GetDetailButtons(Item item)
    {
        Dictionary<DetailButtonType, DetailButton> detailButton = new Dictionary<DetailButtonType, DetailButton>();
        if (item.holder.GetHolderType() == HolderType.Player)
        {
            detailButton[DetailButtonType.Up] = null;
            detailButton[DetailButtonType.Middle] = null;
            detailButton[DetailButtonType.Down] = new DetailButton("存入", Color.white, () =>
            {
                UIManager.GetInstance().ShowPanel<DepositPanel>(DepositPanel.prefabsPath,PanelLayer.Max,null,(panel)=> 
                {
                    panel.SetItem(boxBagPanel.GetHolder(),item);
                });

                UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
            });
        }
        else
        {
            detailButton[DetailButtonType.Up] = null;
            detailButton[DetailButtonType.Middle] = null;
            detailButton[DetailButtonType.Down] = new DetailButton("取出", Color.white, () =>
            {
                UIManager.GetInstance().ShowPanel<DepositPanel>(DepositPanel.prefabsPath, PanelLayer.Max, null, (panel) =>
                {
                    panel.SetItem(itemBagPanel.GetHolder(), item);
                });

                UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
            });
        }
        return detailButton;
    }

    public override string GetPanelName()
    {
        return BoxPanel.prefabsPath;
    }
}
