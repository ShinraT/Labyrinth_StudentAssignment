using UnityEngine;

public class DirectedEdge
{

    public int From { get; }
    public int To { get; }
    public double Weight { get; }

    public DirectedEdge(int _from, int _to, double _weight)
    {
        this.From = _from;
        this.To = _to;
        this.Weight = _weight;

    }

}
