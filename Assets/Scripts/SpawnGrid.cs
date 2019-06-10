using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnGrid : MonoBehaviour
{
    public int rows, cols;

    public int perlinSeed;
    public float perlinScale = 8f;
    public float perlinHeightMod = 0.2f;

    public float biomeSeedMod = 0.5f;


    
    public bool edgesAreWalls;
    public List<GameObject> outerWall, lowLand, medLand, highLand, Special;
    public TraversableNode[,] gridNodes;

    private void Start()
    {
        rows = Mathf.Abs(rows);
        cols = Mathf.Abs(cols);

        GenerateNewMap();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl))
            if(Input.GetKeyDown(KeyCode.Q))
                GenerateNewMap();
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
    public void GenerateNewMap()
    {
        ClearBoard();

        gridNodes = new TraversableNode[cols, rows];

        for(int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject gridCell = null;

                float xCoord = (float)i/(float)cols;
                float yCoord = (float)j/(float)rows;

                float perlinHeight = GetPerlinNoiseValue(xCoord, yCoord, perlinSeed, perlinScale);
                float height = (int)(perlinHeight * perlinScale) * perlinHeightMod;
                

                bool isWall = (i == 0 || j == 0 || i == cols-1 || j == rows-1) && edgesAreWalls;
                Vector3 scale = isWall ? new Vector3(1f, 16f, 1f) : new Vector3(1f, 1f,  1f);


                float biomeScale = 8f;
                System.Func<int, int> getBiomeNoise = (int valueMod) =>
                    (int)GetPerlinNoiseValue(xCoord, yCoord, (long)(perlinSeed * biomeSeedMod), biomeScale, valueMod);
                
                int perlinFort = (int)GetPerlinNoiseValue(xCoord, yCoord, perlinSeed, 32f, 10);


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
                    int hexOffset = (i % 2);

                    gridCell.name = $"[{i}, {j}]";
                    gridCell.transform.parent = transform;
                    gridCell.transform.localPosition = new Vector3(((-cols / 2) + i) * 0.85f, height, ((-rows / 2) + j) + (hexOffset * 0.5f));
                    //gridCell.transform.localPosition = new Vector3(((-cols / 2) + i), 0, ((-rows / 2) + j * 1.1f) + (hexOffset * 0.5f));
                    gridCell.transform.Rotate(Vector3.up, 30);
                    gridCell.transform.localScale = Vector3.one;// + Vector3.up * height * 16;

                    gridNodes[i, j] = (gridCell.GetComponent<TraversableNode>());
                    gridNodes[i, j] = gridNodes[i, j] == null ? gridCell.AddComponent<TraversableNode>() : gridNodes[i, j];
                    
                    gridNodes[i, j].xCoord = i;
                    gridNodes[i, j].yCoord = j;
                    gridNodes[i, j].travelCost = resistance + height;
                    gridNodes[i, j].isTraversable = !isWall;
                    
                    if(j > 0)
                    {
                        gridNodes[i, j].AddNeighbor(gridNodes[i, j - 1]);
                    }

                    if(i > 0)
                    {
                        gridNodes[i, j].AddNeighbor(gridNodes[i - 1, j]);

                        int nextJ = j + (hexOffset * 2 - 1);

                        if(nextJ >= 0 && nextJ < rows)
                        {
                            gridNodes[i, j].AddNeighbor(gridNodes[i - 1, nextJ]);
                        }
                    }
                }
            }
        }
    }


    float GetPerlinNoiseValue(float xCoord, float yCoord, long seed = 0, float scale = 1f, float valueMod = 1f)
    {   
        xCoord += seed;
        yCoord += seed;

        xCoord *= scale;
        yCoord *= scale;

        return Mathf.Clamp01(Mathf.PerlinNoise(xCoord, yCoord)) * valueMod;
    }
}