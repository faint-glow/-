using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IQueue<E>
{
    int Count { get; }

    bool IsEmpty { get; }

    void Enque(E e);

    E Deque();

    E Peek();
}
