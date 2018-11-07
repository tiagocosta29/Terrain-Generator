using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    /// <summary>
    /// Total Map size, 128, 254, 512 etc
    /// </summary>
    public int MapSize;

    /// <summary>
    /// % of water in the map, use values from 0 to 100;
    /// </summary>
    public int WaterChance;

    /// <summary>
    /// Sets the division of the map to even Biomes
    /// </summary>
    public bool EvenDivision;

    /// <summary>
    /// Number of cells to have.
    /// 2 will create a total of 4 cells, 4 will create a total of 16 cells
    /// </summary>
    public int DivisionCells;

    /// <summary>
    ///  Water biome reference
    /// </summary>
    public Biome WaterBiome;

    /// <summary>
    /// List of available biomes that are not water
    /// </summary>
    public List<Biome> BiomeList;
    
    /// <summary>
    /// List of biomes on the map, used for debug purposes only
    /// </summary>
    public List<Biome> MapBiomeList;

    /// <summary>
    /// Reference to the ocean prefab
    /// </summary>
    public GameObject WaterObject;

    [Range(0, 1000)]
    public int TerrainHeightScale;
    
    public float[,] TerrainMatrix;
   // public UnityTerrainGenerator TerrainGeneartor;
    
    /// <summary>
    /// Current Biome that is being built
    /// </summary>
    private Biome currentBiome;

    private void Start()
    {
        TerrainMatrix = new float[MapSize, MapSize];
        MapBiomeList = new List<Biome>();
        if (EvenDivision)
        {
            DrawMapEven();
        }
        else
        {
            DrawMapRandom();
        }    }

    /// <summary>
    /// Divides the map with even slices
    /// </summary>
    public void DrawMapEven()
    {
        int terrainSize = MapSize / DivisionCells;
        int xPos = 0;
        int yPos = 0;
        for (int i = 0; i < DivisionCells; i++)
        {
            for (int j = 0; j < DivisionCells; j++)
            {
                currentBiome = RandomBiome();
                MapBiomeList.Add(currentBiome);
                TerrainMatrix = currentBiome.BuildTerrain(terrainSize, xPos, yPos, TerrainMatrix, TerrainHeightScale);
                yPos += terrainSize;                
            }
            yPos = 0;
            xPos += terrainSize;
        }

        float waterScale = MapSize * 1.37f / 1024f;
        WaterObject.transform.localScale = new Vector3(waterScale, 1, waterScale);
        WaterObject.transform.position = new Vector3(MapSize / 2, 2, MapSize / 2);
        //TerrainGeneartor.SetData(TerrainMatrix, TerrainHeightScale, MapSize);
    }

    public void DrawMapRandom()
    {
        int terrainSize = MapSize / 2;
        int subTerrainSize = MapSize / 4;
        int xPos = 0;
        int yPos = 0;
        int subYPos = 0;
        int subXPos = 0;
        bool subdivided = false;
        int divisionCount = 0;

        var rand = new System.Random();
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                subdivided = rand.NextDouble() > 0.5 ? true : false;
                if (subdivided)
                {
                    subXPos = xPos;
                    subYPos = yPos;

                    divisionCount++;
                    for (int y = 0; y < 2; y++)
                    {
                        for (int x = 0; x < 2; x++)
                        {
                            currentBiome = RandomBiome();
                            MapBiomeList.Add(currentBiome);
                            TerrainMatrix = currentBiome.BuildTerrain(subTerrainSize, subXPos, subYPos, TerrainMatrix, TerrainHeightScale);
                            subYPos += subTerrainSize;
                        }
                        subYPos = yPos;
                        subXPos += subTerrainSize;
                    }
                    yPos += terrainSize;
                    continue;
                }
                currentBiome = RandomBiome();
                MapBiomeList.Add(currentBiome);
                TerrainMatrix = currentBiome.BuildTerrain(terrainSize, xPos, yPos, TerrainMatrix, TerrainHeightScale);
                yPos += terrainSize;                

            }
                yPos = 0;
                xPos += terrainSize;            
        }
        SetWater();
    }
 
    private void SetWater()
    {
        float waterScale = MapSize * 1.37f / 1024f;
        WaterObject.transform.localScale = new Vector3(waterScale, 1, waterScale);
        WaterObject.transform.position = new Vector3(MapSize / 2, 2, MapSize / 2);
    }


    /// <summary>
    /// Picks a random Biome type and apply's its filters
    /// </summary>
    /// <returns></returns>
    private Biome RandomBiome()
    {
        var isLand = Random.Range(0, 100);
        if(isLand > WaterChance)
        {
            var biomeIndex = Random.Range(0, BiomeList.Count);
            return BiomeList[biomeIndex];
        }
        else
        {
            return WaterBiome;
        }        
    }    
}
