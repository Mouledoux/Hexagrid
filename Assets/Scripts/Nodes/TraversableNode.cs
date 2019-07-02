﻿using System.Collections;
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
    public float GetNeighboorTravelCost(TraversableNode aNode, bool invert = false)
    {
        return !invert ? 
        CheckIsNeighbor(aNode) ? (this.travelCost - aNode.travelCost) : float.MaxValue :
        aNode.GetNeighboorTravelCost(this, false);
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static float Distance(TraversableNode aNode, TraversableNode bNode)
    {
        float a = (aNode.xCoord - bNode.xCoord);
        float b = (aNode.yCoord - bNode.yCoord);

         a *= a;
         b *= b;

        return Mathf.Sqrt(a + b);

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
            return GetNeighboorTravelCost(parentNode, invert) + _parentNode.gValue;
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public bool CheckForNodeInParentChain(TraversableNode targetNode)
    {
        if(this == targetNode) return true;


        if(this != null && ValidateParentChain(this))
        {
            TraversableNode tNode = this;

            while(tNode.parentNode != null && tNode.parentNode != this)
            {
                if(tNode.parentNode == targetNode) return true;

                else tNode = tNode.parentNode;
            }
        }        


        return false;
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static bool ValidateParentChain(TraversableNode aNode)
    {
        if(aNode == null) return false;

        List<TraversableNode> parents = new List<TraversableNode>();

        while(aNode.parentNode != null)
        {
            if(parents.Contains(aNode.parentNode))
            {
                aNode.parentNode = null;
                return true;
            }
            
            else
            {
                parents.Add(aNode.parentNode);
                aNode = aNode.parentNode;
            }
        }

        return false;
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static void ReverseParents(TraversableNode currentNode)
    {
        TraversableNode previousNode = null;
        TraversableNode nextNode = null;

        do
        {
            nextNode = currentNode.parentNode;
            currentNode.parentNode = previousNode;
            previousNode = currentNode;
            currentNode = nextNode;

        } while(currentNode != null);
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private static TraversableNode GetRootParent(TraversableNode aNode)
    {
        return (aNode.parentNode == null ? aNode : GetRootParent(aNode.parentNode));
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static bool operator >(TraversableNode lhs, TraversableNode rhs)
    {
        return lhs.fValue > rhs.fValue;
    }
    public static bool operator <(TraversableNode lhs, TraversableNode rhs)
    {
        return lhs.fValue < rhs.fValue;
    }
}