using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : NPC
{
    private int forgetSkillCoin = 1000;
    protected override void InitialBaseChoice_Code()
    {
        Choice choice = new Choice();
        choice.SetAction("遗忘技能", ShowRequest_ForgetSkill);
        AddBaseChoice_Code(choice);

        Choice choice_1 = new Choice();
        choice_1.SetAction("银行", ShowBank);
        AddBaseChoice_Code(choice_1);
    }

    public void ShowBank()
    {
        UIManager.GetInstance().ShowPanelAsync<BankPanel>(BankPanel.prefabsPath, PanelLayer.Top,null,(panel)=> 
        {
            panel.SetBankName(name_NPC);
        });
    }

    public void ShowRequest_ForgetSkill()
    {
        if(PlayerMgr.Instance.GetCoin() < forgetSkillCoin)
        {
            UIManager.GetInstance().ShowTips("你的金币少于"+ forgetSkillCoin+"无法遗忘技能");
            return;
        }
        UIManager.GetInstance().ShowPanelAsync<RequestPanel>(RequestPanel.prefabsPath, PanelLayer.Max,null,(panel)=> 
        {
            panel.SetRequestInfo("你将花费"+forgetSkillCoin+"金币遗忘所有技能，你确定要这么做吗", Refuse_ForgetSkill, Agree_ForgetSkill);
        });
    }
    private void Refuse_ForgetSkill()
    {
        UIManager.GetInstance().HidePanel(RequestPanel.prefabsPath);
    }
    private void Agree_ForgetSkill()
    {
        PlayerMgr.Instance.ForgetAllSkill();
        UIManager.GetInstance().HidePanel(RequestPanel.prefabsPath);
        UIManager.GetInstance().ShowTips("已遗忘所有技能");
    }
}
