using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IslandGenerator : MonoBehaviour {

    public int TerrainSize;
    
    public float[,] TerrainMatrix;

    public bool CanDraw = false;

    public Enums.MaskType Mask;

    public float Radius;
    public float DiamondRacio;

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.1F;
    public float Heightscale;


    private void Awake()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        TerrainMatrix = new float[TerrainSize, TerrainSize];
        FillWithNoise();

        CanDraw = true;
    }
	
    private void FillWithNoise()
    {
        // For each pixel in the texture...
        
        for (float i = 0; i < TerrainSize; i++)
        {
            for (float j = 0; j < TerrainSize; j++)
            {
                float xCoord = xOrg + i / TerrainSize * scale;
                float yCoord = yOrg + j / TerrainSize * scale;
                TerrainMatrix[(int)i, (int)j] = Mathf.PerlinNoise(xCoord, yCoord);// * Heightscale;
                //TerrainMatrix[(int)i, (int)j] *= Mathf.Sin(TerrainMatrix[(int)i, (int)j]);
                //TerrainMatrix[(int)i, (int)j] *= Mathf.Cos(TerrainMatrix[(int)i, (int)j]);

                //float y = (2 * Mathf.PI * i) / TerrainSize;
                //float x = (2 * Mathf.PI * j) / TerrainSize;
                //float result = (x + y) / 2;
                //TerrainMatrix[(int)i, (int)j] += result * 5;
            }
        }        

     //   ApplyMask();
    }

    /// <summary>
    ///     Apply's the selected mask to the Terrain
    /// </summary>
    public void ApplyMask()
    {
        for (int i = 0; i < TerrainSize; i++)
        {
            for (int j = 0; j < TerrainSize; j++)
            {
                float distance_x = Mathf.Abs(i - TerrainSize * 0.5f);
                float distance_y = Mathf.Abs(j - TerrainSize * 0.5f);
                float distance = 0.0f;

                switch (Mask)
                {
                    case Enums.MaskType.Circle:
                        distance = Mathf.Sqrt(distance_x * distance_x + distance_y * distance_y); 
                        break;
                    case Enums.MaskType.Square:
                        distance = Mathf.Max(distance_x, distance_y); 
                        break;
                    case Enums.MaskType.Rectangule:
                        distance = Mathf.Max(distance_x, distance_y / 2); 
                        break;
                    case Enums.MaskType.Diamond:
                        distance = distance_x + distance_y / DiamondRacio; 
                        break;
                    case Enums.MaskType.Oval:
                        distance = Mathf.Sqrt(distance_x * distance_x / 2 + distance_y * distance_y); 
                        break;
                    case Enums.MaskType.Test:
                       // distance = (Mathf.Sqrt(j * distance_y) / (Mathf.Sqrt(i * distance_x)) * 40f);
                        break;
                }

                float max_width = TerrainSize * 0.5f - Radius;
                float delta = distance / max_width;
                float gradient = delta * delta;

                TerrainMatrix[i, j] *= Mathf.Max(0.0f, 1.0f - gradient);
            }
        }
    }


    public void ReDraw()
    {
            CanDraw = true;

    }
}
