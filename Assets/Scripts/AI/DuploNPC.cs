using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// DEPRICATED, use NodeNavAgent instead.
public class DuploNPC : MonoBehaviour
{
    public Material mat;
    public UnityEngine.Events.UnityEvent OnPathComplete;

    private Stack<TraversableNode> path;
    private TraversableNode _startNode, _endNode;

    void Start()
    {
        Debug.LogError("DuploNPC has been replaced with 'NodeNavAgent', please use it instead");
    }

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
                if(_startNode != null) _startNode.ResetMaterial();

                path = null;
                _startNode = tn;
                _startNode.GetComponent<Renderer>().material = mat;
                transform.position = _startNode.transform.position;
            }

            else if(Input.GetMouseButtonDown(0))
            {
                _endNode = tn;
                _endNode.GetComponent<Renderer>().material = mat;
                transform.position = _startNode.transform.position;

                if(_startNode != null && _endNode != null)
                {
                    path = NodeNav.TwinStarII(_startNode, _endNode, true);
                }
            }
        }
    }

    private void WalkPath()
    {
        if(path != null && path.Count > 0)
        {
            _startNode = path.Peek();

            Vector3 dir = (path.Peek().transform.position - transform.position);
            transform.GetChild(0).transform.localPosition = Vector3.up + (path.Peek().transform.up * Mathf.Sin(dir.magnitude * 3.14f));

            dir.Normalize();
            transform.Translate(dir * 2.5f * Time.deltaTime);

            if(Vector3.Distance(transform.position, path.Peek().transform.position) <= 0.05f)
            {
                path.Pop().GetComponent<Renderer>().material = mat;
            }
            
            if(path.Count == 0)
            {
                OnPathComplete.Invoke();
                path = null;
            }
        }
    }
}