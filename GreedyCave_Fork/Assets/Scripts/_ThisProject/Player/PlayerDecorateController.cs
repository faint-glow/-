using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDecorateController
{
    public PlayerDecorateController()
    {
        EventCenter.GetInstance().AddEventListener<Scene>(EventCenterEnum.切换场景后.ToString(), (scene) =>
        {
            ShowDecorate(SceneMgr.GetInstance().GetCurrentScene().name);
        });
    }
    public void ClearRunTimeData()
    {
        if(playerDecorateDic != null)
            playerDecorateDic.Clear();
    }
    //场景名字 对应 位置对应装饰物
    private Dictionary<string, Dictionary<Vector3, IDecorateAble>> playerDecorateDic = new Dictionary<string, Dictionary<Vector3, IDecorateAble>>();

    public Dictionary<string, Dictionary<Vector3, IDecorateAble>> GetPlayerDecorateDic()
    {
        return playerDecorateDic;
    }
    public void LoadDecorate(PlayerData playerData)
    {
        Dictionary<string, Dictionary<string, ItemSaveData>> playerDecorateDic = playerData.playerDecorateDic;
        if (playerDecorateDic == null)
        {
            return;
        }
        foreach (var item in playerDecorateDic)
        {
            string sceneName = item.Key;
            foreach (var itemElement in item.Value)
            {
                string myVector3 = itemElement.Key;
                ItemSaveData itemSaveData = itemElement.Value;
                Vector3 targetPos = PlayerDataMgr.StringToVector3(myVector3);
                if (itemSaveData == null)
                {
                    Debug.Log("加载数据为空");
                    continue;
                }
                IDecorateAble decorateAble = itemSaveData.GetItem() as IDecorateAble;
                if (decorateAble == null)
                {
                    Debug.Log("转化失败");
                    continue;
                }
                if (!this.playerDecorateDic.ContainsKey(sceneName))
                    this.playerDecorateDic[sceneName] = new Dictionary<Vector3, IDecorateAble>();
                this.playerDecorateDic[sceneName][targetPos] = decorateAble;
            }
        }
    }

    public void ShowDecorate(string sceneName)
    {
        if (!playerDecorateDic.ContainsKey(sceneName))
            return;
        foreach (KeyValuePair<Vector3, IDecorateAble> item in playerDecorateDic[sceneName])
        {
            DecorateMgr.GetInstance().ShowDecorate(item.Key, item.Value);
        }
    }

    public void AddDecorate(Vector3 pos, IDecorateAble decorateAble, string sceneName = "")
    {
        if (sceneName == "")
            sceneName = SceneMgr.GetInstance().GetCurrentScene().name;

        if (!playerDecorateDic.ContainsKey(sceneName))
            playerDecorateDic[sceneName] = new Dictionary<Vector3, IDecorateAble>();

        playerDecorateDic[sceneName][pos] = decorateAble;
    }

    public IDecorateAble RemoveDecorate(Vector3 pos, string sceneName = "")
    {
        if (sceneName == "")
            sceneName = SceneMgr.GetInstance().GetCurrentScene().name;

        //Debug.Log(playerDecorateDic[sceneName].ContainsKey(pos));
        //foreach (KeyValuePair<Vector3, IDecorateAble> item in playerDecorateDic[sceneName])
        //{
        //    Debug.Log(item.Key + ":" + item.Value);
        //}
        if (!playerDecorateDic.ContainsKey(sceneName))
            return null;
        if (!playerDecorateDic[sceneName].ContainsKey(pos))
            return null;

        IDecorateAble decorateAble = playerDecorateDic[sceneName][pos];
        playerDecorateDic[sceneName].Remove(pos);

        //foreach (KeyValuePair<Vector3, IDecorateAble> item in playerDecorateDic[sceneName])
        //{
        //    Debug.Log(item.Key + ":" + item.Value);
        //}
        return decorateAble;
    }
}
