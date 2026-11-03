using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

//JsonUtility不可以转化列表，但可以转化可序列化的一个类中的列表
//JsonUtility不可转化可序列的类的二维数组
public class MsgBase
{
    public MsgBase()
    {
        protoName = this.GetType().ToString();
    }
    public string protoName = "";
    ////编码
    //public static byte[] Encode(MsgBase msgBase)
    //{
    //    string s = JsonUtility.ToJson(msgBase);
    //    Debug.Log(s);
    //    return System.Text.Encoding.UTF8.GetBytes(s);
    //}
    ////解码
    //public static MsgBase Decode(string protoName,byte[] bytes,int offset,int count)
    //{
    //    string s = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
    //    MsgBase msgBase = (MsgBase)JsonUtility.FromJson(s,Type.GetType(protoName));

    //    return msgBase;
    //}

    //编码
    public static byte[] Encode(MsgBase msgBase)
    {
        string s = JsonConvert.SerializeObject(msgBase);
        return System.Text.Encoding.UTF8.GetBytes(s);
    }
    //解码
    public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
    {
        string s = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
        MsgBase msgBase = (MsgBase)JsonConvert.DeserializeObject(s, Type.GetType(protoName));
        return msgBase;
    }
    //编码协议名（2字节长度+字符串）
    public static byte[] EncodeName(MsgBase msgBase)
    {
        //名字bytes和长度
        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.protoName);
        Int16 len = (Int16)nameBytes.Length;
        //申请bytes数组
        byte[] bytes = new byte[2 + len];
        //组装两字节的长度信息
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        //组装名字bytes
        Array.Copy(nameBytes, 0, bytes, 2, len);

        return bytes;
    }
    //解码协议名
    public static string DecodeName(byte[] bytes,int offset,out int count)
    {
        count = 0;
        //bytes的长度必须大于两字节
        if (offset + 2 > bytes.Length)
            return "";

        Int16 len = (Int16)((bytes[offset + 1] << 8) | (bytes[offset]));
        if (len <= 0)
            return "";
        if (offset + 2 + len > bytes.Length)
            return "";
        //解析
        count = 2 + len;
        string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
        return name;
    }
}
