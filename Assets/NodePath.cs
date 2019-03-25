using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class NodePath : MonoBehaviour
{
    public Material current, open, closed;

    public TraversableNode _startNode, _endNode;

    private List<TraversableNode> _closedList = new List<TraversableNode>();
    private List<TraversableNode> _openList = new List<TraversableNode>();

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
        TraversableNode currentNode;
        currentNode = _startNode;
        _openList.Add(currentNode);

        while(_openList.Count > 0)
        {
            foreach(TraversableNode node in currentNode.GetNeighbors())
            {
                if(!_closedList.Contains(node) && !_openList.Contains(node))
                {
                    node._parentNode = node._parentNode == null ? currentNode : node._parentNode;

                    node._hValue = TraversableNode.Distance(node, _endNode);
                    node._gValue = TraversableNode.Distance(node, _startNode);

                    AddToSortedList(node, ref _openList);
                    node.GetComponent<Renderer>().material = open;
                }

                if(node == _endNode)
                {
                    yield break;//return NodeStackPath(node);
                }
                
                yield return null;
            }

            _closedList.Add(currentNode);
            currentNode.GetComponent<Renderer>().material = closed;

            currentNode = _openList[0];
            currentNode.GetComponent<Renderer>().material = current;

            _openList.Remove(currentNode);

            yield return null;
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