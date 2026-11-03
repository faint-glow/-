using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RemoveSelectElement : MapPanel
{
    public static string prefabsPath = "RemoveSelectElement";
    [SerializeField] private RemoveSelectType removeSelectType;
    private RemoveSelectPanel removeSelectPanel;
    private Button clickButton;
    private Image selectImage;
    private int index;
    public void SetRemoveSelectPanel(RemoveSelectPanel removeSelectPanel)
    {
        this.removeSelectPanel = removeSelectPanel;
    }
    public override void ShowMeFirst()
    {
        index = transform.GetSiblingIndex();
        clickButton = GetPanel("Button").GetUI<Button>();
        selectImage = GetPanel("Button/Image").GetUI<Image>();
        removeSelectPanel = (RemoveSelectPanel)UIManager.GetInstance().GetExistPanel(RemoveSelectPanel.prefabsPath);
        selectImage.gameObject.SetActive(removeSelectPanel.GetSelectInfo(removeSelectType,index));
        clickButton.onClick.AddListener(() =>
        {
            removeSelectPanel.ChangeSelectInfo(removeSelectType,index);
            selectImage.gameObject.SetActive(removeSelectPanel.GetSelectInfo(removeSelectType,index));
        });
    }
}
