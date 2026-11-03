using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

public class CreateIdToNameDic : MonoBehaviour
{
    private static string dicName = "IdToNameDic";

    [MenuItem("MenuCommand/CreateIdToNameDic", false)]
    public static void Create()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log(path); //从Assets开始，获得选中的文件的路径，注意这里是/
        path = Path.GetDirectoryName(path);
        Debug.Log(path);//从Assets开始，获得选中的文件的文件夹的路径，注意这里是\

        if (Directory.Exists(path))
        {
            int nameIndex = 0; //循环内部有meta的干扰文件，印象排序的id，需要自己设置一个新的增加变量
            List<FileInfo> files = GetAllFiles(path);
            StringBuilder sb = new StringBuilder();
            sb.Append($"    public static Dictionary<int, string> {dicName} = new Dictionary<int, string>() {{ \n");
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                    continue;
                //Debug.Log("文件名:" + files[i].Name);
                //Debug.Log("文件绝对路径:" + files[i].FullName);
                //Debug.Log("文件所在目录:" + files[i].DirectoryName);
                //string oldName = files[i].FullName;    //获取文件全路径
                //string extension = files[i].Extension; // 获取后缀名
                //string newName = files[i].DirectoryName + $@"\{nameIndex}{extension}";
                string fileName = files[i].Name;
                fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                string elementStr = $"      {{{nameIndex},\"{fileName}\"}},\n";
                sb.Append(elementStr);
                nameIndex++;
            }
            sb.Append(" };");

            string filePath = (Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "\\" + path).Replace("/", "\\") + $"\\_{dicName}.txt";
            Debug.Log(filePath);
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                string content = sb.ToString();//"这是要写入文件的内容。";
                // 将字符串内容转换为字节数组
                byte[] buffer = Encoding.UTF8.GetBytes(content);
                // 写入数据到文件
                fs.Write(buffer, 0, buffer.Length);
            }
            AssetDatabase.Refresh();
        }
    }
    public static List<FileInfo> GetAllFiles(string folderPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
        FileInfo[] fileInfos = directoryInfo.GetFiles();
        List<FileInfo> files = new List<FileInfo>();

        //返回值大于0表示x1>x2,等于0表示x1=x2,小于0表示x1<x2。Array.Sort内部会根据这个返回值来判断x1和x2的大小关系，并把小的元素放在前面
        System.Array.Sort(fileInfos, (x1, x2) => {
            Match m1 = Regex.Match(x1.Name, @"\d+");
            Match m2 = Regex.Match(x2.Name, @"\d+");
            if (m1 != null && m2 != null && m1.Value != null && m2.Value != null && m1.Value != "" && m2.Value != "") //当名字中不含数字。m1,m2为"",无法转整形，就会报错，处理一下
                return int.Parse(m1.Value).CompareTo(int.Parse(m2.Value));
            return x1.Name.CompareTo(x2.Name);
        });
        foreach (FileInfo file in fileInfos)
        {
            //if (file.Name.EndsWith(".meta")) //如果后缀名是.meta就不加入，当然也可以自己在具体实现的时候自己加上
            //    continue;
            files.Add(file);
        }
        return files;
    }
    [MenuItem("MenuCommand/CreateIdToNameDic", true)]//参数为true是，这个方发的返回值决定了和这个方法的参数一相同且参数二为false的方法，是否可以点击
    static private bool IsSelectPath()
    {
        return (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(Selection.activeObject)));//如果选中的物体的路径为空，则不显示按钮（返回false）
    }
}
