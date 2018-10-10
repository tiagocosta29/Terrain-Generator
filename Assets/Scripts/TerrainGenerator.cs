using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    public Terrain terrain;

    public IslandGenerator island;
    public GameObject Water;
    // Use this for initialization
    void Start () {
    }
	
    public void SetData()
    {
        Water.transform.position = new Vector3(island.TerrainSize / 2, 1, island.TerrainSize / 2);
        Water.transform.localScale = new Vector3(island.TerrainSize / 2, 1, island.TerrainSize / 2);

        terrain.terrainData.heightmapResolution = island.TerrainSize + 1;
        terrain.terrainData.size = new Vector3(island.TerrainSize, island.Heightscale, island.TerrainSize);
        terrain.terrainData.SetHeights(0, 0, island.TerrainMatrix);
    }
}
