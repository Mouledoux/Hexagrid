﻿using System.Threading;
using System.Collections;
using System.Collections.Generic;

public static class NodeNav
{
    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static Stack<ITraversable> TwinStarT(ITraversable begNode, ITraversable endNode, bool dualSearch = true)
    {
        bool foundPath = false;

        if(dualSearch)
        {
            Thread backwards = new Thread(() => SoloStar(endNode, begNode, ref foundPath, false, 1f, 2f));
            backwards.Start();
        }

        return SoloStar(begNode, endNode, ref foundPath);
    }



    private static Stack<ITraversable> SoloStar(ITraversable begNode, ITraversable endNode, ref bool foundPath, bool canReturn = true, float hMod = 1f, float gMod = 1f)
    {
        if(begNode == endNode || begNode == null || endNode == null || !endNode.isTraversable) return null;


        List<ITraversable> openList = new List<ITraversable>();
        List<ITraversable> closedList = new List<ITraversable>();

        openList.Add(begNode);

        begNode.origin = null;
        ITraversable currentNode;

        while(!foundPath && openList.Count > 0)
        {
            currentNode = openList[0];

            foreach (ITraversable neighborNode in currentNode.GetConnectedTraversables())
            {
                if(neighborNode == null || neighborNode.isTraversable == false) { continue; }

                else if(neighborNode.CheckOriginChainFor(endNode))
                {
                    if(!canReturn) return null;

                    foundPath = true;
                    neighborNode.ReverseOriginChain();
                    neighborNode.origin = currentNode;
                    Stack<ITraversable> returnStack = TraversableStackPath(endNode);
                    
                    foreach(ITraversable tn in closedList)
                    {
                        tn.origin = null;
                    }

                    return returnStack;
                }

                else
                {
                    if(!closedList.Contains(neighborNode))
                    {
                        if(!openList.Contains(neighborNode))
                        {
                            neighborNode.origin = currentNode;
                            neighborNode.pathingValues[1] = neighborNode.GetTravelCostToRootOrigin() * gMod;
                            neighborNode.pathingValues[2] = neighborNode.GetDistanceTo(endNode) * hMod;

                            AddToSortedList<ITraversable>(neighborNode, ref openList);
                        }
                    }

                    // We have already been to this node, so see if it's cheaper to the current node from here
                    else if(neighborNode.origin != currentNode &&
                        neighborNode.pathingValues[1] < currentNode.pathingValues[1])
                    {
                        currentNode.origin = neighborNode;
                    }
                }
            }

            closedList.Add(currentNode);
            openList.Remove(currentNode);
        }

        return null;
    }


    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    [System.ObsoleteAttribute("TwinStar has been seperated into SoloStar, and TwinStarT")]
    public static Stack<TraversableNode> TwinStarII(TraversableNode begNode, TraversableNode endNode, bool dualSearch = false)
    {
        throw new System.NotSupportedException("TwinStar has been seperated into SoloStar, and TwinStarT");
        /*
        *
        *
        if(begNode == endNode || begNode == null || endNode == null || !endNode.isTraversable) return null;

        List<TraversableNode>[] openList = new List<TraversableNode>[] {new List<TraversableNode>(), new List<TraversableNode>()};
        List<TraversableNode> closedList = new List<TraversableNode>();

        TraversableNode[] currentNode = new TraversableNode[] {begNode, endNode};
        begNode.parentNode = endNode.parentNode = null;

        openList[0].Add(currentNode[0]);
        openList[1].Add(currentNode[1]);



        // As long as there are nodes to check
        while(openList[0].Count > 0 || openList[1].Count > 0)
        {
            // If dualSearch is enabled, the we will check from the start and end node until the 2 meet
            for(int i = 0; i < 2; i += dualSearch ? 1 : 0)
            {
                // For each of the neighbor nodes of our current node
                foreach(TraversableNode neighborNode in currentNode[i].GetNeighbors())
                {
                    // If the neighbor cannot be traversed,
                    // move to the next one
                    if(neighborNode == null || neighborNode.isTraversable == false) { continue; }


                    // If it CAN be traversed AND it's root parent is the goal node,
                    // return the path
                    else if(i == 0 && neighborNode.CheckForNodeInParentChain(endNode))
                    {
                        TraversableNode.ReverseParents(neighborNode);
                        neighborNode.parentNode = currentNode[0];
                        return NodePathStack(endNode);
                    }


                    // Else, the node IS traversable, and NOT connected to the goal node
                    else
                    {
                        // Check if the node has already been traversed, or is on the list to be checked,
                        // and add it to the list if it needs to be
                        if(!closedList.Contains(neighborNode))
                        {
                            if(!openList[i].Contains(neighborNode))
                            {
                                neighborNode.parentNode = currentNode[i];
                                neighborNode.hValue = TraversableNode.Distance(neighborNode, endNode);
                                neighborNode.gValue = neighborNode.GetGValue(i==1);

                                AddToSortedList(neighborNode, ref openList[i]);
                            }
                        }

                        // If the neighbor's G value is less than the parent's,
                        // reparent the current node to the neighbor
                        else if(neighborNode.safeParentNode != currentNode[i] &&
                            neighborNode.gValue < currentNode[i].safeParentNode.gValue)
                        {
                            currentNode[i].parentNode = neighborNode;
                        }
                    }
                }

                AddToSortedList(currentNode[i], ref closedList);
                currentNode[i] = openList[i][0];
                openList[i].Remove(currentNode[i]);
            }
        }

        // A path could not be found, so return an empty path stack
        return NodePathStack(begNode);
        *
        *
        */
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private static Stack<ITraversable> TraversableStackPath(ITraversable endNode)
    {
        Stack<ITraversable> returnStack = new Stack<ITraversable>();
        ITraversable currentNode = endNode;

        currentNode.ValidateOriginChain();

        while(currentNode != null && currentNode.origin != currentNode)
        {
            try
            {
                returnStack.Push(currentNode);
                currentNode = currentNode.origin;
            }
            catch(System.OutOfMemoryException)
            {
                UnityEngine.Debug.Log(returnStack.Count);
            }
        }

        return returnStack;
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private static int AddToSortedList<T>(T node, ref List<T> sortedList) where T : System.IComparable<T>
    {
        for(int i = 0; i < sortedList.Count; i++)
        {
            if(node.CompareTo(sortedList[i]) < 0)
            {
                sortedList.Insert(i, node);
                return i;
            }
        }

        sortedList.Add(node);
        return sortedList.Count;
    }
}





public interface ITraversable : System.IComparable<ITraversable>
{
    ITraversable origin {get; set;}
    int[] coordinates {get; set;}
    float travelCost {get; set;}
    float[] pathingValues {get; set;}

    bool isOccupied {get; set;}
    bool isTraversable {get; set;}


    ITraversable GetRootOrigin();
    ITraversable[] GetConnectedTraversables();

    void ReverseOriginChain();
    void ValidateOriginChain();
    bool CheckOriginChainFor(ITraversable higherOrigin);

    float GetTravelCostToRootOrigin();
    float GetDistanceTo(ITraversable destination);
}