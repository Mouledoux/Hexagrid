using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnGrid : MonoBehaviour
{
    public int perlinSeed;
    public float perlinScale = 8f;
    
    public List<GameObject> outerWall, lowLand, medLand, highLand, Special;
    public int rows, cols;
    public TraversableNode[,] gridNodes;

    IEnumerator Start()
    {
        gridNodes = new TraversableNode[cols, rows];

        for(int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject gridCell = null;

                float xCord = (float)i/(float)cols;
                float yCord = (float)j/(float)rows;

                float perlinHeight = GetPerlinNoiseValue(xCord, yCord, perlinScale, perlinSeed);
                float height = (int)(perlinHeight * perlinScale) * 0.2f;
                

                bool isWall = (i == 0 || j == 0 || i == cols-1 || j == rows-1);
                Vector3 scale = isWall ? new Vector3(1f, 16f, 1f) : new Vector3(1f, 1f,  1f);


                float biomeScale = 8f;
                System.Func<int, int> getBiomeNoise = (int valueMod) =>
                    (int)GetPerlinNoiseValue(xCord, yCord, biomeScale, perlinSeed/2, valueMod);
                
                int perlinFort = (int)GetPerlinNoiseValue(xCord, yCord, 32f, perlinSeed, 10);

                float resistance = 0f;
                


                if(isWall)
                {
                    gridCell = Instantiate(outerWall[getBiomeNoise(outerWall.Count)]) as GameObject;
                    resistance = float.MaxValue;
                }
                else if(perlinFort == 9)
                {
                    gridCell = Instantiate(Special[getBiomeNoise(Special.Count)]) as GameObject;
                }
                else if(perlinHeight >= 0.8f)
                {
                    gridCell = Instantiate(highLand[getBiomeNoise(highLand.Count)]) as GameObject;
                    resistance = 24f;
                }
                else  if(perlinHeight >= 0.4f)
                {
                    gridCell = Instantiate(medLand[getBiomeNoise(medLand.Count)]) as GameObject;
                    resistance = 8f;
                }
                else
                {
                    gridCell = Instantiate(lowLand[getBiomeNoise(lowLand.Count)]) as GameObject;
                    resistance = 16f;
                }


                if(gridCell != null)
                {
                    gridCell.name = $"[{i}, {j}]";
                    gridCell.transform.parent = transform;
                    gridCell.transform.localPosition = new Vector3(((-cols / 2) + i) * 0.85f, height, ((-rows / 2) + j) + ((i % 2) * 0.5f));
                    gridCell.transform.Rotate(Vector3.up, 30);
                    gridCell.transform.localScale = scale;

                    gridNodes[i, j] = (gridCell.AddComponent<TraversableNode>());
                    gridNodes[i, j].xCoord = i;
                    gridNodes[i, j].yCoord = j;
                    gridNodes[i, j].travelCost = resistance + height;
                    gridNodes[i, j].isTraversable = !isWall;
                    
                    if(j > 0)
                        gridNodes[i, j].AddNeighbor(gridNodes[i, j - 1]);

                    if(i > 0)
                    {
                        gridNodes[i, j].AddNeighbor(gridNodes[i - 1, j]);

                        int nextJ = j + ( i % 2 == 0 ? -1 : 1);

                        if(nextJ >= 0 && nextJ < rows)
                        {
                            gridNodes[i, j].AddNeighbor(gridNodes[i - 1, nextJ]);
                        }
                    }
                }
            }
            yield return null;
        }
        yield return null;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl))
            if(Input.GetKeyDown(KeyCode.Q))
                NewMap();
    }

    bool ClearBoard()
    {
        if(gridNodes == null || gridNodes[rows-1, cols-1] == null)
            return false;

        for(int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Destroy(gridNodes[i, j].gameObject);
            }
        }

        return true;
    }

    [ContextMenu("NewMap")]
    public void NewMap()
    {
        if(ClearBoard())
            StartCoroutine(Start());
    }


    float GetPerlinNoiseValue(float xCord, float yCord, float scale, long seed = 0, float valueMod = 1f)
    {   
        xCord += seed;
        yCord += seed;

        xCord *= scale;
        yCord *= scale;

        return Mathf.Clamp01(Mathf.PerlinNoise(xCord, yCord)) * valueMod;
    }
}