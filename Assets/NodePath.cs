using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class NodePath : MonoBehaviour
{
    public Material current, open, closed;
    public TraversableNode _startNode, _endNode;
    [Range(0.01f, 1f)]
    public float gWeight, hWeight;

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

    [ContextMenu("Get Path")]
    public void BeginAStar()
    {
        StartCoroutine(AStar());
    }

    public IEnumerator AStar()
    {
        List<TraversableNode> _closedList = new List<TraversableNode>();
        List<TraversableNode> _openList = new List<TraversableNode>();

        TraversableNode currentNode;
        currentNode = _startNode;
        _openList.Add(currentNode);

        while(_openList.Count > 0)
        {
            foreach(TraversableNode node in currentNode.GetNeighbors())
            {
                if(!_closedList.Contains(node))
                {
                    if(!_openList.Contains(node))
                    {
                        node._parentNode = currentNode;

                        node._hValue = TraversableNode.Distance(node, _endNode) / hWeight;
                        node._gValue = node.GetGValue() / gWeight;

                        AddToSortedList(node, ref _openList);
                        node.GetComponent<Renderer>().material = open;
                    }
                }

                else
                {
                    //node._parentNode = node.GetNeighboorTravelCost(currentNode) < node._gValue ? currentNode : node._parentNode;
                }

                if(node == _endNode)
                {
                    TraversableNode n = node;
                    while(n != null)
                    {
                        n.GetComponent<Renderer>().material = current;
                        n = n._parentNode;
                        yield return null;
                    }

                    yield break;
                }
            }

            _closedList.Add(currentNode);
            currentNode.GetComponent<Renderer>().material = closed;

            currentNode = _openList[0];
            currentNode.GetComponent<Renderer>().material = current;

            _openList.Remove(currentNode);

            //yield return null;
        }

        yield return null;
    }

    public Stack<TraversableNode> NodeStackPath(TraversableNode endNode)
    {
        Stack<TraversableNode> returnStack = new Stack<TraversableNode>();

        TraversableNode currentNode = endNode;

        while(currentNode._parentNode != null)
        {
            returnStack.Push(currentNode);
            currentNode = currentNode._parentNode;
        }

        return returnStack;
    }
}