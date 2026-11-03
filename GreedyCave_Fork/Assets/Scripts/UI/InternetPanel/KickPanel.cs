using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KickPanel : MapPanel
{
    public static string prefabsPath = "InternetPanel/KickPanel";

    public override void ShowMeFirst()
    {
        Button quitButton = GetPanel("Main/Button").GetUI<Button>();
        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
