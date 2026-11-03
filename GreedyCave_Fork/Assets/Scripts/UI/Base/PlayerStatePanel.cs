using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatePanel : BasePanel
{
    public static string prefabsPath = "PlayerStatePanel";

    private IPlayerAble player;
    //private PlayerController playerController;
    private RawImage playerAvataImage;
    private Image hpImage;
    private TMP_Text hpText;
    private Image mpImage;
    private TMP_Text mpText;
    private Image expImage;
    private TMP_Text expText;
    private TMP_Text levelText;
    public override void ShowMeFirst()
    {
        playerAvataImage = GetPanel("Main/Avata/Mask/RawImage").GetUI<RawImage>();
        hpImage = GetPanel("Main/Property/Hp/Image").GetUI<Image>();
        hpText = GetPanel("Main/Property/Hp/Text (TMP)").GetUI<TMP_Text>();

        mpImage = GetPanel("Main/Property/Mp/Image").GetUI<Image>();
        UIManager.InitialPropertyImage(hpImage);

        mpText = GetPanel("Main/Property/Mp/Text (TMP)").GetUI<TMP_Text>();
        UIManager.InitialPropertyImage(mpImage);

        expImage = GetPanel("Main/Property/Exp/Image").GetUI<Image>();
        UIManager.InitialPropertyImage(expImage);
        expText = GetPanel("Main/Property/Exp/Text (TMP)").GetUI<TMP_Text>();

        levelText = GetPanel("Main/Level/Text (TMP)").GetUI<TMP_Text>();

        ShowPanelAsync<BuffPanel>(BuffPanel.prefabsPath, transform, null, (buffPanel) =>
        {
            buffPanel.SetWarrior(PlayerMgr.Instance.GetWarrior());
            RectTransform rectTrans = buffPanel.transform as RectTransform;

            if (rectTrans == null)
                return;
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 150);
            rectTrans.anchoredPosition = new Vector2(370, 0);
        });
        
    }

    public override void ShowMeEveryTime()
    {
        RectTransform rectTrans = transform.GetComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(400, 150);
    }
    public void SetPlayer(IPlayerAble player)
    {
        this.player = player;
        //this.playerController = playerController;
        playerAvataImage.texture = player.GetStaticAvatar();
        RefreshPanel();
    }

    public void RefreshPanel()
    {
        Warrior warrior = player.GetWarrior();
        playerAvataImage.texture = player.GetStaticAvatar();

        UIManager.SetPropertyPanel(warrior, PropertyType.生命值上限, PropertyType.生命值, hpImage, hpText, false);
        UIManager.SetPropertyPanel(warrior, PropertyType.魔法值上限, PropertyType.魔法值, mpImage, mpText, false);
        UIManager.SetPropertyPanel(warrior, PropertyType.经验值上限, PropertyType.经验值, expImage, expText, false);

        levelText.text = warrior.GetLevel().ToString();
    }

}
