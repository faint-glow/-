using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.Networking;
using System;

public class SaveByJson : MonoBehaviour
{
    #region json保存测试代码
    //if (Input.GetKeyUp(KeyCode.S))
    //{
    //    Debug.Log("Input.GetKeyUp(KeyCode.S)");
    //    MazeGrid grid = new MazeGrid(1, 1);
    //    string s = JsonUtility.ToJson(grid);
    //    //string s = JsonUtility.ToJson(wallGrid[26]);
    //    SaveByJson.Save(@"Save\Test1.txt", s);
    //}
    //if (Input.GetKeyUp(KeyCode.R))
    //{
    //    Debug.Log("Input.GetKeyUp(KeyCode.R)");
    //    string s = SaveByJson.Read(@"Save\Test1.txt");
    //    MazeGrid grid = (MazeGrid)JsonUtility.FromJson(s, typeof(MazeGrid));
    //    Tile newTile = ScriptableObject.CreateInstance<Tile>();
    //    newTile.sprite = grid.GetSprite();
    //    tileMap.SetTile(new Vector3Int(-1, -1, 0), newTile);
    //}
    #endregion

    #region 地图测试代码
    //读取
    //string s = SaveByJson.Read("Save/Test.txt");
    //Debug.Log(s.Length);
    //MsgSyneMaze msg = (MsgSyneMaze)JsonConvert.DeserializeObject(s, typeof(MsgSyneMaze));
    //OnMsgSyneMaze(msg);
    //int len = s.Length;
    //Debug.Log(len);
    //byte[] sendBytes = new byte[4 + len];
    //sendBytes[0] = (byte)(len % 256);
    //sendBytes[1] = (byte)(len / 256);
    //sendBytes[2] = (byte)(len / 65536);
    //sendBytes[3] = (byte)(len / 16777216);
    //Int32 bodyLength = (Int32)((sendBytes[3] << 24) | (sendBytes[2] << 16) | (sendBytes[1] << 8) | (sendBytes[0]));
    //Debug.Log(bodyLength);


    //保存
    //MsgSyneMaze msg = new MsgSyneMaze();
    //msg.mapInfoName = mapInfo.MapInfoName;
    //msg.layerCount = mapInfo.LayerCount;
    //msg.mazsGrids = randomMazeMgr.MapGrid;
    //NetManager.Send(msg);
    //Debug.Log("Send");
    //string s = JsonConvert.SerializeObject(msg);
    //SaveByJson.Save("Save/Test.txt", s);
    #endregion
    //public static string BasePath =
    //         Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    //             UnityEngine.Application.productName);

    //public static string BaseAndroidPath =
    //Path.Combine(Application.temporaryCachePath, UnityEngine.Application.productName);
    public static bool IsHaveFile(string path)
    {
        string allPath = "";
        if (Application.platform == RuntimePlatform.Android)
            allPath = Application.persistentDataPath + "/" + path;
        else
            allPath = Application.streamingAssetsPath + "/" + path;

        return File.Exists(allPath);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">路径从Application.streamingAssetsPath后开始</param>
    /// <param name="content"></param>
    public static void Save(string path, string content)
    {
        string allPath = "";

        if (Application.platform == RuntimePlatform.Android)
            allPath = Application.persistentDataPath + "/" + path;
        else
            allPath = Application.streamingAssetsPath + "/" + path;

        string directory = Path.GetDirectoryName(allPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        FileStream file = new FileStream(allPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        using (file)
        {
            file.SetLength(0);//清空之前的内容
            byte[] newBt = Encoding.UTF8.GetBytes(content);
            file.Write(newBt, 0, newBt.Length);
            file.Close();
        }
        //Debug.Log(allPath);
    }
    public static string Read(string path)
    {
        string allPath = "";
        string s = "";
        if (Application.platform == RuntimePlatform.Android)
            allPath = Application.persistentDataPath + "/" + path;
        else
            allPath = Application.streamingAssetsPath + "/" + path;

        string directory = Path.GetDirectoryName(allPath);
        if (!Directory.Exists(directory))
        {
            return "";
        }
        FileStream file = new FileStream(allPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
        using (file)
        {
            byte[] bytes = new byte[1024 * 1024 * 5];
            int num = file.Read(bytes, 0, bytes.Length);
            s = Encoding.UTF8.GetString(bytes, 0, num);

            file.Close();
        }
        return s;
    }

    public static void Delete(string path)
    {
        string allPath = "";
        if (Application.platform == RuntimePlatform.Android)
            allPath = Application.persistentDataPath + "/" + path;
        else
            allPath = Application.streamingAssetsPath + "/" + path;

        if (File.Exists(allPath))
            File.Delete(allPath);
    }
}
