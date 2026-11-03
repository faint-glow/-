using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EquipButton : MapPanel,IDragUpAble<IDragAble>
{
    [SerializeField] private WearEquipType equipType;
    [SerializeField] private int index = 0;
    private Equip equip;
    private Button button;

    private Image equipGridImage;
    private Image equipImage;
    private TMP_Text strengthenLevel;
    private TMP_Text lockedText;
    public override void ShowMeFirst()
    {
        equipImage = GetPanel("Image (1)").GetUI<Image>();
        equipGridImage = ThisPanel.GetUI<Image>();
        strengthenLevel = GetPanel("Text (TMP)").GetUI<TMP_Text>();
        lockedText = GetPanel("Text (TMP) (1)").GetUI<TMP_Text>();
        button = gameObject.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            if (equip == null)
                return;
            UIManager.GetInstance().ShowPanelAsync<DetailPanel>(DetailPanel.prefabsPath, PanelLayer.Top_Up, null,
            (panel) =>
            {
                panel.SetItem(equip);
            });
        });
    }
    public override void ShowMeEveryTime()
    {
        RefreshEquipButton();
    }
    public void RefreshEquipButton()
    {
        Dictionary<WearEquipType, List<Equip>> dic = PlayerMgr.Instance.GetEquips();
        SetItem_UI(dic[equipType][index]);
    }

    public void SetItem_UI(IGetAble obj)
    {
        Equip equip = obj as Equip;
        this.equip = equip;

        if (this.equip != null)
        {
            equipImage.sprite = this.equip.itemSprite;
            equipGridImage.sprite = this.equip.ItemGridSprite;
            equipImage.gameObject.SetActive(true);

            if (equip.GetStrengthenLevel() >= 1)
                strengthenLevel.text = "+" + equip.GetStrengthenLevel();
            else
                strengthenLevel.text = "";

            if (equip.IsLocked == true)
                lockedText.text = "已锁定";
            else
                lockedText.text = "";
        }
        else
        {
            equipGridImage.sprite = Item.WhiteGridSprite;
            equipImage.gameObject.SetActive(false);
            strengthenLevel.text = "";
            this.lockedText.text = "";
        }
    }

    public IDragAble SetItem_Drag(IDragAble obj)
    {
        Equip equip = obj as Equip;
        if (equip == null)
            return null;
        WearEquipType type = PlayerEquipController.EquipTypeConversion(equip.equipType);
        if (type != this.equipType)
        {
            UIManager.GetInstance().ShowTips("这个装备不应该在这哦");//Debug.Log("这个装备不应该在这哦");//这应该是游戏提示。目前先这样
            return null;
        }
        AudioMgr.Instance.PlayEffect(AudioClipPath.equip);
        PlayerMgr.Instance.WearEquip(equip, index);
        return null;
    }
}
