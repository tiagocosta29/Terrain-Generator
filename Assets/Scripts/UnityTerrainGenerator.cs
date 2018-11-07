using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnityTerrainGenerator : MonoBehaviour
{
    private GameObject terrainPrefab;
    private TerrainData terrainData;
    private Terrain terrain;
    private int terrainMaxHeight;

    public void Init()
    {
        terrainPrefab = new GameObject("Terrain");
        terrainPrefab.AddComponent<Terrain>();
        terrain = terrainPrefab.GetComponent<Terrain>();
    }

    /// <summary>
    /// Builds the Mesh for the terrain
    /// </summary>
    /// <param name="terrainMatrix">Matrix with heithmap data</param>
    public void SetData(float[,] terrainMatrix, int heightScale, int terrainSize, int xpos, int ypos)
    {
        Init();
        terrainMaxHeight = heightScale;
        terrainPrefab.transform.position = new Vector3(xpos, 0, ypos);
        terrainData = new TerrainData();
        terrain.terrainData = terrainData;

        terrain.terrainData.heightmapResolution = terrainSize + 1;
        terrain.terrainData.size = new Vector3(terrainSize, heightScale, terrainSize);

        terrain.terrainData.SetHeights(0, 0, terrainMatrix);
    }

    public void SetTextures(Texture2D[] textures)
    {
        List<SplatPrototype> terrainTextures = new List<SplatPrototype>();

        for (int i = 0; i < textures.Length; i++)
        {
            terrainTextures.Add(new SplatPrototype());
            terrainTextures[i].texture = textures[i];
            terrainTextures[i].tileOffset = new Vector2(0, 0);
            terrainTextures[i].tileSize = new Vector2(15, 15);
            terrainTextures[i].texture.Apply();
        }

        terrain.terrainData.splatPrototypes = terrainTextures.ToArray();
        SetSplatMap();
    }

    public void SetSplatMap()
    {
        // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y / (float)terrainData.alphamapHeight;
                float x_01 = (float)x / (float)terrainData.alphamapWidth;

                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapHeight), Mathf.RoundToInt(x_01 * terrainData.heightmapWidth));

                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[terrainData.alphamapLayers];

                float influence = height / terrainMaxHeight;

                splatWeights[0] = 0;
                splatWeights[1] = 0;
                splatWeights[2] = 0;

                if (influence < 0.1f)
                {
                    splatWeights[0] = 1;
                }
                else
                if (influence < 0.5f)
                {
                    splatWeights[1] = 1;
                }
                else
                {
                    splatWeights[2] = 1;
                }


                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {

                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;

                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }

        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }
}
