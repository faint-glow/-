using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public abstract class ExchangePanel : MapPanel
{
    protected Item item;
    protected IHolderAble targetHolder;

    protected Image itemImage;
    protected TMP_Text itemNameText;
    protected TMP_Text priceCountText;
    protected Slider slider;
    protected Button cancelButton;
    protected Button confirmButton;
    protected TMP_Text confirmText;
    protected TMP_InputField inputField;

    public override void ShowMeFirst()
    {
        itemImage = GetPanel("Main/Panel/Image/Image").GetUI<Image>();
        itemNameText = GetPanel("Main/Panel/Text (TMP)").GetUI<TMP_Text>();
        priceCountText = GetPanel("Main/Panel/Image (1)/Text (TMP)").GetUI<TMP_Text>();
        slider = GetPanel("Main/Panel/Slider").GetUI<Slider>();
        cancelButton = GetPanel("Main/Panel/Button").GetUI<Button>();
        confirmButton = GetPanel("Main/Panel/Button (1)").GetUI<Button>();
        confirmText = GetPanel("Main/Panel/Button (1)/Text (TMP)").GetUI<TMP_Text>();
        inputField = GetPanel("Main/Panel/InputField (TMP)").transform.GetComponent<TMP_InputField>();

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(Cancel);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(Confirm);

        slider.onValueChanged.AddListener((num) => { inputField.text = num.ToString(); });//添加slider拖动事件
        inputField.onEndEdit.AddListener((content) =>
        {
            //if (content != null && content != "")
            //{
            //    float num = float.Parse(content);
            //    if (num > slider.maxValue)
            //    {
            //        num = slider.maxValue;
            //        inputField.text = slider.maxValue.ToString();
            //        slider.value = slider.maxValue;
            //        return;
            //    }
            //    slider.value = num;
            //}
            if (content != null && content != "")
            {
                float num = float.Parse(content);
                if (num > slider.maxValue)
                    num = slider.maxValue;

                inputField.text = slider.maxValue.ToString();
                slider.value = num;
            }
        });//添加输入完毕事件。https://blog.csdn.net/weixin_45023328/article/details/116716317
    }

    public override void ShowMeEveryTime()
    {
        slider.value = 1;
    }

    public void SetItem(IHolderAble targetHolder, Item item)
    {
        this.targetHolder = targetHolder;
        this.item = item;

        if(targetHolder != null)
        {
            Dictionary<string, string> forbidItemDic = targetHolder.GetForbidItemDic();
            if (forbidItemDic != null && forbidItemDic.ContainsKey(item.GetName()))//如果目标容器不可加入的物品里有该物品，则直接结束
            {
                UIManager.GetInstance().ShowTips(forbidItemDic[item.GetName()]);
                UIManager.GetInstance().HidePanel(GetPrefabsPath());
                return;
            }
        }
        if (targetHolder == PlayerMgr.Instance)
        {
            if (OperiateByPlayer(targetHolder, item) == false)
            {
                UIManager.GetInstance().HidePanel(GetPrefabsPath());
                return;
            }
            this.slider.maxValue = GetSliderMaxValueByPlayer(targetHolder, item);
            this.confirmText.text = GetConfirmTextByPlayer();
        }
        else if(targetHolder != null)
        {
            if (OperiateByOther(targetHolder, item) == false)
            {
                UIManager.GetInstance().HidePanel(GetPrefabsPath());
                return;
            }
            this.slider.maxValue = GetSliderMaxValueByOther(targetHolder, item);
            this.confirmText.text = GetConfirmTextByOther();
        }

        this.itemImage.sprite = item.itemSprite;
        this.itemNameText.text = item.itemName;
        this.priceCountText.text = item.holder.GetItemPrice(item).ToString();

        this.slider.minValue = 0;
        this.slider.wholeNumbers = true;//勾选此项，拖动滑动条将按整型数（最小为1）进行改变指定值
    }
    protected virtual void Cancel()
    {
        AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
        UIManager.GetInstance().HidePanel(GetPrefabsPath());
    }
    protected virtual void Confirm()
    {
        AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
        if ((int)slider.value <= 0)
        {
            UIManager.GetInstance().HidePanel(GetPrefabsPath());
            return;
        }

        ExchangeItem(targetHolder, item, (int)slider.value, OperiateCoin);
        UIManager.GetInstance().HidePanel(GetPrefabsPath());
    }
    public static void ExchangeItem(IHolderAble targetHolder, Item item, int count, Action<IHolderAble, IHolderAble, Item, int> coinOperiate = null)
    {
        IHolderAble olderHolder = item.holder;
        Item newItem = item.holder.RemoveItem(item, count);
        int newItemCount = newItem.GetCount();
        int useCount = newItemCount;

        if (targetHolder != null)
        {
            Item overflowItem = targetHolder.AddItem(newItem);
            if (overflowItem != null)
            {
                useCount = newItemCount - overflowItem.GetCount();
                olderHolder.AddItem(overflowItem);//把多余的物体返回给原来的主人
                                                  //UIManager.GetInstance().ShowTips("目标背包已满");
            }
            if(coinOperiate == null)
                DefaultOperiateCoin(olderHolder, targetHolder, item, useCount);
            else
                coinOperiate(olderHolder, targetHolder, item, useCount);
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
        }
    }
    public static void DefaultOperiateCoin(IHolderAble oldHolder, IHolderAble newHolder, Item item, int useCount)
    {
        int price = item.holder.GetItemPrice(item);
        oldHolder.ChangeCoin(price * useCount);
        newHolder.ChangeCoin(-price * useCount);
    }
    protected virtual void OperiateCoin(IHolderAble oldHolder, IHolderAble newHolder, Item item, int useCount)
    {
        
    }
    //当物品被操作放入玩家背包时获取滑动条的最大值
    public virtual int GetSliderMaxValueByPlayer(IHolderAble targetHolder, Item item)
    {
        return item.GetCount();
    }
    //当物品被操作放入非玩家背包时获取滑动条的最大值
    public virtual int GetSliderMaxValueByOther(IHolderAble targetHolder, Item item)
    {
        return item.GetCount();
    }
    public abstract string GetPrefabsPath();
    /// <summary>
    /// 当物品被操作放入玩家背包时的处理
    /// </summary>
    /// <param name="targetHolder"></param>
    /// <param name="item"></param>
    /// <returns>返回false结束外界方法</returns>
    public virtual bool OperiateByPlayer(IHolderAble targetHolder, Item item)
    {
        return true;
    }
    /// <summary>
    /// 当物品被操作放入非玩家背包时的处理
    /// </summary>
    /// <param name="targetHolder"></param>
    /// <param name="item"></param>
    /// <returns>返回false结束外界方法</returns>
    public virtual bool OperiateByOther(IHolderAble targetHolder, Item item)
    {
        return true;
    }
    //当物品被操作放入玩家背包时的确认按钮的名字
    public abstract string GetConfirmTextByPlayer();
    //当物品被操作放入非玩家背包时的确认按钮的名字
    public abstract string GetConfirmTextByOther();
}
