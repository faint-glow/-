using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MapPanel, ISetAble<IGetAble>
{
    public static string prefabsPath = "Button/CharacterButton";
    public MapPanel FatherPanel { get; set; }
    public int Index { get; set; }
    private Button selectButton;
    private Image edgIng;
    private RawImage characterImage;

    private SelectCharacterInfo selectCharacterInfo;
    public override void ShowMeFirst()
    {
        characterImage = GetPanel("CharacterImage").GetUI<RawImage>();
        edgIng = GetPanel("Edging").GetUI<Image>();
        selectButton = ThisPanel.GetUI<Button>();

        selectButton.onClick.AddListener(()=> 
        {
            SelectCharacterPanel father = FatherPanel as SelectCharacterPanel;
            if (father == null)
                return;
            father.SetCurrentCharacterButton(this);
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
        SelectCharacterInfo selectCharacterInfo = obj as SelectCharacterInfo;
        if (selectCharacterInfo == null)
            return;
        this.selectCharacterInfo = selectCharacterInfo;
        this.characterImage.texture = selectCharacterInfo.GetStaticAvatar();
    }
    public SelectCharacterInfo GetSelectCharacterInfo()
    {
        return selectCharacterInfo;
    }
}
