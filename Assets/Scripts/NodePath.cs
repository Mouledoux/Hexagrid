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


    [ContextMenu("Get Path")]
    public void BeginAStar()
    {
        StartCoroutine(AStar());
    }

    public IEnumerator AStar()
    {
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
                        node.hValue = TraversableNode.Distance(node, _endNode) / hWeight;
                        node.gValue = 1f + node.GetGValue() / gWeight;

                        AddToSortedList(node, ref openList);
                        node.GetComponent<Renderer>().material = open;
                    }
                }

                if(node.gValue < currentNode.parentNode.gValue)
                {
                    currentNode.parentNode = node;
                    node.GetComponent<Renderer>().material = reparented;
                    rp = true;
                }

                if(node == _endNode)
                {
                    TraversableNode n = node;
                    while(n != null)
                    {
                        closedList.Remove(n);
                        n.GetComponent<Renderer>().material = current;
                        n = n.parentNode;
                    }

                    while(closedList.Count > 0)
                    {
                        //closedList[0].ResetMaterial();
                        closedList.RemoveAt(0);
                    }
                    
                    while(openList.Count > 0)
                    {
                        //openList[0].ResetMaterial();
                        openList.RemoveAt(0);
                    }
                    
                    

                    print($"Distance: {NodeStackPath(_endNode).Count}, Cost: {_endNode.gValue * gWeight}");
                    yield break;
                }
            }

            closedList.Add(currentNode);
            currentNode.GetComponent<Renderer>().material = rp ? reparent : closed;

            currentNode = openList[0];
            currentNode.GetComponent<Renderer>().material = current;

            openList.Remove(currentNode);

            if(Visualize) yield return null;
        }

        yield return null;
    }



    public Stack<TraversableNode> NodeStackPath(TraversableNode endNode)
    {
        Stack<TraversableNode> returnStack = new Stack<TraversableNode>();

        TraversableNode currentNode = endNode;

        while(currentNode.parentNode != currentNode)
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