using UnityEngine;
using System.Collections.Generic;
using Unity.Properties;
using System.Collections;

public class Graph
{

    private int edgeCount;
    private int nodeCount;

    public int V() { return nodeCount; }
    public int E() { return edgeCount; }
    public IEnumerable<int> Adj(int v) { return adj[v]; }
    private List<int>[] adj;

    public Graph(int _nodeCount)
    {
        this.nodeCount = _nodeCount;
        adj = new List<int>[nodeCount];
        for (int i = 0; i < nodeCount; i++)
        {
            adj[i] = new List<int>();
        }
    }

    public void AddEdge(int nodeA, int nodeB)
    {
        adj[nodeA].Add(nodeB);
        adj[nodeB].Add(nodeA);
        edgeCount++;
    }

    public void AddDirectedEdge(int nodeA, int nodeB)
    {
        adj[nodeA].Add(nodeB);
        edgeCount++;

    }
}
