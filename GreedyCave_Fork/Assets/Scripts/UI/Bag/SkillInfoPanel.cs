using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillInfoPanel : MapPanel
{
    public static string prefabsPath = "SkillInfoPanel";

    private TMP_Text skillNameText;
    private TMP_Text skillLevelText;
    private TMP_Text skillInfoText;

    private Transform levelObj;
    private Image skillImage;
    private Button triggerButton;
    private TMP_Text triggerButtonText;
    private TMP_Text tipText;
    private TMP_Text needSkillPointText;
    private TMP_Text currentSkillPointText;

    private SkillInfoMgr skillInfoMgr;
    private SkillDamageType skillType;
    public override void ShowMeFirst()
    {
        skillNameText = GetPanel("Main/Panel/Image/Text (TMP)").GetUI<TMP_Text>();
        skillLevelText = GetPanel("Main/Panel/Level/Image/Text (TMP)").GetUI<TMP_Text>();
        levelObj  = GetPanel("Main/Panel/Level").transform;
        skillInfoText = GetPanel("Main/Panel/Scroll View/Viewport/Content/Text (TMP)").GetUI<TMP_Text>();

        skillImage = GetPanel("Main/Panel/Image/Image").GetUI<Image>();
        triggerButton = GetPanel("Main/Panel/Button").GetUI<Button>();
        triggerButtonText = GetPanel("Main/Panel/Button/Text (TMP)").GetUI<TMP_Text>();
        tipText = GetPanel("Main/Panel/TipText").GetUI<TMP_Text>();
        triggerButtonText.text = "学习";
        needSkillPointText = GetPanel("Main/Panel/NeedSkillPoint").GetUI<TMP_Text>();
        currentSkillPointText = GetPanel("Main/Panel/CurrentSkillPoint").GetUI<TMP_Text>();


        triggerButton.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);

            if (skillInfoMgr == null)
                return;
            SkillController skillController = PlayerMgr.Instance.GetSkillController();
            skillController.LearnSkill(skillInfoMgr);
            SetSkill(skillInfoMgr);
        });

        EventCenter.GetInstance().AddEventListener<int>(EventCenterEnum.升级.ToString(), LevelUp);
    }
    public override void DestroyMe()
    {
        EventCenter.GetInstance().RemoveEventListener<int>(EventCenterEnum.升级.ToString(), LevelUp);
    }
    private void LevelUp(int level)
    {
        if (skillInfoMgr == null)
            return;
        SetSkill(skillInfoMgr);
    }
    public void SetSkillType(SkillDamageType skillType)
    {
        this.skillType = skillType;
        List<SkillInfoMgr> skills = PlayerMgr.Instance.GetSkills(skillType);
        if (skills != null && skills.Count > 0)
            SetSkill(skills[0]);
    }

    public void SetSkill(SkillInfoMgr skillInfoMgr)
    {
        this.skillInfoMgr = skillInfoMgr;
        if (skillInfoMgr == null || skillInfoMgr.skill == null)
        {
            this.skillNameText.text = "";
            this.skillLevelText.text = "1";
            this.skillInfoText.text = "";
            this.skillImage.color = Color.clear;
            this.levelObj.gameObject.SetActive(false);

            this.needSkillPointText.gameObject.SetActive(false);
            this.currentSkillPointText.gameObject.SetActive(false);
            this.triggerButton.gameObject.SetActive(false);
            this.tipText.gameObject.SetActive(false);
            return;
        }
        Skill skill = skillInfoMgr.skill;
        SkillController skillController = PlayerMgr.Instance.GetSkillController();

        this.skillNameText.text = skill.SkillName;
        this.skillLevelText.text = "1";//暂时不管技能等级
        this.skillInfoText.text = skill.GetSkillInfoText();
        this.skillImage.sprite = skill.SkillSprite;
        this.skillImage.color = Color.white;

        this.levelObj.gameObject.SetActive(true);
        this.needSkillPointText.gameObject.SetActive(true);
        this.currentSkillPointText.gameObject.SetActive(true);
        this.needSkillPointText.text = "需求技能点：" + skill.LearnPoint.ToString();
        this.currentSkillPointText.text = "技能点：" + skillController.CurrentSkillPoint.ToString();

        if (skillController.IsLearn(skillInfoMgr))
        {
            ShowTip();
            this.tipText.text = ToolsMgr.GetColorText("已学习", ToolsMgr.bleack);
        }
        else
        {
            if (skillController.GetLearnIndex(skill.skillDamageType) > skillInfoMgr.learnIndex)
            {
                ShowTip();
                this.tipText.text = ToolsMgr.GetColorText("不可学习", ToolsMgr.red);
            }
            else if (skillController.GetLearnIndex(skill.skillDamageType) == skillInfoMgr.learnIndex)
            {
                ShowButton();
            }
            else
            {
                ShowTip();
                this.tipText.text = ToolsMgr.GetColorText("请先学习之前的技能", ToolsMgr.red);
            }
        }
    }

    private void ShowButton()
    {
        this.triggerButton.gameObject.SetActive(true);
        this.tipText.gameObject.SetActive(false);
    }
    private void ShowTip()
    {
        this.triggerButton.gameObject.SetActive(false);
        this.tipText.gameObject.SetActive(true);
    }
}
