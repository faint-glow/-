using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbandonPanel : ExchangePanel
{
    public static string prefabsPath = "AbandonPanel";

    public override string GetConfirmTextByOther()
    {
        return "¶ªÆú";
    }
    public override string GetConfirmTextByPlayer()
    {
        return "ÊÕ»Ø";
    }
    public override string GetPrefabsPath()
    {
        return prefabsPath;
    }

}
