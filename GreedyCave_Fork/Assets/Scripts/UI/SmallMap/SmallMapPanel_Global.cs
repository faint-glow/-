using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SmallMapPanel_Global : BasePanel
{
    public static string prefabsPath = "SmallMapPanel_Global";//主要背包
    private Button closeButton;
    private RawImage mapImage;
    private float currentRatio = 1;
    private Vector3 originalMapImageScale = new Vector3(1, 1, 1);

    private RectTransform father;
    private Vector3[] childCorners = new Vector3[4];
    private Vector3[] parentCorners = new Vector3[4];
    private Vector2 middlePos;
    private Vector2 targetPivot;


    private ScrollRect scroll;
    private bool lastIsScale;//安卓版控制需要
    public override void ShowMeFirst()
    {
        closeButton = GetPanel("CloseButton").GetUI<Button>();
        closeButton.onClick.AddListener(() =>
        {
            SmallMapMgr.GetInstance().ShowFolowSmallMap(PlayerMgr.Instance.GetPlayerObject());
        });
        mapImage = GetPanel("Scroll View/Viewport/Content").GetUI<RawImage>();
        father = GetPanel("Scroll View/Viewport").transform as RectTransform;


        scroll = GetPanel("Scroll View").GetUI<ScrollRect>();
    }

    public void SetTexture(Texture texture)
    {
        mapImage.texture = texture;
        if (Application.platform == RuntimePlatform.Android)
            InputMgr.GetInstance().AddMouseInput(InputType.SmallMapMouseInput, ImageScaleControll_Android);
        else
            InputMgr.GetInstance().AddMouseInput(InputType.SmallMapMouseInput, ImageScaleControll_PC);
    }
    public override void HideMe()
    {
        InputMgr.GetInstance().RemoveMouseInput(InputType.SmallMapMouseInput);
    }
    public void ImageScaleControll_PC()
    {
        float scrollDeltaY = Input.mouseScrollDelta.y;
        Vector3 mousePos = Input.mousePosition;
        currentRatio += scrollDeltaY * 0.1f * currentRatio;
        currentRatio = Mathf.Clamp(currentRatio, 1, 10);
        originalMapImageScale.Set(currentRatio, currentRatio, 1);
        if (scrollDeltaY != 0 && mousePos.x >= 0 && mousePos.x <= Screen.width && mousePos.y >= 0 && mousePos.y <= Screen.height)
            AdjustAnchorPoint(mapImage.rectTransform);
        mapImage.transform.localScale = Vector3.Lerp(mapImage.transform.localScale, originalMapImageScale, Time.deltaTime * 10);
    }
    public void ImageScaleControll_Android()
    {
        float scrollDeltaY = InputMgr.GetInstance().GetTouchScale() * 0.015f;
        bool currentIsScale = InputMgr.GetInstance().IsStartScale();
        if(currentIsScale == true)
            scroll.enabled = false;
        else if(lastIsScale == true)
            scroll.enabled = true;

        Vector3 mousePos = Input.mousePosition;
        currentRatio += scrollDeltaY * 0.1f * currentRatio;
        currentRatio = Mathf.Clamp(currentRatio, 1, 10);
        originalMapImageScale.Set(currentRatio, currentRatio, 1);

        if (scrollDeltaY != 0 && mousePos.x >= 0 && mousePos.x <= Screen.width && mousePos.y >= 0 && mousePos.y <= Screen.height)
            AdjustAnchorPoint(mapImage.rectTransform);
        mapImage.transform.localScale = Vector3.Lerp(mapImage.transform.localScale, originalMapImageScale, Time.deltaTime * 10);

        lastIsScale = currentIsScale;
    }
    private void AdjustAnchorPoint(RectTransform rectTransform)
    {
        if (father == null || rectTransform == null)
            return;
        AdjustAnchorPoint_Mouse(rectTransform);
        //AdjustAnchor_FatherCenter(father, mapImage.rectTransform, parentCorners, childCorners);
    }

    private void AdjustAnchor_FatherCenter(RectTransform father, RectTransform rectTransform, Vector3[] parentCorners, Vector3[] childCorners)
    {
        father.GetWorldCorners(parentCorners);
        rectTransform.GetWorldCorners(childCorners);
        Vector3 pivot = rectTransform.pivot;
        Vector3 sizeDelta = rectTransform.sizeDelta;
        Vector3 point = rectTransform.anchoredPosition;

        //左下，左上，右上，右下
        middlePos.Set((parentCorners[2].x - parentCorners[0].x) / 2 + parentCorners[0].x, (parentCorners[2].y - parentCorners[0].y) / 2 + parentCorners[0].y);//获取父物体的中间位置
        targetPivot.Set((middlePos.x - childCorners[0].x) / (childCorners[2].x - childCorners[0].x), (middlePos.y - childCorners[0].y) / (childCorners[2].y - childCorners[0].y));
        rectTransform.pivot = targetPivot;

        point.x += (targetPivot.x - pivot.x) * rectTransform.localScale.x * sizeDelta.x;
        point.y += (targetPivot.y - pivot.y) * rectTransform.localScale.y * sizeDelta.x;
        rectTransform.anchoredPosition = point;
    }
    private void AdjustAnchorPoint_Mouse(RectTransform rectTransform)
    {
        Vector3 point = rectTransform.anchoredPosition;
        Vector3 sizeDelta = rectTransform.sizeDelta;

        Vector3 pivot = rectTransform.pivot;
        Vector3 vector3 = rectTransform.InverseTransformPoint(Input.mousePosition);//鼠标位置和rectTransform的pivot之间的距离
        float x = ((vector3.x) + (pivot.x * sizeDelta.x)) / sizeDelta.x;
        float y = ((vector3.y) + (pivot.y * sizeDelta.y)) / sizeDelta.y;

        targetPivot.Set(x, y);
        rectTransform.pivot = targetPivot;

        point.x += (x - pivot.x) * rectTransform.localScale.x * sizeDelta.x;
        point.y += (y - pivot.y) * rectTransform.localScale.y * sizeDelta.x;

        rectTransform.anchoredPosition = point;
    }

}
