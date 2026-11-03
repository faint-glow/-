//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SplitMaze
//{
//    private static int road = 0;
//    private static int wall = 1;

//    //public static bool IsWall(int num)
//    //{
//    //    return num == wall;
//    //}
//    public static int GetRoad()
//    {
//        return road;
//    }
//    public static int GetWall()
//    {
//        return wall;
//    }

//    public static int[,] CreateMap(int wideth,int height)
//    {
//        int[,] map = new int[wideth, height];
//        InitialMap(map);
//        Split(map, 1, 1, map.GetLength(0) - 2, map.GetLength(1) - 2);
//        return map;
//    }

//    private static void InitialMap(int[,] map)
//    {
//        int rowCount = map.GetLength(0);//相当于x轴
//        int columnCount = map.GetLength(1);//相当于y轴

//        if (rowCount != columnCount)//如果长不等于宽
//        {
//            //map.GetLength(0)参数为0表示一行有多少列（宽度），参数为1表示一列有多少行（高度）
//            for (int i = 0; i < rowCount; i++)//把
//            {
//                map[i, 0] = wall;
//                map[i, columnCount - 1] = wall;
//            }
//            for (int i = 0; i < columnCount; i++)
//            {
//                map[0, i] = wall;
//                map[rowCount - 1, i] = wall;
//            }
//        }
//        else
//        {
//            int N = map.GetLength(0);
//            for (int i = 0; i < N; i++)
//            {
//                map[i, 0] = wall;
//                map[i, N - 1] = wall;
//                map[0, i] = wall;
//                map[N - 1, i] = wall;
//            }
//        }

//    }

//    public static void Split(int[,] map,int x1,int y1,int x2,int y2)
//    {
//        int lengthX = x2 - x1;
//        int lengthY = y2 - y1;
//        if (lengthX < 5 || lengthY < 5)//有少概率出现较大的空旷地区
//        {
//            if (Random.Range(0, Mathf.Min(lengthX, lengthY) * 2) == 0)
//                return;
//        }


//        if (lengthX < 2 || lengthY < 2)
//            return;

//        int x = Random.Range(x1 + 1, x2);
//        int y = Random.Range(y1 + 1, y2);

//        for (int i = x1; i <= x2; i++)
//        {
//            map[i,y] = wall;//C#二维数组前面表示列，后面表示行，C++相反（目前推测）
//        }
//        for (int i = y1; i <=  y2; i++)
//        {
//            map[x,i] = wall;
//        }

//        Split(map, x1, y1, x - 1, y - 1);
//        Split(map, x + 1, y + 1,x2, y2);
//        Split(map, x + 1, y1, x2, y-1);
//        Split(map, x1, y + 1, x-1, y2);

//        int[] r = new int[4];
//        r[Random.Range(0, 4)] = 1;
//        for (int i = 0; i < 4; i++)
//        {
//            if (r[i] == 1)
//                continue;
//            int rx = x;
//            int ry = y;

//            int choiceCount = 0;
//            switch (i)
//            {
//                case 0:
//                    choiceCount = 0;

//                    do
//                    {
//                        if (choiceCount > 5)//保险措施，虽然运气最差，也有1/2的概率结束循环
//                        {
//                            rx += Random.Range(0, 2)==1 ? -1 : 1;
//                            break;
//                        }
//                        choiceCount++;

//                        rx = Random.Range(x1, x);
//                    } while (map[rx - 1, ry] + map[rx + 1, ry] + map[rx, ry - 1] + map[rx, ry + 1] > 2);
//                    break;
//                case 1:
//                    choiceCount = 0;
//                    do
//                    {
//                        if (choiceCount > 5)
//                        {
//                            ry += Random.Range(0, 2) == 1 ? -1 : 1;
//                            break;
//                        }
//                        choiceCount++;

//                        ry = Random.Range(y + 1, y2);
//                    } while (map[rx - 1, ry] + map[rx + 1, ry] + map[rx, ry - 1] + map[rx, ry + 1] > 2);
//                    break;
//                case 2:
//                    choiceCount = 0;
//                    do
//                    {
//                        if (choiceCount > 5)
//                        {
//                            rx += Random.Range(0, 2) == 1 ? -1 : 1;
//                            break;
//                        }
//                        choiceCount++;

//                        rx = Random.Range(x + 1, x2);
//                    } while (map[rx - 1, ry] + map[rx + 1, ry] + map[rx, ry - 1] + map[rx, ry + 1] > 2);
//                    break;
//                case 3:
//                    choiceCount = 0;
//                    do
//                    {
//                        if (choiceCount > 5)
//                        {
//                            ry += Random.Range(0, 2) == 1 ? -1 : 1;
//                            break;
//                        }
//                        choiceCount++;

//                        ry = Random.Range(y1, y);
//                    } while (map[rx - 1, ry] + map[rx + 1, ry] + map[rx, ry - 1] + map[rx, ry + 1] > 2);
//                    break;
//                default:
//                    break;
//            }
//            map[rx, ry] = road;
//        }

//    }

//    public int NoRepeatRandom(int[,]map,int rx,int ry,int min,int max)
//    {
//        int length = max - min;
//        int[] array = new int[length];
//        for (int j = 0; j < length; j++)
//        {
//            array[j] = j + min;
//        }
//        for (int j = length; j > 0; j--)
//        {
//            int num = Random.Range(0, j);
//            Swap(array, num, j);
//            rx = array[j];
//            if (map[rx - 1, ry] + map[rx + 1, ry] + map[rx, ry - 1] + map[rx, ry + 1] < 2)
//                return rx;
//        }
//        return -1;
//    }
//    private void Swap(int[] array, int x, int y)
//    {
//        int temp = array[x];
//        array[x] = array[y];
//        array[y] = temp;
//    }
//}
