using System.Collections;
using System.Collections.Generic;

public class Node<T>
{
    private T m_nodeType;
    public T nodeType => m_nodeType;

    private List<Node<T>> m_neighbors = new List<Node<T>>();
    private List<object> m_information = new List<object>();

    private Node() {}
    public Node(T node)
    {
        m_nodeType = node;
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public Node<T>[] GetNeighbors()
    {
        Node<T>[] myNeighbors = m_neighbors.ToArray();
        return myNeighbors;
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public Node<T>[] GetNeighborhood(uint a_layers = 1)
    {   
        int index = 0;
        int neighbors = 0;

        List<Node<T>> neighborhood = new List<Node<T>>();
        neighborhood.Add(this);

        for(int i = 0; i < a_layers; i++)
        {
            neighbors = neighborhood.Count;
            for(int j = index; j < neighbors; j++)
            {
                foreach(Node<T> n in neighborhood[j].GetNeighbors())
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
    public Node<T>[] GetNeighborhoodLayers(uint a_innerBand, uint a_bandWidth = 1)
    {
        //innerBand--;
        Node<T>[] n1 = GetNeighborhood(a_innerBand + a_bandWidth - 1);
        Node<T>[] n2 = GetNeighborhood(a_innerBand);

        List<Node<T>> neighborhood = new List<Node<T>>();

        foreach (Node<T> n in n1)
            neighborhood.Add(n);

        foreach (Node<T> n in n2)
            neighborhood.Remove(n);
        
        return neighborhood.ToArray();
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public int AddNeighbor(Node<T> a_newNeighbor)
    {
        if(a_newNeighbor == null)
            return -1;
        else if(a_newNeighbor == this)
            return -2;

        if(!m_neighbors.Contains(a_newNeighbor))
            m_neighbors.Add(a_newNeighbor);

        if(!a_newNeighbor.m_neighbors.Contains(this))
            a_newNeighbor.AddNeighbor(this);

        return 0;
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public bool CheckIsNeighbor(Node<T> a_node)
    {
        return m_neighbors.Contains(a_node);
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public int RemoveNeighbor(Node<T> a_oldNeighbor)
    {
        m_neighbors.Remove(a_oldNeighbor);
        a_oldNeighbor.RemoveNeighbor(this);

        return 0;
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public int ClearNeighbors()
    {
        foreach(Node<T> n in m_neighbors)
            n.RemoveNeighbor(this);
        
        m_neighbors.Clear();

        return 0;
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public int TradeNeighbors(Node<T> a_neighbor)
    {
        if(a_neighbor == null) return -1;
        else if(!m_neighbors.Contains(a_neighbor)) return -2;

        Node<T>[] myNeighbors;


        // // Remove eachother as neighbors, so they aren't neighbors to themselves
        // RemoveNeighbor(a_neighbor);
        // a_neighbor.RemoveNeighbor(this);

        // Save this node neighbors to a temp array
        myNeighbors = m_neighbors.ToArray();
    

        ClearNeighbors();                               // Clear this node's neighbors
        foreach(Node<T> n in a_neighbor.GetNeighbors())    // For each neighbor of my neighbor
            AddNeighbor(n);                                 // Copy it to this node's neighbors
        AddNeighbor(a_neighbor);                        // Add the neighbor back to this node's neighbors


        a_neighbor.ClearNeighbors();                    // Clear the neighbor's neighbors
        foreach(Node<T> n in myNeighbors)                  // For each node in the temp array
            a_neighbor.AddNeighbor(n);                        // Copy it to the neighbor's new neighbors
        a_neighbor.AddNeighbor(this);                   // Add this node back to the neighbor's neighbors

        return 0;
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public int AddInformation(object a_info)
    {
        m_information.Add(a_info);
        return 0;
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public int RemoveInformation(object a_info)
    {
        if(m_information.Contains(a_info))
            m_information.Remove(a_info);

        return 0;
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public bool CheckInformationFor(object a_info)
    {
        return m_information.Contains(a_info);
    }

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public U[] GetInformation<U>()
    {
        List<U> returnList = new List<U>();

        foreach(object info in m_information)
        {
            if(info.GetType() == typeof(U))
            {
                returnList.Add((U)info);
            }
        }
        return returnList.ToArray();
    }
}
