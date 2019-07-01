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


    private void Update() 
    {
        if(navAgent.goalPositionNode != null)
            Debug.DrawRay(navAgent.goalPositionNode.transform.position, Vector3.up * 2);   
    }

    public IEnumerator NewRandomPathRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, waitTime));

        foreach(Node na in navAgent.currentPositionNode.GetNeighborhood(3))
        {
            NodeNavAgent[] agentsInRange = na.GetInformation<NodeNavAgent>();
            if(agentsInRange.Length > 0)
            {
                Node[] potentialTargets = agentsInRange[0].currentPositionNode.GetNeighborhoodLayers(2, 1);
                TraversableNode goal = potentialTargets[Random.Range(0, potentialTargets.Length)] as TraversableNode;
                navAgent.goalPositionNode = goal;

                Debug.DrawLine(navAgent.currentPositionNode.transform.position, navAgent.goalPositionNode.transform.position, Color.red, 10f, false);
                yield break;
            }
        }

        navAgent.SetRandomDestination(2, 2);
    }
}