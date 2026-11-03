using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MinHeap<E> where E : IComparable<E>
{
    private E[] heap;
    private int N;

    public MinHeap(int capacity)
    {
        heap = new E[capacity];
    }

    public MinHeap() : this(10) { }

    public int Count { get { return N; } }

    public bool IsEmpty { get { return N == 0; } }

    public void Add(E e)
    {
        if (N == heap.Length)
            ResetCapacity(heap.Length * 2);

        heap[N] = e;
        Swim(N);
        N++;
    }

    public E Min()
    {
        if (IsEmpty)
            throw new ArgumentException("堆为空");

        return heap[0];
    }

    public E RemoveMin()
    {
        if (IsEmpty)
        {
            throw new ArgumentException("堆为空");
        }

        E min= Min();
        Swap(0, N - 1);
        heap[N - 1] = default(E);
        N--;
        Sink(0);

        if (N == heap.Length / 4 && heap.Length > 10)//如果不加第二个条件，有可能出现heap.length等于0
            ResetCapacity(heap.Length / 2);

        return min;
    }

    public bool Contain(E e)
    {
        for(int i = 0;i < N; i++)
        {
            if(heap[i].CompareTo(e) == 0)
            {
                return true;
            }
        }
        return false;
    }

    public void Clear()
    {
        heap = new E[10];
        N = 0;
    }

    private void Swim(int k)
    {
        while(k > 0 && heap[k].CompareTo(heap[(k - 1) / 2]) < 0)
        {
            Swap(k,(k-1)/2);
            k = (k - 1) / 2;
        }
    }

    private void Sink(int k)
    {
        while (k * 2 + 1 < N) 
        {
            int j = k * 2 + 1;
            if (j + 1 < N && heap[j].CompareTo(heap[j+1]) > 0)
                j += 1;
            if (heap[k].CompareTo(heap[j]) < 0)
                break;
            Swap(k, j);
            k = j;
        }
    }

    private void ResetCapacity(int n)
    {
        E[] newHeap = new E[n];
        for (int i = 0; i < N; ++i)
        {
            newHeap[i] = heap[i];
        }
        heap = newHeap;
    }

    private void Swap(int i,int j)
    {
        E temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }
}
