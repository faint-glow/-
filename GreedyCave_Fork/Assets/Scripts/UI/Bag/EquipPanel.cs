using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipPanel : MonoBehaviour
{
    private List<EquipButton> equipButtonPanels;
    public List<EquipButton> EquipButtonPanels
    {
        get 
        { 
            if(equipButtonPanels == null)
            {
                equipButtonPanels = new List<EquipButton>(transform.childCount);

                for (int i = 0; i < transform.childCount; i++)
                {
                    EquipButton equipButton = transform.GetChild(i).GetComponent<EquipButton>();
                    equipButtonPanels.Add(equipButton);
                }
            }
            return equipButtonPanels;
        }
    }

    public void RefreshEquipPanel()
    {
        for (int i = 0; i < EquipButtonPanels.Count; i++)
        {
            EquipButtonPanels[i].RefreshEquipButton();
        }
    }
}
