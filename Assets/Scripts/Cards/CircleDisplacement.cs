using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDisplacement : MonoBehaviour
{
    [Range(0, 360)]
    public float angleMax;
    
    [Range(-360, 360)]
    public float angleOffset;

    public float radius = 2;
    public float radiusMod = 0f;

    public bool faceInward;


    private List<Transform> trackedTeansforms = new List<Transform>();

    public void UpdateTrackedObjectsPosition()
    {
        for(int i = 0; i < trackedTeansforms.Count; i++)
        {
            float angle = (((float)i + 1f) / ((float)trackedTeansforms.Count + 1f)) * angleMax;
            angle += angleOffset;
            angle *= Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * (radius + (radiusMod * i));
            float y = Mathf.Sin(angle) * (radius + (radiusMod * i));

            trackedTeansforms[i].transform.localPosition = new Vector3(x, 0, y);

            if(faceInward)
                trackedTeansforms[i].LookAt(transform.position);
        }
    }
}
