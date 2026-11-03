using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneNamePanel : MapPanel
{
    public static string prefabsPath = "SceneNamePanel";
    private TMP_Text sceneNameText;
    public override void ShowMeFirst()
    {
        sceneNameText = GetPanel("Back/Text (TMP)").GetUI<TMP_Text>();

        EventCenter.GetInstance().AddEventListener<Scene>(EventCenterEnum.ÇÐ»»³¡¾°ºó.ToString(), (Scene scene) => 
        {
            string text = "";
            if (scene.name == SceneName.MazeScene)
            {
                MapInfo mapInfo = GameManager.Instance.GetMapInfo();
                if (mapInfo != null)
                    text = mapInfo.MapName + "£º" + mapInfo.LayerCount;
            }
            else
            {
                if(SceneController.Instance != null)
                    text = SceneController.Instance.GetSceneName();
            }

            sceneNameText.text = text;
        });
    }
}
