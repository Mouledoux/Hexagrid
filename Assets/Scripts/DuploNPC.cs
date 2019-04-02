using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DuploNPC : MonoBehaviour
{
    private Stack<TraversableNode> path;

    void Update()
    {
        if(path != null && path.Count > 0)
        {
            Vector3 dir = path.Peek().transform.position - transform.position;
            transform.Translate(dir* Time.deltaTime);

            if(Vector3.Distance(transform.position, path.Peek().transform.position) <= 0.05f)
            {
                path.Pop();
            }
        }
    }
}
