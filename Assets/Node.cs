using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerClickHandler, IPointerUpHandler
{
    private List<Node> _neighbors = new List<Node>();
    private float _travelCost = 0f;
    public float travelCost => _travelCost;

    public int AddNeighbor(Node newNeighbor)
    {
        if(newNeighbor == null)
            return -1;

        if(!_neighbors.Contains(newNeighbor))
            _neighbors.Add(newNeighbor);

        if(!newNeighbor.GetNeighbors().Contains(this))
            newNeighbor.AddNeighbor(this);

        return 0;
    }
    public List<Node> GetNeighbors()
    {
        List<Node> myNeighbors = _neighbors;
        return myNeighbors;
    }
    public void RemoveNeighbor(Node oldNeighbor)
    {
        if(_neighbors.Contains(oldNeighbor))
            _neighbors.Remove(oldNeighbor);
    }
    public void ClearNeighbors()
    {
        foreach(Node n in _neighbors)
            n.RemoveNeighbor(this);
        
        _neighbors.Clear();
    }


    public float GetDistanceTo(Node goalNode)
    {
        return Vector3.Distance(transform.position, goalNode.transform.position);
    }

    public IEnumerator PropigateOut(Node root, List<Node> affected, int maxProp)
    {
        if(affected == null)
        {
            affected = new List<Node>();
            print("New List!");
        }

        else if(affected.Count >= maxProp)
            yield break;

        foreach(Node n in root._neighbors)
        {
            if(!affected.Contains(n))
            {
                //n.gameObject.GetComponent<Renderer>().material.color =
                //    new Color(1, (affected.Count % 2) / 100f, affected.Count / 100f);

                //n.transform.localScale = new Vector3(0.9f, 0.1f, 0.9f);
                n.transform.localPosition -= Vector3.down;
                affected.Add(n);
            }
            //yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        foreach(Node n in root._neighbors)
            foreach(Node o in n._neighbors)
                if(!affected.Contains(o))
                    n.StartCoroutine(PropigateOut(n, affected, maxProp));
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //StartCoroutine(PropigateOut(this, null, 64));
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        //StartCoroutine(PropigateOut(this, null, 100));
    }
}