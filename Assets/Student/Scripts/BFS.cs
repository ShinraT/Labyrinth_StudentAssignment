using System.Collections.Generic;
using UnityEngine;

public class BFS
{

    private bool[] marked;
    private int[] edgeTo;
    private int source;


    public BFS(Graph _graph, int _source)
    {
        marked = new bool[_graph.V()];
        edgeTo = new int[_graph.V()];
        this.source = _source;
        bfs(_graph, source);
        
    }

    private void bfs(Graph _graph, int _source)
    {
        Queue<int> _queue = new Queue<int>();
        marked[_source] = true;
        _queue.Enqueue(source);
        while (_queue.Count > 0)
        {
            int currentN = _queue.Dequeue();
            foreach (var nodeToVisit in _graph.Adj(currentN))
            {
                if (!marked[nodeToVisit])
                {
                    // w 
                    edgeTo[nodeToVisit] = currentN; // nuvarande node har en edge till nodeToVisit
                    marked[nodeToVisit] = true; // Vi sätter sedan den noden som är visited till marked. 
                    _queue.Enqueue(nodeToVisit); 
                    // Vi sätter in den noden i queuen för att upprepa samma process, på den som om den vore currentN. 
                }

            }
        }
    }

    public bool hasPathTo(int v)
    {
        return marked[v];
    }
     
    public IEnumerable<int> pathTo(int _goal)
    {
        if (!hasPathTo(_goal)) return null;
        Stack<int> path = new Stack<int>();

        for (int current = _goal; current != source; current = edgeTo[current])
            path.Push(current);
        path.Push(source);
        return path;
    }
    
}
