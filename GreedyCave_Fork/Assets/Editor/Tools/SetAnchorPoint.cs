using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SetAnchorPoint : EditorWindow
{
    //这个文章及其牛逼
    //RectTransform知识:https://zhuanlan.zhihu.com/p/139252379

    public enum SetType
    {
        Point,
        Rectangle
    }

    private static SetType setType = SetType.Point;

    public static void ShowWindow()
    {
        //显示现有窗口实例。如果没有，请创建一个。
        EditorWindow.GetWindow(typeof(SetAnchorPoint));
    }
    void OnGUI()
    {
        EditorGUILayout.LabelField("如果点击的是Hierarchy的预制体则可以撤销，Assets的不可以撤销");

        //**********************自建window显示类变量****************************
        //EditorGUILayout.PropertyField(showObj, true);
        //serObj.ApplyModifiedProperties();//应用更改

        if (GUILayout.Button("Resolve"))
        {
            Set();
        }

    }
    [MenuItem("MenuCommand/SetAnchorPoint", false)]

    public static void Set()
    {
        GameObject childObj = Selection.activeGameObject;
        RectTransform childTransform = childObj.GetComponent<RectTransform>();
        Transform parent = childObj.transform.parent;
        RectTransform parentTransform = parent.GetComponent<RectTransform>();
        if (childTransform == null || parentTransform == null)
        {
            Debug.Log("所选物体不含RectTransform或父物体不含RectTransform");
            return;
        }
        //Debug.Log(cildTransform.localPosition);//子物体于父物体pivot的距离（应该是）

        //当Anchors分开的时候，Pivot在自身区域的比例，映射到Anchors上的点，再与Pivot的相对位置，就是anchoredPosition。
        //所以当：当Anchors重合的时候，Anchors与Pivot的相对位置就是anchoredPosition。
        //rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);

        //当Anchors不重合的时候，设置sizeDelta就不能正确控制RectTransform的大小
        //此时可以使用SetSizeWithCurrentAnchors(axis, size)来设置rect的width和height。

        //rectTransform.anchorMin = new Vector2();//x左，y下
        //rectTransform.anchorMax = new Vector2();//x右，y上

        Vector3[] childCorners = new Vector3[4];
        Vector3[] parentCorners = new Vector3[4];
                                                    //0     1     2     3

        if(setType == SetType.Rectangle)
        {
            childTransform.GetWorldCorners(childCorners);//左下、左上、右上、右下
        }
        else if(setType == SetType.Point)
        {
            childCorners[0] = childTransform.transform.position;
            childCorners[2] = childTransform.transform.position;
        }

        parentTransform.GetWorldCorners(parentCorners);//左下、左上、右上、右下

        //计算目标锚点
        Vector2 anchorMin = new Vector2();
        anchorMin.x = (childCorners[0].x - parentCorners[0].x) / (parentCorners[2].x - parentCorners[0].x);
        anchorMin.y = (childCorners[0].y - parentCorners[0].y) / (parentCorners[2].y - parentCorners[0].y);
        Vector2 anchorMax = new Vector2();
        anchorMax.x = (childCorners[2].x - parentCorners[0].x) / (parentCorners[2].x - parentCorners[0].x);
        anchorMax.y = (childCorners[2].y - parentCorners[0].y) / (parentCorners[2].y - parentCorners[0].y);

        //修改锚点会让矩形方框大小和位置改变，这里记录用于保持原样。
        //UI的那几个位置和宽高乱的扣(查看上方连接可解)，而且因为锚点的改变，那几个位置和宽高基本都会因此改变
        //这种方式可以完美解决
        float width = childTransform.rect.width;
        float height = childTransform.rect.height;
        Vector3 pos = childTransform.position;

        //修改锚点
        childTransform.anchorMin = anchorMin;//x左，y下
        childTransform.anchorMax = anchorMax;//x右，y上

        //恢复
        childTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);//水平方向的宽度
        childTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);//竖直方式的高度
        childTransform.transform.position = pos;
    }
}
