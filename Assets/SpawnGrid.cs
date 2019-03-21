using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eric
{
    public class SpawnGrid : MonoBehaviour
    {
        public List<GameObject> lowLand, medLand, highLand, Special;
        public int rows, cols;
        public Node[,] gridNodes;

        IEnumerator Start()
        {
            gridNodes = new Node[cols, rows];

            for(int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    float perlinScale = 16f;
                    float perlinHeight = Mathf.PerlinNoise(((float)i/(float)cols) * perlinScale,((float)j/(float)rows) * perlinScale);
                    
                    GameObject gridCell = null;

                    if(Random.value >= 0.99f)
                        gridCell = Instantiate(Special[Random.Range(0, Special.Count)]) as GameObject;

                    else if(perlinHeight >= 0.8f)
                        gridCell = Instantiate(highLand[Random.Range(0, highLand.Count)]) as GameObject;

                    else  if(perlinHeight >= 0.4f)
                        gridCell = Instantiate(medLand[Random.Range(0, medLand.Count)]) as GameObject;

                    else
                        gridCell = Instantiate(lowLand[Random.Range(0, lowLand.Count)]) as GameObject;


                    gridCell.name = $"[{i}, {j}]";
                    gridCell.transform.parent = transform;
            
                    gridCell.transform.localPosition = new Vector3(i, perlinHeight * 4, j + (i%2==0?.5f:0f));
                    gridCell.transform.Rotate(Vector3.up, 30);
                    //Vector3 scale = new Vector3(0.9f, perlinHeight * 16, 0.9f);
                    //gridCell.transform.localScale = scale;

                    //Color perlinColor = new Color(scale.y / 3f, 1, scale.y / 5f);
                    //gridCell.GetComponent<Renderer>().material.color = perlinColor;

                    gridNodes[i, j] = (gridCell.AddComponent<Node>());
                    
                    if(j > 0)
                        gridNodes[i, j].AddNeighbor(gridNodes[i, j-1]);

                    if(i > 0)
                    {
                        gridNodes[i, j].AddNeighbor(gridNodes[i-1, j]);

                        int nextJ = j+(i%2==0?1:-1);
                        if(nextJ >= 0 && nextJ < rows)
                            gridNodes[i, j].AddNeighbor(gridNodes[i-1, nextJ]);
                    }
                    
                    //yield return null;
                }
                yield return null;
            }

            yield return null;
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}