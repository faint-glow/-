using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffButton : MapPanel, ISetAble<IGetAble>,ITipInfoAble
{
    public static string prefabsPath = "Button/BuffButton";

    private static Sprite[] buffButtonImage;
    public static Sprite[] BuffButtonImage
    {
        get 
        {
            if(buffButtonImage == null || buffButtonImage.Length <= 0)
                buffButtonImage = ResMgr.GetInstance().LoadAll<Sprite>("Sprite/BuffGrid");
            return buffButtonImage; 
        }
    }
    private Buff buff;
    private TMP_Text buffNameText;
    private Image buttonBack;
    private Image maskImage;
    public MapPanel FatherPanel { get; set; }
    public int Index { get; set; }
    public override void ShowMeFirst()
    {
        Button clickButton = ThisPanel.GetUI<Button>();
        clickButton.onClick.AddListener(()=> 
        {
            UIManager.GetInstance().ShowPanelAsync<TipInfoPanel>(TipInfoPanel.prefabsPath, PanelLayer.Top_Down,null,(panel)=> 
            {
                RectTransform thisRect = transform as RectTransform;
                RectTransform rectTrans = panel.transform as RectTransform;
                if (rectTrans == null)
                    return;
                Vector3[] worldCorners = new Vector3[4];
                thisRect.GetWorldCorners(worldCorners);
                float offsetX = (worldCorners[2].x - worldCorners[0].x) / 2;
                float offsetY = (worldCorners[2].y - worldCorners[0].y) / 2;

                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 300);
                rectTrans.position = transform.position + new Vector3(-offsetX, -offsetY,0);

                panel.SetContent(this);
            });
        });
        
        buffNameText = GetPanel("BuffName").GetUI<TMP_Text>();
        buttonBack = ThisPanel.GetUI<Image>();
        maskImage = GetPanel("Mask").GetUI<Image>();
    }
    public string GetTipInfo()
    {
        if (buff == null)
            return "";
        return buff.GetName() + "\n" + buff.GetBuffInfoText();
    }
    public override void ShowMeEveryTime()
    {
        buttonBack.sprite = BuffButtonImage[Random.Range(0, BuffButtonImage.Length - 1)];
    }
    private void Update()
    {
        if (buff == null)
            return;

        maskImage.fillAmount = buff.countdownTimerInfo.CurrentTime / buff.liveTime;
    }
    public void SetItem_Container(IGetAble obj)
    {
        Buff buff = obj as Buff;
        if (buff == null)
            return;

        this.buff = buff;
        this.Refresh();
    }

    private void Refresh()
    {
        this.buffNameText.text = buff.GetName();
    }
    public bool IsHideOnRepeatShow()
    {
        return true;
    }
}
