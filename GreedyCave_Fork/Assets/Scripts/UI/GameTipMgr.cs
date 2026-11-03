using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTipMgr : BaseManager<GameTipMgr>
{
    private float baseIntervalTime = 0.5f;
    private float currentTime = 1f;//一开始大于baseIntervalTime，这样就
    private Queue<string> contents = new Queue<string>();
    public GameTipMgr()
    {
        MonoMgr.GetInstance().AddUpdateListener(Updata);
        EventCenter.GetInstance().AddEventListener<Scene>(EventCenterEnum.切换场景后.ToString(), (scene) => 
        {
            contents.Clear();
        });
    }

    public void AddTipsToQue(string content)
    {
        contents.Enqueue(content);
    }

    private void Updata()
    {
        currentTime += Time.deltaTime;
        if (contents.Count > 0 && currentTime >= baseIntervalTime)
        {
            currentTime = 0;
            UIManager.GetInstance().ShowTip_Instant(contents.Dequeue());
        }
    }
}
