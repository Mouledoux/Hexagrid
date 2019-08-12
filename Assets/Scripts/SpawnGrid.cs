﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnGrid : MonoBehaviour
{
    public GameObject gridNode;


    [Header("Grid Dimensions")]
    [SerializeField] private uint _rows;
    [SerializeField] private uint _cols;

    public uint rows
    {
        get { return _rows; }
        set { _rows = value; }
    }
    public uint cols
    {
        get { return _cols; }
        set { _cols = value; }
    }
    public float maxHeight = 1f;
    public bool edgesAreWalls;


    [Header("Perlin Generation")]
    public string perlinSeed;
    public float perlinScale = 8f;


    [Header("Custom Map!")]
    public Texture2D mapTexture;
    public TraversableNode[,] gridNodes;


    // private IEnumerator Start()
    // {
    //     while(Application.isPlaying)
    //     {
    //         if(gridNodes != null && gridNodes.Length > 0)
    //         {
    //             foreach (Node node in gridNodes)
    //             {
    //                 if(gridNodes[0,0] == node) continue;

    //                 Vector3 avPos = Vector3.zero;

    //                 foreach (Node n in node.GetNeighbors())
    //                 {
    //                     avPos += n.transform.position;
    //                     avPos -= new Vector3(1, 0, 1) * (1 - Mathf.Clamp01(Vector3.Distance(node.transform.position, n.transform.position)));
    //                 }


    //                 node.transform.position = avPos;
    //                 yield return null;
    //             }
    //         }

    //         yield return null;
    //     }
    //     //GenerateNewGrid(cols, rows, mapTexture == null ? GeneratePerlinTexture(perlinSeed, perlinScale) : mapTexture);
    // }

    private void Update()
    {
        GeneratePerlinTexture(perlinSeed, perlinScale);

        if(Input.GetKey(KeyCode.LeftControl))
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                GenerateNewHexGrid(cols, rows, mapTexture == null ? GeneratePerlinTexture(perlinSeed, perlinScale) : mapTexture);
            }

            if (Input.GetKey(KeyCode.W))
            {
                GeneratePerlinTexture(perlinSeed, perlinScale);
            }
        }
    }



    bool ClearBoard()
    {
        if(gridNodes == null)
            return false;

        foreach (TraversableNode node in gridNodes)
        {
            Destroy(node.gameObject);
        }

        gridNodes = null;
        return true;
    }


    public void GenerateNewHexGrid(uint xSize, uint ySize, Texture2D sampleTexture)
    {
        ClearBoard();
        cols = xSize;
        rows = ySize;
        gridNodes = new TraversableNode[cols, rows];

        System.Func<int, uint, bool> isEdge = delegate(int index, uint max) {return index == 0 || index == max - 1;};

        int txWidth = sampleTexture.width;
        int txHeight = sampleTexture.height;

        int pixX, pixY;

        for(int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                pixX = (int)(((float)i/cols) * txWidth);
                pixY = (int)(((float)j/rows) * txHeight);

                GameObject gridCell = Instantiate(gridNode) as GameObject;

                // biome, temperature, elevation
                float hueSample, satSample, valSample;
                Color.RGBToHSV(sampleTexture.GetPixel(pixX, pixY), out hueSample, out satSample, out valSample);


                bool isWall = edgesAreWalls && (isEdge(i, cols) || isEdge(j, rows));
                Vector3 scale = isWall ? new Vector3(1f, (maxHeight * 4f), 1f) : Vector3.one + (Vector3.up * (int)(valSample * maxHeight));


                gridCell.name = $"[{i}, {j}]";
                gridCell.transform.parent = transform;

                int hexOffset = (i % 2);
                gridCell.transform.localPosition = new Vector3(((-cols / 2) + i) * 0.85f, 0, ((-rows / 2) + j) + (hexOffset * 0.5f));
                gridCell.transform.Rotate(Vector3.up, 30);
                gridCell.transform.localScale = scale;


                gridNodes[i, j] = (gridCell.GetComponent<TraversableNode>());
                gridNodes[i, j] = gridNodes[i, j] == null ? gridCell.AddComponent<TraversableNode>() : gridNodes[i, j];
                
                gridNodes[i, j].xCoord = i;
                gridNodes[i, j].yCoord = j;
                gridNodes[i, j].travelCost = 1;
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



                // Temperature clouds            
                if(!isWall && satSample > 0.3 && satSample < 0.6)
                {
                    bool cloud = false;

                    foreach (Node node in gridNodes[i, j].GetNeighborhood(1))
                    {
                        if(node.transform.GetChild(0).gameObject.activeInHierarchy)
                        {
                            cloud = true;
                            break;
                        }
                    }

                    if(cloud == false)
                    {
                        Transform child = gridCell.transform.GetChild(0);
                        child.gameObject.SetActive(true);
                        child.parent = null;
                        child.transform.position -= (Vector3.up * (child.transform.position.y - maxHeight));
                        child.localScale = new Vector3(0.05f, 0.025f, 0.05f);
                        child.parent = gridCell.transform;
                    }
                }

                
            }
        }
    }




    private Texture2D GeneratePerlinTexture(string seed, float scale = 1f)
    {
        Texture2D perlinTexture = new Texture2D((int)cols, (int)rows);
        Color[] pixels = new Color[perlinTexture.width * perlinTexture.height];

        seed = seed.GetHashCode().ToString();
        int seedLen = seed.Length;

        for(int i = 0; i < perlinTexture.height; i++)
        {
            for(int j = 0; j < perlinTexture.width; j++)
            {
                float sampleH = GetPerlinNoiseValue(j, i, seed.Substring(0,             seedLen / 2), scale, 1f);
                float sampleS = GetPerlinNoiseValue(j, i, seed.Substring(seedLen / 4,   seedLen / 2), scale, 1f);
                float sampleV = GetPerlinNoiseValue(j, i, seed.Substring(seedLen / 2,   seedLen / 2), scale, 1f);

                pixels[(i * perlinTexture.width) + j] = Color.HSVToRGB(sampleH, sampleS, sampleV);
            }
        }


        perlinTexture.SetPixels(pixels);
        perlinTexture.Apply();

        Renderer r = GetComponent<Renderer>();
        if(r != null)
            r.material.mainTexture = perlinTexture;

        return perlinTexture;
    }



    float GetPerlinNoiseValue(float xCoord, float yCoord, string seed = "", float scale = 1f, float valueMod = 1f)
    {   
        float seedHash = (float)(seed.GetHashCode() % (seed.Length));

        xCoord /= cols;
        yCoord /= rows;

        xCoord *= scale;
        yCoord *= scale;

        xCoord += seedHash;
        yCoord += seedHash;

        return Mathf.Clamp01(Mathf.PerlinNoise(xCoord, yCoord)) * valueMod;
    }





    /// DEPRECIATED METHODS // DEPRECIATED METHODS // DEPRECIATED METHODS // DEPRECIATED METHODS // DEPRECIATED METHODS //
    #region DEPRECIATED METHODS

    [System.Obsolete("depreciated: use GenerateTextureGrid instead")]
    public void GenerateNewMap()
    {
        return;
        // ClearBoard();

        // gridNodes = new TraversableNode[cols, rows];

        // for(int i = 0; i < cols; i++)
        // {
        //     for (int j = 0; j < rows; j++)
        //     {
        //         GameObject gridCell = null;

        //         float xCoord = (float)i/(float)cols;
        //         float yCoord = (float)j/(float)rows;

        //         float perlinHeight = GetPerlinNoiseValue(xCoord, yCoord, perlinSeed, perlinScale);
        //         float height = (int)(perlinHeight * perlinScale) * perlinHeightMod;
                

        //         bool isWall = edgesAreWalls && (i == 0 || j == 0 || i == cols-1 || j == rows-1);
        //         Vector3 scale = isWall ? new Vector3(1f, 16f, 1f) : new Vector3(1f, 1f,  1f);


        //         float biomeScale = 8f;
        //         System.Func<int, int> getBiomeNoise = (int valueMod) =>
        //             (int)GetPerlinNoiseValue(xCoord, yCoord, (perlinSeed + biomeSeed), biomeScale, valueMod);
                
        //         int perlinFort = (int)GetPerlinNoiseValue(xCoord, yCoord, perlinSeed.Substring(perlinSeed.Length - 1), 4, 10);


        //         float resistance = 0f;

        //         if(isWall)
        //         {
        //             gridCell = Instantiate(outerWall[getBiomeNoise(outerWall.Count)]) as GameObject;
        //             resistance = float.MaxValue;
        //         }
        //         else if(perlinFort == 9)
        //         {
        //             gridCell = Instantiate(Special[getBiomeNoise(Special.Count)]) as GameObject;
        //             resistance = 7f;
        //         }
        //         else if(perlinHeight >= 0.8f)
        //         {
        //             gridCell = Instantiate(highLand[getBiomeNoise(highLand.Count)]) as GameObject;
        //             resistance = 24f;
        //         }
        //         else  if(perlinHeight >= 0.4f)
        //         {
        //             gridCell = Instantiate(medLand[getBiomeNoise(medLand.Count)]) as GameObject;
        //             resistance = 8f;
        //         }
        //         else
        //         {
        //             gridCell = Instantiate(lowLand[getBiomeNoise(lowLand.Count)]) as GameObject;
        //             resistance = 16f;
        //         }

        //         //height += perlinFort > 3 ? perlinHeightMod : 0;

        //         if(gridCell != null)
        //         {
        //             int hexOffset = (i % 2);

        //             gridCell.name = $"[{i}, {j}]";
        //             gridCell.transform.parent = transform;
        //             //gridCell.transform.localPosition = new Vector3(((-cols / 2) + i) * 0.85f, height, ((-rows / 2) + j) + (hexOffset * 0.5f));
        //             gridCell.transform.localPosition = new Vector3(((-cols / 2) + i) * 0.85f, 0, ((-rows / 2) + j) + (hexOffset * 0.5f));
        //             gridCell.transform.Rotate(Vector3.up, 30);
        //             gridCell.transform.localScale = Vector3.one + Vector3.up * height * 8;

        //             gridNodes[i, j] = (gridCell.GetComponent<TraversableNode>());
        //             gridNodes[i, j] = gridNodes[i, j] == null ? gridCell.AddComponent<TraversableNode>() : gridNodes[i, j];
                    
        //             gridNodes[i, j].xCoord = i;
        //             gridNodes[i, j].yCoord = j;
        //             gridNodes[i, j].travelCost = resistance + height;
        //             gridNodes[i, j].isTraversable = !isWall;
                    
        //             if(j > 0)
        //             {
        //                 gridNodes[i, j].AddNeighbor(gridNodes[i, j - 1]);
        //             }

        //             if(i > 0)
        //             {
        //                 gridNodes[i, j].AddNeighbor(gridNodes[i - 1, j]);

        //                 int nextJ = j + (hexOffset * 2 - 1);

        //                 if(nextJ >= 0 && nextJ < rows)
        //                 {
        //                     gridNodes[i, j].AddNeighbor(gridNodes[i - 1, nextJ]);
        //                 }
        //             }
        //         }
        //     }
        // }
    }
    
    
    #endregion
}