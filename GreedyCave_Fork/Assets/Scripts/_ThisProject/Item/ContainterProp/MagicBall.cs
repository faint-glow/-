using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITemporaryItemAble
{
    public IHolderAble GetMyHolder();
}
public class MagicBall : ContainerProp, ITemporaryItemAble
{
    public MagicBall(MagicBallInfo magicBallInfo, ItemQuality itemQuality, int count) :
        base(magicBallInfo, itemQuality, count)
    {

    }
    public MagicBall(MagicBall magicBall, int count)//用于复制
       : base(magicBall, count)
    {
        
    }
    public override void SetDetailButtons()
    {
        MagicBallButtons();
    }
    private void MagicBallButtons()
    {
        detailButtons[DetailButtonType.Up] = null;
        detailButtons[DetailButtonType.Middle] = null;
        detailButtons[DetailButtonType.Down] = new DetailButton("使用", Color.white, () =>
        {
            UseProp(UseMe);

            UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
            UIManager.GetInstance().HidePanel(BagPanel.prefabsPath);
        });
    }
    private void UseMe()
    {
        UIManager.GetInstance().ShowPanelAsync<BoxPanel>(BoxPanel.prefabsPath, PanelLayer.Top, null, (panel) =>
        {
            ContainerProp showItem = this;
            panel.SetBox(showItem);
        });
    }
    //注意：魔法球是Item，所以他有主人，同时，他自己也有背包效果，所以他自己也是一个主人，这里返回的是ITemporaryItemAble所需要的自己的主人
    public IHolderAble GetMyHolder()
    {
        return holder;
    }

    public override Dictionary<string, string> GetForbidItemDic()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>() 
        {
            { "回城卷轴", "该物品不可存入魔法球" },
            { "小生命药剂", "该物品不可存入魔法球" },
            { "小魔法药剂", "该物品不可存入魔法球" },
            { "肉块", "该物品不可存入魔法球" },
        };
        return dic;
    }
}
