using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBagPanel : ContainerPanel<ItemButton, Item>
{
    public static string prefabsPath = "ItemBagPanel";//主要背包
    public static string prefabsPath_Storage = "ItemBagPanel_Storage";//主要背包
    public static string prefabsPath_Strengthen = "ItemBagPanel_Strengthen";//主要背包
    public static string prefabsPath_Box = "ItemBagPanel_Box";//主要背包
    public static string prefabsPath_Discard = "ItemBagPanel_Discard";//主要背包

    public static Sprite normalBackSprite;
    public static Sprite NormalBackSprite
    {
        get
        {
            if (normalBackSprite == null)
                normalBackSprite = ResMgr.GetInstance().Load<Sprite>("UI/Sprite/NormalBack_ItemBag");
            return normalBackSprite;
        }
    }
    public static Sprite selectBackSprite;
    public static Sprite SelectBackSprite
    {
        get
        {
            if (selectBackSprite == null)
                selectBackSprite = ResMgr.GetInstance().Load<Sprite>("UI/Sprite/SelectBack_ItemBag");
            return selectBackSprite;
        }
    }

    private TargetPanel mainPanel;//item的直属父物体
    private ScrollRect itemScrollRect;
    private TMP_Text coinText;
    private TMP_Text capacityText;
    private TMP_Text bagNameText;
    private ShortcutPanel shortcutPanel;
    private Button tidyButton;
    private Button clearButton;
    private TMP_Text clearButtonText;

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
        itemScrollRect = GetPanel("Main/Middle/Main").GetUI<ScrollRect>();
        mainPanel = GetPanel("Main/Middle/Main/Viewport/BagContent");
        coinText = GetPanel("Main/Down/MainInfo/Panel/Image (2)/Image/Text (TMP)").GetUI<TMP_Text>();
        capacityText = GetPanel("Main/Down/MainInfo/Panel/Image/Text (TMP)").GetUI<TMP_Text>();
        bagNameText = GetPanel("Main/Down/MainInfo/Panel/Image (1)/Text (TMP)").GetUI<TMP_Text>();

        tidyButton = GetPanel("Main/Down/MainInfo/Panel (1)/Button").GetUI<Button>();
        tidyButton.onClick.AddListener(()=> 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            IHolderAble holder = GetHolder();
            if (holder == null)
                return;
            holder.GetBag().Tidy_Quality_Category();
            RefreshPanel();
        });
        TargetPanel clearPanel = GetPanel("Main/Down/MainInfo/Panel (2)/Button");
        if(clearPanel != null)
            clearButton = clearPanel.GetUI<Button>();
        if(clearButton != null)
        {
            clearButton.onClick.AddListener(() =>
            {
                AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
                UIManager.GetInstance().ShowPanelAsync<RemoveSelectPanel>(RemoveSelectPanel.prefabsPath, PanelLayer.Top_Up,null,(panel)=> 
                {
                    StorePanel storePanel = CurrentBagPanel.currentBasePanel as StorePanel;
                    if (storePanel != null)
                        panel.SetTargetHolder(storePanel.GetMerchant());
                });
            });
        }
        TargetPanel clearButtonTextPanel = GetPanel("Main/Down/MainInfo/Panel (2)/Button/Text (TMP)");
        if (clearButtonTextPanel != null)
            clearButtonText = clearButtonTextPanel.GetUI<TMP_Text>();
        if (clearButtonText != null)
        {
            if (CurrentBagPanel.currentBagPanelType == BagPanelType.StorePanel)
                clearButtonText.text = "出售";
            else if(CurrentBagPanel.currentBagPanelType == BagPanelType.NormalPanel)
                clearButtonText.text = "丢弃";
        }

        shortcutPanel = GetPanel("Main/Down/ShortcutPanel").transform.GetComponent<ShortcutPanel>();

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
    public override void ShowMeEveryTime()
    {
        //Debug.Log("=================================");
        //PlayerMgr.Instance.GetBag().ForeachAllItems((item) => { Debug.Log("ItemBagPanel" + ":" + item.GetName() + ":" + item.IsLocked); });

        shortcutPanel.SetShortcutType(ShortcutType.道具);
        GetHolder().GetBag().RefreshBag();
        TriggerButton(allItemButton, ItemCategory.无);
        //PlayerMgr.Instance.GetBag().ForeachAllItems((item) => { Debug.Log("ItemBagPanel" + ":" + item.GetName() + ":" + item.IsLocked); });
    }
    private void RefreshPanel(ItemCategory itemCategory)
    {
        List<Item> itemList;
        if (itemCategory != ItemCategory.无)
            itemList = PlayerMgr.Instance.GetBagItems(itemCategory);
        else
            itemList = PlayerMgr.Instance.GetBag().GetMainBag();
        RefreshContainer(itemList);
        RefreshUI();
    }
    public void RefreshPanel()
    {
        if (currentButton == null)
            currentCategory = ItemCategory.无;
        RefreshPanel(currentCategory);
    }

    public void RefreshUI()
    {
        //暂缓一帧刷新UI
        MonoMgr.GetInstance().DelayAction(Time.deltaTime, () => 
        {
            coinText.text = GetHolder().GetCoin().ToString();
            int upCapacityCount = GetHolder().GetBag().CurrentCapacity;
            string upCapacityStr = upCapacityCount.ToString();
            if (upCapacityCount <= 0)
                upCapacityStr = "∞";

            bagNameText.text = "背包类型:" + GetHolder().GetBagName();
            capacityText.text = "背包容量:" + GetHolder().GetBag().Count + "/" + upCapacityStr;
        });
    }

    private void AddListener_ItemButton(Button button, ItemCategory itemCategory)
    {
        button.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.change);
            TriggerButton(button, itemCategory);
        });
    }

    private void TriggerButton(Button button, ItemCategory itemCategory)
    {
        if (currentButton != null)
        {
            currentButton.GetComponent<Image>().sprite = NormalBackSprite;
            currentButton.GetComponentInChildren<TMP_Text>().color = new Color(70 / 255f, 70 / 255f, 70 / 255f, 1);
        }
        button.GetComponent<Image>().sprite = SelectBackSprite;
        button.GetComponentInChildren<TMP_Text>().color = Color.white;
        RefreshPanel(itemCategory);

        currentButton = button;
        currentCategory = itemCategory;
    }
    public override IHolderAble GetHolder()
    {
        return PlayerMgr.Instance;
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
        GetHolder().GetBag().RefreshBag();
    }
}
