using System.Collections;
using System.Collections.Generic;

public class MsgCreateRoom : MsgBase
{
    public RoomInfo roomInfo;//服务器填充
    public int result = 0;//0成功，1失败
}

public class MsgEnterRoom : MsgBase
{
    public RoomInfo roomInfo;//客户端填充ID，服务端填充内容
    public string enterPlayerID;//服务器填充
    public int result = 0;//0成功，1失败
    public string errInfo;//错误信息
}
public class MsgLeaveRoom : MsgBase
{
    public RoomInfo roomInfo;//服务器填充
    public string leavePlayerID;//服务器填充
    public int result = 0;//0成功，1失败
}

public class MsgGetRoomInfos : MsgBase
{
    public List<RoomInfo> roomInfos;//服务器填充
}
