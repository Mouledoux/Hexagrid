using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeNavAgent : MonoBehaviour
{
    public bool leader;
    public int scanRange;
    public Material pathMaterial, rangeMaterial;

    [SerializeField]
    private float _speed = 1f;
    public float speed
    {
        get {return _speed; }
        set { _speed = value; }
    }

    [SerializeField]
    private bool _autoRepath = true;
    public bool autoRepath
    {
        get { return _autoRepath; }
        set { _autoRepath = value; }
    }


    private TraversableNode _currentPositionNode;
    public TraversableNode currentPositionNode => (_currentPositionNode);

    private TraversableNode _goalPositionNode;
    public TraversableNode goalPositionNode
    {
        get { return _goalPositionNode; }
        set
        {
            _goalPositionNode = value;
            _nodePathStack = NodeNav.TwinStarII(currentPositionNode, _goalPositionNode, true);
        }
    }

    private Stack<TraversableNode> _nodePathStack;

    public bool hasPath => (_nodePathStack != null && _nodePathStack.Count > 0);

    public float remainingDistance => TraversableNode.Distance(currentPositionNode, goalPositionNode);


    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void Update()
    {
        if(leader)  
        {
            ClickSetPath();

            if(!hasPath && currentPositionNode != null)
            {
                ScanNeighbors(scanRange);
                foreach(TraversableNode aNode in currentPositionNode.GetNeighborhood(scanRange))
                {
                    if(aNode.isOccupied)
                    {
                        NodeNavAgent[] otherAgents = aNode.GetInformation<NodeNavAgent>();
                        if(otherAgents.Length > 0)
                        {
                            foreach (NodeNavAgent agent in otherAgents)
                            {
                                print(agent.scanRange);
                            }
                        }
                    }
                }
            }
        }


        TraversePath();
        VeggieJump();
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void TraversePath()
    {
        if(hasPath)
        {
            if(!_nodePathStack.Peek().isTraversable || _nodePathStack.Peek().isOccupied)
            {
                if(autoRepath)
                {
                    _nodePathStack = NodeNav.TwinStarII(currentPositionNode, goalPositionNode, true);

                }
                
                else
                {
                    _nodePathStack = null;
                    return;
                }
            }


            Vector3 dir = (_nodePathStack.Peek().transform.position - transform.position);

            dir.Normalize();
            transform.Translate(dir * speed * Time.deltaTime);

            if(Vector3.Distance(transform.position, _nodePathStack.Peek().transform.position) <= 0.05f)
            {
                if(currentPositionNode.CheckInformationFor(this))
                    currentPositionNode.RemoveInformation(this);

                currentPositionNode.isOccupied = false;

                _currentPositionNode = _nodePathStack.Pop();

                currentPositionNode.AddInformation(this);
                currentPositionNode.isOccupied = true;

                currentPositionNode.GetComponent<Renderer>().material = pathMaterial;
            }

            if(_nodePathStack.Count == 0)
            {
                _nodePathStack = null;
            }
        }
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void VeggieJump()
    {
        if(_nodePathStack != null)
        {
            TraversableNode nextNode = _nodePathStack.Peek();
            float dist = Vector3.Distance(transform.position, nextNode.transform.position);

            transform.GetChild(0).transform.localPosition =
            Vector3.up + (nextNode.transform.up * (Mathf.Sin(dist * 3.14f)));
        }
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void ClickSetPath()
    {
        RaycastHit hit;
        
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            TraversableNode tn = hit.collider.GetComponent<TraversableNode>();
            if(tn == null) return;

            else if(Input.GetMouseButtonDown(1))
            {   
                _nodePathStack = null;
                _currentPositionNode = tn;
                currentPositionNode.isOccupied = true;
                currentPositionNode.AddInformation(this);

                transform.position = currentPositionNode.transform.position;
            }

            else if(Input.GetMouseButtonDown(0))
            {
                if(currentPositionNode != null)
                {
                    _nodePathStack = NodeNav.TwinStarII(currentPositionNode, tn, true);
                }
            }
        }
    }


    private void ScanNeighbors(int range)
    {
        if(currentPositionNode == null) return;

        foreach(Node n in _currentPositionNode.GetNeighborhood(range))
        {
            n.GetComponent<Renderer>().material = rangeMaterial;
        }
    }
}