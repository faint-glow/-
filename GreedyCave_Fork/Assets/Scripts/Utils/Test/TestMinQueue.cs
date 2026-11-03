using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMinQueue : MonoBehaviour
{
    float[] a = { 2,2,2,2,2,2,5,10,1000,100,15,10,11,11,11,19};
    int[] b = { 1, 2, 1 };

    private void Awake()
    {
        MinQue<float> minque = new MinQue<float>();

        for (int i = 0; i < a.Length; i++)
        {
            minque.Enque(a[i]);
        }

        //for (int i = 0; i < a.Length; i++)
        //{
        //    Debug.Log(minque.Deque());
        //}
        print(minque.Deque());
        print(minque.Deque());
        print(minque.Deque());
        print(minque.Deque());
        print(minque.Deque());
        print(minque.Deque());
        print(minque.Deque());
        print(minque.Deque());
        print(minque.Deque());
        print(minque.Deque());
        print(minque.Deque());
        print(minque.Deque());
    }
}
