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

    public int _xCoord, _yCoord;

    private float _travelCost;
    public float travelCost
    {
        get { return _travelCost; }
        set { _travelCost = value; }
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

    public float fValue => (_hValue + _gValue);


    public float GetNeighboorTravelCost(TraversableNode aNode, bool invert = false)
    {
        return  !invert ? 
        CheckIsNeighbor(aNode) ? (this.travelCost - aNode.travelCost) : float.MaxValue :
        aNode.GetNeighboorTravelCost(this, false);
    }

    public static float Distance(TraversableNode aNode, TraversableNode bNode)
    {
        float a = Mathf.Abs(aNode._xCoord - bNode._xCoord);
        float b = Mathf.Abs(aNode._yCoord - bNode._yCoord);

        // a *= a;
        // b *= b;

        return (a + b);
    }


    public float GetGValue()
    {
        if(_parentNode == null)
            return 0f;
        
        else
            return GetNeighboorTravelCost(_parentNode) + _parentNode._gValue;
    }

    public static bool operator >(TraversableNode lhs, TraversableNode rhs)
    {
        return lhs.fValue > rhs.fValue;
    }
    public static bool operator <(TraversableNode lhs, TraversableNode rhs)
    {
        return lhs.fValue < rhs.fValue;
    }
}