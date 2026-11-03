using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : DragPanel, ISetAble<IGetAble>
{
    public static string prefabsPath = "Button/SkillButton";

    public MapPanel FatherPanel { get; set; }
    public int Index { get; set; }

    private int learnIndex;
    private SkillInfoMgr skillInfoMgr;
    private Button clickButton;
    private Image skillImage;
    private TMP_Text skillNameText;
    public override void ShowMeFirst()
    {
        learnIndex = transform.GetSiblingIndex() / 2;
        clickButton = ThisPanel.GetUI<Button>();
        clickButton.onClick.AddListener(() =>
        {
            Vector2 mousPos = Input.mousePosition;
            if ((mousPos - startPos).sqrMagnitude > 50)
                return;

            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            BagPanel bagPanel = (BagPanel)UIManager.GetInstance().GetExistPanel(BagPanel.prefabsPath);
            if (bagPanel == null)
                return;
            SkillInfoPanel skillInfoPanel = (SkillInfoPanel)bagPanel.GetExistPanel(SkillInfoPanel.prefabsPath);
            if (skillInfoPanel == null)
                return;
            skillInfoPanel.SetSkill(skillInfoMgr);
        });
        skillImage = GetPanel("Image (1)/Image").GetUI<Image>();
        skillNameText = GetPanel("Image/Text (TMP)").GetUI<TMP_Text>();
    }

    protected override bool IsCanDrag()
    {
        if (skillInfoMgr == null)
            return false;
        if (skillInfoMgr.skill == null)
            return false;
        if (skillInfoMgr.skill._skillType == SkillType.±»¶¯)
            return false;
        SkillController skillController = PlayerMgr.Instance.GetSkillController();
        return skillController.IsLearn(this.skillInfoMgr);
    }

    public override IDragAble GetItem_Drag()
    {
        return skillInfoMgr.skill;
    }

    public override ScrollRect GetScrollRect()
    {
        ContainerPanel<SkillButton, SkillInfoMgr> fatherPanel = FatherPanel as ContainerPanel<SkillButton, SkillInfoMgr>;
        return fatherPanel.GetScrollRect();
    }

    public void SetItem_Container(IGetAble obj)
    {
        SkillInfoMgr skillInfoMgr = obj as SkillInfoMgr;
        if (skillInfoMgr == null || skillInfoMgr.skill == null)
        {
            this.skillInfoMgr = null;
            this.skillNameText.text = "";
            this.skillImage.gameObject.SetActive(false);
            return;
        }
        else
        {
            Skill skill = skillInfoMgr.skill;

            this.skillInfoMgr = skillInfoMgr;
            this.skillNameText.text = skill.SkillName;
            this.skillImage.sprite = skill.SkillSprite;
            this.skillImage.gameObject.SetActive(true);

            SkillController skillController = PlayerMgr.Instance.GetSkillController();
            skillImage.material = skillController.IsLearn(skillInfoMgr) ? null: SkillController.GreyMaterial;
        }
    }

    public override void OperiateOverflowItem_Drag(IDragAble drageAble)
    {
        if (drageAble == null)
            return;
    }
}
