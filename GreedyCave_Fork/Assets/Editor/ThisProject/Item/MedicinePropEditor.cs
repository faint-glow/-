using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CanEditMultipleObjects]
[CustomEditor(typeof(MedicinePropInfo))]
public class MedicinePropEditor : Editor
{
    //包含有继承自适应高度的类的属性在展开时会让窗口一闪，所有属性都不显示，但是如果像这样，有一个看起来什么都没实现的编辑脚本，就没事了
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
