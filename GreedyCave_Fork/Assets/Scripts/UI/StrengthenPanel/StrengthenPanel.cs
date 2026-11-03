using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StrengthenPanel : CurrentBagPanel
{
    public static string prefabsPath = "StrengthenPanel";

    private StrengthenButton[] buttons = new StrengthenButton[3];
    private StrengthenButton targetButton;

    private TMP_Text costInfoText;
    private TMP_Text baseInfoText;
    private TMP_Text extraInfoText;
    private TMP_Text allRatioInfoText;

    private Dictionary<MaterialInfo, int> materialInfoCountDic = new Dictionary<MaterialInfo, int>();
    private Dictionary<PropertyType, float> strengthenProperty = new Dictionary<PropertyType, float>();
    private float successRatio;
    private MaterialInfo targetMaterialInfo;
    public override void ShowMeFirst()
    {
        Button closeButton = GetPanel("ClosePanel").GetUI<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Close);

        Transform bagPanelFather = GetPanel("Right/StrengthenBagPanel/Right").transform;
        ShowPanelAsync<ItemBagPanel>(ItemBagPanel.prefabsPath_Strengthen, bagPanelFather);

        Button strengthButton = GetPanel("Left/Down/Button").GetUI<Button>();
        strengthButton.onClick.RemoveAllListeners();
        strengthButton.onClick.AddListener(StrengthenEquip);

        Transform buttonFather = GetPanel("Left/Up").transform;

        costInfoText = GetPanel("Left/Scroll/Viewport/Panel/Text (TMP)").GetUI<TMP_Text>();
        baseInfoText = GetPanel("Left/Scroll/Viewport/Panel/Text (TMP) (1)").GetUI<TMP_Text>();
        extraInfoText = GetPanel("Left/Scroll/Viewport/Panel/Text (TMP) (2)").GetUI<TMP_Text>();
        allRatioInfoText = GetPanel("Left/Scroll/Viewport/Panel/Text (TMP) (3)").GetUI<TMP_Text>();

        for (int i = 0; i < buttonFather.childCount; i++)
        {
            StrengthenButton strengthenButton = buttonFather.GetChild(i).GetComponent<StrengthenButton>();
            if (i <= 2)
            {
                buttons[i] = strengthenButton;
                buttons[i].SetButtonType(true);
            }
            else
            {
                targetButton = strengthenButton;
                targetButton.SetButtonType(false);
            }
        }
    }

    public override void ShowMeEveryTime()
    {
        materialInfoCountDic.Clear();
        costInfoText.text = "花费金币：";
        baseInfoText.text = "";
        extraInfoText.text = "";
        allRatioInfoText.text = "总概率：";
    }
    protected override BagPanelType GetBagPanelType()
    {
        return BagPanelType.StrengthPanel;
    }
    public void AddMaterial(StrengthenMaterial material)
    {
        if (material == null)
            return;

        for (int i = 0; i < material.materialInfo.Length; i++)
        {
            MaterialInfo materialInfo = material.materialInfo[i];
            bool isSame = false;
            foreach (var item in materialInfoCountDic)
            {
                if (item.Key.MyEquals(materialInfo))
                {
                    materialInfoCountDic[item.Key]++;
                    isSame = true;
                    break;
                }
            }
            if (isSame == false)
                materialInfoCountDic[materialInfo] = 1;
        }
    }

    public void RemoveMaterial(StrengthenMaterial material)
    {
        if (material == null)
            return;
        List<MaterialInfo> targetRemove = new List<MaterialInfo>();
        for (int i = 0; i < material.materialInfo.Length; i++)
        {
            MaterialInfo materialInfo = material.materialInfo[i];
            foreach (var item in materialInfoCountDic)
            {
                if (item.Key.MyEquals(materialInfo))
                {
                    materialInfoCountDic[item.Key]--;
                    if (materialInfoCountDic[item.Key] <= 0)
                        targetRemove.Add(item.Key);
                    break;
                }
            }
        }
        for (int i = 0; i < targetRemove.Count; i++)
        {
            materialInfoCountDic.Remove(targetRemove[i]);
        }
    }

    public bool CheckCoin(IHolderAble holder, Equip equip)
    {
        if (equip == null || holder == null)
            return false;

        if (holder.GetCoin() >= NeedCoin(equip))
            return true;
        return false;
    }
    public static int NeedCoin(Equip equip)
    {
        if (equip == null)
            return 0;
        int qualityValue = (int)equip.itemQuality;
        int strengthenLevel = (equip.GetStrengthenLevel() + 1);
        int itemLevel = equip.level;
        return (int)(20 * (1 + strengthenLevel * strengthenLevel / 15.0f) * (1 + qualityValue * qualityValue / 10.0f) * (1 + (itemLevel - 1) * 0.1f));
    }
    public void RefreshPanel()
    {
        Equip equip = targetButton.GetItem() as Equip;
        if (equip == null)
        {
            this.costInfoText.text = "花费金币：";
            this.baseInfoText.text = "";
            this.extraInfoText.text = "";
            this.allRatioInfoText.text = "总概率：";
            return;
        }

        this.strengthenProperty.Clear();
        this.costInfoText.text = "花费金币：" + NeedCoin(equip).ToString();
        this.baseInfoText.text = "";
        foreach (var item in equip.GetOriginalPropertys())//获取全部初始属性，每强化一次，属性增加十分之一
        {
            PropertyType type = item.Key;
            float value = equip.GetBaseProperty(type);
            float extraValue = equip.GetOriginalProperty(type) * 0.05f;
            this.baseInfoText.text += type + "：" + value + "->" + (value + extraValue) + "\n";

            this.strengthenProperty[type] = extraValue;
        }

        int materialCount = 0;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].GetItem() == null)
                continue;
            materialCount++;
        }
        float successRatio = (10 - equip.GetStrengthenLevel()) * 10 * (materialCount / 3.0f);
        this.extraInfoText.text = "";
        Dictionary<MaterialInfo, float> materialInfoRatioDic = new Dictionary<MaterialInfo, float>();
        foreach (var item in materialInfoCountDic)
        {
            MaterialInfo info = item.Key;
            float ratio = info.ratio * successRatio * item.Value / materialCount;

            float originalNum = equip.GetExtraProperty(info.propertyType);
            extraInfoText.text += info.propertyType + "：" + originalNum + "->" + (originalNum + info.minNum) + "~" + (originalNum + info.maxNum) + "(" + ratio + "%" + ")" + "\n";

            materialInfoRatioDic[info] = ratio;
        }
        this.allRatioInfoText.text = "总概率：" + successRatio + "%";
        this.successRatio = successRatio;
        this.targetMaterialInfo = RewardLibrary.GetRandomType<MaterialInfo>(materialInfoRatioDic);
    }

    public void StrengthenEquip()
    {
        Equip equip = targetButton.GetItem() as Equip;
        if (equip == null)
        {
            UIManager.GetInstance().ShowTips("没有目标强化装备");
            return;
        }

        StrengthenButton strengthenButton = equip.holder as StrengthenButton;
        if (strengthenButton == null)
            return;
        if (CheckCoin(strengthenButton.originalItemHolder,equip) == false)
        {
            UIManager.GetInstance().ShowTips("余额不足");
            return;
        }
        strengthenButton.originalItemHolder.ChangeCoin(-NeedCoin(equip));
        float num = Random.Range(0, 100f);
        if (num <= successRatio)
        {
            UIManager.GetInstance().ShowTips("强化成功");
            foreach (var item in strengthenProperty)
            {
                Debug.Log(item.Key + ":" + item.Value);
                equip.ChangeStrengthenProperty(item.Key, item.Value);
            }

            float extraValue = (float)Random.Range(targetMaterialInfo.minNum,targetMaterialInfo.maxNum + 1);
            Debug.Log(targetMaterialInfo.propertyType + ":" + extraValue);
            equip.ChangeExtraProperty(targetMaterialInfo.propertyType, extraValue);
            equip.levelUpOne_Strengthen();
        }
        else
        {
            UIManager.GetInstance().ShowTips("强化失败");
        }

        //foreach (var item in  equip.GetStrengthenPropertys())
        //{
        //    Debug.Log("StrengthenProperty:" + equip.GetBaseProperty(item.Key));
        //}
        //foreach (var item in equip.GetExtraPropertys())
        //{
        //    Debug.Log("Extra:" + equip.GetExtraProperty(item.Key));
        //}

        for (int i = 0; i < buttons.Length; i++)
            buttons[i].RemoveMaterial();

        targetButton.Refresh();

        PlayerMgr.Instance.SavePlayerData();
    }

    private void Close()
    {
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].ReturnItem();

        targetButton.ReturnItem();
        AudioMgr.Instance.PlayEffect(AudioClipPath.close);
        UIManager.GetInstance().HidePanel(StrengthenPanel.prefabsPath);
    }

    public override Dictionary<DetailButtonType, DetailButton> GetDetailButtons(Item item)
    {
        Dictionary<DetailButtonType, DetailButton> detailButton = new Dictionary<DetailButtonType, DetailButton>();
        if (item.holder.GetHolderType() == HolderType.Player)
        {
            detailButton[DetailButtonType.Up] = null;
            detailButton[DetailButtonType.Middle] = null;
            detailButton[DetailButtonType.Down] = new DetailButton("放入", Color.white, () =>
            {
                if (item is Equip)
                {
                    if (targetButton.GetItem() != null)
                        targetButton.ReturnItem();
                    targetButton.AddItem(item);
                }
                else
                {
                    bool isSuccess = false;
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        if (buttons[i].IsEmpty())
                        {
                            buttons[i].AddItem(item);
                            isSuccess = true;
                            break;
                        }
                    }
                    if (isSuccess == false)
                    {
                        UIManager.GetInstance().ShowTips("强化材料已满");
                    }
                }

                UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
            });
        }
        else
        {
            detailButton[DetailButtonType.Up] = null;
            detailButton[DetailButtonType.Middle] = null;
            detailButton[DetailButtonType.Down] = new DetailButton("取回", Color.white, () =>
            {
                StrengthenButton strengthenButton = item.holder as StrengthenButton;
                strengthenButton.ReturnItem();

                UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
            });
        }
        return detailButton;
    }

    public override string GetPanelName()
    {
        return StrengthenPanel.prefabsPath;
    }
}
