using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UITools
{
    /// <summary>
    /// 为UI组件添加事件
    /// </summary>
    /// <param name="obj">需要添加事件的UI组件</param>
    /// <param name="type"></param>
    /// <param name="call"></param>
    public static void AddEvent(UIBehaviour obj, EventTriggerType type, UnityAction<BaseEventData> call)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = obj.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(call);

        trigger.triggers.Add(entry);
    }

}
