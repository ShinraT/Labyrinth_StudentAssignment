using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
public class DijkstraSP
{
    private DirectedEdge[] edgeTo; // 
    private double[] distTo; // Representerar kostnaden för att komma till den nodens index. 
    private IndexMinPQ<double> pq;
    private int source;
    public DijkstraSP(EdgeWeightedDigraph _graph, int _source)
    {
        source = _source;
        edgeTo = new DirectedEdge[_graph.V()];
        distTo = new double[_graph.V()];
        pq = new IndexMinPQ<double>(_graph.V());

        // Vi sätter alla noder till oändlighet förutom source som vi sätter till 0.0
        for (int v = 0; v < _graph.V(); v++)
            distTo[v] = double.PositiveInfinity;
        distTo[_source] = 0.0;


        pq.Insert(_source, distTo[_source]);
        while (!pq.IsEmpty())
        {
            int v = pq.DelMin();
            //För alla noder i graphen kollar relaxar vi alla dess edges

            foreach (var edge in _graph.Adj(v))
            {
                Relax(edge);
            }
        }
    }
    // Relax tar in en edge, vi kommer att relaxa alla 
    private void Relax(DirectedEdge _edge) 
    {
        int from = _edge.From,
            to = _edge.To;
        
        if (distTo[to] > distTo[from] + _edge.Weight)
        {
            distTo[to] = distTo[from] + _edge.Weight;
            edgeTo[to] = _edge;

            // O(1) konstant tid för contains. 
            if (pq.Contains(to))
            {
                // O(log n) Tid för decreaseKey i binary heap. Vi uppdaterar noden i pq med dess distTo värde som nyckel.
                pq.DecreaseKey(to, distTo[to]);
            }
            else
            {
                // O(log n) Tid för insert i binary heap. Vi lägger in noden i pq med dess distTo värde som nyckel.
                pq.Insert(to, distTo[to]);
            }
        }
    }

    public bool hasPathTo(int v)
    {
        return distTo[v]  < Double.PositiveInfinity;
    }

    // Vi lägger dem i en stack precis som i BFS för att vi vill ha dem i rätt ordning. (Baklänges)
    public IEnumerable<DirectedEdge> pathTo(int goal)
    {
        if (!hasPathTo(goal)) return null;
        Stack<DirectedEdge> _path = new Stack<DirectedEdge>();
        for(DirectedEdge edge = edgeTo[goal]; edge != null; edge = edgeTo[edge.From])
        {
            _path.Push(edge);
        }
        
        return _path;
    }

    public IEnumerable<int> newPathTo(int goal)
    {
        if (!hasPathTo(goal))
            return null;

        Stack<int> path = new Stack<int>();

        int current = goal;
        path.Push(current);

        while (current != source)
        {
            DirectedEdge edge = edgeTo[current];

            current = edge.From;
            path.Push(current);
        }

        return path;
    }

}