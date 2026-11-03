using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyDictionary/Map/JarInfo", fileName = "NewJar")]
public class JarInfo : ScriptableObject
{
    public Sprite sprite;
    public GameObject obj;
    public Sprite ashesSprite;
    public List<ItemInfoAndRatio> itemInfoAndRatioList = new List<ItemInfoAndRatio>();
}

