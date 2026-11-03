using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContainerBagPanel : ContainerPanel<ItemButton, Item>,ITemporaryBagAble
{
    public static string prefabsPath_Bag = "ContainerBagPanel_Box";//普通背包
    public static string prefabsPath_Discard = "ContainerBagPanel_Discard";//普通背包

    private TargetPanel mainPanel;//item的直属父物体
    private ScrollRect itemScrollRect;
    private TMP_Text capacityText;
    private TMP_Text bagNameText;

    private IHolderAble holder;

    private Button allItemButton;
    private Button equipItemButton;
    private Button foodItemButton;
    private Button medicineItemButton;
    private Button materialItemButton;
    private Button sundryItemButton;

    private Button currentButton;
    private ItemCategory currentCategory = ItemCategory.无;
    public override void ShowMeFirst()
    {
        mainPanel = GetPanel("Main/Middle/Main/Viewport/BagContent");
        itemScrollRect = GetPanel("Main/Middle/Main").GetUI<ScrollRect>();
        capacityText = GetPanel("Main/Down/MainInfo/Panel/Image/Text (TMP)").GetUI<TMP_Text>();
        bagNameText = GetPanel("Main/Down/MainInfo/Panel/Image (1)/Text (TMP)").GetUI<TMP_Text>();

        allItemButton = GetPanel("Main/Up/Main/Panel/Button (5)").GetUI<Button>();
        AddListener_ItemButton(allItemButton, ItemCategory.无);
        equipItemButton = GetPanel("Main/Up/Main/Panel/Button (4)").GetUI<Button>();
        AddListener_ItemButton(equipItemButton, ItemCategory.Equip);
        foodItemButton = GetPanel("Main/Up/Main/Panel/Button (3)").GetUI<Button>();
        AddListener_ItemButton(foodItemButton, ItemCategory.Food);
        medicineItemButton = GetPanel("Main/Up/Main/Panel/Button (2)").GetUI<Button>();
        AddListener_ItemButton(medicineItemButton, ItemCategory.Medicine);
        materialItemButton = GetPanel("Main/Up/Main/Panel/Button (1)").GetUI<Button>();
        AddListener_ItemButton(materialItemButton, ItemCategory.Material);
        sundryItemButton = GetPanel("Main/Up/Main/Panel/Button").GetUI<Button>();
        AddListener_ItemButton(sundryItemButton, ItemCategory.Sundary);
    }
    private void AddListener_ItemButton(Button button, ItemCategory itemCategory)
    {
        button.onClick.AddListener(() =>
        {
            TriggerButton(button, itemCategory);
        });
    }
    private void TriggerButton(Button button, ItemCategory itemCategory)
    {
        if (currentButton != null)
        {
            currentButton.GetComponent<Image>().sprite = ItemBagPanel.NormalBackSprite;
            currentButton.GetComponentInChildren<TMP_Text>().color = new Color(70 / 255f, 70 / 255f, 70 / 255f, 1);
        }
        button.GetComponent<Image>().sprite = ItemBagPanel.SelectBackSprite;
        button.GetComponentInChildren<TMP_Text>().color = Color.white;
        RefreshPanel(itemCategory);

        currentButton = button;
        currentCategory = itemCategory;
    }
    public override IHolderAble GetHolder()
    {
        return holder;
    }
    public void SetHolder(IHolderAble holderAble)
    {
        ITemporaryItemAble temporaryItemAble = holderAble as ITemporaryItemAble;
        if (temporaryItemAble != null)
        {
            IHolderAble itemHolder = temporaryItemAble.GetMyHolder();
            Debug.Log(itemHolder);
            Item item = itemHolder.GetBagMgr().UseTemporaryBag(this, temporaryItemAble as Item);
            holderAble = item as IHolderAble;

            itemHolder.RefreshBag();
        }
        if (holderAble == null)
            return;

        this.holder = holderAble;
        this.holder.GetBagMgr().GetMainBag().RefreshBag();
        TriggerButton(allItemButton, ItemCategory.无);
        RefreshUI();
    }
    public void RefreshPanel()
    {
        if (currentButton == null)
            currentCategory = ItemCategory.无;

        RefreshUI();
        RefreshPanel(currentCategory);
    }
    private void RefreshUI()
    {
        if (this.holder == null)
            return;
        int upCapacityCount = this.holder.GetBagMgr().GetMainBag().CurrentCapacity;
        string upCapacityStr = upCapacityCount.ToString();
        if (upCapacityCount <= 0)
            upCapacityStr = "∞";

        bagNameText.text = "背包类型:" + this.holder.GetBagName();
        capacityText.text = "背包容量:" + this.holder.GetBagMgr().GetMainBag().Count + "/" + upCapacityStr;
    }
    private void RefreshPanel(ItemCategory itemCategory)
    {
        List<Item> itemList;
        if (itemCategory != ItemCategory.无)
            itemList = this.holder.GetBagMgr().GetMainBag().GetCategoryBagItems(itemCategory.ToString());///.GetBagItems(itemCategory);
        else
            itemList = this.holder.GetBagMgr().GetMainBag().GetMainBag();
        RefreshContainer(itemList);
    }
    public override string GetCellPath()
    {
        return ItemButton.prefabsPath;
    }
    public override Transform GetContainterTrans()
    {
        return mainPanel.transform;
    }
    public override ScrollRect GetScrollRect()
    {
        return itemScrollRect;
    }
    public override void HideMe()
    {
        ITemporaryItemAble temporaryItemAble = holder as ITemporaryItemAble;
        if (temporaryItemAble != null)
            temporaryItemAble.GetMyHolder().GetBagMgr().UseEndTemporaryBag(this);
    }
}
