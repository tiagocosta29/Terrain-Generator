using UnityEngine;

public class UnityTerrainGenerator : MonoBehaviour
{
    public Terrain terrain;
    private TerrainData terrainData;
    
    /// <summary>
    /// Builds the Mesh for the terrain
    /// </summary>
    /// <param name="terrainMatrix">Matrix with heithmap data</param>
    public void SetData(float[,] terrainMatrix, int heightScale, int terrainSize)
    {      
        terrainData = new TerrainData();
        terrain.terrainData = terrainData;
        
        terrain.terrainData.heightmapResolution = terrainSize + 1;
        terrain.terrainData.size = new Vector3(terrainSize, heightScale, terrainSize);

        terrain.terrainData.SetHeights(0, 0, terrainMatrix);
    }    
}
