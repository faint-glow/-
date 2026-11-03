using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

public enum DetailButtonType
{
    Up,
    Middle,
    Down
}

public class DetailButton
{
    public string buttonName;
    public UnityAction action;
    public Color color;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buttonName">按钮的名字</param>
    /// <param name="color">white未默认颜色绿色，此处只需要填写白色或者红色</param>
    /// <param name="action">按钮调用的方法</param>
    public DetailButton(string buttonName, Color color, UnityAction action)
    {
        this.buttonName = buttonName;
        this.action = action;
        this.color = color;
    }
}

public class DetailPanel : MapPanel
{
    public static string prefabsPath = "DetailPanel";
    private Item currentItem;
    private Image itemGridImage;
    private Image itemImage;

    private TMP_Text itemName;
    private TMP_Text itemPrice;
    private TMP_Text strengthenLevel;
    private TMP_Text lockedText;

    private TMP_Text itemLevelInfo;
    private TMP_Text itemBaseProerty;
    private TMP_Text itemRandomProerty;
    private TMP_Text itemExtraProerty;
    private TMP_Text itemBuffProerty;
    private TMP_Text itemBaseInfo;
    private TMP_Text itemExtraInfo;

    private GameObject lineOne;
    private GameObject lineTwo;
    private GameObject lineThree;
    private GameObject lineFour;
    private GameObject lineFive;

    private Button lockButton;
    private Image lockButtonImageNormal;
    private Image lockButtonImageSpecial;

    private Button upButton;
    private TMP_Text upButtonText;

    private Button middleButton;
    private TMP_Text middleButtonText;

    private Button downButton;
    private TMP_Text downButtonText;

    private List<GameObject> images = new List<GameObject>();

    private Action hideAction;
    public override void ShowMeFirst()
    {
        itemGridImage = GetPanel("Main/Image").GetUI<Image>();
        itemImage = GetPanel("Main/Image/Image").GetUI<Image>();
        itemName = GetPanel("Main/Image/Text (TMP)").GetUI<TMP_Text>();
        itemPrice = GetPanel("Main/Image/Text (TMP) (1)").GetUI<TMP_Text>();
        strengthenLevel = GetPanel("Main/Image/Text (TMP) (2)").GetUI<TMP_Text>();
        lockedText = GetPanel("Main/Image/Text (TMP) (3)").GetUI<TMP_Text>();

        itemLevelInfo = GetPanel("Main/Scroll/Viewport/Panel/Text (TMP)").GetUI<TMP_Text>();
        itemBaseProerty = GetPanel("Main/Scroll/Viewport/Panel/Text (TMP) (1)").GetUI<TMP_Text>();
        itemRandomProerty = GetPanel("Main/Scroll/Viewport/Panel/Text (TMP) (2)").GetUI<TMP_Text>();
        itemExtraProerty = GetPanel("Main/Scroll/Viewport/Panel/Text (TMP) (3)").GetUI<TMP_Text>();
        itemBuffProerty = GetPanel("Main/Scroll/Viewport/Panel/Text (TMP) (4)").GetUI<TMP_Text>();

        itemBaseInfo = GetPanel("Main/Scroll/Viewport/Panel/Text (TMP) (5)").GetUI<TMP_Text>();
        itemExtraInfo = GetPanel("Main/Scroll/Viewport/Panel/Text (TMP) (6)").GetUI<TMP_Text>();

        lineOne = GetPanel("Main/Scroll/Viewport/Panel/LineOne").transform.gameObject;
        lineTwo = GetPanel("Main/Scroll/Viewport/Panel/LineTwo").transform.gameObject;
        lineThree = GetPanel("Main/Scroll/Viewport/Panel/LineThree").transform.gameObject;
        lineFour = GetPanel("Main/Scroll/Viewport/Panel/LineFour").transform.gameObject;
        lineFive = GetPanel("Main/Scroll/Viewport/Panel/LineFive").transform.gameObject;

        lockButton = GetPanel("Main/Button").GetUI<Button>();
        lockButtonImageSpecial = GetPanel("Main/Button/Image (1)").GetUI<Image>();
        lockButton.onClick.AddListener(()=> 
        {
            if (currentItem == null)
                return;
            currentItem.IsLocked = !currentItem.IsLocked;
            lockButtonImageSpecial.gameObject.SetActive(currentItem.IsLocked);

            if (currentItem.IsLocked == true)
                lockedText.text = "已锁定";
            else
                lockedText.text = "";
        });


        upButton = GetPanel("Main/Button (1)").GetUI<Button>();
        upButtonText = GetPanel("Main/Button (1)/Text (TMP)").GetUI<TMP_Text>();

        middleButton = GetPanel("Main/Button (2)").GetUI<Button>();
        middleButtonText = GetPanel("Main/Button (2)/Text (TMP)").GetUI<TMP_Text>();

        downButton = GetPanel("Main/Button (3)").GetUI<Button>();
        downButtonText = GetPanel("Main/Button (3)/Text (TMP)").GetUI<TMP_Text>();

        ThisPanel.GetUI<Button>().onClick.RemoveAllListeners();
        ThisPanel.GetUI<Button>().onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.close);
            UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
        });
    }

    public void SetHideAction(Action hideAction)
    {
        this.hideAction = hideAction;
    }

    public void SetItem(Item item)
    {
        if (item == null)
        {
            currentItem = null;
            strengthenLevel.text = "";
            lockedText.text = "";
            lockButtonImageSpecial.gameObject.SetActive(false);
            return;
        }
        currentItem = item;
        itemGridImage.sprite = item.ItemGridSprite;
        itemLevelInfo.text = "物品等级:" + item.level;
        itemImage.sprite = item.itemSprite;
        itemName.text = item.itemName;

        Equip equip = item as Equip;
        if (equip != null && equip.GetStrengthenLevel() >= 1)
            strengthenLevel.text = "+" + equip.GetStrengthenLevel();
        else
            strengthenLevel.text = "";

        if (item.IsLocked == true)
            lockedText.text = "已锁定";
        else
            lockedText.text = "";
        lockButtonImageSpecial.gameObject.SetActive(item.IsLocked);

        itemPrice.text = "售价" + item.holder.GetItemPrice(item);
        itemBaseInfo.text = item.itemBaseInfo;
        itemExtraInfo.text = item.itemExtraInfo;
        if (itemBaseInfo.text == "" && itemExtraInfo.text == "")
            lineFive.gameObject.SetActive(false);
        else
            lineFive.gameObject.SetActive(true);

        ExtraInfoOperate(item);
        DetailPanelOperate(item);
    }
    //按钮处理
    private void DetailPanelOperate(Item item)
    {

        Dictionary<DetailButtonType, DetailButton> detailButtons = CurrentBagPanel.currentBasePanel.GetDetailButtons(item);

        if (detailButtons == null)
            detailButtons = item.DetailButtons;
        foreach (KeyValuePair<DetailButtonType, DetailButton> button in detailButtons)
        {
            if (button.Key == DetailButtonType.Up)
                SetButton(upButton, upButtonText, button.Value);
            else if (button.Key == DetailButtonType.Middle)
                SetButton(middleButton, middleButtonText, button.Value);
            else
                SetButton(downButton, downButtonText, button.Value);
        }
    }

    private void SetButton(Button button, TMP_Text buttonText, DetailButton buttonInfo)
    {
        if (buttonInfo == null)
        {
            button.gameObject.SetActive(false);
            buttonText.gameObject.SetActive(false);
            return;
        }
        button.gameObject.SetActive(true);
        buttonText.gameObject.SetActive(true);

        buttonText.text = buttonInfo.buttonName;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            if (buttonInfo.action != null)
                buttonInfo.action.Invoke();
        });
        button.GetComponent<Image>().color = buttonInfo.color;
    }

    private void ExtraInfoOperate(Item item)
    {

        if (item is Equip)
        {
            Equip equip = item as Equip;

            //设置基础数值的UI展示
            SetPropertyText(equip, equip.GetOriginalPropertys(), itemBaseProerty, lineOne, (Equip equip, PropertyType propertyType) =>
             {
                 return equip.GetBaseProperty(propertyType);//GetBaseProperty为OriginalProperty+strengthenProperty
             });
            //设置武器生成时随机属性的UI面板展示
            SetPropertyText(equip, equip.GetRandomPropertys(), itemRandomProerty, lineTwo, (Equip equip, PropertyType propertyType) =>
            {
                return equip.GetRandomProperty(propertyType);
            });
            //设置武器附魔属性的UI面板展示
            SetPropertyText(equip, equip.GetExtraPropertys(), itemExtraProerty, lineThree, (Equip equip, PropertyType propertyType) =>
            {
                return equip.GetExtraProperty(propertyType);
            });

            itemBuffProerty.text = "";
            if (equip.GetAllEquipBuff().Count > 0)
            {
                itemBuffProerty.gameObject.SetActive(true);
                lineFour.gameObject.SetActive(true);
                for (int i = 0; i < equip.GetAllEquipBuff().Count; i++)
                {
                    Buff buff = equip.GetAllEquipBuff()[i];
                    List<Effect> effects = buff.GetAllEffects();
                    if (effects != null)
                    {
                        for (int j = 0; j < effects.Count; j++)
                        {
                            itemBuffProerty.text += effects[j].GetEffectInfoText() + "\n";
                        }
                    }
                }
            }
            else
            {
                itemBuffProerty.gameObject.SetActive(false);
                lineFour.gameObject.SetActive(false);
            }
        }
        else
        {
            lineOne.gameObject.SetActive(false);
            itemBaseProerty.gameObject.SetActive(false);

            lineTwo.gameObject.SetActive(false);
            itemRandomProerty.gameObject.SetActive(false);

            lineThree.gameObject.SetActive(false);
            itemExtraProerty.gameObject.SetActive(false);

            lineFour.gameObject.SetActive(false);
            itemBuffProerty.gameObject.SetActive(false);

            //lineFive.gameObject.SetActive(false);
        }
    }
    private void SetPropertyText(Equip equip, Dictionary<PropertyType, float> propertyDic, TMP_Text propertyText, GameObject upLine, Func<Equip, PropertyType, float> GetPropertyFunc)
    {
        if (equip == null)
            return;
        propertyText.text = "";
        if (propertyDic.Count >= 1)
        {
            propertyText.gameObject.SetActive(true);
            upLine.gameObject.SetActive(true);
            foreach (var property in propertyDic)
            {
                float propertyValue = 0;
                if (GetPropertyFunc != null)
                    propertyValue = GetPropertyFunc.Invoke(equip, property.Key);
                propertyText.text += property.Key.ToString() + ":" + propertyValue.ToString("0.0")/*equip.GetBaseProperty(property.Key)/*property.Value*/ + "\n";
            }
        }
        else
        {
            propertyText.gameObject.SetActive(false);
            upLine.gameObject.SetActive(false);
        }
    }

    public override void HideMe()
    {
        if (hideAction != null)
            hideAction.Invoke();
    }
}
