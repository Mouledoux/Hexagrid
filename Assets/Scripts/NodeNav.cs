using System.Collections;
using System.Collections.Generic;

public static class NodeNav
{
    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static Stack<TraversableNode> TwinStarII(TraversableNode begNode, TraversableNode endNode, bool dualSearch = false)
    {
        
        if(begNode == endNode || begNode == null || endNode == null ||
        !endNode.isTraversable || endNode.isOccupied) return null;

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
                    if(neighborNode.isTraversable == false) { continue; }


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
                        //else if(currentNode[i].parentNode != null &&
                        else if(neighborNode.safeParentNode != currentNode[i] &&
                            neighborNode.gValue < currentNode[i].parentNode.gValue)
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
        return NodePathStack(closedList[0]);
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static Stack<TraversableNode> NodePathStack(TraversableNode endNode)
    {
        Stack<TraversableNode> returnStack = new Stack<TraversableNode>();
        TraversableNode currentNode = endNode;

        TraversableNode.ValidateParentChain(currentNode);

        while(currentNode.parentNode != null)
        {
            returnStack.Push(currentNode);
            currentNode = currentNode.parentNode;
        }

        return returnStack;
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static int AddToSortedList(TraversableNode node, ref List<TraversableNode> sortedList)
    {
        for(int i = 0; i < sortedList.Count; i++)
        {
            if(node < sortedList[i])
            {
                sortedList.Insert(i, node);
                return i;
            }
        }

        sortedList.Add(node);
        return sortedList.Count;
    }
}