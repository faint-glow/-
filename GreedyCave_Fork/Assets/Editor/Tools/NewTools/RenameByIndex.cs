using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class RenameByIndex : Editor
{
    [MenuItem("MenuCommand/RameByIndex",false)]
    public static void Rename()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log(path); //从Assets开始，获得选中的文件的路径，注意这里是/
        path = Path.GetDirectoryName(path);
        Debug.Log(path);//从Assets开始，获得选中的文件的文件夹的路径，注意这里是\
        if (Directory.Exists(path))
        {
            int nameIndex = 0;
            List<FileInfo> files = GetAllFiles(path);
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                    continue;
                //Debug.Log("文件名:" + files[i].Name);
                //Debug.Log("文件绝对路径:" + files[i].FullName);
                //Debug.Log("文件所在目录:" + files[i].DirectoryName);
                string oldName = files[i].FullName;    //获取文件全路径
                string extension = files[i].Extension; // 获取后缀名
                string newName = files[i].DirectoryName + $@"\{nameIndex}{extension}";

                File.Move(oldName, newName);//移动文件，有改名字的效果
                nameIndex++;
            }
            AssetDatabase.Refresh();
        }
    }

    ////按照文件创建时间排序
    //DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
    //FileInfo[] fileInfos = directoryInfo.GetFiles();
    //fileInfos.OrderBy(f => f.CreationTime); //根据创建时间排序
    //fileInfos.OrderBy(f => f.Name);  //根据文件名排序

    // DirectoryInfo.GetFiles()获取到的文件会产生1，10，11，2，3的情况
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

    [MenuItem("MenuCommand/RameByIndex",true)]//参数为true是，这个方发的返回值决定了和这个方法的参数一相同且参数二为false的方法，是否可以点击
    static private bool IsSelectPath()
    {
        return (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(Selection.activeObject)));//如果选中的物体的路径为空，则不显示按钮（返回false）
    }
}
