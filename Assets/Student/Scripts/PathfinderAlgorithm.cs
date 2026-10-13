using System.Collections.Generic;
using UnityEngine;

public static class PathfindingAlgorithm
{
    // TODO: Implement the pathfinding assignment here.
    // Find the lowest-cost path from start to goal using the map dimensions, wall costs and directed jumps exposed by IMapData.
    public static List<Vector2Int> FindShortestPath(Vector2Int start, Vector2Int goal, IMapData mapData)
    {
        // Most of your solution should be implemented in this method.
        Graph graph = new Graph(mapData.Height * mapData.Width);

        // Itererar över alla noder // alla Vector2Int som är giltiga i mappen. 


        // For loopen lägger in bågar mellan de traverserbara noderna.  
        for (int y = 0; y < mapData.Height; y++)
        {
            for (int x = 0; x < mapData.Width; x++)
            {
                Vector2Int currentCell = new Vector2Int(x, y);
                int currentNodeIndex;
                int neighBorNodeIndex;

                Vector2Int _neighbor = new Vector2Int(currentCell.x - 1, currentCell.y); // Vänster Neighbor
                if (IsInsideMap(_neighbor, mapData) && !IsMovementBlocked(currentCell, _neighbor, mapData))
                {
                    NodeIndexer(currentCell,_neighbor,  out currentNodeIndex, out neighBorNodeIndex, mapData);
                    graph.AddDirectedEdge(currentNodeIndex, neighBorNodeIndex);
                    BetterDebugDraw(currentCell, _neighbor);
                }

                _neighbor = new Vector2Int(currentCell.x + 1, currentCell.y); // Höger Neighbor
                if (IsInsideMap(_neighbor, mapData) && !IsMovementBlocked(currentCell, _neighbor, mapData))
                {
                    NodeIndexer(currentCell, _neighbor, out currentNodeIndex, out neighBorNodeIndex, mapData);
                    graph.AddDirectedEdge(currentNodeIndex, neighBorNodeIndex);
                    BetterDebugDraw(currentCell, _neighbor);
                }

                _neighbor = new Vector2Int(currentCell.x, currentCell.y + 1); // Upp Neighbor
                if (IsInsideMap(_neighbor, mapData) && !IsMovementBlocked(currentCell, _neighbor, mapData))
                {
                    NodeIndexer(currentCell, _neighbor, out currentNodeIndex, out neighBorNodeIndex, mapData);
                    graph.AddDirectedEdge(currentNodeIndex, neighBorNodeIndex);
                    BetterDebugDraw(currentCell, _neighbor);
                }

                _neighbor = new Vector2Int(currentCell.x, currentCell.y - 1); // Ned neighbor
                if (IsInsideMap(_neighbor, mapData) && !IsMovementBlocked(currentCell, _neighbor, mapData))
                {
                    NodeIndexer(currentCell, _neighbor, out currentNodeIndex, out neighBorNodeIndex, mapData);
                    graph.AddDirectedEdge(currentNodeIndex, neighBorNodeIndex);
                    BetterDebugDraw(currentCell, _neighbor);
                }
            }
        }

        int startIndex = start.y * mapData.Width + start.x;
        BFS BreadthFirstSearch = new BFS(graph, startIndex);
        IEnumerable<int> tempPath = BreadthFirstSearch.pathTo(goal.y * mapData.Width +  goal.x);

        List<Vector2Int> path = new List<Vector2Int>();


        // Konverterar tillbaka nodeIndexen till en Vector2Int med hjälp av modulo operator. 
        foreach(int index in tempPath)
        {
            int x = index % mapData.Width;
            int y = index / mapData.Width;
            path.Add(new Vector2Int(x, y));
        }



        Debug.Log($"Antal Edges{graph.E()}");

        return path;
        Debug.LogWarning("FindShortestPath is not implemented.");
        return null;
    }

    public static bool IsMovementBlocked(Vector2Int from, Vector2Int to, IMapData mapData)
    {
        if (mapData.TryGetJumpCost(from, to, out _))
            return false;

        int deltaX = to.x - from.x;
        int deltaY = to.y - from.y;

        if (Mathf.Abs(deltaX) + Mathf.Abs(deltaY) != 1)
            return true;

        if (deltaX != 0)
        {
            int wallX = deltaX > 0 ? to.x : from.x;
            float wallCost = mapData.GetVerticalWallCost(wallX, from.y);
            return float.IsPositiveInfinity(wallCost) || wallCost >= float.MaxValue;
        }

        int wallY = deltaY > 0 ? to.y : from.y;
        float horizontalWallCost = mapData.GetHorizontalWallCost(from.x, wallY);
        return float.IsPositiveInfinity(horizontalWallCost) || horizontalWallCost >= float.MaxValue;
    }

    private static bool IsInsideMap(Vector2Int cell, IMapData mapData)
    {
        return cell.x >= 0 && cell.x < mapData.Width && cell.y >= 0 && cell.y < mapData.Height;
    }

    private static void BetterDebugDraw(Vector2Int _from, Vector2Int _to)
    {
        Debug.DrawLine(new Vector3(_from.x + 0.5f, 0, _from.y + 0.5f), new Vector3(_to.x + 0.5f, 0, _to.y + 0.5f), Color.black, 60f);
    }
    private static void NodeIndexer(Vector2Int _currentNode , Vector2Int _neighbor, out int _current, out int _neighborIndex, IMapData _mapData)
    {
        _current = _currentNode.y * _mapData.Width + _currentNode.x;
        _neighborIndex = _neighbor.y * _mapData.Width + _neighbor.x;
    }




}
