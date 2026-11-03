using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class RoomInfo : ICollectAble<RoomInfo>,IGetAble
{
    public int roomID;
    public string roomName;
    public string homeownerID;
    public int playerMaxCount;
    public List<string> players;

    private int count = 1;

    public bool AddPlayer(string id)
    {
        if (players.Count >= playerMaxCount)
            return false;

        players.Add(id);
        return true;
    }
    public bool RemovePlayer(string id)
    {
        players.Remove(id);

        if (players.Count <= 0)
            return true;

        if (homeownerID == id)
            homeownerID = players[0];

        return false;
    }

    public bool IsContainsPlayer(string ID)
    {
        return players.Contains(ID);
    }

    public int GetCount()
    {
        return count;
    }

    public string GetName()
    {
        return roomName;
    }

    public bool IsCanCombine()
    {
        return false;
    }
    public bool IsSame(RoomInfo item)
    {
        return false;
    }
    public RoomInfo DecreaseRules(int count)
    {
        this.count = 0;
        return new RoomInfo();
    }
    public RoomInfo StackRules(RoomInfo item,int countUpLimit)
    {
        return null;
    }

    public List<RoomInfo> SplitRules(int countUpperLimit)
    {
        return new List<RoomInfo>() { this};
    }

    public void ReverseSpltRules(RoomInfo item)
    {
        
    }

    HashSet<string> ICollectAble<RoomInfo>.GetCategory()
    {
        return null;
    }

    public void UseGrid()
    {
        throw new NotImplementedException();
    }

    public int Compare(RoomInfo item)
    {
        return 0;
    }
}