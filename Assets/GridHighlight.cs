using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHighlight : MonoBehaviour
{
    Material mat;
    Vector3 mp;
    RaycastHit rayHit;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponentInChildren<Renderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit, 64);
        mp = rayHit.point;
        mat.SetVector("_MousePos", mp);
    }
}
