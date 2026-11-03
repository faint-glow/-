using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShortcutButtonUI : ShortcutButton, IDragUpAble<IDragAble>
{
    public IDragAble SetItem_Drag(IDragAble obj)
    {
        IShortcutAble shortcutAble = obj as IShortcutAble;
        if (shortcutAble == null)
        {
            UIManager.GetInstance().ShowTips("此物品不可加入快捷键");
            return null;
        }
        if(shortcutAble.GetShortcutType() != this.shorcutType)
        {
            UIManager.GetInstance().ShowTips("这不是对应快捷键");
            return null;
        }
        if (PlayerMgr.Instance.IsBelongMe(shortcutAble) == false)
        {
            UIManager.GetInstance().ShowTips("无效操作");
            return null;
        }
        //SetShortcut(shortcutAble);
        PlayerMgr.Instance.SetShortcut(shortcutAble,this.shorcutType ,this.index);
        return null;
    }
    protected override Image GetFrameBackImage()
    {
        return transform.GetComponent<Image>();
    }
    protected override Image GetShortcutBackImage()
    {
        return transform.GetChild(0).GetComponent<Image>();
    }
    protected override Image GetShortcutImage()
    {
        return transform.GetChild(1).GetComponent<Image>();
    }
}
