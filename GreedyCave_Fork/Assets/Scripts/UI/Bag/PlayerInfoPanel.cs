using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerInfoPanel : MapPanel, ITipInfoAble
{
    public static string prefabsPath = "PlayerInfoPanel";
    private Warrior currentWarrior;
    private RawImage playerImage;
    private EquipPanel equipPanel;
    private Image hpImage;
    private Image mpImage;
    private Image expImage;
    private TMP_Text hpValueText;
    private TMP_Text mpValueText;
    private TMP_Text expValueText;

    public static HashSet<PropertyType> showPropertyDic = new HashSet<PropertyType>()
    {
        PropertyType.生命值上限,
        PropertyType.魔法值上限,
        PropertyType.攻击力,
        PropertyType.魔力,
        PropertyType.物理防御,
        PropertyType.魔法防御,
        PropertyType.暴击率,
        PropertyType.暴击效果,
        PropertyType.物理穿透,
        PropertyType.魔法穿透,
        PropertyType.攻速,
        PropertyType.闪避率,
        PropertyType.命中率,
        PropertyType.经验值上限,
        PropertyType.饱腹度上限,
    };
    public EquipPanel GetEquipPanel()
    {
        return equipPanel;
    }
    public override void ShowMeFirst()
    {
        equipPanel = GetPanel("Main/Equip").transform.GetComponent<EquipPanel>();
        playerImage = GetPanel("Main/PlayerUI/RawImage").GetUI<RawImage>();

        hpImage = GetPanel("Main/PlayerUI/Property/Hp/Image").GetUI<Image>();
        InitialImage(hpImage);
        mpImage = GetPanel("Main/PlayerUI/Property/Mp/Image").GetUI<Image>();
        InitialImage(mpImage);
        expImage = GetPanel("Main/Down/EXP/Image").GetUI<Image>();
        InitialImage(expImage);

        hpValueText = GetPanel("Main/PlayerUI/Property/Hp/Text (TMP) (1)").GetUI<TMP_Text>();
        mpValueText = GetPanel("Main/PlayerUI/Property/Mp/Text (TMP) (1)").GetUI<TMP_Text>();
        expValueText = GetPanel("Main/Down/EXP/Image/Text (TMP)").GetUI<TMP_Text>();

        UITools.AddEvent(playerImage, UnityEngine.EventSystems.EventTriggerType.PointerDown, PointerDown);
        //UITools.AddEvent(playerImage, UnityEngine.EventSystems.EventTriggerType.PointerUp, PointerUp);

        UIManager.InitialAbandonButton(this, "Main/ChangeClothes");
        UIManager.InitialAbandonButton(this, "Main/PlayerUI/ChangeDress");
        UIManager.InitialAbandonButton(this, "Main/PlayerUI/League");
        UIManager.InitialAbandonButton(this, "Main/PlayerUI/Undress");
    }
    private void InitialImage(Image image)
    {
        if (image == null)
            return;
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;
        image.fillOrigin = 0;
    }
    public override void ShowMeEveryTime()
    {
        RefreshAvata();
        RefreshState();
    }
    public void RefreshAvata()
    {
        playerImage.texture = PlayerMgr.Instance.GetDynamicAvatar();
    }
    public void RefreshState()
    {
        currentWarrior = PlayerMgr.Instance.GetWarrior();
        float maxHp = currentWarrior.GetCurrentProperty(PropertyType.生命值上限, null);
        float currentHp = currentWarrior.GetCurrentProperty(PropertyType.生命值, null);
        float maxMp = currentWarrior.GetCurrentProperty(PropertyType.魔法值上限, null);
        float currentMp = currentWarrior.GetCurrentProperty(PropertyType.魔法值, null);
        float maxExp = currentWarrior.GetCurrentProperty(PropertyType.经验值上限, null);
        float currentExp = currentWarrior.GetCurrentProperty(PropertyType.经验值, null);

        hpImage.fillAmount = currentHp / maxHp;
        mpImage.fillAmount = currentMp / maxMp;
        expImage.fillAmount = currentExp / maxExp;

        hpValueText.text = ((int)currentHp).ToString() + "/" + ((int)maxHp).ToString();
        mpValueText.text = ((int)currentMp).ToString() + "/" + ((int)maxMp).ToString();
        expValueText.text = (expImage.fillAmount * 100).ToString("0.0") + "%";

        TipInfoPanel panel = (TipInfoPanel)UIManager.GetInstance().GetShowPanel(TipInfoPanel.prefabsPath);
        if (panel != null)
            panel.RefreshContent();
    }
    private void PointerDown(BaseEventData baseEventData)
    {
        RefreshPropertyPanel();
    }
    private void RefreshPropertyPanel()
    {
        UIManager.GetInstance().ShowPanelAsync<TipInfoPanel>(TipInfoPanel.prefabsPath, PanelLayer.Top_Up, null, (panel) =>
        {
            RectTransform thisRect = playerImage.rectTransform;
            RectTransform rectTrans = panel.transform as RectTransform;
            if (rectTrans == null)
                return;
            Vector3[] worldCorners = new Vector3[4];
            thisRect.GetWorldCorners(worldCorners);
            float offsetX = (worldCorners[2].x - worldCorners[0].x) / 2;
            float offsetY = (worldCorners[2].y - worldCorners[0].y) / 2;

            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 300);

            rectTrans.position = playerImage.transform.position + new Vector3(-offsetX, offsetY, 0);//注意playerImage的x是被反转过的
            panel.SetContent(this);
        });
    }
    public override void HideMe()
    {
        PlayerMgr.Instance.AvatarUsingEnd();
        UIManager.GetInstance().HidePanel(TipInfoPanel.prefabsPath);
    }
    public string GetTipInfo()
    {
        string content = "";
        string currentProperty = "";
        if (currentWarrior != null)
        {
            foreach (var item in showPropertyDic)
            {
                if (currentProperty != "")
                    content += "\n";

                if (Property.RatioValueDic.Contains(item))
                    currentProperty = item + ":" + (currentWarrior.GetCurrentProperty(item) * 100).ToString("0.0") + "%";
                else
                    currentProperty = item + ":" + (currentWarrior.GetCurrentProperty(item)).ToString("0.0");//保留一位小数

                content += currentProperty;
            }
        }
        return content;
    }
    public bool IsHideOnRepeatShow()
    {
        return true;
    }
}
