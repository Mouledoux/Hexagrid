using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    private Vector2 _coordinates = Vector2.zero;
    public Vector2 coordinates => _coordinates;

    private float _value = 0f;
    public float value => _value;

    private List<Node> _neighbors = new List<Node>();



    public Node[] GetNeighbors()
    {
        Node[] myNeighbors = _neighbors.ToArray();
        return myNeighbors;
    }
    public int AddNeighbor(Node newNeighbor)
    {
        if(newNeighbor == null)
            return -1;

        if(!_neighbors.Contains(newNeighbor))
            _neighbors.Add(newNeighbor);

        if(!newNeighbor._neighbors.Contains(this))
            newNeighbor.AddNeighbor(this);

        return 0;
    }


    public int ClearNeighbors()
    {
        foreach(Node n in _neighbors)
            n.RemoveNeighbor(this);
        
        _neighbors.Clear();

        return 0;
    }
    public int RemoveNeighbor(Node oldNeighbor)
    {
        if(_neighbors.Contains(oldNeighbor))
        {
            _neighbors.Remove(oldNeighbor);
            oldNeighbor.RemoveNeighbor(this);
        }

        return 0;
    }



    public static bool operator <(Node lhs, Node rhs)
    {
        return lhs.value < rhs.value;
    }
    public static bool operator >(Node lhs, Node rhs)
    {
        return lhs.value > rhs.value;
    }
}