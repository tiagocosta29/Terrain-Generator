using UnityEngine;

public class Biome : MonoBehaviour
{    
    [Range(0.0f, 1.0f)]
    public float TerrainMinHeight;

    [Range(0.0f, 1.0f)]
    public float TerrainMaxHeight;

    [Range(0f, 100f)]
    public float TerrainRadius;

    public Enums.MaskType TerrainMask;

    /// <summary>
    /// Biome Size
    /// </summary>
    private int terrainSize;

    /// <summary>
    /// Starting point of X position in the Biome
    /// </summary>
    private int posX;

    /// <summary>
    /// Starting point of Y position in the Biome
    /// </summary>
    private int posY;
    
    /// <summary>
    /// Creates a new perlin noise map
    /// </summary>
    /// <param name="size"></param>
    /// <param name="positionX"></param>
    /// <param name="positionY"></param>
    /// <param name="terrainMatrix"></param>
    public virtual float[,] BuildTerrain(int size, int positionX, int positionY, float[,] terrainMatrix)
    {
        terrainSize = size;
        posX = positionX;
        posY = positionY;
        float xOrg = 0.0f;
        float yOrg = 0.0f;
        float xCoord = 0.0f;
        float yCoord = 0.0f;

        for (int i = positionX; i < posX + terrainSize; i++)
        {
            for (int j = 0; j < posY + terrainSize; j++)
            {
                while(terrainMatrix[i,j] > TerrainMaxHeight && terrainMatrix[i,j] < TerrainMinHeight)
                {
                    xCoord = xOrg + i / terrainMatrix.GetLength(0);
                    yCoord = yOrg + j / terrainMatrix.GetLength(1);
                    terrainMatrix[(int)i, (int)j] = Mathf.PerlinNoise(xCoord, yCoord);
                }
            }
        }
        return terrainMatrix;
    }

    public virtual void DrawTextures()
    {

    }
}
