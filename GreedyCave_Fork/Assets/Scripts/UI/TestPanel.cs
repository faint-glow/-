using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPanel : BasePanel
{
    public static string prefabsPath = "TestPanel";
    Button testButton;

    public override void ShowMeFirst()
    {
        testButton = GetPanel("TestButton").GetUI<Button>();
        testButton.onClick.RemoveAllListeners();
        testButton.onClick.AddListener(() =>
        {
            //Item item = InputMgr.GetInstance().Library.GetRandomItem();
            //PlayerMgr.Instance.AddItem(item);
        });
    }
}
