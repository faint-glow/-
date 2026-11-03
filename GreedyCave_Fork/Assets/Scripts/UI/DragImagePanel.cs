using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragImagePanel : MapPanel
{
    public static string prefabsPath = "DragImagePanel";
    private IDragAble obj;
    public IDragAble Obj
    {
        get { return obj; }
    }
    private Image objImage;
    public Image ObjImage
    {
        get 
        {
            if (objImage == null)
            {
                objImage = GetPanel("Image").GetUI<Image>();
                objImage.preserveAspect = true;
            }
            return objImage; 
        }
    }

    public void SetObj(IDragAble obj)
    {
        if (obj == null)
            return;

        this.obj = obj;
        this.ObjImage.sprite = obj.GetSprite_Drag();
    }
    public IDragAble GetObj()
    {
        return obj;
    }
}
