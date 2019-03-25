using System.Collections;
using System.Collections.Generic;

public class Node<T>
{
    private T _value;
    public T value => _value;

    private List<Node<T>> _neighbors = new List<Node<T>>();





    public Node<T>[] GetNeighbors()
    {
        Node<T>[] myNeighbors = _neighbors.ToArray();
        return myNeighbors;
    }

    public int AddNeighbor(Node<T> newNeighbor)
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
        foreach(Node<T> n in _neighbors)
            n.RemoveNeighbor(this);
        
        _neighbors.Clear();

        return 0;
    }

    public int RemoveNeighbor(Node<T> oldNeighbor)
    {
        if(_neighbors.Contains(oldNeighbor))
        {
            _neighbors.Remove(oldNeighbor);
            oldNeighbor.RemoveNeighbor(this);
        }

        return 0;
    }
}