using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MinQue<E> : IQueue<E> where E : IComparable<E>
{
    private MinHeap<E> heap;
    public int Count { get { return heap.Count; } }

    public bool IsEmpty { get { return heap.IsEmpty; } }

    public MinQue(int capacity)
    {
        heap = new MinHeap<E>(capacity);
    }

    public MinQue()
    {
        heap = new MinHeap<E>();
    }

    public E Deque()
    {
        return heap.RemoveMin();
    }

    public void Enque(E e)
    {
        heap.Add(e);
    }

    public E Peek()
    {
        return heap.Min();
    }

    public bool Contain(E e)
    {
        return heap.Contain(e);
    }

    public void Clear()
    {
        heap.Clear();
    }
}
