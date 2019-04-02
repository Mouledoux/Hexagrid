using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DuploNPC : MonoBehaviour
{
    public Material mat;
    private Stack<TraversableNode> path;
    private TraversableNode _startNode, _endNode;

    void Update()
    {
        ClickSetPath();
        WalkPath();
    }


    private void ClickSetPath()
    {
        RaycastHit hit;
        
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            TraversableNode tn = hit.collider.GetComponent<TraversableNode>();
            if(tn == null) return;

            else if(Input.GetMouseButtonDown(1))
            {
                if(tn == _startNode) _startNode = null;
                else
                {
                    if(_startNode != null) _startNode.ResetMaterial();

                    _startNode = tn;
                    _startNode.GetComponent<Renderer>().material = mat;
                    transform.position = _startNode.transform.position;
                }
            }

            else if(Input.GetMouseButtonDown(0))
            {
                if(tn == _endNode) return;
                else
                {
                    _endNode = tn;
                    _endNode.GetComponent<Renderer>().material = mat;

                    if(_startNode != null && _endNode != null)
                    {
                        path = NodePath.TwinStarII(_startNode, _endNode, true);
                    }
                }
            }
        }
    }


    private void WalkPath()
    {
        if(path != null && path.Count > 0)
        {
            Vector3 dir = (path.Peek().transform.position - transform.position).normalized;
            transform.Translate(dir * Time.deltaTime);

            if(Vector3.Distance(transform.position, path.Peek().transform.position) <= 0.05f)
            {
                path.Pop().GetComponent<Renderer>().material = mat;
            }
        }
    }
}
