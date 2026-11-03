using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemButton : DragPanel,ISetAble<IGetAble>
{
    public static string prefabsPath = "Button/ItemButton";
    public MapPanel FatherPanel { get; set; }
    public int Index { get; set; }
    private Item item;
    private Image itemImage;
    private Image itemGridImage;
    private TMP_Text countText;
    private TMP_Text strengthenLevel;
    private TMP_Text lockedText;

    public override void ShowMeFirst()
    {
        ThisPanel.GetUI<Button>().onClick.RemoveAllListeners();
        ThisPanel.GetUI<Button>().onClick.AddListener(() =>
        {
            Vector2 mousPos = Input.mousePosition;
            if ((mousPos - startPos).sqrMagnitude > 50)
                return;
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            if (item == null)
                return;
            UIManager.GetInstance().ShowPanelAsync<DetailPanel>(DetailPanel.prefabsPath, PanelLayer.Top_Up, null,
            (panel) =>
                {
                    if (item == null)
                        return;
                    panel.SetItem(item);
                    panel.SetHideAction(() => 
                    {
                        if (item == null)
                            return;
                        if (item.IsLocked == true)
                            lockedText.text = "已锁定";
                        else
                            lockedText.text = "";
                    });
                });
        });

        itemImage = GetPanel("Image (1)/Image").GetUI<Image>();
        itemGridImage = GetPanel("Image (1)").GetUI<Image>();
        countText = GetPanel("Image (1)/Text (TMP)").GetUI<TMP_Text>();
        strengthenLevel = GetPanel("StrengthenLevel").GetUI<TMP_Text>();
        lockedText = GetPanel("Locked").GetUI<TMP_Text>();
    }

    public void SetItem_Container(IGetAble obj)
    {
        Item item = obj as Item;
        if (item == null)
        {
            this.item = null;
            this.itemImage.gameObject.SetActive(false);
            this.itemGridImage.sprite = Item.WhiteGridSprite;
            this.countText.gameObject.SetActive(false);
            this.lockedText.text = "";
            this.strengthenLevel.text = "";
            return;
        }
        this.item = item;
        itemImage.sprite = item.itemSprite;
        itemImage.gameObject.SetActive(true);

        itemGridImage.sprite = item.ItemGridSprite;

        int count = item.GetCount();
        if(count > 1)
            countText.gameObject.SetActive(true);
        else
            countText.gameObject.SetActive(false);

        countText.text = count.ToString();
        Equip equip = item as Equip;
        if (equip != null && equip.GetStrengthenLevel() >= 1)
            strengthenLevel.text = "+" + equip.GetStrengthenLevel();
        else
            strengthenLevel.text = "";

        if(item.IsLocked == true)
            lockedText.text = "已锁定";
        else
            lockedText.text = "";
    }

    public override ScrollRect GetScrollRect()
    {
        ContainerPanel<ItemButton,Item> fatherPanel = FatherPanel as ContainerPanel<ItemButton, Item>;
        return fatherPanel.GetScrollRect();
    }

    public override IDragAble GetItem_Drag()
    {
        return item;
    }

    public override void OperiateOverflowItem_Drag(IDragAble drageAble)
    {
        if (drageAble == null)
            return;
        ContainerPanel<ItemButton, Item> fatherPanel = FatherPanel as ContainerPanel<ItemButton, Item>;
        fatherPanel.GetHolder().AddItem(drageAble as Item);
    }
}
