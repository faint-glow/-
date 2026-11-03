using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallMapPanel_Follow : MapPanel
{
    public static string prefabsPath = "SmallMapPanel_Follow";//Ö÷Òª±³°ü
    private RawImage mapImage;
    private Button openButton;
    public override void ShowMeFirst()
    {
        openButton = GetPanel("Button").GetUI<Button>();
        openButton.onClick.AddListener(() => 
        {
            MapInfo mapInfo = GameManager.Instance.GetMapInfo();
            if (mapInfo == null)
                return;
            SmallMapMgr.GetInstance().ShowGlobalSmallMap(mapInfo.Wideth,mapInfo.Height);
        });
        mapImage = GetPanel("Button/Mask/MapTexture").GetUI<RawImage>();
    }


    public void SetTexture(Texture texture)
    {
        mapImage.texture = texture;
    }
}
