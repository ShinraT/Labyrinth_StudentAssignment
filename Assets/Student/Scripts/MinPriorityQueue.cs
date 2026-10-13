using UnityEngine;
using System.Collections.Generic;
using System;

public class MinPriorityQueue<T> where T : IComparable<T>
{
    private int count;
    private T[] pq;

    public MinPriorityQueue(int capacity)
    {
        pq = new T[capacity + 1];
        count = 0;
    }


    public void Enqueue(T item)
    {
        if (count == pq.Length - 1)
            Resize(2 * pq.Length);
        pq[++count] = item;
        Swim(count);
    }

    private void Resize(int capacity)
    {
        T[] temp = new T[capacity];
        for (int i = 1; i <= count; i++)
        {
            temp[i] = pq[i];
        }
        pq = temp;
    }

    public T Dequeue()
    {
        if (count == 0)
            throw new InvalidOperationException("Priority queue is empty.");
        T min = pq[1];
        Swap(1, count--);
        Sink(1);
        pq[count + 1] = default(T); // Avoid loitering
        if (count > 0 && count == (pq.Length - 1) / 4)
            Resize(pq.Length / 2);
        return min;
    }

    private void Sink(int k)
    {
        // Move the item at index k down the heap until the heap property is restored
        while (2 * k <= count)
        {
            int j = 2 * k;
            if (j < count && pq[j].CompareTo(pq[j + 1]) > 0)
                j++;
            if (pq[k].CompareTo(pq[j]) <= 0)
                break;
            Swap(k, j);
            k = j;
        }
    }

    private void Swim(int k)
    {
        // Move the item at index k up the heap until the heap property is restored
        while (k > 1 && pq[k].CompareTo(pq[k / 2]) < 0)
        {   
            Swap(k, k / 2);
            k = k / 2;
        }
    }
 
    public T PeekMin()
    {
        if (count == 0)
            throw new InvalidOperationException("Priority queue is empty.");
        return pq[1];
    }

    public bool IsEmpty()
    {
        return count == 0;
    }

    private void Swap(int i, int j)
    {
        T temp = pq[i];
        pq[i] = pq[j];
        pq[j] = temp;
    }
}