using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EffectProperty
{
    public SerializedProperty effectInfo;
    public SerializedProperty effectCondition;
    public SerializedProperty triggerTypes;
    public SerializedProperty intervalTime;
    public SerializedProperty triggerRatio;
    public SerializedProperty applyTargetType;
    public SerializedProperty effectType;
    public SerializedProperty targetProperty;
    public SerializedProperty value;
    public SerializedProperty controlInfos;
    public SerializedProperty buffInfos;
    public SerializedProperty fxMgr;
    public SerializedProperty audioMgr;
}

//GUIUtility.systemCopyBuffer//unity的剪贴板，可以访问也可以读取
// 只能在 position 指定的范围内绘制
//EditorGUI.BeginProperty(position, label, property);
//EditorStyles.textField.wordWrap = true;自动换行样式,不过使用TextArea可以直接拥有，但是如果加上ScrollView需要使用这个了
[CustomPropertyDrawer(typeof(Effect))]
public class EffectEditor : SelfAdaptionPropertyDrawer
{
    private static EffectProperty GetEffectProperty(SerializedProperty property)
    {
        EffectProperty effectProperty = new EffectProperty();
        effectProperty.effectInfo = property.FindPropertyRelative("effectInfo");
        effectProperty.effectCondition = property.FindPropertyRelative("effectCondition");
        effectProperty.triggerTypes = property.FindPropertyRelative("triggerTypes");
        effectProperty.intervalTime = property.FindPropertyRelative("intervalTime");
        effectProperty.triggerRatio = property.FindPropertyRelative("triggerRatio");
        effectProperty.applyTargetType = property.FindPropertyRelative("applyTargetType");
        effectProperty.effectType = property.FindPropertyRelative("effectType");
        effectProperty.targetProperty = property.FindPropertyRelative("targetProperty");
        effectProperty.value = property.FindPropertyRelative("value");
        effectProperty.controlInfos = property.FindPropertyRelative("controlInfos");
        effectProperty.buffInfos = property.FindPropertyRelative("buffInfos");
        effectProperty.fxMgr = property.FindPropertyRelative("fxMgr");
        effectProperty.audioMgr = property.FindPropertyRelative("audioMgr");
        return effectProperty;
    }
    //public override string GetSpreadName(SerializedProperty property)
    //{
    //    SerializedProperty effectType = property.FindPropertyRelative("effectType");
    //    SerializedProperty targetProperty = property.FindPropertyRelative("targetProperty");
    //    string spreadName = ((EffectType)effectType.enumValueIndex).ToString();
    //    if ((EffectType)effectType.enumValueIndex == EffectType.修改属性)
    //    {
    //        spreadName += "：" + ((PropertyType)targetProperty.enumValueIndex).ToString();
    //    }
    //    return spreadName;
    //}
    public override string GetSpreadName(SerializedProperty property)
    {
        EffectProperty effectProperty = GetEffectProperty(property);

        string useTextInfo = "";
        string content = GetTextInfo(effectProperty);

        if (IsUseTextInfo(effectProperty))
            useTextInfo = "(已使用)";
        else
            useTextInfo = "(未使用)";
        content = useTextInfo + content;
        int index = content.IndexOf("\n");
        if (index > 0)
            content = content.Substring(0, index) + "{多行省略}";

        return content;
    }

    private bool IsUseTextInfo(EffectProperty effectProperty)
    {
        return TextInfoEditor.IsUseTextInfo(effectProperty.effectInfo);
    }

    public string buffName = "NewBuff";
    public float defaultTriggerRatio = 1;

    public override void InitialProperty(SerializedProperty property)
    {
        SerializedProperty applyTargetType = property.FindPropertyRelative("applyTargetType");
        applyTargetType.enumValueIndex = (int)ApplyTargetType.Target;
        SerializedProperty triggerRatio = property.FindPropertyRelative("triggerRatio");
        triggerRatio.floatValue = 1;
    }

    public override void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EffectProperty effectProperty = GetEffectProperty(property);
        TextInfoEditor.SetAudioTextInfo(effectProperty.effectInfo, GetTextInfo(effectProperty));
        ShowPropertyVertical(effectProperty.effectInfo);
        ShowPropertyVertical(effectProperty.effectCondition);
        ShowPropertyVertical(effectProperty.triggerTypes);
        if (IsHaveTargetTriggerType(effectProperty.triggerTypes, TriggerType.间隙触发) == false)
            AddHideProperty(effectProperty.intervalTime);
        ShowPropertyVertical(effectProperty.intervalTime);
        ShowPropertyVertical(effectProperty.triggerRatio);
        ShowPropertyVertical(effectProperty.effectType);

        if ((EffectType)effectProperty.effectType.enumValueIndex != EffectType.控制效果)
            AddHideProperty(effectProperty.controlInfos);
        if ((EffectType)effectProperty.effectType.enumValueIndex == EffectType.控制效果)
        {
            AddHideProperty(effectProperty.targetProperty);
            AddHideProperty(effectProperty.value);
        }
        if ((EffectType)effectProperty.effectType.enumValueIndex != EffectType.施加buff)
            AddHideProperty(effectProperty.buffInfos);
        if ((EffectType)effectProperty.effectType.enumValueIndex == EffectType.施加buff)
        {
            AddHideProperty(effectProperty.targetProperty);
            AddHideProperty(effectProperty.value);
        }
        if ((EffectType)effectProperty.effectType.enumValueIndex == EffectType.造成伤害)
            ChangePropertyDisplayName(effectProperty.targetProperty, "伤害类型");
        if ((EffectType)effectProperty.effectType.enumValueIndex == EffectType.触发特效)
        {
            AddHideProperty(effectProperty.targetProperty);
            AddHideProperty(effectProperty.value);
            AddHideProperty(effectProperty.buffInfos);
        }
        if ((EffectType)effectProperty.effectType.enumValueIndex == EffectType.清除当前Buff)
        {
            AddHideProperty(effectProperty.applyTargetType);
            AddHideProperty(effectProperty.targetProperty);
        }

        ShowPropertyVertical(effectProperty.applyTargetType);
        ShowPropertyVertical(effectProperty.targetProperty, "目标属性");

        ShowPropertyVertical(effectProperty.value);
        ShowPropertyVertical(effectProperty.controlInfos);
        ShowPropertyVertical(effectProperty.buffInfos, "", 0, 0, () =>
        {
            buffName = ShowTextArea(buffName);
            CreateButtonMgr.CreateBuff_List(GetRect_Height(20), effectProperty.buffInfos, buffName, true);
        });
        ShowPropertyVertical(effectProperty.fxMgr);
        ShowPropertyVertical(effectProperty.audioMgr);
    }
    public static bool IsHaveTargetTriggerType(SerializedProperty property, TriggerType type)
    {
        for (int i = 0; i < property.arraySize; i++)
        {
            if ((TriggerType)property.GetArrayElementAtIndex(i).enumValueIndex == type)
                return true;
        }
        return false;
    }
    public static string GetTextInfo(EffectProperty effectProperty)
    {
        string content = "";
        if (effectProperty.effectType.enumValueIndex == (int)EffectType.修改属性)
            content = GetAutoEffectInfo_ChangeProperty(effectProperty);
        else if (effectProperty.effectType.enumValueIndex == (int)EffectType.施加buff)
            content = GetAutoEffectInfo_AddBuff(effectProperty);
        else if (effectProperty.effectType.enumValueIndex == (int)EffectType.造成伤害)
            content = GetAutoEffectInfo_CauseDamage(effectProperty);
        else if (effectProperty.effectType.enumValueIndex == (int)EffectType.触发特效)
            content = GetAutoEffectInfo_TriggerEffect(effectProperty);
        else if (effectProperty.effectType.enumValueIndex == (int)EffectType.控制效果)
            content = GetAutoEffectInfo_AddControl(effectProperty);
        else if (effectProperty.effectType.enumValueIndex == (int)EffectType.调整受到伤害)
            content = GetAutoEffectInfo_AdjustInjuredValue(effectProperty);
        else if (effectProperty.effectType.enumValueIndex == (int)EffectType.清除当前Buff)
            content = GetAutoEffectInfo_RemoveBuff(effectProperty);
        return content;
    }
    public static string GetAutoEffectInfo_ChangeProperty(EffectProperty effectProperty)
    {
        string content = "";
        content += GetTriggerTypeText(effectProperty) + GetTriggerRatioText(effectProperty) + GetTarget(effectProperty) + GetAutoEffectPlusInfo(effectProperty.value) + GetAutoValueInfo(effectProperty) + "的" + ((PropertyType)effectProperty.targetProperty.enumValueIndex).ToString() + "。";
        return content;
    }
    public static string GetAutoEffectInfo_AddBuff(EffectProperty effectProperty)
    {
        string content = "";
        if (effectProperty.buffInfos.arraySize <= 0)
            content = "buff数量为0";
        for (int i = 0; i < effectProperty.buffInfos.arraySize; i++)
        {
            BuffInfo buffInfo = effectProperty.buffInfos.GetArrayElementAtIndex(i).objectReferenceValue as BuffInfo;
            if (buffInfo == null)
            {
                content += "未添加buff文件";
                continue;
            }
            content += GetTriggerTypeText(effectProperty) + GetTriggerRatioText(effectProperty) + "对" + GetTarget(effectProperty) + "施加" + buffInfo.GetBuffName() + "buff。";
            content += "\n";
            content += "(" + buffInfo.GetBuffInfoText() + ")";
        }
        return content;
    }
    public static string GetAutoEffectInfo_CauseDamage(EffectProperty effectProperty)
    {
        string content = "";
        content += GetTriggerTypeText(effectProperty) + GetTriggerRatioText(effectProperty) + "对" + GetTarget(effectProperty) + "造成" + GetAutoValueInfo(effectProperty) + "的" + Property.GetAttackTypeText((PropertyType)effectProperty.targetProperty.enumValueIndex) + "伤害" + "。";
        if (ValueEditor.IsPlus(effectProperty.value))
            content += "(造成伤害必须要为负值,请修改)";
        return content;
    }
    public static string GetAutoEffectInfo_TriggerEffect(EffectProperty effectProperty)
    {
        string content = "";
        content += GetTriggerTypeText(effectProperty) + GetTriggerRatioText(effectProperty) + "播放音乐或触发特效";
        return content;
    }
    public static string GetAutoEffectInfo_AddControl(EffectProperty effectProperty)
    {
        string content = "";
        for (int i = 0; i < effectProperty.controlInfos.arraySize; i++)
        {
            ControlInfo controlInfo = (effectProperty.controlInfos.GetArrayElementAtIndex(i).objectReferenceValue as ControlInfo);
            if (controlInfo == null)
            {
                content += "未添加控制文件";
                continue;
            }
            content += GetTriggerTypeText(effectProperty) + GetTriggerRatioText(effectProperty) + "对" + GetTarget(effectProperty) + "施加" + controlInfo.GetControlInfoText() + "效果。";
        }
        return content;
    }

    public static string GetAutoEffectInfo_AdjustInjuredValue(EffectProperty effectProperty)
    {
        string content = "";
        content += GetTriggerTypeText(effectProperty) + GetTriggerRatioText(effectProperty) + GetAutoEffectPlusInfo(effectProperty.value) + GetAutoValueInfo(effectProperty) + "的" + Property.GetAttackTypeText((PropertyType)effectProperty.targetProperty.enumValueIndex) + "伤害";
        return content;
    }

    public static string GetAutoEffectInfo_RemoveBuff(EffectProperty effectProperty)
    {
        string content = "";
        content += GetTriggerTypeText(effectProperty) + GetEffectConditionInfo(effectProperty) + GetTriggerRatioText(effectProperty) + "清除Buff";
        return content;
    }

    private static string GetTarget(EffectProperty effectProperty)
    {
        return Warrior.GetApplyTargetTypeInfo((ApplyTargetType)effectProperty.applyTargetType.enumValueIndex);
    }
    private static string GetEffectConditionInfo(EffectProperty effectProperty)
    {
        return EffectConditionEditor.GetEffectConditionInfo(effectProperty.effectCondition);
    }
    public static string GetTriggerTypeText(EffectProperty effectProperty)
    {
        List<string> contents = new List<string>();
        for (int i = 0; i < effectProperty.triggerTypes.arraySize; i++)
        {
            string content = "";//待修改
            if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.使用道具触发)
                content = "使用道具时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.升级触发)
                content = "升级时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.受击触发)
                content = "受击时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.存在触发)
                content = "存在时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.战斗结束触发)
                content = "战斗结束时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.技能命中敌人触发)
                content = "技能命中敌人时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.普攻命中敌人触发)
                content = "普攻命中敌人时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.死亡触发)
                content = "死亡时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.释放技能对所有目标触发)
                content = "释放技能时对所有目标";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.间隙触发)
                content = "每间隔" + effectProperty.intervalTime.floatValue + "秒";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.动画触发)
                content = "播放动画时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.普攻命中队友触发)
                content = "普攻命中队友时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.技能命中队友触发)
                content = "技能命中队友时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.使用道具命中敌人触发)
                content = "使用道具命中敌人时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.修改属性触发)
                content = "修改属性时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.施加buff触发)
                content = "施加buff时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.普攻造成伤害后触发)
                content = "普攻造成伤害后";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.技能造成伤害后触发)
                content = "技能造成伤害后";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.普攻造成伤害前触发)
                content = "普攻造成伤害前";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.技能造成伤害前触发)
                content = "技能造成伤害前";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.释放技能时触发)
                content = "释放技能时";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.受到普攻伤害前触发)
                content = "受到普攻伤害前";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.受到技能伤害前触发)
                content = "受到技能伤害前";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.受到普攻伤害后触发)
                content = "受到普攻伤害后";
            else if (effectProperty.triggerTypes.GetArrayElementAtIndex(i).enumValueIndex == (int)TriggerType.受到技能伤害后触发)
                content = "受到技能伤害后";
            contents.Add(content);
        }
        string endContent = "";
        for (int i = 0; i < contents.Count; i++)
        {
            endContent += contents[i];
            if (i < contents.Count - 1)
                endContent += "或";
        }
        return endContent;
    }
    public static string GetTriggerRatioText(EffectProperty effectProperty)
    {
        string content = "";
        if (effectProperty.triggerRatio.floatValue != 1)
            content = "有" + effectProperty.triggerRatio.floatValue * 100 + "%" + "的概率";
        return content;
    }
    public static string GetAutoValueInfo(EffectProperty effectProperty)
    {
        return ValueEditor.GetAutoValueInfo(effectProperty.value, (PropertyType)effectProperty.targetProperty.enumValueIndex);
    }
    public static string GetAutoEffectPlusInfo(SerializedProperty value)
    {
        return ValueEditor.GetAutoEffectPlusInfo(value);
    }

    public static string GetFXMgrInfo()
    {
        string content = "";

        return content;
    }
}
