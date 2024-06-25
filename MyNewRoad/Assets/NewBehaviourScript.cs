using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3[] points; // �������� ����� �������

    private Terrain terrain;
    private TerrainData terrainData;
    public float liftingHeight = 0.1f;
    private void Start()
    {
        points = new Vector3[4];
        points[0] = new Vector3(13395, 0, 13395);
        points[1] = new Vector3(13398, 0, 13395);
        points[2] = new Vector3(13398, 0, 13398);
        points[3] = new Vector3(13395, 0, 13398);

        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        // ������ ������ ������� ��� ��������� ������
        LiftMesh();
    }

    void LiftMesh()
    {
        // �������� ������ � ������ ��������
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        // �������� ������� ������ ��������
        float[,] heights = terrainData.GetHeights(0, 0, width, height);

        Vector3 terrainPosition = terrain.transform.position;

        // �������� �� ������ ���� ���������������� ����� � ������� � ��������� ��� ����� ����
        for (int i = 0; i < points.Length - 1; i++)
        {            
            // ������� ������� ��������, ����������� ����� A � B
            float minX = Mathf.Min(points[0].x, points[1].x, points[2].x, points[3].x);
            float minZ = Mathf.Min(points[0].z, points[1].z, points[2].z, points[3].z);
            float maxX = Mathf.Max(points[0].x, points[1].x, points[2].x, points[3].x);
            float maxZ = Mathf.Max(points[0].z, points[1].z, points[2].z, points[3].z);

            // �������� �� ���� ������ �������� � ��������� ��� ����� ������� A � B
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    // ��������� ������� ���������� ������� ����� �� ��������
                    Vector3 worldPos = new Vector3(terrainPosition.x + x * terrainData.heightmapScale.x, 0, terrainPosition.z + z * terrainData.heightmapScale.z);

                    // ���������, ��������� �� ����� ������ ��������� ��������
                    if (worldPos.x >= minX && worldPos.x <= maxX && worldPos.z >= minZ && worldPos.z <= maxZ)
                    {
                        
                        // ������������� ������ �����
                        heights[z, x] = liftingHeight;
                    }
                }
            }
        }

        // ��������� ���������� ������ � ��������
        terrain.terrainData.SetHeights(0, 0, heights);
    }
}
