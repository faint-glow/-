using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskPanel : ContainerPanel<TaskButton,Task>
{
    public static string prefabsPath = "TaskPanel";

    private Button openButton;
    private Button closeButton;
    private RectTransform rectTrans;
    private Transform taskButtonFather;
    private VerticalLayoutGroup taskButtonFatherGroup;
    private int top = 4;
    public override string GetCellPath()
    {
        return TaskButton.prefabsPath;
    }

    public override Transform GetContainterTrans()
    {
        return taskButtonFather;
    }

    public override ScrollRect GetScrollRect()
    {
        return null;
    }

    public override IHolderAble GetHolder()
    {
        return null;
    }
    public override void ShowMeFirst()
    {
        openButton = GetPanel("Scroll View").GetUI<Button>();
        closeButton = GetPanel("CloseButton").GetUI<Button>();
        rectTrans = GetPanel("Scroll View").transform.GetComponent<RectTransform>();
        taskButtonFather = GetPanel("Scroll View/Viewport/Content").transform;
        taskButtonFatherGroup = taskButtonFather.GetComponent<VerticalLayoutGroup>();
        openButton.onClick.AddListener(()=> 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.open);
            //sizeDelta在Anchors不重合时，无法正确设置高宽。但是这样可以解决
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 600);

            closeButton.gameObject.SetActive(true);
            ForeachChild((TaskButton taskButton) => { taskButton.IsMask(closeButton.gameObject.activeInHierarchy); });
        });
        closeButton.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.close);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 110);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);

            closeButton.gameObject.SetActive(false);
            ForeachChild((TaskButton taskButton) => { taskButton.IsMask(closeButton.gameObject.activeInHierarchy); });
        });
    }

    public void Refresh()
    {
        RefreshContainer(TaskMgr.GetInstance().GetProcessTask(),null,(TaskButton taskButton)=> 
        {
            taskButton.IsMask(closeButton.gameObject.activeInHierarchy);
        });
        
        taskButtonFatherGroup.padding.top = top;//字体更换后的自适应不太刷新大概会晚一个刷新帧，这样设置，可以让他在刷新一次
        top += 1;
        if (top >= 6)
            top = 4;
    }

}
