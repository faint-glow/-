using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTest : MonoBehaviour
{
    public string testName = "aaa";
    [Preview]
    public Sprite sprite;
}

//PropertyAttribute：一般用于控制属性
//AttributesLearn : PropertyDrawer一般用于控制界面表现
public class Preview : PropertyAttribute
{

}

