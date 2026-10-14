using UnityEngine;
using System.Collections.Generic;

public class EdgeWeightedDigraph
{

    private List<DirectedEdge>[] adj;

    private int nodeCount;
    private int edgeCount;


    public int V() { return nodeCount; }
    public int E() { return edgeCount; }
    public EdgeWeightedDigraph(int _nodeCount)
    {
        adj = new List<DirectedEdge>[_nodeCount];
        for(int i = 0; i < _nodeCount; i++)
        {
            adj[i] = new List<DirectedEdge>();
        }

    }

    public void AddEdge(DirectedEdge edge)
    {
        int v = edge.From;
        int w = edge.To;
        adj[edge.From].Add(edge);
        edgeCount++;
    }

    public IEnumerable<DirectedEdge> Adj(int _node)
    {
        return adj[_node];
    }
    
}
