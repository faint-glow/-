using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MsgRegister : MsgBase
{
    public string id = "";//客户端填充
    public string pw = "";//客户端填充

    public int result = 0;
}
public class MsgLogin : MsgBase
{
    public string id = "";//客户端填充
    public string pw = "";//客户端填充

    public PlayerData playerData;//服务器填充
    public int result = 0;
}

public class MsgKick : MsgBase
{
    public int reason = 0;
}
