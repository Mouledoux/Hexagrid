using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class NodePath : MonoBehaviour
{
    public bool Visualize;
    public Material current, open, closed, reparent, reparented;
    public TraversableNode _startNode, _endNode;

    [Range(0.01f, 1f)]
    public float gWeight, hWeight;


    public Stack<TraversableNode> path = new Stack<TraversableNode>();

    [ContextMenu("Get Path")]
    public void BeginAStar()
    {
        StartCoroutine(AStar());
    }
    
    [ContextMenu("TwinStar")]
    public void BeginTwinStar()
    {
        StartCoroutine(TwinStar(_startNode, _endNode));
    }

    private void Update()
    {
        // RaycastHit hit;
        
        // if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        // {
        //     TraversableNode tn = hit.collider.GetComponent<TraversableNode>();
        //     if(tn == null) return;

        //     else if(Input.GetMouseButtonDown(1))
        //     {
        //         if(tn == _startNode) _startNode = null;
        //         else
        //         {
        //             StopAllCoroutines();
        //             if(_startNode != null) _startNode.ResetMaterial();

        //             _startNode = tn;
        //             _startNode.GetComponent<Renderer>().material = current;
        //         }
        //     }

        //     else if(Input.GetMouseButtonDown(0))
        //     {
        //         if(tn == _endNode) return;
        //         else
        //         {
        //             _endNode = tn;

        //             if(_startNode != null && _endNode != null)
        //             {
        //                 BeginAStar();
        //             }
        //         }
        //     }
        // }
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
                if(!closedList.Contains(node))
                {
                    if(!openList.Contains(node))
                    {
                        node.parentNode = currentNode;
                        node.hValue = TraversableNode.Distance(node, _endNode);// / hWeight;
                        node.gValue = 1f + node.GetGValue();// / gWeight;

                        AddToSortedList(node, ref openList);
                        if(Visualize)
                            node.GetComponent<Renderer>().material = open;
                    }
                }

                if(node.gValue < currentNode.safeParentNode.gValue)
                {
                    if(node.parentNode == currentNode) break;

                    currentNode.parentNode = node;
                    node.GetComponent<Renderer>().material = reparented;
                    rp = true;
                }

                if(node == _endNode)
                {
                    currentNode = node;
                    while(currentNode != null)
                    {
                        path.Clear();
                        path = NodeStackPath(_endNode);
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


    public IEnumerator TwinStar(TraversableNode begNode, TraversableNode endNode)
    {
        List<TraversableNode>[] openLists = {new List<TraversableNode>(), new List<TraversableNode>()};
        List<TraversableNode>[] closedLists = {new List<TraversableNode>(), new List<TraversableNode>()};

        TraversableNode[] currentNode = {begNode, endNode};

        openLists[0].Add(currentNode[0]);
        openLists[1].Add(currentNode[1]);

        while(openLists[0].Count > 0 && openLists[1].Count > 0)
        {
            for(int i = 0, j = (i+1)%2; i < 2; i++)
            {
                foreach(TraversableNode neighborNode in currentNode[i].GetNeighbors())
                {
                    if(i == 0 && closedLists[1].Contains(neighborNode))
                    {
                        currentNode[1] = neighborNode;
                        TraversableNode tNode;

                        do
                        {
                            tNode = currentNode[1].safeParentNode;
                            currentNode[1].parentNode = currentNode[0];
                            currentNode[0] = currentNode[1];
                            currentNode[1] = tNode;
                        }   while(currentNode[1] != tNode);

                        if(Visualize)
                        {
                            while(tNode != null)
                            {
                                tNode.GetComponent<Renderer>().material = current;
                                tNode = tNode.parentNode;
                            }
                        }

                        yield break;
                    }

                    else
                    {
                        if(!closedLists[i].Contains(neighborNode))
                        {
                            if(!openLists[i].Contains(neighborNode))
                            {
                                neighborNode.parentNode = currentNode[i];
                                neighborNode.hValue = TraversableNode.Distance(neighborNode, endNode) * j;
                                neighborNode.gValue = 1f + neighborNode.GetGValue(i==1);

                                AddToSortedList(neighborNode, ref openLists[0]);

                                if(Visualize) neighborNode.GetComponent<Renderer>().material = open;
                            }
                        }

                        bool rp = false;
                        if(neighborNode.gValue < currentNode[i].safeParentNode.gValue)
                        {
                            if(neighborNode.parentNode != currentNode[i])
                            {
                                rp = true;
                                currentNode[i].parentNode = neighborNode;
                                if(Visualize) neighborNode.GetComponent<Renderer>().material = reparented;
                            }
                        }

                        closedLists[i].Add(currentNode[i]);
                        if(Visualize) currentNode[i].GetComponent<Renderer>().material = rp ? reparent : closed;
                        currentNode[i] = openLists[i][0];
                        if(Visualize) currentNode[i].GetComponent<Renderer>().material = current;
                        openLists[i].Remove(currentNode[i]);
                    }
                }
            }
            if(Visualize) yield return null;
        }
        yield return null;
    }


    public Stack<TraversableNode> NodeStackPath(TraversableNode endNode)
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

    public int AddToSortedList(TraversableNode node, ref List<TraversableNode> sortedList)
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