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
        yield return new WaitForSeconds(waitTime);
        navAgent.SetRandomDestination(2, 2);
    }
}