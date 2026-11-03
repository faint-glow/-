using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollText : MonoBehaviour
{
    private RectTransform child;
    private RectTransform parent;

    private Vector3[] childCorners = new Vector3[4];
    private Vector3[] parentCorners = new Vector3[4];

    private Vector3 childPosOriginal;
    private Vector3 childPos;

    private bool isStart;

    private const int right = 2;
    private const int left = 0;

    private void Awake()
    {
        child = gameObject.GetComponent<RectTransform>();
        parent = gameObject.transform.parent.gameObject.GetComponent<RectTransform>();
        childPosOriginal = child.position;
    }

    //每次关闭文字时，都会终止协程，因此需要在OnEnable()执行isStart = false;
    private void OnEnable()
    {
        IsChildEdgeOverParentEdge(left);
        childCorners[left].x = parentCorners[left].x;

        if (child.rect.width < parent.rect.width)
            return;
        StartCoroutine(RollTextCoroutine());
    }

    IEnumerator RollTextCoroutine()
    {
        while (true)
        {
            yield return new WaitWhile(() =>
            {
                //child.GetWorldCorners(childCorners);//左下、左上、右上、右下
                //parent.GetWorldCorners(parentCorners);//左下、左上、右上、右下
                childPos.Set(child.position.x - Time.deltaTime * 20, child.position.y, 0);
                child.position = childPos;
                return IsChildEdgeOverParentEdge(right);
            });

            yield return new WaitForSeconds(1f);

            yield return new WaitWhile(() =>
            {
                //child.GetWorldCorners(childCorners);//左下、左上、右上、右下
                //parent.GetWorldCorners(parentCorners);//左下、左上、右上、右下
                childPos.Set(child.position.x + Time.deltaTime * 20, child.position.y, 0);
                child.position = childPos;
                return !IsChildEdgeOverParentEdge(left);
            });
            yield return new WaitForSeconds(1f);
        }
    }

    private bool IsChildEdgeOverParentEdge(int edge)
    {
        child.GetWorldCorners(childCorners);//左下、左上、右上、右下
        parent.GetWorldCorners(parentCorners);//左下、左上、右上、右下

        return childCorners[edge].x > parentCorners[edge].x;
    }
}
