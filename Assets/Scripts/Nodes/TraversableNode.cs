using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversableNode : MonoBehaviour, ITraversable
{
    public TraversableNode()
    {
        m_nodeData = new Node();
        m_nodeData.AddInformation(this);
    }

    private Node m_nodeData;
    public Node nodeData
    {
        get => m_nodeData;
        set => m_nodeData = value;
    }

    private ITraversable m_origin;
    public ITraversable origin
    { 
        get
        {
            return m_origin == null ? this : m_origin;
        }
        set
        {
            m_origin = value;
            if(value == null) pathingValues[0] = 0f;
        }
    }

    private int[] m_coordinates = new int[2];
    public int[] coordinates
    {
        get => m_coordinates;
        set => m_coordinates = value;
    }

    private float m_travelCost;
    public float travelCost
    {
        get => m_travelCost;
        set => m_travelCost = value;
    }
    
    // 0 = F value
    // 1 = G value
    // 2 = H value
    private float[] m_pathingValues = new float[3];
    public float[] pathingValues
    {
        get
        {
            m_pathingValues[0] = m_pathingValues[1] + m_pathingValues[2];
            return m_pathingValues;
        }
        set
        {
            m_pathingValues = value;
        }
    }

    private bool m_isOccupied;
    public bool isOccupied
    {
        get => m_isOccupied;
        set => m_isOccupied = value;
    }

    private bool m_isTraversable;
    public bool isTraversable
    {
        get => m_isTraversable;
        set => m_isTraversable = value;
    }


    public ITraversable GetRootOrigin()
    {
        return origin == m_origin ? this : origin.GetRootOrigin();
    }

    public ITraversable[] GetConnectedTraversables()
    {
        List<ITraversable> tn = new List<ITraversable>();
        foreach(Node nd in m_nodeData.GetNeighbors())
        {
            tn.Add(nd.GetInformation<ITraversable>()[0]);
        }
        return tn.ToArray();
    }


    public bool CheckOriginChainFor(ITraversable higherOrigin)
    {
        ITraversable nextNode = this;

        ValidateOriginChain();

        do
        {
            if(nextNode == higherOrigin) return true;
            else nextNode = nextNode.origin;

        } while(nextNode.origin != nextNode);

        return false;
    }

    public void ReverseOriginChain()
    {
        ITraversable currentNode = this;
        ITraversable previousNode = null;
        ITraversable nextNode = null;

        do
        {
            nextNode = m_origin;
            currentNode.origin = previousNode;
            previousNode = currentNode;
            currentNode = nextNode;

        } while(currentNode != null);
    }

    public void ValidateOriginChain()
    {
        ITraversable iterator = this;
        List<ITraversable> originChain = new List<ITraversable>();

        while(iterator.origin != iterator)
        {
            if(originChain.Contains(iterator.origin))
            {
                iterator.origin = null;
            }
            else
            {
                originChain.Add(iterator);
                iterator = iterator.origin;
            }
        }
    }

    public float GetTravelCostToRootOrigin()
    {
        if(origin == m_origin) return 0;

        else return m_pathingValues[0] + origin.GetTravelCostToRootOrigin();
    }

    public double GetDistanceTo(ITraversable destination)
    {
        double lhs = (coordinates[0] - destination.coordinates[0]);
        double rhs = (coordinates[1] - destination.coordinates[1]);

         lhs *= lhs;
         rhs *= rhs;

        return System.Math.Sqrt(lhs + rhs);
    }

    public int CompareTo(ITraversable obj)
    {
        float dif = pathingValues[0] - obj.pathingValues[0];
        return dif == 0 ? 0 : dif < 0 ? -1 : 1;
    }
}