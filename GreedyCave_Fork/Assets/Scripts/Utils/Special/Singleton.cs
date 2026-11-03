using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : new()
{
    private static T instance;
    private static object mutex = new object();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (mutex)
                {
                    if (instance == null)
                        instance = new T();
                }
            }
            return instance;
        }
    }

    public static void ReCreate()
    {
        instance = new T();
    }
}

