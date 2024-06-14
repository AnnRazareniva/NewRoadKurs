using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoadPoints;

public class SideRoad : MonoBehaviour
{
    RoadPoints roadPoints;

    //Points for roadside
    public Vector3[] nARightEquidistantRoadside;
    public Vector3[] nBRightEquidistantRoadside;
    public Vector3[] nALeftEquidistantRoadside;
    public Vector3[] nBLeftEquidistantRoadside;
    
    //Mesh for roadside
    public Vector3[] RoadsideMeshL = new Vector3[1];
    public Vector3[] RoadsideMeshR = new Vector3[1];
   
    public void RoadSideMeshCreate()//creating mesh
    {
        //МЕШ ДЛЯ ОБОЧИНЫ ЛЕВОЙ

        Mesh Roadsidemesh = new Mesh();
        GetComponent<MeshFilter>().mesh = Roadsidemesh;
        Roadsidemesh.name = "SideRoad";

        int meshCount1 = RoadsideMeshL.Length + roadPoints.RoadMeshL.Length;

        Debug.Log(RoadsideMeshL.Length);
        Debug.Log(roadPoints.RoadMeshL.Length);

        Vector3[] MeshNearPoints1 = new Vector3[meshCount1];

        for (int i = 0; i < roadPoints.RoadMeshL.Length; i++)
        {
            MeshNearPoints1[i] = roadPoints.RoadMeshL[i];
        }

        for (int i = 0; i < RoadsideMeshL.Length; i++)
        {
            MeshNearPoints1[i + roadPoints.RoadMeshL.Length] = RoadsideMeshL[i];
        }

        int[] triangle1 = new int[1 * (roadPoints.RoadMeshL.Length - 1) * 6];//количество квадов !!!!//1 или 2 это количество строк(то есть либо 2 полосы дороги либо 1
        for (int ti = 0, vi = 0, i = 0; i < 1; i++, vi++)
        {
            for (int k = 0; k < roadPoints.RoadMeshL.Length - 1; k++, ti += 6, vi++)
            {
                triangle1[ti] = vi;
                triangle1[ti + 1] = vi + roadPoints.RoadMeshL.Length;
                triangle1[ti + 2] = vi + 1;
                triangle1[ti + 3] = vi + roadPoints.RoadMeshL.Length;
                triangle1[ti + 4] = vi + roadPoints.RoadMeshL.Length + 1;
                triangle1[ti + 5] = vi + 1;
            }
        }
        Roadsidemesh.vertices = MeshNearPoints1;
        Roadsidemesh.triangles = triangle1;
        Roadsidemesh.RecalculateNormals();
        // Объединяем меши

    }
    //private void OnDrawGizmos()
    //{
    //    //ЭКВИДИСТАНТЫ
    //    Gizmos.color = Color.white;
    //    for (int i = 1; i < roadPoints.RoadMeshL.Length; i++)
    //    {

    //        Gizmos.DrawLine(nARightEquidistantRoadside[i - 1], nBRightEquidistantRoadside[i - 1]);//right
    //        Gizmos.DrawLine(nALeftEquidistantRoadside[i - 1], nBLeftEquidistantRoadside[i - 1]);//left
    //    }
    //}
    
    void Start()
    {
        RoadPoints[] roadPointsComponents = FindObjectsOfType<RoadPoints>();

        if (roadPoints == null)
        {
            Debug.LogError("RoadPoints component is not found on the same GameObject as RoadSide.");
            return;
        }

        // Подписываемся на событие завершения инициализации второго скрипта
        HandleRoadMeshInitialized();
        
    }
    public void HandleRoadMeshInitialized()
    {
        if (roadPoints == null)
        {
            Debug.LogError("RoadPoints component is not initialized properly.");
            return;
        }

        nARightEquidistantRoadside = new Vector3[roadPoints.RoadMeshL.Length];
        nBRightEquidistantRoadside = new Vector3[roadPoints.RoadMeshL.Length];
        nALeftEquidistantRoadside = new Vector3[roadPoints.RoadMeshL.Length];
        nBLeftEquidistantRoadside = new Vector3[roadPoints.RoadMeshL.Length];
        Debug.Log("nALeftEquidistantRoadside");
        
        var resultSideLeft = roadPoints.CalcLeftRoadEquidistant(roadPoints.RoadMeshL, (float)2, (float)-0.4);
        nALeftEquidistantRoadside = resultSideLeft.Item1;
        nBLeftEquidistantRoadside = resultSideLeft.Item2;
        
        Debug.Log(nALeftEquidistantRoadside);
        Debug.Log(nBLeftEquidistantRoadside);

        var resultSideRight = roadPoints.CalcRightRoadEquidistant(roadPoints.RoadMeshR, (float)2, (float)-0.4);
        nARightEquidistantRoadside = resultSideRight.Item1;
        nBRightEquidistantRoadside = resultSideRight.Item2;

        RoadsideMeshL = roadPoints.MeshNearPoints(nALeftEquidistantRoadside, nBLeftEquidistantRoadside, nALeftEquidistantRoadside.Length, roadPoints.RoadMeshL, 2);
        RoadsideMeshR = roadPoints.MeshNearPoints(nARightEquidistantRoadside, nBRightEquidistantRoadside, nARightEquidistantRoadside.Length, roadPoints.RoadMeshR, 2);

        if (nARightEquidistantRoadside == null || nBRightEquidistantRoadside == null || nALeftEquidistantRoadside == null || nBLeftEquidistantRoadside == null)
        {
            Debug.LogError("One or more arrays are not initialized.");
            return;
        }

        RoadSideMeshCreate();
    }
}
