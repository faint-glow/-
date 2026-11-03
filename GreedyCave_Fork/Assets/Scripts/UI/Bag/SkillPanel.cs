using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : ContainerPanel<SkillButton, SkillInfoMgr>
{
    public static string prefabsPath = "SkillPanel";

    private Transform mainPanel;
    private ScrollRect skillScrollRect;
    private ShortcutPanel shortcutPanel;
    private SkillDamageType skillType;
    public override void ShowMeFirst()
    {
        mainPanel = GetPanel("Main/Middle/Main/Viewport/Content").transform;
        skillScrollRect = GetPanel("Main/Middle/Main").GetUI<ScrollRect>();
        shortcutPanel = GetPanel("Main/Down/ShortcutPanel").transform.GetComponent<ShortcutPanel>();

        UIManager.InitialAbandonButton(this, "Main/Up/Main/Button");
        UIManager.InitialAbandonButton(this, "Main/Up/Main/Button (1)");
        UIManager.InitialAbandonButton(this, "Main/Up/Main/Button (2)");
        UIManager.InitialAbandonButton(this, "Main/Up/Main/Button (3)");
        UIManager.InitialAbandonButton(this, "Main/Up/Main/Button (4)");
        UIManager.InitialAbandonButton(this, "Main/Up/Main/Button (5)");

        EventCenter.GetInstance().AddEventListener<SkillInfoMgr>("学习技能", OnLearnSkill);
    }
    public override void DestroyMe()
    {
        EventCenter.GetInstance().RemoveEventListener<SkillInfoMgr>("学习技能", OnLearnSkill);
    }
    private void OnLearnSkill(SkillInfoMgr skill)
    {
        List<SkillInfoMgr> skills = PlayerMgr.Instance.GetSkills(skillType);
        RefreshPanel(skills);
    }
    public override void ShowMeEveryTime()
    {
        shortcutPanel.SetShortcutType(ShortcutType.技能);
    }
    public void SetSkillType(SkillDamageType skillType)
    {
        this.skillType = skillType;
        List<SkillInfoMgr> skills = PlayerMgr.Instance.GetSkills(skillType);
        RefreshPanel(skills);
    }
    public override IHolderAble GetHolder()
    {
        return PlayerMgr.Instance;
    }
    public override string GetCellPath()
    {
        return SkillButton.prefabsPath;
    }
    public override Transform GetContainterTrans()
    {
        return mainPanel;
    }
    public void RefreshPanel(List<SkillInfoMgr> skillList)
    {
        RefreshContainer(skillList);
    }
    public override ScrollRect GetScrollRect()
    {
        return skillScrollRect;
    }

}
