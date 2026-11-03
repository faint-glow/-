using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatePanelMgr : PoolPanel
{
    public static string prefabsPath = "PlayerStatePanelMgr";
    Dictionary<string, PlayerStatePanel> panelDic = new Dictionary<string, PlayerStatePanel>();

    public void ShowPlayerState(IPlayerAble playerMgr)
    {
        string id = "";
        if (playerMgr.GetPlayerData() != null)
            id = playerMgr.GetPlayerData().ID;
        else
            id = PlayerData.DefaultID;

        if (panelDic.ContainsKey(id))
        {
            panelDic[id].SetPlayer(playerMgr);
        }
        else
        {
            ShowPoorPanelAsync<PlayerStatePanel>(PlayerStatePanel.prefabsPath, transform, null, (panel) =>
            {
                panel.SetPlayer(playerMgr);
                panelDic[id] = panel;
            });
        }
    }

    public void RemovePlayerState(IPlayerAble playerMgr)
    {
        string id = playerMgr.GetPlayerData().ID;
        if (panelDic.ContainsKey(id))
        {
            DestoryPanel(PlayerStatePanel.prefabsPath, panelDic[id]);
            panelDic.Remove(id);
        }
    }

    public void ShowAllPlayerStates(Dictionary<string, IPlayerAble> players)
    {
        foreach (var item in players)
        {
            ShowPlayerState(item.Value);
        }
    }

    public void RemoveAllPlayerStates()
    {
        lock (panelDic)
        {
            foreach (var item in panelDic)
            {
                DestoryPanel(PlayerStatePanel.prefabsPath, item.Value);
            }
            panelDic.Clear();
        }

    }
    /// <summary>
    /// 刷新状态，主要是生命值/魔法值等
    /// </summary>
    /// <param name="player"></param>
    public void RefreshPanel(IPlayerAble player)
    {
        string id = player.GetPlayerData().ID;
        if(panelDic.ContainsKey(id) && panelDic[id] != null)
            panelDic[id].RefreshPanel();
    }

}
