using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DepositPanel : ExchangePanel
{
    public static string prefabsPath = "DepositPanel";

    public override string GetConfirmTextByOther()
    {
        return "放入";
    }

    public override string GetConfirmTextByPlayer()
    {
        return "收回";
    }

    public override string GetPrefabsPath()
    {
        return prefabsPath;
    }
}
