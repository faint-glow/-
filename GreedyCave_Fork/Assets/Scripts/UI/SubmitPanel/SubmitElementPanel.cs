using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitElementPanel : MapPanel, ISetAble<IGetAble>
{
    public MapPanel FatherPanel { get; set; }
    public int Index { get; set; }
    public void SetItem_Container(IGetAble obj)
    {
        throw new System.NotImplementedException();
    }
}
