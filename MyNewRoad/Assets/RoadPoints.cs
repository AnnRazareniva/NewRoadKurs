using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SocialPlatforms;
using static SideRoad;
using System.Reflection;

public class RoadPoints : MonoBehaviour
{
    static string Path = "Resources/XYZpoints.txt";
    public GameObject pointPrefab;

    public static string[] lines = File.ReadAllLines(Path);
    [SerializeField]Vector3[] pointCoordinates = new Vector3[lines.Length];//ось дороги

    ///*[SerializeField] */Vector3[] points = new Vector3[4]; // Указываем размер массива

    //public static event System.Action OnRoadMeshInitialized;
    //Points for road
    public Vector3[] nARightEquidistantRoad = new Vector3[lines.Length];
    public Vector3[] nBRightEquidistantRoad = new Vector3[lines.Length];
    public Vector3[] nALeftEquidistantRoad = new Vector3[lines.Length];
    public Vector3[] nBLeftEquidistantRoad = new Vector3[lines.Length];

    //Mesh for road
    public Vector3[] RoadMeshL = new Vector3[1];//левая сторона дороги
    public Vector3[] RoadMeshR = new Vector3[1];//правая сторона дороги



    //Mesh for roadside
    private Vector3[] RoadsideMeshL = new Vector3[1];//левая сторона обочины
    private Vector3[] RoadsideMeshR = new Vector3[1];//правая сторона обочины
    //ОБОЧИНА

    private Vector3[] nARightEquidistantRoadside = new Vector3[lines.Length];
    private Vector3[] nBRightEquidistantRoadside = new Vector3[lines.Length];
    private Vector3[] nALeftEquidistantRoadside = new Vector3[lines.Length];
    private Vector3[] nBLeftEquidistantRoadside = new Vector3[lines.Length];



    private Mesh[] roadMeshes;

    private void GetCoord()
    {
        string[] coordinates = new string[3];
        float[] start = new float[3];
        float[] coord = new float[3];
        //float x, y, z;

        coordinates = lines[0].Split(' ');//начальные заданные координаты x z y
        
        for (int i = 0; i < 3; i++)
        {
            start[i] = float.Parse(coordinates[i]);//запоминаем нач знач x z y
            //Debug.Log(start[i]);
        }

        for (int i = 0; i < lines.Length; i++)
        {
            coordinates = lines[i].Split(' ');//все заданные координаты x z y

            for (int j = 0; j < 3; j++)//по икс[0], по зэд[1], по игрик[2] происходит сдвиг на нулевую точку
            {
                if (start[j] < 0)
                {
                    coord[j] = float.Parse(coordinates[j]) + Math.Abs(start[j]);
                }
                else
                {
                    coord[j] = float.Parse(coordinates[j]) - start[j];
                }

            }
            pointCoordinates[i] = new Vector3(coord[0], coord[2], coord[1]);// x y z           

            //Debug.Log(pointCoordinates[i]);
        }
    }//координаты оси дороги

    public (Vector3[], Vector3[]) CalcRightRoadEquidistant(Vector3[] pointCoord, float widht, float high)//переделать с параметрами
    {
        Vector3[] nARight = new Vector3[pointCoord.Length];
        Vector3[] nBRight = new Vector3[pointCoord.Length];

        for (int i = 1; i < pointCoord.Length; i++)
        {
            Vector3 avec = new Vector3(pointCoord[i].x - pointCoord[i - 1].x, pointCoord[i].y - pointCoord[i - 1].y, pointCoord[i].z - pointCoord[i - 1].z);

            double a = Math.Sqrt(Math.Pow(avec.x, 2) + Math.Pow(avec.y, 2) + Math.Pow(avec.z, 2));


            Vector3 nvecR = new Vector3((float)(avec.z / a), (float)(avec.y / a), -(float)(avec.x / a));

            //widht это ширина полосы дороги
            //high это высота у дороги
            Vector3 vecAR = new Vector3(pointCoord[i - 1].x + widht * nvecR.x, pointCoord[i - 1].y + nvecR.y * high, pointCoord[i - 1].z + widht * nvecR.z);
            Vector3 vecBR = new Vector3(pointCoord[i].x + widht * nvecR.x, pointCoord[i].y + nvecR.y * high, pointCoord[i].z + widht * nvecR.z);

            nARight[i - 1] = vecAR;
            nBRight[i - 1] = vecBR;
           
        }

        return (nARight, nBRight);
    }

    public (Vector3[], Vector3[]) CalcLeftRoadEquidistant(Vector3[] pointCoord, float widht, float high)//переделать с параметрами
    {
        Vector3[] nALeft = new Vector3[pointCoord.Length];
        Vector3[] nBLeft = new Vector3[pointCoord.Length];
        for (int i = 1; i < pointCoord.Length; i++)
        {
            Vector3 avec = new Vector3(pointCoord[i].x - pointCoord[i - 1].x, pointCoord[i].y - pointCoord[i - 1].y, pointCoord[i].z - pointCoord[i - 1].z);
            //Debug.Log(avec);
            double a = Math.Sqrt(Math.Pow(avec.x, 2) + Math.Pow(avec.y, 2) + Math.Pow(avec.z, 2));

            Vector3 nvecL = new Vector3(-(float)(avec.z / a), (float)(avec.y / a), (float)(avec.x / a));

            //widht это ширина полосы дороги
            //high это высота у дороги
            Vector3 vecAL = new Vector3(pointCoord[i - 1].x + widht * nvecL.x, pointCoord[i - 1].y + nvecL.y * high, pointCoord[i - 1].z + widht * nvecL.z);
            Vector3 vecBL = new Vector3(pointCoord[i].x + widht * nvecL.x, pointCoord[i].y + nvecL.y * high, pointCoord[i].z + widht * nvecL.z);

            nALeft[i - 1] = vecAL;
            nBLeft[i - 1] = vecBL;
           
        }
        return (nALeft, nBLeft);

    }
    public Vector3[] MeshNearPoints(Vector3[] nA, Vector3[] nB, int count, Vector3[] mainLine, float width)
    {

        Vector3[] M = new Vector3[count];
        //int k = 0;// index for meshPoints

        for (int i = 0; i < count; i++) 
        {
            if (i == 0)
            {
                M[i] = nA[i]; 
            }
            else
            {
                if (i == count - 1)
                {
                    M[i] = nB[i-1];
                }
                else
                {
                    if (nA[i].x < nB[i - 1].x) //по иксу, т.к. х это путь вперёд(сгиб внутрь)
                    {
                        Vector3 mid;
                        float midx, midy, midz;

                        midx = (nA[i].x + nB[i - 1].x) / 2;
                        midy = (nA[i].y + nB[i - 1].y) / 2;
                        midz = (nA[i].z + nB[i - 1].z) / 2;

                        mid = new Vector3(midx, midy, midz);

                        Vector3 avec = new Vector3(mid.x - mainLine[i].x, mid.y - mainLine[i].y, mid.z - mainLine[i].z);//разница между сереждиной и осью
                        Vector3 normalizedDirection = avec.normalized;
                        Vector3 C = mainLine[i] + normalizedDirection * width;

                        M[i] = C; 
                    }
                    else
                    {
                        if (nA[i].x > nB[i - 1].x)//по иксу, т.к. х это путь вперёд(сгиб наружу)
                        {
                            Vector3 mid;
                            float midx, midy, midz;

                            midx = (nA[i].x + nB[i - 1].x) / 2;
                            midy = (nA[i].y + nB[i - 1].y) / 2;
                            midz = (nA[i].z + nB[i - 1].z) / 2;

                            mid = new Vector3(midx, midy, midz);

                            Vector3 avec = new Vector3(mid.x - mainLine[i].x, mid.y - mainLine[i].y, mid.z - mainLine[i].z);//разница между сереждиной и осью
                            Vector3 normalizedDirection = avec.normalized;
                            Vector3 C = mainLine[i] + normalizedDirection * width;

                            M[i] = C;       
                        }
                        else
                        {
                            if (nA[i].x == nB[i - 1].x)//одна и та же точка
                            {
                                M[i] = nA[i];
                            }
                        }
                    }
                }
            }
        }

        return M;
    }

    private Mesh RoadMeshCreate(int index)//0, 1, 2
    {
        Mesh mesh = new Mesh();
        int meshCount;
        Vector3[] MeshNearPoints;
        int[] triangle;

        if (index == 0)
        {
            meshCount = RoadMeshR.Length + RoadMeshL.Length + pointCoordinates.Length;
            MeshNearPoints = new Vector3[meshCount];

            for (int i = 0; i < RoadMeshR.Length; i++)
            {
                MeshNearPoints[i] = RoadMeshR[i];
            }

            for (int i = 0; i < pointCoordinates.Length; i++)
            {
                MeshNearPoints[i + RoadMeshR.Length] = pointCoordinates[i];
            }

            for (int i = 0; i < RoadMeshL.Length; i++)
            {
                MeshNearPoints[i + RoadMeshR.Length + pointCoordinates.Length] = RoadMeshL[i];
            }

            triangle = new int[2 * (RoadMeshR.Length - 1) * 6];//количество квадов
            for (int ti = 0, vi = 0, i = 0; i < 2; i++, vi++)
            {
                for (int k = 0; k < RoadMeshR.Length - 1; k++, ti += 6, vi++)
                {
                    triangle[ti] = vi;
                    triangle[ti + 1] = vi + RoadMeshR.Length;
                    triangle[ti + 2] = vi + 1;
                    triangle[ti + 3] = vi + RoadMeshR.Length;
                    triangle[ti + 4] = vi + RoadMeshR.Length + 1;
                    triangle[ti + 5] = vi + 1;
                }
            }

            mesh.vertices = MeshNearPoints;
            mesh.triangles = triangle;
            mesh.RecalculateNormals();
        }
        else
            if (index == 1)
        {
            meshCount = RoadsideMeshL.Length + RoadMeshL.Length;
            MeshNearPoints = new Vector3[meshCount];
            Debug.Log(RoadsideMeshL.Length);
            Debug.Log(RoadMeshL.Length);



            for (int i = 0; i < RoadMeshL.Length; i++)
            {
                MeshNearPoints[i] = RoadMeshL[i];
            }

            for (int i = 0; i < RoadsideMeshL.Length; i++)
            {
                MeshNearPoints[i + RoadMeshL.Length] = RoadsideMeshL[i];
            }

            triangle = new int[1 * (RoadMeshL.Length - 1) * 6];//количество квадов !!!!//1 или 2 это количество строк(то есть либо 2 полосы дороги либо 1
            for (int ti = 0, vi = 0, i = 0; i < 1; i++, vi++)
            {
                for (int k = 0; k < RoadMeshL.Length - 1; k++, ti += 6, vi++)
                {
                    triangle[ti] = vi;
                    triangle[ti + 1] = vi + RoadMeshL.Length;
                    triangle[ti + 2] = vi + 1;
                    triangle[ti + 3] = vi + RoadMeshL.Length;
                    triangle[ti + 4] = vi + RoadMeshL.Length + 1;
                    triangle[ti + 5] = vi + 1;
                }
            }
            mesh.vertices = MeshNearPoints;
            mesh.triangles = triangle;
            mesh.RecalculateNormals();
        }
        else
        if (index == 2)
        {
            meshCount = RoadsideMeshR.Length + RoadMeshR.Length;
            MeshNearPoints = new Vector3[meshCount];
            Debug.Log(RoadsideMeshR.Length);
            Debug.Log(RoadMeshR.Length);

            //для стороны справа от сои дороги приходится идти от обратного, от точек только что найдённых эквидистантов до точек старых(дороги) т.к. построение треугольников по вершинам происходит справа на лево

            for (int i = 0; i < RoadsideMeshR.Length; i++)
            {
                MeshNearPoints[i] = RoadsideMeshR[i];
            }

            for (int i = 0; i < RoadMeshR.Length; i++)
            {
                MeshNearPoints[i + RoadsideMeshR.Length] = RoadMeshR[i];
            }

            triangle = new int[1 * (RoadMeshR.Length - 1) * 6];//количество квадов !!!!//1 или 2 это количество строк(то есть либо 2 полосы дороги либо 1
            for (int ti = 0, vi = 0, i = 0; i < 1; i++, vi++)
            {
                for (int k = 0; k < RoadMeshR.Length - 1; k++, ti += 6, vi++)
                {
                    triangle[ti] = vi;
                    triangle[ti + 1] = vi + RoadMeshR.Length;
                    triangle[ti + 2] = vi + 1;
                    triangle[ti + 3] = vi + RoadMeshR.Length;
                    triangle[ti + 4] = vi + RoadMeshR.Length + 1;
                    triangle[ti + 5] = vi + 1;
                }
            }

            mesh.vertices = MeshNearPoints;
            mesh.triangles = triangle;
            mesh.RecalculateNormals();
        }
            return mesh;
    }

    private void AssignMeshesToObjects(int index)//0, 1, 2
    {
        // Присваивание мешей каждому найденному объекту
        if (index == 0)
        {
            GameObject roadObject = GameObject.Find("Road");

            MeshFilter meshFilter = roadObject.GetComponent<MeshFilter>();
            //MeshRenderer meshRenderer = roadObject.GetComponent<MeshRenderer>();

            //int SideRoadL;
            if (meshFilter != null)
            {
                meshFilter.mesh = roadMeshes[index]; // Присваивание меша
            }
            else
            {
                Debug.LogWarning("MeshFilter component not found on RoadObject.");
            }
        }
        else
            if (index == 1)
        {
            GameObject roadObject = GameObject.Find("SideRoadL");
            MeshFilter meshFilter = roadObject.GetComponent<MeshFilter>();
            //MeshRenderer meshRenderer = roadObject.GetComponent<MeshRenderer>();

            //int SideRoadL;
            if (meshFilter != null)
            {
                meshFilter.mesh = roadMeshes[index]; // Присваивание меша
            }
            else
            {
                Debug.LogWarning("MeshFilter component not found on RoadObject.");
            }
        }
        else
            if (index == 2)
        {
            GameObject roadObject = GameObject.Find("SideRoadR");
            MeshFilter meshFilter = roadObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = roadObject.GetComponent<MeshRenderer>();

            if (meshFilter != null)
            {
                meshFilter.mesh = roadMeshes[index]; // Присваивание меша
            }
            else
            {
                Debug.LogWarning("MeshFilter component not found on RoadObject.");
            }
        }


    }

    //public void RoatMeshCreate()//creating mesh
    //{

    //    //МЕШ ДЛЯ ДОРОГИ
    //    Mesh Roadmesh = new Mesh();
    //    GetComponent<MeshFilter>().mesh = Roadmesh;
    //    Roadmesh.name = "Road";

    //    int meshCount = RoadMeshR.Length + RoadMeshL.Length + pointCoordinates.Length;
    //    Vector3[] MeshNearPoints = new Vector3[meshCount];

    //    for (int i = 0; i < RoadMeshR.Length; i++)
    //    {
    //        MeshNearPoints[i] = RoadMeshR[i];
    //    }

    //    for (int i = 0; i < pointCoordinates.Length; i++)
    //    {
    //        MeshNearPoints[i + RoadMeshR.Length] = pointCoordinates[i];
    //    }

    //    for (int i = 0; i < RoadMeshL.Length; i++)
    //    {
    //        MeshNearPoints[i + RoadMeshR.Length + pointCoordinates.Length] = RoadMeshL[i];
    //    }

    //    int[] triangle = new int[2 * (RoadMeshR.Length - 1) * 6];//количество квадов
    //    for (int ti = 0, vi = 0, i = 0; i < 2; i++, vi++)
    //    {
    //        for (int k = 0; k < RoadMeshR.Length - 1; k++, ti+=6, vi++)
    //        {
    //            triangle[ti] = vi;
    //            triangle[ti + 1] = vi + RoadMeshR.Length;
    //            triangle[ti + 2] = vi + 1;
    //            triangle[ti + 3] = vi + RoadMeshR.Length;
    //            triangle[ti + 4] = vi + RoadMeshR.Length + 1;
    //            triangle[ti + 5] = vi + 1;
    //        }
    //    }

    //    Roadmesh.vertices = MeshNearPoints;
    //    Roadmesh.triangles = triangle;
    //    Roadmesh.RecalculateNormals();

    //    //CombineInstance[] combine = new CombineInstance[2]; // Создаем массив CombineInstance для объединения двух мешей
    //    //combine[0].mesh = Roadmesh; // Устанавливаем первый меш в массиве как меш дороги
    //    //combine[1].mesh = Roadsidemesh; // Устанавливаем второй меш в массиве как новый меш

    //    //Matrix4x4 matrix = transform.worldToLocalMatrix; // Получаем матрицу преобразования
    //    //combine[1].transform = matrix; // Применяем матрицу преобразования ко второму мешу

    //    //Mesh combinedMesh = new Mesh();

    //    //combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

    //    //GetComponent<MeshFilter>().mesh = combinedMesh;
    //    //combinedMesh.name = "RoadcombinedMesh";

    //    //combinedMesh.CombineMeshes(combine); // Объединяем меши
    //}

    //public void RoadsideLeftMeshCreate()//creating mesh
    //{
    //    Mesh Roadsidemesh = new Mesh();
    //    GetComponent<MeshFilter>().mesh = Roadsidemesh;
    //    Roadsidemesh.name = "Roadside";

    //    int meshCount = RoadsideMeshL.Length + RoadMeshL.Length;
    //    Vector3[] MeshNearPoints = new Vector3[meshCount];

    //    for (int i = 0; i < RoadMeshL.Length; i++)
    //    {
    //        MeshNearPoints[i] = RoadMeshL[i];
    //    }

    //    for (int i = 0; i < RoadsideMeshL.Length; i++)
    //    {
    //        MeshNearPoints[i + RoadMeshL.Length] = RoadsideMeshL[i];
    //    }

    //    int[] triangle = new int[2 * (RoadMeshL.Length - 1) * 6];//количество квадов
    //    for (int ti = 0, vi = 0, i = 0; i < 2; i++, vi++)
    //    {
    //        for (int k = 0; k < RoadMeshL.Length - 1; k++, ti += 6, vi++)
    //        {
    //            triangle[ti] = vi;
    //            triangle[ti + 1] = vi + RoadMeshL.Length;
    //            triangle[ti + 2] = vi + 1;
    //            triangle[ti + 3] = vi + RoadMeshL.Length;
    //            triangle[ti + 4] = vi + RoadMeshL.Length + 1;
    //            triangle[ti + 5] = vi + 1;
    //        }
    //    }

    //    Roadsidemesh.vertices = MeshNearPoints;
    //    Roadsidemesh.triangles = triangle;
    //    Roadsidemesh.RecalculateNormals();

    //}



    //public void RoadSideMeshCreate()//creating mesh
    //{
    //    //МЕШ ДЛЯ ОБОЧИНЫ ЛЕВОЙ

    //    Mesh Roadsidemesh = new Mesh();
    //    GetComponent<MeshFilter>().mesh = Roadsidemesh;
    //    Roadsidemesh.name = "SideRoad";

    //    int meshCount1 = RoadsideMeshL.Length + RoadMeshL.Length;

    //    Debug.Log(RoadsideMeshL.Length);
    //    Debug.Log(RoadMeshL.Length);

    //    Vector3[] MeshNearPoints1 = new Vector3[meshCount1];

    //    for (int i = 0; i < RoadMeshL.Length; i++)
    //    {
    //        MeshNearPoints1[i] = RoadMeshL[i];
    //    }

    //    for (int i = 0; i < RoadsideMeshL.Length; i++)
    //    {
    //        MeshNearPoints1[i + RoadMeshL.Length] = RoadsideMeshL[i];
    //    }

    //    int[] triangle1 = new int[1 * (RoadMeshL.Length - 1) * 6];//количество квадов !!!!//1 или 2 это количество строк(то есть либо 2 полосы дороги либо 1
    //    for (int ti = 0, vi = 0, i = 0; i < 1; i++, vi++)
    //    {
    //        for (int k = 0; k < RoadMeshL.Length - 1; k++, ti += 6, vi++)
    //        {
    //            triangle1[ti] = vi;
    //            triangle1[ti + 1] = vi + RoadMeshL.Length;
    //            triangle1[ti + 2] = vi + 1;
    //            triangle1[ti + 3] = vi + RoadMeshL.Length;
    //            triangle1[ti + 4] = vi + RoadMeshL.Length + 1;
    //            triangle1[ti + 5] = vi + 1;
    //        }
    //    }
    //    Roadsidemesh.vertices = MeshNearPoints1;
    //    Roadsidemesh.triangles = triangle1;
    //    Roadsidemesh.RecalculateNormals();
    //    // Объединяем меши

    //}
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

    private void OnDrawGizmos()
    {
        //координаты оси дороги
        //Gizmos.color = Color.yellow;
        //for (int i = 0; i < pointCoordinates.Length; i++)
        //{

        //    Gizmos.DrawSphere(pointCoordinates[i], 0.05f);
        //}

        //ось дороги
        //Gizmos.color = Color.white;
        //for (int i = 1; i < pointCoordinates.Length; i++)
        //{

        //    Gizmos.DrawLine(pointCoordinates[i - 1], pointCoordinates[i]);
        //}

        //точки эквидистант
        /*Gizmos.color = Color.blue;
        for (int i = 1; i < pointCoordinates.Length; i++)
        {
            

            //right
            Gizmos.DrawSphere(nAR[i - 1], 0.05f);
            Gizmos.DrawSphere(nBR[i - 1], 0.05f);

            //left
            Gizmos.DrawSphere(nAL[i - 1], 0.05f);
            Gizmos.DrawSphere(nBL[i - 1], 0.05f);

        }*/

        //ЭКВИДИСТАНТЫ
        //Gizmos.color = Color.white;
        //for (int i = 1; i < pointCoordinates.Length; i++)
        //{

        //    Gizmos.DrawLine(nARightEquidistantRoad[i - 1], nBRightEquidistantRoad[i - 1]);//right
        //    Gizmos.DrawLine(nALeftEquidistantRoad[i - 1], nBLeftEquidistantRoad[i - 1]);//left
        //}
        //for (int i = 1; i < RoadMeshL.Length; i++)
        //{

        //    Gizmos.DrawLine(nARightEquidistantRoad[i - 1], nBRightEquidistantRoad[i - 1]);//right
        //    Gizmos.DrawLine(nALeftEquidistantRoad[i - 1], nBLeftEquidistantRoad[i - 1]);//left
        //}
        //точки эквидистант
        //Gizmos.color = Color.blue;
        //for (int i = 1; i < RoadMeshL.Length; i++)
        //{
        //    //right
        //    Gizmos.DrawSphere(RoadsideMeshR[i - 1], 0.05f);
        //    Gizmos.DrawSphere(RoadsideMeshL[i - 1], 0.05f);
        //}

        //прямые между кусками AiBi
        /*for (int i = 1; i < pointCoordinates.Length - 1; i++)
        {
            Gizmos.DrawLine(nAR[i], nBR[i - 1]);//right
            Gizmos.DrawLine(nAL[i], nBL[i - 1]);//left
        }*/

        //перпендикуляры
        /*Gizmos.color = Color.blue;
        for (int i = 1; i < pointCoordinates.Length; i++)
        {


            //right
            Gizmos.DrawLine(nAR[i - 1], pointCoordinates[i - 1]);
            Gizmos.DrawLine(nBR[i - 1], pointCoordinates[i]);

            //left
            Gizmos.DrawLine(nAL[i - 1], pointCoordinates[i - 1]);
            Gizmos.DrawLine(nBL[i - 1], pointCoordinates[i]);

        }*/

        //Точки середины между перпендикулярами
        /*Gizmos.color = Color.black;
        for (int i = 0; i < RML.Length; i++)
        {
            //left
            Gizmos.DrawSphere(RML[i], 0.05f);
        }
        for (int i = 0; i < RMR.Length; i++)
        {
            //right
            Gizmos.DrawSphere(RMR[i], 0.05f);
        }*/

        /*//Gizmos.color = Color.red;
        //for (int i = 1; i < RMR.Length; i++)
        //{
        //    Gizmos.DrawLine(RMR[i - 1], RMR[i]);
        //}
        //for (int i = 1; i < RML.Length; i++)
        //{
        //    Gizmos.DrawLine(RML[i - 1], RML[i]);
        //}*/
    }

    //private void PointMesh1()//подсчёт 
    //{
    //    var resultRoadLeft = CalcLeftRoadEquidistant(pointCoordinates, (float)3.5, (float)-0.2);//вычисление точек координат для левой линии дороги(проезжая часть)
    //    nALeftEquidistantRoad = resultRoadLeft.Item1;
    //    nBLeftEquidistantRoad = resultRoadLeft.Item2;

    //    Debug.Log(pointCoordinates.Length);
    //    Debug.Log(nALeftEquidistantRoad.Length);

    //    var resultRoadRight = CalcRightRoadEquidistant(pointCoordinates, (float)3.5, (float)-0.2);
    //    nARightEquidistantRoad = resultRoadRight.Item1;
    //    nBRightEquidistantRoad = resultRoadRight.Item2;

    //    RoadMeshL = MeshNearPoints(nALeftEquidistantRoad, nBLeftEquidistantRoad, nALeftEquidistantRoad.Length, pointCoordinates, (float)3.5);
    //    RoadMeshR = MeshNearPoints(nARightEquidistantRoad, nBRightEquidistantRoad, nARightEquidistantRoad.Length, pointCoordinates, (float)3.5);

    //}

    private void PointMesh(Vector3[] RoadMeshLeft, Vector3[] RoadMeshRight, float high, float weight, int index)//0, 1, 
    {
        var resultSideLeft = CalcLeftRoadEquidistant(RoadMeshLeft, weight, high);
        var resultSideRight = CalcRightRoadEquidistant(RoadMeshRight, weight, high);
        Debug.Log("nALeftEquidistantRoadside");
        nALeftEquidistantRoad = resultSideLeft.Item1;
        nBLeftEquidistantRoad = resultSideLeft.Item2;

        nARightEquidistantRoad = resultSideRight.Item1;
        nBRightEquidistantRoad = resultSideRight.Item2; 

        if(index == 0)
        {      
            RoadMeshL = MeshNearPoints(nALeftEquidistantRoad, nBLeftEquidistantRoad, nALeftEquidistantRoad.Length, RoadMeshLeft, weight);
            RoadMeshR = MeshNearPoints(nARightEquidistantRoad, nBRightEquidistantRoad, nARightEquidistantRoad.Length, RoadMeshRight, weight);
        }
        else 
            if(index == 1)
        {        
            RoadsideMeshL = MeshNearPoints(nALeftEquidistantRoad, nBLeftEquidistantRoad, nALeftEquidistantRoad.Length, RoadMeshLeft, weight);
        }
        else
            if (index == 2)
        {
            RoadsideMeshR = MeshNearPoints(nARightEquidistantRoad, nBRightEquidistantRoad, nARightEquidistantRoad.Length, RoadMeshRight, weight);
        }



    }

    private void Start()
    {
        roadMeshes = new Mesh[5];
        
        //ROAD
        GetCoord();

        PointMesh(pointCoordinates, pointCoordinates, (float)-0.2, (float)3.5, 0);
        roadMeshes[0] = RoadMeshCreate(0);
        AssignMeshesToObjects(0);


        Debug.Log("Yes");

        PointMesh(RoadMeshL, RoadMeshR, (float)-0.4, 2, 1);
        roadMeshes[1] = RoadMeshCreate(1);
        AssignMeshesToObjects(1);

        PointMesh(RoadMeshL, RoadMeshR, (float)-0.4, 2, 2);
        roadMeshes[2] = RoadMeshCreate(2);
        AssignMeshesToObjects(2);

    }


}
