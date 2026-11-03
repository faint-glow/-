using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightPanel : BasePanel
{
    public static string prefabsPath = "FightPanel";

    private Image enemyAvataImage;
    private TMP_Text enemyNameText;

    private Image enemyHpImage;
    private TMP_Text enemyHpText;

    private Image runAwayBack;
    private Image runAwayProgress;
    private Button runAwayButton;

    private Enemy enemy;
    private Transform enemyStateTrans;
    public override void ShowMeFirst()
    {
        enemyAvataImage = GetPanel("EnemyStatePanel/AvataImage").GetUI<Image>();
        enemyNameText = GetPanel("EnemyStatePanel/AvataImage/NameText (TMP)").GetUI<TMP_Text>();
        enemyStateTrans = GetPanel("EnemyStatePanel").transform;
        enemyHpImage = GetPanel("EnemyStatePanel/AvataImage/Hp/Image").GetUI<Image>();
        enemyHpText = GetPanel("EnemyStatePanel/AvataImage/Hp/Text (TMP) (1)").GetUI<TMP_Text>();

        runAwayBack = GetPanel("RunAway/Back").GetUI<Image>();
        runAwayProgress = GetPanel("RunAway/Back/Image").GetUI<Image>();
        runAwayButton = GetPanel("RunAway/Button").GetUI<Button>();

        UIManager.InitialPropertyImage(enemyHpImage);
        UIManager.InitialPropertyImage(runAwayProgress);

        runAwayButton.onClick.AddListener(()=>
        {
            runAwayBack.gameObject.SetActive(true);
            PlayerMgr.Instance.StartRun();
        });
    }
    public override void ShowMeEveryTime()
    {
        runAwayBack.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (runAwayBack.gameObject.activeInHierarchy == true)
            runAwayProgress.fillAmount = PlayerMgr.Instance.GetRunProcess();
    }
    public void SetEnemy(Enemy enemy)
    {
        if(enemy == null)
        {
            Debug.Log("Enemy为null");
            return;
        }
        this.enemyAvataImage.preserveAspect = true;
        if(enemy.AvataImage != null)
            this.enemyAvataImage.sprite = enemy.AvataImage;
        this.enemyNameText.text = enemy.GetName() + " Lv." + enemy.GetLevel();
        this.enemy = enemy;
        RefreshPanel();

        ShowPanelAsync<BuffPanel>(BuffPanel.prefabsPath, enemyStateTrans, null, (buffPanel) => 
        {
            buffPanel.SetWarrior(enemy);
            RectTransform rectTrans = buffPanel.transform as RectTransform;
            if (rectTrans == null)
                return;
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 220);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
            rectTrans.anchoredPosition = new Vector2(165, -70);
        });
    }

    public void RefreshPanel()
    {
        UIManager.SetPropertyPanel(enemy, PropertyType.生命值上限, PropertyType.生命值, enemyHpImage, enemyHpText);
    }
}
