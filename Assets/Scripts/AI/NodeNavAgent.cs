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


    public UnityEngine.Events.UnityEvent onDestinationReached;

    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void Update()
    {
        if(leader)  
        {
            ClickSetPath();
        }
        else if(goalPositionNode == null)
        {
            if(currentPositionNode != null)
            {
                foreach(TraversableNode aNode in currentPositionNode.GetNeighborhood(scanRange))
                {
                    if(aNode.isOccupied)
                    {
                        NodeNavAgent[] otherAgents = aNode.GetInformation<NodeNavAgent>();
                        if(otherAgents.Length > 0)
                        {
                            foreach (NodeNavAgent agent in otherAgents)
                            {
                                foreach(TraversableNode bNode in agent.currentPositionNode.GetNeighborhood(scanRange))
                                {
                                    if(bNode.isTraversable && !bNode.isOccupied)
                                    {
                                        goalPositionNode = bNode;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
            SetRandomDestination(5);

        TraversePath();
        VeggieJump();
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void TraversePath()
    {
        if(hasPath)
        {
            TraversableNode nextNode = _nodePathStack.Peek();

            if(!nextNode.isTraversable || nextNode.isOccupied)
            {
                if(autoRepath)
                {
                    _nodePathStack = NodeNav.TwinStarII(currentPositionNode, goalPositionNode, true);
                }
                
                else
                {
                    _nodePathStack = null;
                }
                return;
            }


            Vector3 dir = (nextNode.transform.position - transform.position);

            dir.Normalize();
            transform.Translate(dir * speed * Time.deltaTime);

            if(Vector3.Distance(transform.position, nextNode.transform.position) <= 0.05f)
            {
                currentPositionNode.RemoveInformation(this);
                currentPositionNode.isOccupied = false;

                _currentPositionNode = _nodePathStack.Pop();

                currentPositionNode.AddInformation(this);
                currentPositionNode.isOccupied = true;
                
                foreach(TraversableNode tn in currentPositionNode.GetNeighborhood(6))
                {
                    tn.onOccupy.Invoke();
                }

                currentPositionNode.GetComponent<Renderer>().material = pathMaterial;
            }

            if(_nodePathStack.Count == 0)
            {
                goalPositionNode = null;
                _nodePathStack = null;
                onDestinationReached.Invoke();
            }
            else
            {
                transform.GetChild(0).LookAt(_nodePathStack.Peek().transform.position);
            }
        }
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void VeggieJump()
    {
        if(hasPath)
        {
            TraversableNode nextNode = _nodePathStack.Peek();
            float dist = Vector3.Distance(transform.position, nextNode.transform.position);

            Vector3 nextPos = nextNode.transform.up * (Mathf.Sin(dist * 3.14f));

            transform.GetChild(0).transform.localPosition = Vector3.up * 0.2f + nextPos;
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


    private void ScanNeighbors(int range = 1)
    {
        if(currentPositionNode == null) return;

        foreach(Node n in _currentPositionNode.GetNeighborhood(range))
        {
            n.GetComponent<Renderer>().material = rangeMaterial;
        }
    }

    public void SetRandomDestination(int dist = 1)
    {
        if(currentPositionNode == null) return;

        Node[] neighbors = currentPositionNode.GetNeighborhood(dist);
        TraversableNode destNode = neighbors[Random.Range(0, neighbors.Length)] as TraversableNode;
        
        _nodePathStack = NodeNav.TwinStarII(currentPositionNode, destNode, true);
    }
}