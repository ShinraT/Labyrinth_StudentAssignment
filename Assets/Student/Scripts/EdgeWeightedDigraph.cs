using UnityEngine;
using System.Collections.Generic;

public class EdgeWeightedDigraph
{

    private List<DirectedEdge>[] adj;

    private int nodeCount;
    private int edgeCount;
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
        adj[edge.From].Add(edge);
    }

    public IEnumerable<DirectedEdge> Adj(int _node)
    {
        return adj[_node];
    }
    
}
