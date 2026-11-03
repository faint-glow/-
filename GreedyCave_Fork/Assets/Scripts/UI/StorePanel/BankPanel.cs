using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BankType
{
    取钱,
    存钱,
}
public class BankPanel : MapPanel
{
    public static string prefabsPath = "BankPanel";

    protected string bankName = "";
    protected BankType bankType = BankType.取钱;

    protected Button bankTypeButton;
    protected TMP_Text bankTypeText;

    protected TMP_Text bankCoinText;
    protected TMP_Text bagCoinText;
    protected Slider slider;
    protected Button cancelButton;
    protected Button confirmButton;
    protected TMP_Text confirmText;
    protected TMP_InputField inputField;
    public override void ShowMeFirst()
    {
        bankTypeButton = GetPanel("Main/Panel/Button (2)").GetUI<Button>();
        bankTypeText = GetPanel("Main/Panel/Text (TMP)").GetUI<TMP_Text>();

        bankCoinText = GetPanel("Main/Panel/Image/Text (TMP)").GetUI<TMP_Text>();
        bagCoinText = GetPanel("Main/Panel/Image (1)/Text (TMP)").GetUI<TMP_Text>();
        slider = GetPanel("Main/Panel/Slider").GetUI<Slider>();
        cancelButton = GetPanel("Main/Panel/Button").GetUI<Button>();
        confirmButton = GetPanel("Main/Panel/Button (1)").GetUI<Button>();
        confirmText = GetPanel("Main/Panel/Button (1)/Text (TMP)").GetUI<TMP_Text>();
        inputField = GetPanel("Main/Panel/InputField (TMP)").transform.GetComponent<TMP_InputField>();

        bankTypeButton.onClick.RemoveAllListeners();
        bankTypeButton.onClick.AddListener(ChangeBankType);

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(Cancel);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(Confirm);

        slider.onValueChanged.AddListener((num) => 
        {
            inputField.text = num.ToString(); 
        });//添加slider拖动事件
        inputField.onEndEdit.AddListener((content) => 
        {
            //if(content != null && content != "")
            //{
            //    float num = float.Parse(content);
            //    if (num > slider.maxValue)
            //    {
            //        inputField.text = slider.maxValue.ToString();
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

        this.slider.minValue = 0;
        this.slider.wholeNumbers = true;//勾选此项，拖动滑动条将按整型数（最小为1）进行改变指定值
    }
    public void SetBankName(string bankName)
    {
        this.bankName = bankName;
        this.slider.value = 0;
        this.inputField.text = "0";

        this.CheckBankState();
    }
    public void ChangeBankType()
    {
        int currentBankTypeIndex = (int)this.bankType;
        currentBankTypeIndex += 1;
        currentBankTypeIndex %= 2;//因为现在枚举类型只有两个，所以模2

        this.bankType = (BankType)currentBankTypeIndex;
        this.CheckBankState();
    }
    private void CheckBankState()
    {
        PlayerData playerData = PlayerMgr.Instance.GetPlayerData();
        if (playerData == null)
            return;
        if (bankName == null)
            return;
        if (!playerData.bankDic.ContainsKey(bankName))
            playerData.bankDic[bankName] = 0;

        float bankCoin = playerData.bankDic[bankName];
        float bagCoin = PlayerMgr.Instance.GetWarrior().GetCurrentProperty(PropertyType.金币);

        bankCoinText.text = bankCoin.ToString();
        bagCoinText.text = bagCoin.ToString();

        slider.minValue = 0;
        if (this.bankType == BankType.取钱)
            slider.maxValue = bankCoin;
        else if(this.bankType == BankType.存钱)
            slider.maxValue = bagCoin;

        this.confirmText.text = this.bankType.ToString();
        this.bankTypeText.text = this.confirmText.text;
    }
    protected virtual void Cancel()
    {
        AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
        UIManager.GetInstance().HidePanel(BankPanel.prefabsPath);
    }
    protected virtual void Confirm()
    {
        PlayerData playerData = PlayerMgr.Instance.GetPlayerData();
        if (playerData == null)
            return;
        if (bankName == null)
            return;
        if (!playerData.bankDic.ContainsKey(bankName))
            playerData.bankDic[bankName] = 0;

        float targetValue = slider.value;
        if (this.bankType == BankType.取钱)
            targetValue = slider.value;
        else if (this.bankType == BankType.存钱)
            targetValue = -slider.value;

        playerData.bankDic[bankName] -= targetValue;
        PlayerMgr.Instance.GetWarrior().ChangerProperty(PropertyType.金币, targetValue);

        //float bankCoin = playerData.bankDic[bankName];
        //float bagCoin = PlayerMgr.Instance.GetWarrior().GetCurrentProperty(PropertyType.金币);
        //if (this.bankType == BankType.取钱)
        //{
        //    bagCoin += slider.value;
        //    bankCoin -= slider.value;
        //}
        //else if (this.bankType == BankType.存钱)
        //{
        //    bagCoin -= slider.value;
        //    bankCoin += slider.value;
        //}
        //playerData.bankDic[bankName] = bankCoin;
        //PlayerMgr.Instance.GetWarrior().SetProperty(PropertyType.金币, bagCoin);
        //if (bagCoin == 0)
        //    AudioMgr.Instance.PlayEffect(AudioClipPath.changeCoin);

        AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
        UIManager.GetInstance().HidePanel(BankPanel.prefabsPath);
    }
    
}
