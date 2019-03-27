using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversableNode : Node
{
    public TraversableNode _parentNode;

    public int _xCoord, _yCoord;

    public float _travelCost;
    public float _hValue;
    public float _gValue;
    public float _fValue => (_hValue + _gValue);


    public float GetNeighboorTravelCost(TraversableNode aNode)
    {
        return CheckIsNeighbor(aNode) ? (this._travelCost - aNode._travelCost) : float.MaxValue;
    }

    public static int Distance(TraversableNode aNode, TraversableNode bNode)
    {
        int a = Mathf.Abs(aNode._xCoord - bNode._xCoord);
        int b = Mathf.Abs(aNode._yCoord - bNode._yCoord);

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
        return lhs._fValue > rhs._fValue;
    }
    public static bool operator <(TraversableNode lhs, TraversableNode rhs)
    {
        return lhs._fValue < rhs._fValue;
    }
}