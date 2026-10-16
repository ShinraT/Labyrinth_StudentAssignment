using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
public class DijkstraSP
{
    private DirectedEdge[] edgeTo;
    private double[] distTo;
    private IndexMinPQ<double> pq;

    public DijkstraSP(EdgeWeightedDigraph _graph, int _source)
    {
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
            // O(V * E)
            foreach (var edge in _graph.Adj(v))
            {
                Relax(edge);
            }
        }
    }
    // Relax tar in en edge, vi kommer att relaxa alla 
    private void Relax(DirectedEdge _edge) 
    {
        int from = _edge.From, to = _edge.To;
        
        if (distTo[to] > distTo[from] + _edge.Weight)
        {
            distTo[to] = distTo[from] + _edge.Weight;
            edgeTo[to] = _edge;

            // O(log n) Tid för uppslagning i balanserat binärt söktröd.
            if (pq.Contains(to))
            {
                pq.DecreaseKey(to, distTo[to]);
            }
            else
            {
                // O(log n) Tid för uppslagning i balanserat binärt sökträd. 
                pq.Insert(to, distTo[to]);
            }
        }
    }

    public bool hasPathTo(int v)
    {
        return distTo[v]  < Double.PositiveInfinity;
    }

    // Vi lägger dem i en stack precis som i BFS för att vi vill ha dem i rätt ordning. (Baklänges)
    public IEnumerable<DirectedEdge> pathTo(int v)
    {
        if (!hasPathTo(v)) return null;
        Stack<DirectedEdge> _path = new Stack<DirectedEdge>();
        for(DirectedEdge edge = edgeTo[v]; edge != null; edge = edgeTo[edge.From])
        {
            _path.Push(edge);
        }
        return _path;
    }

}