using System.Collections.Generic;
using UnityEngine;

public class Biome : MonoBehaviour
{
    /// <summary>
    /// Biome name, used for debug purposes only
    /// </summary>
    public string BiomeName;

    /// <summary>
    /// Minimum radius of the island
    /// </summary>
    [Range(0f, 100f)]
    public float TerrainMinRadius;

    /// <summary>
    /// Maximum radius of the island
    /// </summary>
    [Range(0f, 100f)]
    public float TerrainMaxRadius;

    /// <summary>
    /// Use this radius if you're using a diamond shape only, otherwize it will does nothing
    /// </summary>
    [Range(0f, 100f)]
    public float DiamondRadius;
    
    /// <summary>
    /// Reference to the TerrainGenerator Script
    /// </summary>
    public UnityTerrainGenerator TerrainGenerator;

    /// <summary>
    /// Gaussian blur for masking the terrain matrix
    /// </summary>
    public Enums.MaskType TerrainMask;

    /// <summary>
    /// List of the terrain textures, atm must be 3, no more, no less
    /// </summary>
    public List<Texture2D> TerrainTextures;

    /// <summary>
    /// Biome matrix data
    /// </summary>
    public float[,] biomeMatrix;

    #region perlin noise settings
    
    [Range(0.0f, 100.0f)]
    public float MinXOrg = 0.0f;

    [Range(0.0f, 100.0f)]
    public float MaxXOrg = 0.0f;
    
    private float xOrg = 0.0f;

    [Range(0.0f, 100.0f)]
    public float MinYOrg = 0.0f;
    
    [Range(0.0f, 100.0f)]
    public float MaxYOrg = 0.0f;
    
    private float yOrg = 0.0f;

    [Range(0.0f, 100.0f)]
    public float MinScale;

    [Range(0.0f, 100.0f)]
    public float MaxScale;
    
    private float scale;
    #endregion

    /// <summary>
    /// Biome Size
    /// </summary>
    private int terrainSize;

    /// <summary>
    /// Terrain radius after randomize
    /// </summary>
    private float terrainRadius;

    /// <summary>
    /// Randomizes vars
    /// </summary>
    private void Init(int deepenessLevel)
    {
        xOrg = Random.Range(MinXOrg, MaxXOrg);
        yOrg = Random.Range(MinYOrg, MaxYOrg);
        
        scale = Random.Range(MinScale / deepenessLevel, MaxScale / deepenessLevel);
        terrainRadius = Random.Range(TerrainMinRadius, TerrainMaxRadius);
    }
    
    /// <summary>
    /// Creates a new perlin noise map
    /// </summary>
    /// <param name="size"></param>
    /// <param name="positionX"></param>
    /// <param name="positionY"></param>
    public virtual void BuildTerrain(int size, int positionX, int positionY, int heightScale, int deepenessLevel = 1)
    {
        Init(deepenessLevel);
        terrainSize = size;
        biomeMatrix = PerlinNoise();
        TerrainGenerator.SetData(biomeMatrix, heightScale, size, positionX, positionY);
        TerrainGenerator.SetTextures(TerrainTextures.ToArray());    
    }

    /// <summary>
    /// Fills the whole map with perlin noise
    /// </summary>
    public float[,] PerlinNoise()
    {
        float[,] matrix = new float[terrainSize, terrainSize];
        for (float i = 0; i < terrainSize; i++)
        {
            for (float j = 0; j < terrainSize; j++)
            {
                float xCoord = xOrg + i / terrainSize * scale;
                float yCoord = yOrg + j / terrainSize * scale;
                matrix[(int)i, (int)j] = Mathf.PerlinNoise(xCoord, yCoord);

                matrix[(int)i, (int)j] *= Mathf.Sin(matrix[(int)i, (int)j]);
                matrix[(int)i, (int)j] *= Mathf.Cos(matrix[(int)i, (int)j]);
            }
        }
        return ApplyMask(matrix);
    }

    /// <summary>
    ///     Apply's the selected mask to the Terrain
    /// </summary>
    public float[,] ApplyMask(float[,] matrix)
    {
        for (float i = 0; i < terrainSize; i++)
        {
            for (float j = 0; j < terrainSize; j++)
            {
                float distance_x = Mathf.Abs(i - terrainSize * 0.5f);
                float distance_y = Mathf.Abs(j - terrainSize * 0.5f);
                float distance = 0.0f;

                switch (TerrainMask)
                {
                    case Enums.MaskType.Circle:
                        distance = Mathf.Sqrt(distance_x * distance_x + distance_y * distance_y);
                        break;
                    case Enums.MaskType.Square:
                        distance = Mathf.Max(distance_x, distance_y);
                        break;
                    case Enums.MaskType.Rectangule:
                        distance = Mathf.Max(distance_x, distance_y / 1.5f);
                        break;
                    case Enums.MaskType.Diamond:
                        distance = distance_x + distance_y / DiamondRadius;
                        break;
                    case Enums.MaskType.Oval:
                        distance = Mathf.Sqrt(distance_x * distance_x / 2 + distance_y * distance_y);
                        break;
                    case Enums.MaskType.Flat:
                        matrix[(int)i, (int)j] = 0f;
                        continue;
                }

                float max_width = terrainSize * 0.5f - terrainRadius;
                float delta = distance / max_width;
                float gradient = delta * delta;

                matrix[(int)i, (int)j] *= Mathf.Max(0.0f, 1.0f - gradient);
            }
        }

        return matrix;
    }    
}
