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
        neighborhood.Remove(this);
        return neighborhood.ToArray();
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public Node[] GetNeighborhoodLayers(int innerBand, int bandWidth = 1)
    {
        innerBand--;
        Node[] n1 = GetNeighborhood(innerBand + bandWidth);
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