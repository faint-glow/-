using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyDictionary/Skill/BuffInfo", fileName = "NewBuff")]
public class BuffInfo : ScriptableObject, INameAble
{
    public string buffName;//buff的名字
    public float liveTime = 1;//存活时间
    public ExistType existType = ExistType.Time;//是否没有倒计时
    public List<BuffCategory> buffCategoryList = new List<BuffCategory>() { BuffCategory.无 };
    public int overlayType = 1;//可叠加层数
    public List<Effect> effects = new List<Effect>();
    public List<FXInfo> fxInfoList = new List<FXInfo>();
    public EffectFXMgr fxMgr;
    public string GetName()
    {
        return buffName;
    }
    public string GetAddBuffInfo()
    {
        string content = "";
        content += buffName;//"对" + Warrior.GetApplyTargetTypeInfo(applyTargetType) + "施加" + buffName + "Buff";
        return content;
    }
    public string GetBuffName()
    {
        return buffName;
    }
    public string GetBuffInfoText()
    {
        string existInfo = "";
        if (existType == ExistType.Time)
            existInfo = "持续" + liveTime + "秒";
        return buffName + ":" + existInfo + "\n" + Effect.GetEffectDicInfoText(effects, "");
    }
}

[System.Serializable]
public class BuffInfoHandler
{
    public TriggerType triggerType;
    public List<Effect> effects = new List<Effect>();
}


