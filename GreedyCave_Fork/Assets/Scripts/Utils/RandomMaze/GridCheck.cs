using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCheck
{
    private int wall;
    private int road;
    public GridCheck(int wall, int road)
    {
        this.wall = wall;
        this.road = road;
    }

    //1.        2.          3.              -1(说明没有特殊用途的格子)
    //*   *     * * *       * * *
    //* * *     *
    //          *
    public int CheckGrid(int[,]map)
    {
        int wideth = map.GetLength(0);
        int height = map.GetLength(1);

        int cornerCount = 0;
        int lineCount = 0;

        for (int x = 0; x < wideth; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x <= 0 || x >= wideth - 1 || y <= 0 || y >= height - 1)//边缘的墙
                    continue;
                if (map[x - 1, y] == wall && map[x - 1, y + 1] == wall && map[x, y + 1] == wall)//左上
                    cornerCount++;
                if (map[x + 1, y] == wall && map[x + 1, y + 1] == wall && map[x, y + 1] == wall)//右上
                    cornerCount++;
                if (map[x - 1, y] == wall && map[x - 1, y - 1] == wall && map[x, y - 1] == wall)//左下
                    cornerCount++;
                if (map[x + 1, y] == wall && map[x + 1, y - 1] == wall && map[x, y - 1] == wall)//右下
                    cornerCount++;

                if (cornerCount > 2)
                    Debug.Log("这。。这不对吧");
                if (cornerCount == 2)
                    return 1;
                if (cornerCount == 1)
                    return 2;

                if (map[x - 1, y + 1] == wall && map[x, y + 1] == wall && map[x - 1, y + 1] == wall)//上
                    lineCount++;
                if (map[x - 1, y - 1] == wall && map[x, y - 1] == wall && map[x - 1, y - 1] == wall)//下
                    lineCount++;
                if (map[x - 1, y + 1] == wall && map[x - 1, y] == wall && map[x - 1, y - 1] == wall)//左
                    lineCount++;
                if (map[x + 1, y + 1] == wall && map[x + 1, y] == wall && map[x + 1, y - 1] == wall)//左
                    lineCount++;

                if (lineCount > 3)
                    Debug.Log("这有问题");
                if (lineCount == 1)
                    return 3;

            }
        }

        return -1;
    }

}
