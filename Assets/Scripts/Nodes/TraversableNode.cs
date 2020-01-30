using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversableNode : Node
{
    private TraversableNode _parentNode;
    public TraversableNode parentNode
    {
        get { return _parentNode; }
        set {_parentNode = value; }
    }
    public TraversableNode safeParentNode => (parentNode == null ? this : parentNode);

    private bool _isOccupied;
    public bool isOccupied
    {
        get { return _isOccupied; }
        set
        {
            _isOccupied = value;
            if(_isOccupied) onOccupy.Invoke();
        }
    }

    public int xCoord, yCoord;

    private float _travelCost;
    public float travelCost
    {
        get { return _travelCost; }
        set { _travelCost = value; }
    }

    private bool _isTraversable;
    public bool isTraversable
    {
        get { return _isTraversable && !isOccupied; }
        set { _isTraversable = value; }
    }

    private float _hValue;
    public float hValue
    {
        get { return _hValue; }
        set { _hValue = value; }
    }

    private float _gValue;
    public float gValue
    {
        get { return _gValue; }
        set { _gValue = value; }
    }

    public float fValue => (hValue + gValue);


    public UnityEngine.Events.UnityEvent onOccupy;


    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public float GetNeighboorTravelCost(TraversableNode a_node, bool invert = false)
    {
        return !invert ? 
        CheckIsNeighbor(a_node) ? (this.travelCost - a_node.travelCost) : float.MaxValue :
        a_node.GetNeighboorTravelCost(this, false);
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static float Distance(TraversableNode a_node1, TraversableNode a_node2)
    {
        float lhs = (a_node1.xCoord - a_node2.xCoord);
        float rhs = (a_node1.yCoord - a_node2.yCoord);

         lhs *= lhs;
         rhs *= rhs;

        return Mathf.Sqrt(lhs + rhs);

        // float a = Mathf.Abs(aNode.xCoord - bNode.xCoord);
        // float b = Mathf.Abs(aNode.yCoord - bNode.yCoord);

        // return (a + b);
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public float GetGValue(bool invert = false)
    {
        if(parentNode == null)
            return 0f;
        
        else
            return GetNeighboorTravelCost(parentNode, invert) + parentNode.gValue;
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public bool CheckForNodeInParentChain(TraversableNode a_targetNode)
    {
        if(this == a_targetNode) return true;

        TraversableNode tNode = null;


        if(this != null && ValidateParentChain(this))
        {
            tNode = this;

            while(tNode.parentNode != null && tNode.parentNode != this)
            {
                if(tNode.parentNode == a_targetNode) return true;

                else tNode = tNode.parentNode;
            }
        }        


        return false;
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static bool ValidateParentChain(TraversableNode a_childNode)
    {
        if(a_childNode == null) return false;

        List<TraversableNode> parents = new List<TraversableNode>();
    

        while(a_childNode != null && a_childNode.parentNode != null)
        {
            if(parents.Contains(a_childNode.parentNode))
            {
                a_childNode.parentNode = null;
                return true;
            }
            
            else
            {
                parents.Add(a_childNode.parentNode);
                a_childNode = a_childNode.parentNode;
            }
        }

        return false;
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static void ReverseParents(TraversableNode a_currentNode)
    {
        TraversableNode previousNode = null;
        TraversableNode nextNode = null;


        do
        {
            nextNode = a_currentNode.parentNode;
            a_currentNode.parentNode = previousNode;
            previousNode = a_currentNode;
            a_currentNode = nextNode;

        } while(a_currentNode != null);
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private static TraversableNode GetRootParent(TraversableNode a_node)
    {
        return (a_node.parentNode == null ? a_node : GetRootParent(a_node.parentNode));
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static bool operator >(TraversableNode lhs, TraversableNode rhs)
    {
        return lhs.fValue > rhs.fValue;
    }
    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static bool operator <(TraversableNode lhs, TraversableNode rhs)
    {
        return lhs.fValue < rhs.fValue;
    }
}




public class NewTraversableNode : Node, ITraversable
{
    private ITraversable m_originNode;
    public ITraversable origin
    { 
        get
        {
            return m_originNode == null ? this : m_originNode;
        }
        set
        {
            m_originNode = value;
            if(value == null) pathingValues[0] = 0f;
        }
    }

    private int[] m_coordinates = new int[2];
    public int[] coordinates
    {
        get { return m_coordinates; }
        set { m_coordinates = value; }
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
        return origin == null ? this : origin.GetRootOrigin();
    }

    public ITraversable[] GetConnectedTraversables()
    {
        return GetNeighbors() as ITraversable[];
    }


    public bool CheckOriginChainFor(ITraversable higherOrigin)
    {
        ITraversable nextNode = this;

        ValidateOriginChain();

        while(nextNode != null)
        {
            if(nextNode == higherOrigin) return true;
            else nextNode = nextNode.origin;
        }

        return false;
    }

    public void ReverseOriginChain()
    {
        ITraversable currentNode = this;
        ITraversable previousNode = null;
        ITraversable nextNode = null;

        do
        {
            nextNode = currentNode.origin;
            currentNode.origin = previousNode;
            previousNode = currentNode;
            currentNode = nextNode;

        } while(currentNode != null);
    }

    public void ValidateOriginChain()
    {
        ITraversable iterator = this;
        List<ITraversable> originChain = new List<ITraversable>();

        while(iterator.origin != null)
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
        if(origin == null) return 0;

        else return m_pathingValues[0] + origin.GetTravelCostToRootOrigin();
    }

    public float GetDistanceTo(ITraversable destination)
    {
        float lhs = (coordinates[0] - destination.coordinates[0]);
        float rhs = (coordinates[1] - destination.coordinates[1]);

         lhs *= lhs;
         rhs *= rhs;

        return Mathf.Sqrt(lhs + rhs);
    }

    public int CompareTo(ITraversable obj)
    {
        float dif = pathingValues[0] - obj.pathingValues[0];
        return dif == 0 ? 0 : dif < 0 ? -1 : 1;
    }
}