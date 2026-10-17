using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using System.Diagnostics;

public static class PathfindingAlgorithm
{
    // TODO: Implement the pathfinding assignment here.
    // Find the lowest-cost path from start to goal using the map dimensions, wall costs and directed jumps exposed by IMapData.
    static double totalPathCost = 0;
    public static List<Vector2Int> FindShortestPath(Vector2Int start, Vector2Int goal, IMapData mapData)
    {
        totalPathCost = 0;

        Graph graph = new Graph(mapData.Height * mapData.Width);
        EdgeWeightedDigraph weightedDigraph =
            new EdgeWeightedDigraph(mapData.Height * mapData.Width);

        GenerateGraph(graph, mapData);
        GenerateEdgeWeightedGraph(weightedDigraph, mapData);

        int startIndex = start.y * mapData.Width + start.x;
        int goalIndex = goal.y * mapData.Width + goal.x;

        Stopwatch stopwatch = new Stopwatch();


        // ---------------- BFS ----------------

        stopwatch.Start();

        BFS breadthFirstSearch = new BFS(graph, startIndex);

        stopwatch.Stop();

        double bfsTime = stopwatch.Elapsed.TotalMilliseconds;

        IEnumerable<int> tempBFSPath =
            breadthFirstSearch.pathTo(goalIndex);


        // ---------------- DIJKSTRA ----------------

        stopwatch.Restart();

        DijkstraSP dijkstraSP = new DijkstraSP(weightedDigraph, startIndex);

        stopwatch.Stop();
        double dijkstraTime = stopwatch.Elapsed.TotalMilliseconds;

        IEnumerable<DirectedEdge> tempDijkstraPath = dijkstraSP.pathTo(goalIndex);


        // ---------------- A* ----------------

        stopwatch.Restart();

        AStar aStar = new AStar(weightedDigraph, startIndex, goalIndex, mapData);

        stopwatch.Stop();
        double aStarTime = stopwatch.Elapsed.TotalMilliseconds;

        IEnumerable<DirectedEdge> tempAStarPath = aStar.pathTo(goalIndex);


        // =====================================================
        // PATH RESULT
        // =====================================================

        List<Vector2Int> path = new List<Vector2Int>();

        // Dummy-listor för algoritmerna vi inte använder
        List<Vector2Int> dummyBFS = new List<Vector2Int>();
        List<Vector2Int> dummyDijkstra = new List<Vector2Int>();


        // BFS -> dummy
        ConvertPathToVector2Int(tempBFSPath, mapData, dummyBFS);


        // Dijkstra -> dummy
        ConvertPathToVector2Int(tempDijkstraPath, mapData, dummyDijkstra);


        // A* -> den riktiga listan som returneras
        ConvertPathToVector2Int(tempAStarPath, mapData, path);


        // =====================================================
        // RESULTAT
        // =====================================================

        UnityEngine.Debug.Log($"BFS:      {bfsTime} ms");
        UnityEngine.Debug.Log($"Dijkstra: {dijkstraTime} ms");
        UnityEngine.Debug.Log($"A*:       {aStarTime} ms");

        //UnityEngine.Debug.Log($"Antal Edges: {graph.E()}");
        //UnityEngine.Debug.Log($"Antal Weighted Edges: {weightedDigraph.E()}");
        UnityEngine.Debug.Log($"Path cost: {totalPathCost}");

        return path;
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
        UnityEngine.Debug.DrawLine(new Vector3(_from.x + 0.5f, 0, _from.y + 0.5f), new Vector3(_to.x + 0.5f, 0, _to.y + 0.5f), Color.black, 60f);
    }
    private static void NodeIndexer(Vector2Int _currentNode, Vector2Int _neighbor, out int _current, out int _neighborIndex, IMapData _mapData)
    {
        _current = _currentNode.y * _mapData.Width + _currentNode.x;
        _neighborIndex = _neighbor.y * _mapData.Width + _neighbor.x;
    }

    private static void GenerateGraph(Graph graph, IMapData mapData)
    {
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
                    // Normal Oriktad Graph. Metod skapar en riktad graf.
                    NodeIndexer(currentCell, _neighbor, out currentNodeIndex, out neighBorNodeIndex, mapData);
                    graph.AddDirectedEdge(currentNodeIndex, neighBorNodeIndex);
                    //BetterDebugDraw(currentCell, _neighbor);

                }

                _neighbor = new Vector2Int(currentCell.x + 1, currentCell.y); // Höger Neighbor
                if (IsInsideMap(_neighbor, mapData) && !IsMovementBlocked(currentCell, _neighbor, mapData))
                {
                    NodeIndexer(currentCell, _neighbor, out currentNodeIndex, out neighBorNodeIndex, mapData);
                    graph.AddDirectedEdge(currentNodeIndex, neighBorNodeIndex);
                    //BetterDebugDraw(currentCell, _neighbor);
                }

                _neighbor = new Vector2Int(currentCell.x, currentCell.y + 1); // Upp Neighbor
                if (IsInsideMap(_neighbor, mapData) && !IsMovementBlocked(currentCell, _neighbor, mapData))
                {
                    NodeIndexer(currentCell, _neighbor, out currentNodeIndex, out neighBorNodeIndex, mapData);
                    graph.AddDirectedEdge(currentNodeIndex, neighBorNodeIndex);
                    //BetterDebugDraw(currentCell, _neighbor);

                }

                _neighbor = new Vector2Int(currentCell.x, currentCell.y - 1); // Ned neighbor
                if (IsInsideMap(_neighbor, mapData) && !IsMovementBlocked(currentCell, _neighbor, mapData))
                {
                    NodeIndexer(currentCell, _neighbor, out currentNodeIndex, out neighBorNodeIndex, mapData);
                    graph.AddDirectedEdge(currentNodeIndex, neighBorNodeIndex);
                    //BetterDebugDraw(currentCell, _neighbor);
                }
            }
        }

    }

    private static void GenerateEdgeWeightedGraph(EdgeWeightedDigraph weightedDigraph, IMapData mapData)
    {

        float jumpCost = 1.0f;
        float wallCost = 1.0f;
        double jumpCostDouble = 1.0;
        for (int y = 0; y < mapData.Height; y++)
        {
            wallCost = 1.0f;
            for (int x = 0; x < mapData.Width; x++)
            {
                Vector2Int currentCell = new Vector2Int(x, y);
                int currentNodeIndex;
                int neighBorNodeIndex;

                Vector2Int _neighbor = new Vector2Int(currentCell.x - 1, currentCell.y); // Vänster Neighbor

                if (IsInsideMap(_neighbor, mapData) && !IsMovementBlocked(currentCell, _neighbor, mapData))
                {
                    //Konstant uppslagningstid i ett Dictionary. 
                    wallCost = mapData.GetVerticalWallCost(currentCell.x,currentCell.y);
                    if(mapData.HasJumpFrom(_neighbor.x, _neighbor.y))
                    {
                        var jumps = mapData.GetJumpsFrom(_neighbor);
                        foreach(var jump in jumps)
                        {
                            mapData.TryGetJumpCost(_neighbor, jump.To, out jumpCost);
                            DirectedEdge _edge = new DirectedEdge(_neighbor.y * mapData.Width + _neighbor.x, jump.To.y * mapData.Width + jump.To.x, jumpCost);
                            weightedDigraph.AddEdge(_edge);
                        }

                    }

                    NodeIndexer(currentCell, _neighbor, out currentNodeIndex, out neighBorNodeIndex, mapData);
                    DirectedEdge edge = new DirectedEdge(currentNodeIndex, neighBorNodeIndex, wallCost);
                    weightedDigraph.AddEdge(edge);

                }
                _neighbor = new Vector2Int(currentCell.x + 1, currentCell.y); // Höger Neighbor
                if (IsInsideMap(_neighbor, mapData) && !IsMovementBlocked(currentCell, _neighbor, mapData))
                {
                    //Konstant uppslagningstid i ett Dictionary. 
                    wallCost = mapData.GetVerticalWallCost(_neighbor.x, currentCell.y);
                    if (mapData.HasJumpFrom(_neighbor.x, _neighbor.y))
                    {
                        var jumps = mapData.GetJumpsFrom(_neighbor);
                        foreach (var jump in jumps)
                        {
                            mapData.TryGetJumpCost(_neighbor, jump.To, out jumpCost);
                            DirectedEdge _edge = new DirectedEdge(_neighbor.y * mapData.Width + _neighbor.x, jump.To.y * mapData.Width + jump.To.x, jumpCost);
                            weightedDigraph.AddEdge(_edge);
                        }

                    }
                    NodeIndexer(currentCell, _neighbor, out currentNodeIndex, out neighBorNodeIndex, mapData);
                    DirectedEdge edge = new DirectedEdge(currentNodeIndex, neighBorNodeIndex, wallCost);
                    weightedDigraph.AddEdge(edge);
                }
                _neighbor = new Vector2Int(currentCell.x, currentCell.y + 1); // Upp Neighbor
                if (IsInsideMap(_neighbor, mapData) && !IsMovementBlocked(currentCell, _neighbor, mapData))
                {
                    //Konstant uppslagningstid i ett Dictionary. 
                    wallCost = mapData.GetHorizontalWallCost(currentCell.x, _neighbor.y);
                    if (mapData.HasJumpFrom(_neighbor.x, _neighbor.y))
                    {
                        var jumps = mapData.GetJumpsFrom(_neighbor);
                        foreach (var jump in jumps)
                        {
                            mapData.TryGetJumpCost(_neighbor, jump.To, out jumpCost);
                            DirectedEdge _edge = new DirectedEdge(_neighbor.y * mapData.Width + _neighbor.x, jump.To.y * mapData.Width + jump.To.x, jumpCost);
                            weightedDigraph.AddEdge(_edge);
                        }

                    }

                    NodeIndexer(currentCell, _neighbor, out currentNodeIndex, out neighBorNodeIndex, mapData);
                    DirectedEdge edge = new DirectedEdge(currentNodeIndex, neighBorNodeIndex, wallCost);
                    weightedDigraph.AddEdge(edge);
                }
                _neighbor = new Vector2Int(currentCell.x, currentCell.y - 1); // Ned neighbor
                if (IsInsideMap(_neighbor, mapData) && !IsMovementBlocked(currentCell, _neighbor, mapData))
                {
                    //Konstant uppslagningstid i ett Dictionary. 
                    wallCost = mapData.GetHorizontalWallCost(currentCell.x, currentCell.y);
                    if (mapData.HasJumpFrom(_neighbor.x, _neighbor.y))
                    {
                        var jumps = mapData.GetJumpsFrom(_neighbor);
                        foreach (var jump in jumps)
                        {
                            mapData.TryGetJumpCost(_neighbor, jump.To, out jumpCost);
                            DirectedEdge _edge = new DirectedEdge(_neighbor.y * mapData.Width + _neighbor.x, jump.To.y * mapData.Width + jump.To.x, jumpCost);
                            weightedDigraph.AddEdge(_edge);
                        }

                    }
                    NodeIndexer(currentCell, _neighbor, out currentNodeIndex, out neighBorNodeIndex, mapData);
                    DirectedEdge edge = new DirectedEdge(currentNodeIndex, neighBorNodeIndex, wallCost);
                    weightedDigraph.AddEdge(edge);
                }
            }
        }
    }

    // För Dijkstra, konverterar DirectedEdge till Vector2Int och lägger till dem i result-listan.
    private static void ConvertPathToVector2Int(IEnumerable<DirectedEdge> path, IMapData mapData, List<Vector2Int> result)
    {
        // O (
        foreach (DirectedEdge edge in path)
        {
            int x = edge.To % mapData.Width;
            int y = edge.To / mapData.Width;
            result.Add(new Vector2Int(x, y));
            totalPathCost += edge.Weight;
        }
        //UnityEngine.Debug.Log($"Antal Edges i pathen {result.Count}");
    }

    // För BFS , konverterar nodeIndexen till Vector2Int och lägger till dem i result-listan.
    private static void ConvertPathToVector2Int(IEnumerable<int> path, IMapData mapData, List<Vector2Int> result)
    {
        foreach (int nodeIndex in path)
        {
            int x = nodeIndex % mapData.Width;
            int y = nodeIndex / mapData.Width;
            result.Add(new Vector2Int(x, y));
        }
    }


}
