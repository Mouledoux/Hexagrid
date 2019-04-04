using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    private List<Node> _neighbors = new List<Node>();
    private Renderer _renderer;
    private Material _defaultMaterial;
    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _defaultMaterial = _renderer.sharedMaterial;
    }

    public void ResetMaterial()
    {
        _renderer.material = _defaultMaterial;
    }


    public Node[] GetNeighbors()
    {
        Node[] myNeighbors = _neighbors.ToArray();
        return myNeighbors;
    }

    public Node[] GetNeighborhood(int layers = 1)
    {   
        int index = 0;
        int neighbors = 0;

        List<Node> neighborhood = new List<Node>();
        neighborhood.Add(this);

        for(int i = 0; i < layers; i++)
        {
            neighbors = neighborhood.Count;
            for(int j = index; j < neighbors; j++)
            {
                foreach(Node n in neighborhood[j].GetNeighbors())
                {
                    if(!neighborhood.Contains(n))
                    {
                        neighborhood.Add(n);
                    }
                }
                index = j;
            }
        }
        return neighborhood.ToArray();
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

    public bool CheckIsNeighbor(Node aNode)

    {
        return _neighbors.Contains(aNode);
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
}