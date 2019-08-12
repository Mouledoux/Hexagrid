using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    private List<Node> _neighbors = new List<Node>();
    private Renderer _renderer;
    private Material _defaultMaterial;

    private List<object> _information = new List<object>();

    
    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _defaultMaterial = _renderer.sharedMaterial;
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public void ResetMaterial()
    {
        _renderer.sharedMaterial = _defaultMaterial;
    }    

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public void SetMaterialColor(Color color)
    {
        _renderer.material.color = color;
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public Node[] GetNeighbors()
    {
        Node[] myNeighbors = _neighbors.ToArray();
        return myNeighbors;
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public Node[] GetNeighborhood(uint layers = 1)
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
        neighborhood.Remove(this);
        return neighborhood.ToArray();
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public Node[] GetNeighborhoodLayers(uint innerBand, uint bandWidth = 1)
    {
        //innerBand--;
        Node[] n1 = GetNeighborhood(innerBand + bandWidth - 1);
        Node[] n2 = GetNeighborhood(innerBand);

        List<Node> neighborhood = new List<Node>();

        foreach (Node n in n1)
            neighborhood.Add(n);

        foreach (Node n in n2)
            neighborhood.Remove(n);
        
        return neighborhood.ToArray();
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
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

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public bool CheckIsNeighbor(Node aNode)
    {
        return _neighbors.Contains(aNode);
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public int RemoveNeighbor(Node oldNeighbor)
    {
        if(_neighbors.Contains(oldNeighbor))
        {
            _neighbors.Remove(oldNeighbor);
            oldNeighbor.RemoveNeighbor(this);
        }

        return 0;
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public int ClearNeighbors()
    {
        foreach(Node n in _neighbors)
            n.RemoveNeighbor(this);
        
        _neighbors.Clear();

        return 0;
    }

    public int TradeNeighbors(Node neighbor)
    {
        if(neighbor == null) return -1;
        else if(!_neighbors.Contains(neighbor)) return -2;

        // Remove eachother as neighbors, so they aren't neighbors to themselves
        RemoveNeighbor(neighbor);
        neighbor.RemoveNeighbor(this);

        // Save this node neighbors to a temp array
        Node[] myNeighbors = _neighbors.ToArray();
    

        ClearNeighbors();                               // Clear this node's neighbors
        foreach(Node n in neighbor.GetNeighbors())      // For each neighbor of my neighbor
            AddNeighbor(n);                                 // Copy it to this node's neighbors
        AddNeighbor(neighbor);                          // Add the neighbor back to this node's neighbors


        neighbor.ClearNeighbors();                      // Clear the neighbor's neighbors
        foreach(Node n in myNeighbors)                  // For each node in the temp array
            neighbor.AddNeighbor(n);                        // Copy it to the neighbor's new neighbors
        neighbor.AddNeighbor(this);                     // Add this node back to the neighbor's neighbors

        return 0;
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public int AddInformation(object info)
    {
        _information.Add(info);
        return 0;
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public int RemoveInformation(object info)
    {
        if(_information.Contains(info))
            _information.Remove(info);

        return 0;
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public bool CheckInformationFor(object info)
    {
        return _information.Contains(info);
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public T[] GetInformation<T>()
    {
        List<T> returnList = new List<T>();

        foreach(object info in _information)
        {
            if(info.GetType() == typeof(T))
            {
                returnList.Add((T)info);
            }
        }
        return returnList.ToArray();
    }
}