using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHighlight : MonoBehaviour
{
    Material mat;
    Vector3 mp;
    Ray mouseRay => (Camera.main.ScreenPointToRay(Input.mousePosition));
    RaycastHit rayHit;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponentInChildren<Renderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(mouseRay, out rayHit, 64))
        {
            mp = rayHit.collider.transform.position;
            mat.SetVector("_MousePos", mp);
            transform.position = rayHit.point;
        }
    }
}
