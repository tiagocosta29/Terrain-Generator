using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TerrainGenerator : MonoBehaviour {

    public Terrain terrain;
    private TerrainData terrainData;

    public IslandGenerator island;
    public GameObject Water;
	
    public void SetData()
    {
        terrainData = new TerrainData();
        terrain.terrainData = terrainData;
        //Water.transform.position = new Vector3(island.TerrainSize / 1.5f, 1f, island.TerrainSize / 1.5f);
        //Water.transform.localScale = new Vector3(island.TerrainSize / 1.5f, 1f, island.TerrainSize / 1.5f);

        terrain.terrainData.heightmapResolution = island.TerrainSize + 1;
        terrain.terrainData.size = new Vector3(island.TerrainSize, island.Heightscale, island.TerrainSize);

        for (int i = 0; i < island.TerrainMatrix.GetLength(0); i++)
        {

            for (int j = 0; j < island.TerrainMatrix.GetLength(1); j++)
            {
                island.TerrainMatrix[i, j] /= island.Heightscale;
            }
        }
        terrain.terrainData.SetHeights(0, 0, island.TerrainMatrix);
    }
}
