using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHighlight : MonoBehaviour
{
    public Material mat;
    Vector3 mp;
    Ray mouseRay => (Camera.main.ScreenPointToRay(Input.mousePosition));
    RaycastHit rayHit;

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(mouseRay, out rayHit, 64))
        {
            mp = rayHit.collider.transform.position;
            mat.SetVector("_MousePos", mp);
        }
    }
}
