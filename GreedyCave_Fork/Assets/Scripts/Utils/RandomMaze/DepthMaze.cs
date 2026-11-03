using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthMaze
{
    public int wall = 0;
    public int road = 1;

    public int[,] CreateMap(int wideth,int height)
    {
        int[,] map = new int[wideth,height];
        InitialMap(map);
        Dig(map,8, 5);

        return map;
    }

    //初始化地图，让四周全部为路，防止无限迭代
    private void InitialMap(int[,]map)
    {
        
        int rowCount = map.GetLength(0);
        int columnCount = map.GetLength(1);

        if (rowCount != columnCount)
        {
            //map.GetLength(0)参数为0表示一行有多少列（宽度），参数为1表示一列有多少行（高度）
            for (int i = 0; i < rowCount; i++)
            {
                map[i, 0] = road;
                map[i, columnCount - 1] = road;
            }
            for (int i = 0; i < columnCount; i++)
            {
                map[0, i] = road;
                map[rowCount - 1, i] = road;
            }
        }
        else
        {
            int N = map.GetLength(0);
            for (int i = 0; i < N; i++)
            {
                map[i, 0] = road;
                map[i, N - 1] = road;
                map[0, i] = road;
                map[N - 1, i] = road;
            }
        }

    }

    private void Dig(int[,] map,int x,int y)
    {
        //Debug.Log("game");
        if (map[x, y] == road)//如果当前位置是路，就不需要打穿
            return;
        //检测当前墙壁四周是否有两个及以上的路，如果有,打穿就会出现环路，可以选择性打穿，直接返回表示都不打穿
        if (map[x + 1, y] + map[x - 1, y] + map[x, y + 1] + map[x, y - 1] > 1)
            //选择性打穿
            return;
        map[x, y] = road;

        int[] direction = { 0, 1, 2, 3 };
        for (int i = direction.Length - 1; i >= 0; i--)
        {
            int randomNum = Random.Range(0, i+1);//Random.Range取下不取上
            int choice = direction[randomNum];
            Swap(direction, randomNum, i);

            switch(choice)
            {
                case 0:
                    Dig(map, x - 1, y);
                    break;
                case 1:
                    Dig(map, x + 1, y);
                    break;
                case 2:
                    Dig(map, x, y-1);
                    break;
                case 3:
                    Dig(map, x, y+1);
                    break;
                default:
                    break;
            }
        }
    }

    private void Swap(int[] array,int x,int y)
    {
        int temp =  array[x];
        array[x] = array[y];
        array[y] = temp;
    }
}
