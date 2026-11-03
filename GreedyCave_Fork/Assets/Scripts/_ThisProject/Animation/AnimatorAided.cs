using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//同一种类的不同个体
public class AnimatorAided
{
    //事件增删的封装
    public class ClipAided
    {
        public void AddEvent(EventTriggerEnum type, Action action)
        {
            //if (!eventDic.ContainsKey(type) || eventDic[type] == null)
            //    eventDic[type] = action;
            //else
            //    eventDic[type] += action;

            eventDic[type] = action;
        }
        public void RemoveEvent(EventTriggerEnum type, Action action)
        {
            //if (!eventDic.ContainsKey(type) || eventDic[type] == null)
            //    return;
            //eventDic[type] -= action;
            //if (eventDic[type] == null)
            //    eventDic.Remove(type);
            eventDic.Remove(type);
        }
        //public void RemoveEvents(EventTriggerEnum type)
        //{
        //    if (!eventDic.ContainsKey(type))
        //        return;

        //    eventDic.Remove(type);
        //}

        //EventTriggerEnum 对应 方法 
        public Dictionary<EventTriggerEnum, Action> eventDic = new Dictionary<EventTriggerEnum, Action>();
    }
    //动画片段的名字 对应 （EventTriggerEnum 对应 个数） 作用：给一个动画片段的名字，查看这个动画片段身上有几个EventTriggerEnum
    private Dictionary<string, Dictionary<EventTriggerEnum, int>> clipEventUserDic = new Dictionary<string, Dictionary<EventTriggerEnum, int>>();
    //一个种类所有的Animator 对应（动画片段的名字 对应 事件增删封装） 作用：给出目标物体的Animator，和目标动画片段，以及ClipAided中需要的EventTriggerEnum，寻找目标触发事件
    public Dictionary<Animator, Dictionary<string, ClipAided>> events = new Dictionary<Animator, Dictionary<string, ClipAided>>();

    public int GetTypeCount()
    {
        return events.Count;
    }
    public void PrintAnimatorCount(Animator anim,string type)
    {
        Debug.Log(type +"的总数有："+ events.Count + "个");
        if(events.ContainsKey(anim))
        {
            foreach (var item in events[anim])
            {
                //        物体名字      动画Clip名字                使用的方法数量
                Debug.Log(type + "的" + item.Key + "动画使用了：" + item.Value.eventDic.Count + "个方法");
            }
        }

        foreach (var item in clipEventUserDic)
        {
            foreach (var newItem in item.Value)
            {
                //       物体名字       动画Clip名字            EventTriggerEnum           使用者数量
                Debug.Log(type + "的" + item.Key + "动画的：" + newItem.Key + "方法有：" + newItem.Value + "个使用者");
            }
        }
    }

    public Action CheckIsHave(Animator anim, AnimationClip clip, EventTriggerEnum funcEnum)
    {
        if (events.ContainsKey(anim))
        {
            if (clip != null && events[anim].ContainsKey(clip.name))
            {
                if(events[anim][clip.name].eventDic.ContainsKey(funcEnum))
                {
                    return events[anim][clip.name].eventDic[funcEnum];
                }
            }
        }
        return null;
    }

    //此方法应由继承自AnimatorController的脚本调用
    public bool AddEvent(Animator anim, AnimationClip clip, EventTriggerEnum funcEnum, Action action)
    {
        if (!events.ContainsKey(anim))
            events[anim] = new Dictionary<string, ClipAided>();

        bool isSetNewAction = ClipEventUserDicAdd(clip, funcEnum, action,anim);
        if (!events[anim].ContainsKey(clip.name))
            events[anim][clip.name] = new ClipAided();

        events[anim][clip.name].AddEvent(funcEnum, action);//为funcEnum添加方法
        return isSetNewAction;
    }

    public bool RemoveEvent(Animator anim, AnimationClip clip, EventTriggerEnum funcEnum, Action action)
    {
        if (events.ContainsKey(anim) && events[anim].ContainsKey(clip.name))
        {
            events[anim][clip.name].RemoveEvent(funcEnum, action);
            if (events[anim][clip.name].eventDic.Count <= 0)
                events[anim].Remove(clip.name);
            if (events[anim].Count <= 0)
                events.Remove(anim);
        }

        return ClipEventUserDicRemove(clip, funcEnum, action);
    }

    private bool ClipEventUserDicAdd(AnimationClip clip, EventTriggerEnum funcEnum, Action action, Animator anim)
    {
        if (clip == null)
            return false;
        if (!clipEventUserDic.ContainsKey(clip.name))
        {
            clipEventUserDic[clip.name] = new Dictionary<EventTriggerEnum, int>();
            clipEventUserDic[clip.name][funcEnum] = 1;
            return true;
        }
        else
        {
            if(!clipEventUserDic[clip.name].ContainsKey(funcEnum))
            {
                clipEventUserDic[clip.name][funcEnum] = 1;
                return true;
            }
            else
            {
                clipEventUserDic[clip.name][funcEnum] += 1;
                return false;
            }
        }
    }

    private bool ClipEventUserDicRemove(AnimationClip clip, EventTriggerEnum funcEnum, Action action)
    {
        if (clipEventUserDic.ContainsKey(clip.name) && clipEventUserDic[clip.name].ContainsKey(funcEnum))
        {
            clipEventUserDic[clip.name][funcEnum] -= 1;
            //if (clipEventUserDic[clip.name][funcEnum] <= 0)
            //{
            //    clipEventUserDic[clip.name].Remove(funcEnum);
            //    if (clipEventUserDic[clip.name].Count <= 0)
            //        clipEventUserDic.Remove(clip.name);
            //    return true;
            //}
            clipEventUserDic[clip.name][funcEnum] = clipEventUserDic[clip.name][funcEnum] < 0 ? 0 : clipEventUserDic[clip.name][funcEnum];
        }

        return false;
    }

    public Action GetEvent(Animator anim, string clipName, EventTriggerEnum type)
    {

        //if (anim.gameObject.name == "Anim")
        //{
        //    Debug.Log(anim.gameObject.name + ":" + clipName + ":" + type);
        //    Debug.Log(events.ContainsKey(anim));
        //    Debug.Log(events[anim].ContainsKey(clipName));
        //    Debug.Log(events[anim][clipName].eventDic.ContainsKey(type));
        //}
        if (events.ContainsKey(anim) && events[anim].ContainsKey(clipName))
        {
            if (events[anim][clipName].eventDic.ContainsKey(type))
            {
                return events[anim][clipName].eventDic[type];
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public void PrintEvents(Animator anim)
    {
        if (events.ContainsKey(anim))
        {
            foreach (KeyValuePair<string, ClipAided> item in events[anim])
            {
                string content = "";
                content += item.Key + ":";
                if(item.Value.eventDic != null)
                {
                    foreach (KeyValuePair<EventTriggerEnum, Action> newItem in item.Value.eventDic)
                    {
                        content +=  newItem.Key.ToString() + ",";
                    }
                }
                Debug.Log(content);
            }
        }
    }

}
