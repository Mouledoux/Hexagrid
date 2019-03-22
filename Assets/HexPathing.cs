using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPathing : MonoBehaviour
{
    public Node startNode, goalNode;

    public List<Node> path = new List<Node>();
    private List<Node> openList = new List<Node>();
    private List<Node> closedList = new List<Node>();

    public void HexPath(Node start, Node end)
    {
        openList.Add(start);
        Node current = start;

        while(openList.Count > 0)
        {
            if(current.GetNeighbors().Contains(end))
            {
                openList.Add(end);
                break;
            }

            else
            {
                current = LowestHScoreNeighbor(current, end);
                openList.Add(current);
            }
        }
    }

    public Node LowestHScoreNeighbor(Node thisNode, Node goal)
    {
        float currentHScore = float.MaxValue;
        float thisHScore = 0f;
        Node returnNode = thisNode;

        foreach(Node n in thisNode.GetNeighbors())
        {
            thisHScore = n.GetDistanceTo(goal);
            if(thisHScore < currentHScore)
            {
                currentHScore = thisHScore;
                returnNode = n;
            }
        }

        return returnNode;
    }
}