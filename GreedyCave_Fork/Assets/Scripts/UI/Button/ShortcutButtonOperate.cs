using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class ShortcutButtonOperate : ShortcutButton, IPointerDownHandler,IPointerUpHandler
{
    private PointerEventData eventData;
    private bool isPointer = false;
    protected override void Update()
    {
        base.Update();
        if (shortcutObj == null)
            return;
        if (isPointer)
        {
            eventData.position = Input.mousePosition;
            OnPointer(eventData);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (shortcutObj == null)
            return;

        isPointer = true;
        this.eventData = eventData;

        shortcutObj.OnTriggerStart(PlayerMgr.Instance.GetWarrior());
    }
    public void OnPointer(PointerEventData eventData)
    {
        if (shortcutObj == null)
            return;

        shortcutObj.OnTrigger(PlayerMgr.Instance.GetWarrior());
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (shortcutObj == null)
            return;

        if (shortcutObj.GetCurrentCD() <= 0)
        {
            shortcutObj.OnTriggerEnd(PlayerMgr.Instance.GetWarrior(), () =>
            {
                PlayerMgr.Instance.AddShortcutCD(shortcutObj);
            });
        }
        else
        {
            UIManager.GetInstance().ShowTips("¿‰»¥ ±º‰Œ¥Ω· ¯");
        }

        isPointer = false;
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
