﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversableNode : Node
{
    public TraversableNode _parentNode;

    public int _xCoord, _yCoord;

    public float _travelCost;
    public float _hValue;
    public float _gValue => GetGValue();
    public float _fValue => (_hValue + _gValue);


    public float GetNeighboorTravelCost(TraversableNode aNode)
    {
        return CheckIsNeighbor(aNode) ? (aNode._travelCost - this._travelCost) : float.MaxValue;
    }

    public static float Distance(TraversableNode aNode, TraversableNode bNode)
    {
        float a = aNode._xCoord - bNode._xCoord;
        float b = aNode._yCoord - bNode._yCoord;

        a *= a;
        b *= b;

        return Mathf.Sqrt(a + b);
    }


    private float GetGValue()
    {
        if(_parentNode == null)
            return 0f;
        
        else
            return _travelCost + _parentNode._gValue;
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