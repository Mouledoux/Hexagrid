using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnGrid : MonoBehaviour
{
    public int rows, cols;
    public string seed;

    private int perlinSeed => (seed.GetHashCode() >> 16);
    public float perlinScale = 8f;
    public float perlinHeightMod = 0.2f;

    public float biomeSeedMod = 0.5f;

    public Texture2D mapTexture;
    
    public bool edgesAreWalls;
    public List<GameObject> outerWall, lowLand, medLand, highLand, Special;
    public TraversableNode[,] gridNodes;

    private void Start()
    {
        rows = Mathf.Abs(rows);
        cols = Mathf.Abs(cols);

        //GenerateNewMap();
        NewTextureMap();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl))
            if(Input.GetKeyDown(KeyCode.Q))
                GenerateNewMap();
    }

    bool ClearBoard()
    {
        if(gridNodes == null)
            return false;

        foreach (TraversableNode node in gridNodes)
        {
            Destroy(node.gameObject);
        }

        return true;
    }

    public void NewTextureMap()
    {
        ClearBoard();
        gridNodes = new TraversableNode[cols, rows];

        GameObject gridCell;
        int txWidth = mapTexture.width;
        int txHeight = mapTexture.height;

        for(int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                int coX, coY;
                coX = (int)((float)((float)i/(float)cols) * txWidth);
                coY = (int)((float)((float)j/(float)rows) * txHeight);

                int hexOffset = (i % 2);
                float h, s, v;
                Color.RGBToHSV(mapTexture.GetPixel(coX, coY), out h, out s, out v);


                if(v > 0)
                {
                    gridCell = Instantiate(outerWall[0]) as GameObject;

                    gridCell.name = $"[{i}, {j}]";
                    gridCell.transform.parent = transform;
                    gridCell.transform.localPosition = new Vector3(((-cols / 2) + i) * 0.85f, 0, ((-rows / 2) + j) + (hexOffset * 0.5f));
                    gridCell.transform.Rotate(Vector3.up, 30);
                    gridCell.transform.localScale = Vector3.one + Vector3.up * v * 8f;

                    gridNodes[i, j] = (gridCell.GetComponent<TraversableNode>());
                    gridNodes[i, j] = gridNodes[i, j] == null ? gridCell.AddComponent<TraversableNode>() : gridNodes[i, j];
                    
                    gridNodes[i, j].xCoord = i;
                    gridNodes[i, j].yCoord = j;
                    gridNodes[i, j].travelCost = 1;
                    gridNodes[i, j].isTraversable = true;
                    
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
                

                bool isWall = edgesAreWalls && (i == 0 || j == 0 || i == cols-1 || j == rows-1);
                Vector3 scale = isWall ? new Vector3(1f, 16f, 1f) : new Vector3(1f, 1f,  1f);


                float biomeScale = 8f;
                System.Func<int, int> getBiomeNoise = (int valueMod) =>
                    (int)GetPerlinNoiseValue(xCoord, yCoord, (long)(perlinSeed * biomeSeedMod), biomeScale, valueMod);
                
                int perlinFort = (int)GetPerlinNoiseValue(xCoord, yCoord, perlinSeed * 4, 4, 10);


                float resistance = 0f;

                if(isWall)
                {
                    gridCell = Instantiate(outerWall[getBiomeNoise(outerWall.Count)]) as GameObject;
                    resistance = float.MaxValue;
                }
                else if(perlinFort == 9)
                {
                    gridCell = Instantiate(Special[getBiomeNoise(Special.Count)]) as GameObject;
                    resistance = 7f;
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

                //height += perlinFort > 3 ? perlinHeightMod : 0;

                if(gridCell != null)
                {
                    int hexOffset = (i % 2);

                    gridCell.name = $"[{i}, {j}]";
                    gridCell.transform.parent = transform;
                    //gridCell.transform.localPosition = new Vector3(((-cols / 2) + i) * 0.85f, height, ((-rows / 2) + j) + (hexOffset * 0.5f));
                    gridCell.transform.localPosition = new Vector3(((-cols / 2) + i) * 0.85f, 0, ((-rows / 2) + j) + (hexOffset * 0.5f));
                    gridCell.transform.Rotate(Vector3.up, 30);
                    gridCell.transform.localScale = Vector3.one + Vector3.up * height * 8;

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