using UnityEngine;
using System.Collections.Generic;
using Unity.Properties;
using System.Collections;

public class Graph 
{

    private int e;
    private int v;

    public int V() { return v; }
    public int E() { return e; }
    private List<int>[] adj;

    public Graph(int V)
    {
        this.v = V;
        adj = new List<int>[v];
        for(int i = 0; i < v; i++)
        {
            adj[i] = new List<int>();
        }
    }
    

    public void AddEdge(int v, int w)
    {
        adj[v].Add(w);
        adj[w].Add(v);
        e++;
    }
    


    
}
