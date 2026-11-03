using System.Collections;
using System.Collections.Generic;

public class MsgGetPlayerData : MsgBase
{
    public List<string> targetIDs;//客户端需求
    public List<PlayerData> playerDatas;//服务端填充
    public int result = 0;
}

public enum RequestType
{
    Wait,//还未回答
    Accept,//同意
    Refuse,//拒绝
}
public class MsgRequest : MsgBase
{
    public string msgID;//区分不同种类的请求
    public int msgNum;//区分同一类请求中的不同请求
    public string content;//客户端填充请求内容

    public string requestID;//客户端填充请求者

    public string targetID;//返回本次请求的客户端的ID
    public int responseCount;//已经响应请求的个数
    public Dictionary<string, RequestType> targetResults;//请求者客户端填充

    public long existTime = 10;
    public long bornTime;
    public bool isEndRequest;

    public string GetMsgID()
    {
        return requestID + ":" + msgID + ":" + msgNum;
    }
}

public class MsgSyneMaze : MsgBase
{
    public string id;//服务器填充是谁的地图
    public string mapInfoName;
    public int layerCount;
    public MazeGrid[,] mazsGrids;
}

public class MsgSendNextPos:MsgBase
{
    public string id;//服务器填充是谁在移动
    public float x;//目标位置的x
    public float y;//目标位置的y
    public float z;//目标位置的z
}

public class MsgStartBattle : MsgBase
{
    public string behaviourID;//开始战斗的玩家，此处由客户端主机填充
    public int x;//目标敌人所在格子的x坐标
    public int y;//目标敌人所在格子的y坐标
}

public class MsgNormalAttack : MsgBase
{
    public string behaviourID;//开始战斗的玩家，此处由客户端主机填充
}

public class MsgSkill : MsgBase
{
    public string behaviourID;
    public string skillInfoName;
}

public class MsgTip : MsgBase
{
    public string targetID;//目标发送ID;
    public string content;//提示内容
}
