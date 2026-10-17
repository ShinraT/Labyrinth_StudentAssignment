using System.Collections.Generic;
using UnityEngine;

public class DistanceModel
{
    EdgeWeightedDigraph graph;
    private Vector2Int start;
    private double estimate;
    private IMapData mapData;
    private Vector2Int goal;
    public double Est() {  return estimate; }

    public DistanceModel(EdgeWeightedDigraph _graph, int _startIndex, int _goalIndex , IMapData mapData)
    {
        this.mapData = mapData;

        goal = new Vector2Int(_goalIndex % mapData.Width,_goalIndex / mapData.Width);
    }

    public double NewGoalDistEstiamte(int currentIndex, double distToV)
    {
        Vector2Int _current = new Vector2Int(currentIndex % mapData.Width, currentIndex / mapData.Width);

        double heuristic = Mathf.Abs(_current.x - goal.x) + Mathf.Abs(_current.y - goal.y);

        return (distToV + heuristic);

    }
    
}
