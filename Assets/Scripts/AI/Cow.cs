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
        navAgent.payload = this;

        StartCoroutine(NewRandomPathRoutine());
        navAgent.onDestinationReached.AddListener(delegate()
        {
            StartCoroutine(NewRandomPathRoutine());
        });
    }


    [HideInInspector]
    public Cow followedAgent;

    private void Update() 
    {
        if(navAgent.goalPositionNode != null)
        {
            Debug.DrawRay(navAgent.goalPositionNode.transform.position + (Vector3.up * 0.5f), Vector3.up * 2, Color.green);
            Debug.DrawLine(navAgent.transform.position, navAgent.goalPositionNode.transform.position, Color.blue);
            
            if(followedAgent != null)
            {
                if(followedAgent.followedAgent == this)
                    Debug.DrawLine(navAgent.transform.position, followedAgent.transform.position, Color.magenta);
                else
                    Debug.DrawLine(navAgent.transform.position, followedAgent.transform.position, Color.red);
            }
        }   
    }

    public IEnumerator NewRandomPathRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, waitTime));

        foreach(Node n in navAgent.currentPositionNode.nodeData.GetNeighborhood(3))
        {
            NodeNavAgent[] agentsInRange = n.GetInformation<NodeNavAgent>();
            if(agentsInRange.Length > 0 && agentsInRange[0].payload is Cow leader)
            {
                followedAgent = leader;
                TraversableNode[] potentialTargets = followedAgent.navAgent.currentPositionNode.nodeData.GetNeighborhoodLayersInformation<TraversableNode>(1, 1);
                int index = Random.Range(0, potentialTargets.Length - 1);
                TraversableNode goal = potentialTargets[index];
                navAgent.goalPositionNode = goal;
                yield break;
            }
        }
        navAgent.SetRandomDestination(2, 2);
    }
}