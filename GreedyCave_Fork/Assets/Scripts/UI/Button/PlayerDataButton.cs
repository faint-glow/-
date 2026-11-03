using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDataButton : MapPanel, ISetAble<IGetAble>
{
    public static string prefabsPath = "Button/PlayerDataButton";

    private static Sprite createDataSprite;
    private static Sprite CreateDataSprite
    {
        get
        {
            if (createDataSprite == null)
                createDataSprite = ResMgr.GetInstance().Load<Sprite>("Sprite/UI/CreatePlayerData");
            return createDataSprite;
        }
    }

    public MapPanel FatherPanel { get; set; }
    public int Index { get; set; }

    private TMP_Text nameText;
    private Button selectButton;
    private Image edgIng;
    private RawImage characterImage;

    [System.NonSerialized] private PlayerData playerData;
    public override void ShowMeFirst()
    {
        nameText = GetPanel("NameText").GetUI<TMP_Text>();
        characterImage = GetPanel("CharacterImage").GetUI<RawImage>();
        edgIng = GetPanel("Edging").GetUI<Image>();
        selectButton = ThisPanel.GetUI<Button>();

        selectButton.onClick.AddListener(() =>
        {
            SelectDataPanel father = FatherPanel as SelectDataPanel;
            if (father == null)
                return;
            father.SetCurrentPlayerDataButton(this);
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
        });
    }

    public void Select()
    {
        edgIng.color = Color.white;
    }
    public void CancelSelect()
    {
        edgIng.color = Color.clear;
    }
    public void SetItem_Container(IGetAble obj)
    {
        PlayerData playerData = obj as PlayerData;
        if (playerData == null)
        {
            this.playerData = null;
            this.nameText.text = "´´½¨½ÇÉ«";
            this.characterImage.texture = CreateDataSprite.texture;
            this.characterImage.transform.localScale = new Vector3(0.7f, 0.8f, 0);
            return;
        }
        this.playerData = playerData;
        GameObject playerObj = ResMgr.GetInstance().Load<GameObject>(PlayerData.GetPlayerPrefabsPath(playerData.gender), false);
        this.characterImage.texture = AvatarImage.GetStaticTex(playerObj, PlayerClothesController.BodyPhotographOffset);
        this.characterImage.transform.localScale = new Vector3(0.8f, 0.8f, 0);
        this.nameText.text = "Lv." + playerData.level + " " + playerData.playerName;
    }
    public PlayerData GetSelectPlayerData()
    {
        return playerData;
    }
}
