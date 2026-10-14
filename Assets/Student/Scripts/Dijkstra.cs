using UnityEngine;
using System;
using System.Collections.Generic;
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

        for (int v = 0; v < _graph.V(); v++)
            distTo[v] = double.PositiveInfinity;
        distTo[_source] = 0.0;
    }

    private void Relax(DirectedEdge _edge)
    {
        int v = _edge.From, w = _edge.To;

        if (distTo[w] > distTo[v] + _edge.Weight)
        {
            distTo[w] = distTo[v] + _edge.Weight;
            edgeTo[w] = _edge;
            if (pq.Contains(w))
                pq.DecreaseKey(w, distTo[w]);
            else
                pq.Insert(w, distTo[w]);
        }
    }

}