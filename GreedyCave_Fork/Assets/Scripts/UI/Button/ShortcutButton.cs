using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShortcutInfo
{
    /// <summary>
    /// 设置快捷键需要的信息
    /// </summary>
    /// <param name="shortcutObj">被设置的物体</param>
    /// <param name="shortCutType">目标设置的快捷键类型</param>
    /// <param name="index">位置</param>
    public ShortcutInfo(IShortcutAble shortcutObj, ShortcutType shortCutType,int index)
    {
        this.shortcutObj = shortcutObj;
        this.shortCutType = shortCutType;
        this.index = index;
    }
    public IShortcutAble shortcutObj;
    public ShortcutType shortCutType;
    public int index;
}
public abstract class ShortcutButton : MapPanel
{
    private static Sprite shortcutSprite_Prop;
    private static Sprite shortcutSprite_Skill;
    public static Sprite GetShortcutSprite(ShortcutType shortcutType)
    {
        if(shortcutType == ShortcutType.技能)
        {
            if (shortcutSprite_Skill == null)
                shortcutSprite_Skill = ResMgr.GetInstance().Load<Sprite>("Sprite/Shortcut/ShortcutProp_Skill");
            return shortcutSprite_Skill;
        }
        else if (shortcutType == ShortcutType.道具)
        {
            if (shortcutSprite_Prop == null)
                shortcutSprite_Prop = ResMgr.GetInstance().Load<Sprite>("Sprite/Shortcut/ShortcutSprite_Prop");
            return shortcutSprite_Prop;
        }
        return null;
    }

    protected IShortcutAble shortcutObj;
    protected int index;
    [SerializeField] protected ShortcutType shorcutType;

    protected Image mask;
    protected Image shortcutImage;
    protected Image shortcutBackImage;
    protected TMP_Text countText;
    protected abstract Image GetFrameBackImage();
    protected abstract Image GetShortcutImage();
    protected abstract Image GetShortcutBackImage();
    public void SetShortcutType(ShortcutType shorcutType)
    {
        this.shorcutType = shorcutType;
        this.GetFrameBackImage().sprite = GetShortcutSprite(shorcutType);
        this.RefreshShortcutButton();
    }
    public override void ShowMeFirst()
    {
        index = transform.GetSiblingIndex();
        shortcutImage = GetShortcutImage();
        shortcutBackImage = GetShortcutBackImage();
        mask = GetPanel("Image (1)/Mask").GetUI<Image>();
        countText = GetPanel("CountText").GetUI<TMP_Text>();

        shortcutImage.preserveAspect = true;
        shortcutBackImage.preserveAspect = true;
        RefreshShortcutButton();
        GetFrameBackImage().sprite = GetShortcutSprite(shorcutType);
        EventCenter.GetInstance().AddEventListener<ShortcutInfo>("ChangeShortcut", SetShortcut);
    }
    public override void DestroyMe()
    {
        EventCenter.GetInstance().RemoveEventListener<ShortcutInfo>("ChangeShortcut", SetShortcut);
    }
    protected virtual void Update()
    {
        if (shortcutObj == null)
        {
            mask.fillAmount = 0;
            countText.text = "";
            return;
        }
        if (shortcutObj.GetCount() <= 1)
            countText.text = "";
        else
            countText.text = shortcutObj.GetCount().ToString();

        if (shortcutObj.GetAllCD() == 0)
            mask.fillAmount = 0;
        else
            mask.fillAmount = shortcutObj.GetCurrentCD() / shortcutObj.GetAllCD();

        if (shortcutObj.IsRunout() == true)
            RemoveShortcut();
    }
    private void RefreshShortcutButton()
    {
        IShortcutAble[] shortcuts = PlayerMgr.Instance.GetShortcuts(shorcutType);
        if(this.index < shortcuts.Length)
        {
            SetShortcut(new ShortcutInfo(shortcuts[index],this.shorcutType ,index));
        }
    }

    public void SetShortcut(ShortcutInfo shortcutInfo)
    {
        if (shortcutInfo.index != this.index || shortcutInfo.shortCutType != this.shorcutType)
            return;
        if(shortcutInfo.shortcutObj == null)
        {
            this.shortcutObj = null;
            this.shortcutImage.gameObject.SetActive(false);
            return;
        }
        this.shortcutObj = shortcutInfo.shortcutObj;
        shortcutImage.sprite = shortcutObj.GetSprite_Drag();
        shortcutImage.gameObject.SetActive(true);
    }

    public void RemoveShortcut()
    {
        this.shortcutObj = null;
        this.shortcutImage.gameObject.SetActive(false);
        PlayerMgr.Instance.RemoveShortcut(this.shorcutType,index);
    }
}
