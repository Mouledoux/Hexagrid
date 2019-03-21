using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerClickHandler, IPointerUpHandler
{
    private List<Node> neighbors = new List<Node>();

    Vector3 originalPos;
    private void Start()
    {
        originalPos = transform.localPosition;
    }

    private void Update()
    {
        //transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime);
    }
    public int AddNeighbor(Node newNeighbor)
    {
        if(newNeighbor == null)
            return -1;

        if(!neighbors.Contains(newNeighbor))
            neighbors.Add(newNeighbor);

        if(!newNeighbor.GetNegibors().Contains(this))
            newNeighbor.AddNeighbor(this);

        return 0;
    }

    public List<Node> GetNegibors()
    {
        List<Node> myNeighbors = neighbors;
        return myNeighbors;
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

        foreach(Node n in root.neighbors)
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

        yield return null;

        foreach(Node n in root.neighbors)
            foreach(Node o in n.neighbors)
                if(!affected.Contains(o))
                    n.StartCoroutine(PropigateOut(n, affected, maxProp));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //StartCoroutine(PropigateOut(this, null, 50));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //StartCoroutine(PropigateOut(this, null, 100));
    }
}
