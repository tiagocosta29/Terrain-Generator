using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    public IslandGenerator island;

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;

    public float scale;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        
        if (island == null)
        {
            island = GetComponent<IslandGenerator>();
        }
    }

    private void CreateVertices()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int i = 0; i < island.TerrainSize; i++)
        {
            for (int j = 0; j < island.TerrainSize; j++)
            {
                vertices.Add(new Vector3(i *scale, j * scale, island.TerrainMatrix[i,j] * scale));
            }
        }

        for (int i = 0; i < vertices.Count ; i++)
        {
            if ((i + 1) % island.TerrainSize != 0)
            {
                triangles.Add(i);
                triangles.Add(i + island.TerrainSize);
                triangles.Add(i + 1);

                triangles.Add(i + 1);
                triangles.Add(i + island.TerrainSize);
                triangles.Add(i + island.TerrainSize + 1);
            }


            if (i + island.TerrainSize >= vertices.Count - 1)
            {
                break;
            }
        }

    }
    
    private void CreateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    private void Update()
    {
        if (island.CanDraw)
        {
            Debug.Log("CREATING MAP");
            CreateVertices();
            CreateMesh();
            this.gameObject.transform.position = new Vector3((-island.TerrainSize * scale / 2), 1, (island.TerrainSize * scale / 2));

            Debug.Log("MAP CREATED");
            island.CanDraw = false;
        }
    }


}
