using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farrier : NPC
{

    protected override void InitialBaseChoice_Code()
    {
        Choice choice = new Choice();
        choice.SetAction("Ç¿»¯", () => 
        { 
            UIManager.GetInstance().ShowPanelAsync<StrengthenPanel>(StrengthenPanel.prefabsPath, PanelLayer.Top);
        });

        AddBaseChoice_Code(choice);
    }
}
