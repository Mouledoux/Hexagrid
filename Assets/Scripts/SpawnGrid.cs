using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnGrid : MonoBehaviour
{
    public GameObject gridNode;


    [Header("Grid Dimensions")]
    [SerializeField] private int _rows;
    [SerializeField] private int _cols;

    public int rows
    {
        get { return Mathf.Abs(_rows); }
        set { _rows = Mathf.Abs(value); }
    }
    public int cols
    {
        get { return Mathf.Abs(_cols); }
        set { _cols = Mathf.Abs(value); }
    }
    public float maxHeight = 1f;
    public bool edgesAreWalls;


    [Header("Perlin Generation")]
    public string perlinSeed;
    public float perlinScale = 8f;


    [Header("Custom Map!")]
    public Texture2D mapTexture;
    public TraversableNode[,] gridNodes;


    private void Start()
    {
        //GenerateNewGrid(cols, rows, mapTexture == null ? GeneratePerlinTexture(perlinSeed, perlinScale) : mapTexture);
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl))
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                GenerateNewGrid(cols, rows, mapTexture == null ? GeneratePerlinTexture(perlinSeed, perlinScale) : mapTexture);
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


    public void GenerateNewGrid(int xSize, int ySize, Texture2D sampleTexture)
    {
        ClearBoard();
        cols = xSize;
        rows = ySize;
        gridNodes = new TraversableNode[cols, rows];


        int txWidth = sampleTexture.width;
        int txHeight = sampleTexture.height;


        for(int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                int pixX, pixY;
                pixX = (int)(((float)i/cols) * txWidth);
                pixY = (int)(((float)j/rows) * txHeight);


                // biome, temperature, elevation
                float hueSample, satSample, valSample;
                Color.RGBToHSV(sampleTexture.GetPixel(pixX, pixY), out hueSample, out satSample, out valSample);


                GameObject gridCell = Instantiate(gridNode) as GameObject;
                int hexOffset = (i % 2);

                bool isWall = edgesAreWalls && (i == 0 || j == 0 || i == cols-1 || j == rows-1);
                Vector3 scale = isWall ? new Vector3(1f, maxHeight * 2f, 1f) : Vector3.one + (Vector3.up * (valSample * maxHeight));


                gridCell.name = $"[{i}, {j}]";
                gridCell.transform.parent = transform;

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
            }
        }
    }




    private Texture2D GeneratePerlinTexture(string seed, float scale = 1f)
    {
        Texture2D perlinTexture = new Texture2D(cols, rows);
        Color[] pixels = new Color[perlinTexture.width * perlinTexture.height];

        seed = seed.GetHashCode().ToString();
        int seedLength = seed.Length;

        for(int i = 0; i < perlinTexture.height; i++)
        {
            for(int j = 0; j < perlinTexture.width; j++)
            {
                float xCoord = j;
                float yCoord = i;

                float sampleH =  GetPerlinNoiseValue(xCoord, yCoord, seed.Substring(0, seedLength / 2), scale);
                float sampleS =  GetPerlinNoiseValue(xCoord, yCoord, seed.Substring(seedLength / 4, seedLength / 2), scale);
                float sampleV =  GetPerlinNoiseValue(xCoord, yCoord, seed.Substring(seedLength / 2, seedLength / 2), scale);

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
        float seedHash = seed.GetHashCode() >> (seed.Length << seed.Length);

        xCoord += seedHash;
        yCoord += seedHash;

        xCoord *= scale;
        yCoord *= scale;

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