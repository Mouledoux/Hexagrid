using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeNavAgent : MonoBehaviour
{
    // ----- ----- ----- ----- -----
    [SerializeField]
    private bool _useTwinStar;
    public bool useTwinStar
    {
        get { return _useTwinStar; }
        set
        {
            _useTwinStar = value;
        }
    }



    // ----- ----- ----- ----- -----
    [SerializeField]
    private float _speed = 1f;
    public float speed
    {
        get {return _speed; }
        set { _speed = value; }
    }



    // ----- ----- ----- ----- -----
    [SerializeField]
    private bool _autoRepath = true;
    public bool autoRepath
    {
        get { return _autoRepath; }
        set { _autoRepath = value; }
    }



    // ----- ----- ----- ----- -----
    private TraversableNode _currentPositionNode;
    public TraversableNode currentPositionNode
    {
        get{ return _currentPositionNode; }
        set { _currentPositionNode = value; }
    }



    // ----- ----- ----- ----- -----
    private TraversableNode _goalPositionNode;
    public TraversableNode goalPositionNode
    {
        get { return _goalPositionNode; }
        set
        {
            _goalPositionNode = value;
            //System.Threading.Thread forward = new System.Threading.Thread(() =>
                _nodePathStack = NodeNav.TwinStarT<TraversableNode>(currentPositionNode, _goalPositionNode, useTwinStar);
            
            //forward.Start();
        }
    }

    // ----- ----- ----- ----- -----
    private Stack<TraversableNode> _nodePathStack;

    // ----- ----- ----- ----- -----
    public TraversableNode nextNode 
    {
        get{ return _nodePathStack == null ? null : _nodePathStack.Peek() as TraversableNode; }
    }
    // ----- ----- ----- ----- -----
    public bool hasPath
    {
        get { return _nodePathStack != null && _nodePathStack.Count > 0; }
    }
    // ----- ----- ----- ----- -----
    public double remainingDistance
    {
        get { return currentPositionNode.GetDistanceTo(goalPositionNode); }
    }
    
    // ----- ----- ----- ----- -----
    public UnityEngine.Events.UnityEvent onDestinationReached;



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void Update()
    {
        if(CheckForPath())
        {
            TraversePath();
            VeggieJump();
        }
    }


    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private bool CheckForPath()
    {
        if(!hasPath || nextNode == null || !nextNode.isTraversable)
        {
            if(autoRepath)
                goalPositionNode = _goalPositionNode;
            
            else
            {
                goalPositionNode = null;
                return false;   
            }
        }

        return hasPath;
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void TraversePath()
    {
        Vector3 dir = (nextNode.transform.position - transform.position);

        dir.Normalize();
        transform.Translate(dir * speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, nextNode.transform.position) <= 0.05f)
        {
            currentPositionNode.nodeData.RemoveInformation(this);
            currentPositionNode.isOccupied = false;

            currentPositionNode = _nodePathStack.Pop();

            currentPositionNode.nodeData.AddInformation(this);
            currentPositionNode.isOccupied = true;
        }

        if(_nodePathStack.Count == 0)
        {
            goalPositionNode = null;
            _nodePathStack = null;
            onDestinationReached.Invoke();
        }
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void VeggieJump()
    {
        if(hasPath)
        {
            TraversableNode nextNode = _nodePathStack.Peek();
            float dist = Vector3.Distance(transform.position, nextNode.transform.position);

            Vector3 nextPos = nextNode.transform.up * (Mathf.Sin(dist * Mathf.PI));

            transform.GetChild(0).transform.localPosition = Vector3.up * 0.2f + nextPos;
            transform.GetChild(0).LookAt(_nodePathStack.Peek().transform.position);
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
                if(currentPositionNode != null)
                {
                    currentPositionNode.isOccupied = false;
                    currentPositionNode.nodeData.RemoveInformation(this);
                }

                _nodePathStack = null;
                currentPositionNode = tn;
                currentPositionNode.isOccupied = true;
                currentPositionNode.nodeData.AddInformation(this);

                transform.position = currentPositionNode.transform.position;
            }

            else if(Input.GetMouseButtonDown(0))
            {
                if(currentPositionNode != null && tn != null)
                {
                    goalPositionNode = tn;
                }
            }
        }
    }





    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void ScanNeighbors(uint range = 1)
    {
        // if(currentPositionNode == null) return;

        // foreach(Node n in _currentPositionNode.GetNeighborhood(range))
        // {
        //     n.GetComponent<Renderer>().material = rangeMaterial;
        // }
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public void SetRandomDestination(uint min, uint range)
    {
        if(currentPositionNode == null) return;

        List<Node<TraversableNode>> neighbors =  new List<Node<TraversableNode>>();
        
        foreach(Node<TraversableNode> n in currentPositionNode.nodeData.GetNeighborhoodLayers(min, range))
        {
            neighbors.Add(n);
        }

        TraversableNode destNode;
        do
        {
            destNode = neighbors[Random.Range(0, neighbors.Count)].nodeType;
            neighbors.Remove(destNode.nodeData);

        } while(!destNode.isTraversable && neighbors.Count > 0);

        
        goalPositionNode = destNode.isTraversable ? destNode : currentPositionNode;
    }
}