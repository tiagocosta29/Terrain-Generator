using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    /// <summary>
    /// Total Map size, 128, 254, 512 etc
    /// </summary>
    public int MapSize;

    /// <summary>
    /// Probability of map division occurring
    /// </summary>
    [Range(0, 1)]
    public double DivisionChance;

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
    
    /// <summary>
    /// Placeholder for Unity chan character
    /// </summary>
    public GameObject UnityChan;
    
    /// <summary>
    /// Current Biome that is being built
    /// </summary>
    private Biome currentBiome;

    /// <summary>
    /// Use this to change water chance during generation
    /// </summary>
    private int tempWaterChance;

    private void Start()
    {
        MapBiomeList = new List<Biome>();
        tempWaterChance = WaterChance;
        if (EvenDivision)
        {
            DrawMapEven();
        }
        else
        {
            DrawMapRandomRecursive();
        }
        SetWater();
        UnityChan.transform.position = new Vector3(MapSize / 2, 4, MapSize / 2);
    }

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
                currentBiome.BuildTerrain(terrainSize, xPos, yPos, TerrainHeightScale);
                yPos += terrainSize;                
            }
            yPos = 0;
            xPos += terrainSize;
        }

        float waterScale = MapSize * 1.37f / 1024f;
        WaterObject.transform.localScale = new Vector3(waterScale, 1, waterScale);
        WaterObject.transform.position = new Vector3(MapSize / 2, 2, MapSize / 2);
    }

    /// <summary>
    /// Old division algorithm, not being used atm
    /// </summary>
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
                subdivided = rand.NextDouble() > 0.7 ? true : false;
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
                            currentBiome.BuildTerrain(subTerrainSize, subXPos, subYPos, TerrainHeightScale);
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
                currentBiome.BuildTerrain(terrainSize, xPos, yPos, TerrainHeightScale);
                yPos += terrainSize;                

            }
                yPos = 0;
                xPos += terrainSize;            
        }
        SetWater();
    }    

    /// <summary>
    /// Recursive division algorithm
    /// </summary>
    /// <param name="x">Starting X position of the array</param>
    /// <param name="y">Starting Y position of the array</param>
    /// <param name="currentLevel">Current division deepness</param>
    public void DrawMapRandomRecursive(int x = 0, int y = 0, int currentLevel = 1)
    {
        int level = currentLevel;
        int terrainSize = MapSize / (int)(Math.Pow(2, currentLevel));
        int xPos = x;
        int yPos = y;
        bool subdivided;
        var rand = new System.Random();        

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                subdivided = rand.NextDouble() > DivisionChance ? true : false;
                if (level >= DivisionCells)
                {
                    subdivided = false;
                }

                if (subdivided)
                {
                    DrawMapRandomRecursive(xPos, yPos, level + 1);
                    yPos += terrainSize;
                    continue;
                }
                currentBiome = RandomBiome();
                MapBiomeList.Add(currentBiome);
                currentBiome.BuildTerrain(terrainSize, xPos, yPos, TerrainHeightScale, (level == 1 ? 1 : level - 1));
                yPos += terrainSize;
            }
            yPos = y;
            xPos += terrainSize;
        }        
    }

    /// <summary>
    /// Scales the water object to the terrain scale
    /// </summary>
    private void SetWater()
    {
        float waterScale = MapSize * 1.37f / 1024f;
        WaterObject.transform.localScale = new Vector3(waterScale, 1, waterScale);
        WaterObject.transform.position = new Vector3(MapSize / 2, 3, MapSize / 2);
    }


    /// <summary>
    /// Picks a random Biome type and apply's its filters
    /// </summary>
    /// <returns></returns>
    private Biome RandomBiome()
    {
        var isLand = UnityEngine.Random.Range(0, 100);
        if (isLand > tempWaterChance)
        {
            tempWaterChance = WaterChance;
            var biomeIndex = UnityEngine.Random.Range(0, BiomeList.Count);
            return BiomeList[biomeIndex];
        }
        else
        {
            tempWaterChance -= 10;
            return WaterBiome;
        }        
    }    
}
