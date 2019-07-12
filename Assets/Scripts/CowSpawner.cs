using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowSpawner : MonoBehaviour
{
    private Camera mainCamera;

    public GameObject cow;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(cameraRay, out rayHit))
            {
                TraversableNode tn = rayHit.collider.GetComponent<TraversableNode>();
                if(tn == null) return;

                else
                {
                    GameObject go = Instantiate(cow, tn.transform.position, Quaternion.identity);
                    go.GetComponent<NodeNavAgent>().currentPositionNode = tn;
                }
            }
        }
    }
}
