using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AStar
{

    private DirectedEdge[] edgeTo;
    private double[] distTo;
    private IndexMinPQ<double> pq;
    private DistanceModel distModel;
    private HashSet<int> reached;
    public AStar(EdgeWeightedDigraph _graph, int _source, int goal,IMapData mapData)
    {
        edgeTo = new DirectedEdge[_graph.V()];
        distTo = new double[_graph.V()];
        pq = new IndexMinPQ<double>(_graph.V());

        reached = new HashSet<int>();

        distModel = new DistanceModel(_graph, _source, goal , mapData);


        distTo[_source] = 0.0;
        reached.Add(_source);

        pq.Insert(_source, distTo[_source]);
        while (!pq.IsEmpty())
        {
            int v = pq.DelMin();
            if (v == goal)
                break; //Vi har hittat en väg, då breakar vi.

            foreach (var edge in _graph.Adj(v))
            {
                Relax(edge);
            }
        }
    }

    private void Relax(DirectedEdge _edge)
    {
        int from = _edge.From;
        int to = _edge.To;

        
        if (!reached.Contains(to) ||  distTo[to] > distTo[from] + _edge.Weight)
        {
            distTo[to] = distTo[from] + _edge.Weight;
            edgeTo[to] = _edge;

            reached.Add(to);
            double estimate = distModel.NewGoalDistEstiamte(to, distTo[to]);
            // O(log n) Tid för uppslagning i balanserat binärt söktröd.
            if (pq.Contains(to))
            {
                pq.DecreaseKey(to, estimate);
            }
            else
            {
                // O(log n) Tid för uppslagning i balanserat binärt sökträd. 
                pq.Insert(to, estimate);
            }
        }
    }

    public bool hasPathTo(int v)
    {
        return distTo[v] < Double.PositiveInfinity;
    }

    // Vi lägger dem i en stack precis som i BFS för att vi vill ha dem i rätt ordning. (Baklänges)
    public IEnumerable<DirectedEdge> pathTo(int goal)
    {
        if (!hasPathTo(goal)) return null;
        Stack<DirectedEdge> _path = new Stack<DirectedEdge>();
        for (DirectedEdge edge = edgeTo[goal]; edge != null; edge = edgeTo[edge.From])
        {
            _path.Push(edge);
        }
        return _path;
    }

}
