using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ShortcutType
{
    道具,
    技能,
    无,
}

public class ShortcutPanel : MapPanel
{
    private Button baseSelectButton;
    private Transform selectButtonFather;
    private List<Button> selectButtonList = new List<Button>();
    private List<ShortcutButtonUI> shortcutButtonUIs = new List<ShortcutButtonUI>();
    private ShortcutType currentShortcutType = ShortcutType.道具;
    //private Dictionary<ShortcutType, UnityAction> refreshTypeDic = new Dictionary<ShortcutType, UnityAction>();
    private List<ShortcutType> allShortcutType = new List<ShortcutType>();
    public override void ShowMeFirst()
    {
        Transform shortcutButtonFather = GetPanel("Shortcut").transform;
        for (int i = 0; i < shortcutButtonFather.childCount; i++)
        {
            shortcutButtonUIs.Add(shortcutButtonFather.GetChild(i).GetComponent<ShortcutButtonUI>());
        }
        selectButtonFather = GetPanel("SelectShortcut").transform;
        baseSelectButton = GetPanel("SelectShortcut/Button").GetUI<Button>();

        InitialAllSelectButton();
        InitialBaseButton();
    }

    public void SetShortcutType(ShortcutType shortcutType)
    {
        this.currentShortcutType = shortcutType;
        this.baseSelectButton.gameObject.SetActive(true);
        this.baseSelectButton.GetComponentInChildren<TMP_Text>().text = shortcutType.ToString();
        for (int i = 0; i < shortcutButtonUIs.Count; i++)
            shortcutButtonUIs[i].SetShortcutType(currentShortcutType);
    }

    private void InitialAllSelectButton()
    {
        allShortcutType.Add(ShortcutType.道具);
        allShortcutType.Add(ShortcutType.技能);
    }

    private void InitialBaseButton()
    {
        baseSelectButton.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);

            List<Action> hideMaskActions = new List<Action>();
            string lastBaseSelectButtonName = baseSelectButton.GetComponentInChildren<TMP_Text>().text;
            Action<MaskPanel> masAction = (MaskPanel mask) =>
            {
                ClearSelectButtons(selectButtonList);
                SetShortcutType(currentShortcutType);

                ToolsMgr.InvokeAllActions(hideMaskActions);
            };
            MaskPanelMgr.GetInstance().ShowMask(selectButtonFather, masAction, hideMaskActions);

            ShowSelectButton(selectButtonFather, currentShortcutType, () =>
            {
                ToolsMgr.InvokeAllActions(hideMaskActions);
            });
            for (int i = 0; i < allShortcutType.Count; i++)
            {
                if (allShortcutType[i] == currentShortcutType)
                    continue;
                ShowSelectButton(selectButtonFather, allShortcutType[i], () =>
                {
                    ToolsMgr.InvokeAllActions(hideMaskActions);
                });
            }
            baseSelectButton.gameObject.SetActive(false);
        });
    }

    private void ShowSelectButton(Transform father, ShortcutType shortcutType,Action action)
    {
        Button newButton = GameObject.Instantiate(baseSelectButton);
        TMP_Text text = newButton.GetComponentInChildren<TMP_Text>();
        newButton.transform.SetParent(father);
        text.text = shortcutType.ToString();
        newButton.onClick.RemoveAllListeners();
        newButton.onClick.AddListener(()=> 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);

            if (action != null)
                action.Invoke();

            ClearSelectButtons(selectButtonList);
            SetShortcutType(shortcutType);
        });
        selectButtonList.Add(newButton);
    }
    private void ClearSelectButtons(List<Button> selectButtonList)
    {
        for (int i = 0; i < selectButtonList.Count; i++)
        {
            if (selectButtonList[i] != null)
                GameObject.Destroy(selectButtonList[i].gameObject);
        }
        selectButtonList.Clear();
    }
}
