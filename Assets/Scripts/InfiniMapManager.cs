using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniMapManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gridcellPrefab;

    
    [Header("Perlin Generation"), SerializeField]
    private string perlinSeed;
    public float perlinScale = 8f;


    private TraversableNode[,] gridNodes;
    private List<GameObject> gridcellPool = new List<GameObject>();


    public int spawnDistance;


    private int hexOffset => (int)transform.position.x % 2;

    Vector3 lastPos;

    bool hasMoved
    {
        get
        {
            Vector3 cPos = transform.position;
            if(Vector3.Distance(cPos, lastPos) > 1)
            {
                cPos.x = (int)cPos.x;
                cPos.y = (int)cPos.y;
                cPos.z = (int)cPos.z;

                lastPos = cPos;
                return true;
            }
            else return false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void UpdateMap()
    {

    }
}
