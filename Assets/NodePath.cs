using System.Collections;
using System.Collections.Generic;

public sealed class NodePath
{
    private Node _startNode, _endNode;

    private Queue<Node> _closedQueue;
    private Stack<Node> _openStack;

    public Queue<Node> GetNodePath(Node start, Node goal)
    {
        

        return _closedQueue;
    }





    private float GetHScore(Node aNode)
    {
        return(Node.Distance(aNode, _endNode));
    }

    private float GetGScore(Node aNode)
    {
        return(Node.Distance(aNode, _startNode));
    }

    private float GetFScore(Node aNode)
    {
        return(GetHScore(aNode) + GetGScore(aNode));
    }
}
