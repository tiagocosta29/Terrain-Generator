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
    /// Number of cells to have.
    /// 2 will create a total of 4 cells, 4 will create a total of 16 cells
    /// </summary>
    public int DivisionCells;

    /// <summary>
    ///  Water biome reference
    /// </summary>
    public Water WaterBiome;

    /// <summary>
    /// List of available biomes that are not water
    /// </summary>
    public List<Biome> BiomeList;

    public float[,] TerrainMatrix;

    [Range(0, 1000)]
    public int TerrainHeightScale;

    [Range(0.0f, 100.0f)]
    public float Scale;

    public UnityTerrainGenerator TerrainGeneartor;

    // The origin of the sampled area in the plane.
    [Range(0.0f, 100.0f)]
    public float xOrg = 0.0f;
    [Range(0.0f, 100.0f)]
    public float yOrg = 0.0f;
    
    /// <summary>
    /// Current Biome that is being built
    /// </summary>
    private Biome currentBiome;
    

    private int matrixPositionX;
    private int matrixPositionY;

    private void Start()
    {
        TerrainMatrix = new float[MapSize, MapSize];
        DrawMap();
    }

    /// <summary>
    /// 
    /// </summary>
    public void DrawMap()
    {
        PerlinNoise();

        int terrainSize = MapSize / DivisionCells;
        int xPos = 0;
        int yPos = 0;
        //for (int i = 0; i < DivisionCells; i++)
        //{
        //    for (int j = 0; j < DivisionCells; j++)
        //    {
        //        currentBiome = RandomBiome();
        //        TerrainMatrix = currentBiome.BuildTerrain(terrainSize, xPos, yPos, TerrainMatrix);
        //        yPos += terrainSize;                
        //    }
        //    yPos = 0;
        //    xPos += terrainSize;
        //}

        TerrainGeneartor.SetData(TerrainMatrix, TerrainHeightScale, MapSize);
    }

    /// <summary>
    /// Fills the whole map with perlin noise
    /// </summary>
    public void PerlinNoise()
    {

        for (float i = 0; i < MapSize; i++)
        {
            for (float j = 0; j < MapSize; j++)
            {
                float xCoord = xOrg + i / MapSize * Scale;
                float yCoord = yOrg + j / MapSize * Scale;
                TerrainMatrix[(int)i, (int)j] = Mathf.PerlinNoise(xCoord, yCoord);
            }
        }
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
            var biomeIndex = Random.Range(0, BiomeList.Count - 1);
            return BiomeList[biomeIndex];
        }
        else
        {
            return WaterBiome;
        }        
    }    
}
