using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NodeNavAgent))]
public class Cow : MonoBehaviour
{
    private NodeNavAgent _navAgent;
    public NodeNavAgent navAgent
    {
        get { return _navAgent; }
        set { _navAgent = value; }
    }

    [Range(0f, 9f)]
    public float waitTime;

    private void Start()
    {
        navAgent = GetComponent<NodeNavAgent>();

        StartCoroutine(NewRandomPathRoutine());
        navAgent.onDestinationReached.AddListener(delegate()
        {
            StartCoroutine(NewRandomPathRoutine());
        });
    }


    public IEnumerator NewRandomPathRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, waitTime));

        foreach(Node na in navAgent.currentPositionNode.GetNeighborhood())
        {
            NodeNavAgent[] agentsInRange = na.GetInformation<NodeNavAgent>();
            if(agentsInRange.Length > 0)
            {
                Node[] potentialTargets = agentsInRange[0].currentPositionNode.GetNeighborhoodLayers(2, 2);
                navAgent.goalPositionNode = potentialTargets[Random.Range(0, potentialTargets.Length)] as TraversableNode;
            }
        }

        navAgent.SetRandomDestination(2, 2);
    }
}