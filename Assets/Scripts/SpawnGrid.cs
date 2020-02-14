using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnGrid : MonoBehaviour
{
    //[HideInInspector]
    public PerlinTile[] perlinTiles;
    private Dictionary<string, PerlinTile> gridTiles = new Dictionary<string, PerlinTile>();
    private List<PerlinTile.Biomes> setBiomes = new List<PerlinTile.Biomes>();
    private List<PerlinTile.Elevations> setElevations = new List<PerlinTile.Elevations>();
    private List<PerlinTile.Tempreatures> setTempreatures = new List<PerlinTile.Tempreatures>();




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

    public float xOffset, zOffset;

    [Header("Perlin Generation")]
    public string perlinSeed;
    public float perlinScale = 8f;


    [Header("Custom Map!")]
    public Texture2D mapTexture;
    public TraversableNode[,] gridNodes;





    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void Start()
    {
        Initialize();
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private void Update()
    {
        //GeneratePerlinTexture(perlinSeed, perlinScale);

        if(Input.GetKey(KeyCode.LeftControl))
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                GenerateNewHexGrid(cols, rows, mapTexture == null ? GeneratePerlinTexture(perlinSeed, perlinScale) : mapTexture);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                GeneratePerlinTexture(perlinSeed, perlinScale);
            }
        }
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
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



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public void GenerateNewHexGrid(uint a_xSize, uint a_ySize, Texture2D a_sampleTexture)
    {
        GameObject gridCell;
        
        int txWidth = a_sampleTexture.width;
        int txHeight = a_sampleTexture.height;

        int pixX, pixY;
        float hueSample, satSample, valSample;

        int hexOffset;
        Vector3 pos;
        Vector3 scale;

        bool isWall;
        System.Func<int, uint, bool> IsEdge = delegate(int index, uint max)
        {
            return index == 0 || index == max - 1;
        };



        ClearBoard();
        cols = a_xSize;
        rows = a_ySize;
        gridNodes = new TraversableNode[cols, rows];

        for(int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                pixX = (int)(((float)i/cols) * txWidth);
                pixY = (int)(((float)j/rows) * txHeight);


                // biome, temperature, elevation
                Color.RGBToHSV(a_sampleTexture.GetPixel(pixX, pixY), out hueSample, out satSample, out valSample);

                PerlinTile.Biomes biome = setBiomes[Mathf.RoundToInt((setBiomes.Count - 1) * hueSample)];
                PerlinTile.Elevations elevation = setElevations[Mathf.RoundToInt((setElevations.Count - 1) * valSample)];
                PerlinTile.Tempreatures tempreature = setTempreatures[Mathf.RoundToInt((setTempreatures.Count - 1) * satSample)];

                PerlinTile newTile = null;
                gridCell = null;
                do
                { 
                    if(gridTiles.TryGetValue($"{biome}-{tempreature}-{elevation}", out newTile))
                    {
                        gridCell = newTile.tilePrefab;
                    }

                    if(setElevations.LastIndexOf(elevation) == 0)
                    {
                        elevation = setElevations[setElevations.Count -1];
                        
                        if(setTempreatures.LastIndexOf(tempreature) == 0)
                        {
                            tempreature = setTempreatures[setTempreatures.Count -1];

                            if(setBiomes.LastIndexOf(biome) == 0)
                            {
                                biome = setBiomes[(setBiomes.Count - 1)];
                            }
                            else
                            {
                                biome = setBiomes[(setBiomes.LastIndexOf(biome) - 1)];
                            }
                        }
                        else
                        {
                            tempreature = setTempreatures[(setTempreatures.LastIndexOf(tempreature) - 1)];
                        }
                    }
                    else
                    {
                        elevation = setElevations[(setElevations.LastIndexOf(elevation) - 1)];
                    }

                } while(gridCell == null);


                gridCell = Instantiate(gridCell);


                hexOffset = (i % 2);
                isWall = edgesAreWalls && (IsEdge(i, cols) || IsEdge(j, rows));

                pos = new Vector3(
                    ((-cols / 2) + i) * xOffset,
                    (valSample * maxHeight),
                    (((-rows / 2) + j) + (hexOffset * 0.5f)) * zOffset);

                scale = isWall ? new Vector3(1f, (maxHeight + 1f), 1f) : Vector3.one;


                gridCell.name = $"[{i}, {j}]";
                gridCell.transform.parent = transform;

                gridCell.transform.localPosition = pos * 2f;
                gridCell.transform.Rotate(Vector3.up, 30);
                gridCell.transform.localScale = scale;


                gridNodes[i, j] = (gridCell.GetComponent<TraversableNode>());
                gridNodes[i, j] = gridNodes[i, j] == null ? gridCell.AddComponent<TraversableNode>() : gridNodes[i, j];
                
                gridNodes[i, j].coordinates[0] = i;
                gridNodes[i, j].coordinates[1] = j;
                gridNodes[i, j].travelCost = 1;
                gridNodes[i, j].isTraversable = !isWall;
                


                // Set node neighbors -----
                {
                    if(j > 0)
                    {
                        gridNodes[i, j].nodeData.AddNeighbor(gridNodes[i, j - 1].nodeData);
                    }

                    if(i > 0)
                    {
                        int nextJ = j + (hexOffset * 2 - 1);


                        gridNodes[i, j].nodeData.AddNeighbor(gridNodes[i - 1, j].nodeData);

                        if(nextJ >= 0 && nextJ < rows)
                        {
                            gridNodes[i, j].nodeData.AddNeighbor(gridNodes[i - 1, nextJ].nodeData);
                        }
                    }
                }
            }
        }
    }


    private void Initialize()
    {
        foreach(PerlinTile pt in perlinTiles)
        {
            string key = $"{pt.biome}-{pt.tempreature}-{pt.elevation}";
            if(gridTiles.ContainsKey(key)) continue;
            else gridTiles.Add(key, pt);

            if(!setBiomes.Contains(pt.biome)) setBiomes.Add(pt.biome);
            if(!setElevations.Contains(pt.elevation)) setElevations.Add(pt.elevation);
            if(!setTempreatures.Contains(pt.tempreature)) setTempreatures.Add(pt.tempreature);
        }
    }


    private GameObject GetTileOfTypes(PerlinTile.Biomes biome, PerlinTile.Elevations elevation, PerlinTile.Tempreatures tempreature, float choiceMod = 0f)
    {
        List<PerlinTile> returnTiles = new List<PerlinTile>();

        foreach(PerlinTile pt in perlinTiles)
            if(pt.biome == biome && pt.elevation == elevation && pt.tempreature == tempreature)
                returnTiles.Add(pt);

        if(returnTiles.Count == 0)
        {
            Debug.LogWarning($"No tile of type(s): {tempreature}, {elevation}, {biome}, in tile array");
            return null;
        }

        int returnIndex = (int)(returnTiles.Count / choiceMod);
        return returnTiles[returnIndex].tilePrefab;
    }


    private void NormalizeTileHeights(ref PerlinTile[] tiles)
    {
        // float sum = 0;
        // tiles[0].heightOffset = 0f;

        // for(int i = 1; i < tiles.Length; i++)
        // {
        //     sum += tiles[i].heightOffset;
        // }
        // for(int i = 0; i < tiles.Length; i++)
        // {
        //     tiles[i].heightOffset /= sum;
        // }
    }


    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private Texture2D GeneratePerlinTexture(string a_seed, float a_scale = 1f)
    {
        Renderer renderer = GetComponent<Renderer>();
        Texture2D perlinTexture = new Texture2D((int)cols, (int)rows);
        Color[] pixels = new Color[perlinTexture.width * perlinTexture.height];

        float sampleH, sampleS, sampleV;

        int seedLen;


        a_seed = a_seed.GetHashCode().ToString();
        seedLen = a_seed.Length;

        for(int i = 0; i < perlinTexture.height; i++)
        {
            for(int j = 0; j < perlinTexture.width; j++)
            {
                sampleH = GetPerlinNoiseValue(j, i, a_seed.Substring(0,             seedLen / 2), a_scale, 1f);
                sampleS = GetPerlinNoiseValue(j, i, a_seed.Substring(seedLen / 4,   seedLen / 2), a_scale, 1f);
                sampleV = GetPerlinNoiseValue(j, i, a_seed.Substring(seedLen / 2,   seedLen / 2), a_scale, 1f);

                pixels[(i * perlinTexture.width) + j] = Color.HSVToRGB(sampleH, sampleS, sampleV);
            }
        }


        perlinTexture.SetPixels(pixels);
        perlinTexture.Apply();

        if(renderer != null)
            renderer.material.SetTexture("_BaseMap", perlinTexture);

        return perlinTexture;
    }



    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    float GetPerlinNoiseValue(float a_xCoord, float a_yCoord, string a_seed = "", float a_scale = 1f, float a_valueMod = 1f)
    {   
        float seedHash = (float)(a_seed.GetHashCode() % (a_seed.Length));


        a_xCoord /= cols;
        a_yCoord /= rows;

        a_xCoord *= a_scale;
        a_yCoord *= a_scale;

        a_xCoord += seedHash;
        a_yCoord += seedHash;

        return Mathf.Clamp01(Mathf.PerlinNoise(a_xCoord, a_yCoord)) * a_valueMod;
    }





    /// DEPRECIATED METHODS // DEPRECIATED METHODS // DEPRECIATED METHODS // DEPRECIATED METHODS // DEPRECIATED METHODS //
    #region DEPRECIATED METHODS

    [System.Obsolete("depreciated: use GenerateTextureGrid instead")]
    public void GenerateNewMap()
    {
        return;
        // ClearBoard();

        // m_gridNodes = new TraversableNode[cols, m_rows];

        // for(int i = 0; i < m_cols; i++)
        // {
        //     for (int j = 0; j < m_rows; j++)
        //     {
        //         GameObject gridCell = null;

        //         float xCoord = (float)i/(float)cols;
        //         float yCoord = (float)j/(float)rows;

        //         float perlinHeight = GetPerlinNoiseValue(xCoord, yCoord, m_perlinSeed, m_perlinScale);
        //         float height = (int)(perlinHeight * m_perlinScale) * perlinHeightMod;
                

        //         bool isWall = m_edgesAreWalls && (i == 0 || j == 0 || i == m_cols-1 || j == m_rows-1);
        //         Vector3 scale = isWall ? new Vector3(1f, 16f, 1f) : new Vector3(1f, 1f,  1f);


        //         float biomeScale = 8f;
        //         System.Func<int, int> getBiomeNoise = (int valueMod) =>
        //             (int)GetPerlinNoiseValue(xCoord, yCoord, (m_perlinSeed + biomeSeed), biomeScale, valueMod);
                
        //         int perlinFort = (int)GetPerlinNoiseValue(xCoord, yCoord, m_perlinSeed.Substring(m_perlinSeed.Length - 1), 4, 10);


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

        //             m_gridNodes[i, j] = (gridCell.GetComponent<TraversableNode>());
        //             m_gridNodes[i, j] = m_gridNodes[i, j] == null ? gridCell.AddComponent<TraversableNode>() : m_gridNodes[i, j];
                    
        //             m_gridNodes[i, j].xCoord = i;
        //             m_gridNodes[i, j].yCoord = j;
        //             m_gridNodes[i, j].travelCost = resistance + height;
        //             m_gridNodes[i, j].isTraversable = !isWall;
                    
        //             if(j > 0)
        //             {
        //                 m_gridNodes[i, j].AddNeighbor(m_gridNodes[i, j - 1]);
        //             }

        //             if(i > 0)
        //             {
        //                 m_gridNodes[i, j].AddNeighbor(m_gridNodes[i - 1, j]);

        //                 int nextJ = j + (hexOffset * 2 - 1);

        //                 if(nextJ >= 0 && nextJ < m_rows)
        //                 {
        //                     m_gridNodes[i, j].AddNeighbor(m_gridNodes[i - 1, nextJ]);
        //                 }
        //             }
        //         }
        //     }
        // }
    }
    
    
    #endregion
}

[System.Serializable]
public class PerlinTile
{
    [SerializeField]
    public GameObject tilePrefab;

    public Biomes biome;
    public Elevations elevation;
    public Tempreatures tempreature;
    public enum Biomes
    {
        WATER,
        COAST,
        LAND,
        MOUNTAINS,
    }
    public enum Elevations
    {
        ABYSS,
        SUBTERRAIN,
        SEA_LEVEL,
        ELEVATED,
        MOUNTAIN,
        SUMMIT,
    }
    public enum Tempreatures
    {
        FREEZING,
        COLD,
        WARM,
        HOT,
        INFERNO,
    }
}


[CreateAssetMenu(fileName = "NewBiome", menuName = "Biome")]
public class Biome : ScriptableObject
{
    [Range(0f, 1f)]
    public float rangeToNextBiome;
}
