using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IndexMinPQ<Key> : IEnumerable<int> where Key : IComparable<Key>
{
    private readonly int maxN;
    private int n;

    // Binary heap using 1-based indexing
    private readonly int[] pq;

    // Inverse of pq:
    // qp[pq[i]] = pq[qp[i]] = i
    private readonly int[] qp;

    // keys[i] = priority of index i
    private readonly Key[] keys;


    public IndexMinPQ(int maxN)
    {
        if (maxN < 0)
            throw new ArgumentException();

        this.maxN = maxN;
        n = 0;

        keys = new Key[maxN + 1];
        pq = new int[maxN + 1];
        qp = new int[maxN + 1];

        for (int i = 0; i <= maxN; i++)
        {
            qp[i] = -1;
        }
    }


    public bool IsEmpty()
    {
        return n == 0;
    }


    public bool Contains(int i)
    {
        ValidateIndex(i);

        return qp[i] != -1;
    }


    public int Size()
    {
        return n;
    }


    public void Insert(int i, Key key)
    {
        ValidateIndex(i);

        if (Contains(i))
        {
            throw new ArgumentException(
                "Index is already in the priority queue");
        }

        n++;

        qp[i] = n;
        pq[n] = i;
        keys[i] = key;

        Swim(n);
    }


    public int MinIndex()
    {
        if (n == 0)
        {
            throw new InvalidOperationException(
                "Priority queue underflow");
        }

        return pq[1];
    }


    public Key MinKey()
    {
        if (n == 0)
        {
            throw new InvalidOperationException(
                "Priority queue underflow");
        }

        return keys[pq[1]];
    }


    public int DelMin()
    {
        if (n == 0)
        {
            throw new InvalidOperationException(
                "Priority queue underflow");
        }

        int min = pq[1];

        Exch(1, n);
        n--;

        Sink(1);

        qp[min] = -1;
        keys[min] = default;
        pq[n + 1] = -1;

        return min;
    }


    public Key KeyOf(int i)
    {
        ValidateIndex(i);

        if (!Contains(i))
        {
            throw new InvalidOperationException(
                "Index is not in the priority queue");
        }

        return keys[i];
    }


    public void ChangeKey(int i, Key key)
    {
        ValidateIndex(i);

        if (!Contains(i))
        {
            throw new InvalidOperationException(
                "Index is not in the priority queue");
        }

        keys[i] = key;

        Swim(qp[i]);
        Sink(qp[i]);
    }


    public void Change(int i, Key key)
    {
        ChangeKey(i, key);
    }


    public void DecreaseKey(int i, Key key)
    {
        ValidateIndex(i);

        if (!Contains(i))
        {
            throw new InvalidOperationException(
                "Index is not in the priority queue");
        }

        if (keys[i].CompareTo(key) == 0)
        {
            throw new ArgumentException(
                "Calling DecreaseKey() with an equal key");
        }

        if (keys[i].CompareTo(key) < 0)
        {
            throw new ArgumentException(
                "Calling DecreaseKey() with a greater key");
        }

        keys[i] = key;

        Swim(qp[i]);
    }


    public void IncreaseKey(int i, Key key)
    {
        ValidateIndex(i);

        if (!Contains(i))
        {
            throw new InvalidOperationException(
                "Index is not in the priority queue");
        }

        if (keys[i].CompareTo(key) == 0)
        {
            throw new ArgumentException(
                "Calling IncreaseKey() with an equal key");
        }

        if (keys[i].CompareTo(key) > 0)
        {
            throw new ArgumentException(
                "Calling IncreaseKey() with a smaller key");
        }

        keys[i] = key;

        Sink(qp[i]);
    }


    public void Delete(int i)
    {
        ValidateIndex(i);

        if (!Contains(i))
        {
            throw new InvalidOperationException(
                "Index is not in the priority queue");
        }

        int index = qp[i];

        Exch(index, n);
        n--;

        Swim(index);
        Sink(index);

        keys[i] = default;
        qp[i] = -1;
    }


    private void ValidateIndex(int i)
    {
        if (i < 0)
        {
            throw new ArgumentException(
                $"Index is negative: {i}");
        }

        if (i >= maxN)
        {
            throw new ArgumentException(
                $"Index >= capacity: {i}");
        }
    }


    private bool Greater(int i, int j)
    {
        return keys[pq[i]].CompareTo(keys[pq[j]]) > 0;
    }


    private void Exch(int i, int j)
    {
        int swap = pq[i];

        pq[i] = pq[j];
        pq[j] = swap;

        qp[pq[i]] = i;
        qp[pq[j]] = j;
    }


    private void Swim(int k)
    {
        while (k > 1 && Greater(k / 2, k))
        {
            Exch(k, k / 2);

            k = k / 2;
        }
    }


    private void Sink(int k)
    {
        while (2 * k <= n)
        {
            int j = 2 * k;

            // Pick the smaller child
            if (j < n && Greater(j, j + 1))
            {
                j++;
            }

            // Heap property is restored
            if (!Greater(k, j))
            {
                break;
            }

            Exch(k, j);

            k = j;
        }
    }


    public IEnumerator<int> GetEnumerator()
    {
        IndexMinPQ<Key> copy = new IndexMinPQ<Key>(maxN);

        for (int i = 1; i <= n; i++)
        {
            copy.Insert(pq[i], keys[pq[i]]);
        }

        while (!copy.IsEmpty())
        {
            yield return copy.DelMin();
        }
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}