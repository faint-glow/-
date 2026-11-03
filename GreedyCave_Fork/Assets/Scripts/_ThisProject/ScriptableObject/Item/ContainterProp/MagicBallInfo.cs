using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyDictionary/Item/MagicBall", fileName = "NewMagicBall")]
public class MagicBallInfo : ContainerPropInfo
{
    public override Item GetItem(ItemQuality itemQuality, int level = 1)
    {
        return new MagicBall(this, itemQuality, 1);
    }
}
