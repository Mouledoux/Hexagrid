using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class NodePath : MonoBehaviour
{
    public bool Visualize, twinStar;
    public Material current, open, closed, reparent, reparented;
    public TraversableNode _startNode, _endNode;

    [Range(0.01f, 1f)]
    public static float hWeight, gWeight;

    public Stack<TraversableNode> path = new Stack<TraversableNode>();

    
    [ContextMenu("TwinStar")]
    public void BeginTwinStar()
    {
        StartCoroutine(TwinStar(_startNode, _endNode, twinStar));
    }

    private void Update()
    {
        RaycastHit hit;
        
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            TraversableNode tn = hit.collider.GetComponent<TraversableNode>();
            if(tn == null) return;

            else if(Input.GetMouseButtonDown(1))
            {
                if(tn == _startNode) _startNode = null;
                else
                {
                    StopAllCoroutines();
                    if(_startNode != null) _startNode.ResetMaterial();

                    _startNode = tn;
                    _startNode.GetComponent<Renderer>().material = current;
                }
            }

            else if(Input.GetMouseButtonDown(0))
            {
                if(tn == _endNode) return;
                else
                {
                    _endNode = tn;

                    if(_startNode != null && _endNode != null)
                    {
                        BeginTwinStar();
                    }
                }
            }
        }
    }

    public IEnumerator AStar()
    {
        if(_startNode == _endNode) yield break;

        while(path.Count > 0)
        {
            path.Pop().ResetMaterial();
        }

        List<TraversableNode> closedList = new List<TraversableNode>();
        List<TraversableNode> openList = new List<TraversableNode>();

        TraversableNode currentNode;
        currentNode = _startNode;
        openList.Add(currentNode);

        while(openList.Count > 0)
        {
            bool rp = false;
            foreach(TraversableNode node in currentNode.GetNeighbors())
            {
                if(node.isTraversable == false) { continue; }
                print(node.isTraversable);

                if(!closedList.Contains(node))
                {
                    if(!openList.Contains(node))
                    {
                        node.parentNode = currentNode;
                        node.hValue = TraversableNode.Distance(node, _endNode);// / hWeight;
                        node.gValue = node.GetGValue();// / gWeight;

                        AddToSortedList(node, ref openList);

                        if(Visualize)
                            node.GetComponent<Renderer>().material = open;
                    }
                }

                if(node.gValue < currentNode.safeParentNode.gValue)
                {
                    if(node.parentNode != currentNode)
                    {
                        currentNode.parentNode = node;
                        node.GetComponent<Renderer>().material = reparented;
                        rp = true;
                    }
                }

                if(node == _endNode)
                {
                    currentNode = node;
                    while(currentNode != null)
                    {
                        path.Clear();
                        path = NodePathStack(_endNode);
                        closedList.Remove(currentNode);
                        currentNode.GetComponent<Renderer>().material = current;
                        currentNode = currentNode.parentNode;

                        if(Visualize)
                            yield return null;
                    }

                    while(closedList.Count > 0)
                    {
                        closedList[0].ResetMaterial();
                        closedList.RemoveAt(0);
                    }
                    
                    while(openList.Count > 0)
                    {
                        openList[0].ResetMaterial();
                        openList.RemoveAt(0);
                    }
                    
                    

                    print($"Distance: {path.Count}, Cost: {_endNode.gValue * gWeight}");
                    yield break;
                }
            }

            closedList.Add(currentNode);

            if(Visualize)
                currentNode.GetComponent<Renderer>().material = rp ? reparent : closed;

            currentNode = openList[0];

            if(Visualize)
                currentNode.GetComponent<Renderer>().material = current;

            openList.Remove(currentNode);

            if(Visualize) yield return null;
        }

        yield return null;
    }


    public IEnumerator TwinStar(TraversableNode begNode, TraversableNode endNode, bool dualSearch = false)
    {
        List<TraversableNode>[] openLists = new List<TraversableNode>[] {new List<TraversableNode>(), new List<TraversableNode>()};
        List<TraversableNode> closedList = new List<TraversableNode>();

        TraversableNode[] currentNode = new TraversableNode[] {begNode, endNode};
        begNode.parentNode = endNode.parentNode = null;

        openLists[0].Add(currentNode[0]);
        openLists[1].Add(currentNode[1]);

        bool rp = false;
        int cycles = 0;

        while(openLists[0].Count > 0 && openLists[1].Count > 0)
        {
            for(int i = 0, j = (i+1)%2; i < 2; i += dualSearch ? 1 : 0)
            {
                foreach(TraversableNode neighborNode in currentNode[i].GetNeighbors())
                {
                    cycles++;
                    if(neighborNode.isTraversable == false) { continue; }

                    // If the 2 paths have overlapped
                    if(i == 0 && neighborNode.CheckForNodeInParentChain(_endNode))
                    {
                        TraversableNode tNode = _endNode;

                        TraversableNode.ReverseParents(neighborNode);
                        neighborNode.parentNode = currentNode[0];

                        if(Visualize)
                        {
                            foreach(TraversableNode tn in closedList)
                                tn.ResetMaterial();
                            foreach(TraversableNode tn in openLists[0])
                                tn.ResetMaterial();
                            foreach(TraversableNode tn in openLists[1])
                                tn.ResetMaterial();

                            while(tNode != null)
                            {
                                tNode.GetComponent<Renderer>().material = current;
                                tNode = tNode.parentNode;
                            }
                        }
                        print(cycles);
                        yield break;
                    }


                    else
                    {
                        if(!closedList.Contains(neighborNode))
                        {
                            if(!openLists[i].Contains(neighborNode))
                            {
                                neighborNode.parentNode = currentNode[i];
                                neighborNode.hValue = TraversableNode.Distance(neighborNode, endNode);
                                neighborNode.gValue = 0.1f + neighborNode.GetGValue(i==1);

                                AddToSortedList(neighborNode, ref openLists[i]);

                                if(Visualize) neighborNode.GetComponent<Renderer>().material = open;
                            }
                        }
                        
                        else if(currentNode[i].parentNode != null &&
                            neighborNode.gValue < currentNode[i].parentNode.gValue &&
                                neighborNode.parentNode != currentNode[i])
                        {
                            rp = true;
                            currentNode[i].parentNode = neighborNode;
                            if(Visualize) neighborNode.GetComponent<Renderer>().material = reparented;
                        }
                    }
                }

                closedList.Add(currentNode[i]);
                if(Visualize) currentNode[i].GetComponent<Renderer>().material = rp ? reparent : closed;
                rp = false;

                currentNode[i] = openLists[i][0];
                openLists[i].Remove(currentNode[i]);
                if(Visualize)
                {
                    currentNode[i].GetComponent<Renderer>().material = current;
                    yield return null;
                }
            }
        }
        yield return null;
    }


    public static Stack<TraversableNode> TwinStarII(TraversableNode begNode, TraversableNode endNode, bool dualSearch = false)
    {
        List<TraversableNode>[] openList = new List<TraversableNode>[] {new List<TraversableNode>(), new List<TraversableNode>()};
        List<TraversableNode> closedList = new List<TraversableNode>();

        TraversableNode[] currentNode = new TraversableNode[] {begNode, endNode};
        begNode.parentNode = endNode.parentNode = null;

        openList[0].Add(currentNode[0]);
        openList[1].Add(currentNode[1]);

        // As long as there are nodes to check
        while(openList[0].Count > 0 && openList[1].Count > 0)
        {
            // If dualSearch is enabled, the we will check from the start and end node until the 2 meet
            for(int i = 0, j = (i+1)%2; i < 2; i += dualSearch ? 1 : 0)
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
                        if(!closedList.Contains(neighborNode) & !openList[i].Contains(neighborNode))
                        {
                            neighborNode.parentNode = currentNode[i];
                            neighborNode.hValue = TraversableNode.Distance(neighborNode, endNode);
                            neighborNode.gValue = 0.1f + neighborNode.GetGValue(i==1);

                            AddToSortedList(neighborNode, ref openList[i]);
                        }
                        

                        // If the neighbor's G value is less than the parent's,
                        // reparent the current node to the neighbor
                        else if(currentNode[i].parentNode != null &&
                            neighborNode.gValue < currentNode[i].parentNode.gValue &&
                                neighborNode.parentNode != currentNode[i])
                        {
                            currentNode[i].parentNode = neighborNode;
                        }
                    }
                }

                closedList.Add(currentNode[i]);
                currentNode[i] = openList[i][0];
                openList[i].Remove(currentNode[i]);
            }
        }

        // A path could not be found, so return an empty path stack
        return new Stack<TraversableNode>();
    }


    public static Stack<TraversableNode> NodePathStack(TraversableNode endNode)
    {
        Stack<TraversableNode> returnStack = new Stack<TraversableNode>();

        TraversableNode currentNode = endNode;

        while(currentNode.parentNode != null)
        {
            returnStack.Push(currentNode);
            currentNode = currentNode.parentNode;
        }

        return returnStack;
    }

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