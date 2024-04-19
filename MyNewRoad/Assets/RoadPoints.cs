using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SocialPlatforms;

public class RoadPoints : MonoBehaviour
{
    static string Path = "Resources/XYZpoints.txt";
    public GameObject pointPrefab;

    static string[] lines = File.ReadAllLines(Path);
    [SerializeField]Vector3[] pointCoordinates = new Vector3[lines.Length];

    ///*[SerializeField] */Vector3[] points = new Vector3[4]; // ”казываем размер массива

    Vector3[] nAR = new Vector3[lines.Length];
    Vector3[] nBR = new Vector3[lines.Length];
    Vector3[] nAL = new Vector3[lines.Length];
    Vector3[] nBL = new Vector3[lines.Length];

    Vector3[] RML = new Vector3[1];
    Vector3[] RMR = new Vector3[1];

    /*public void GetCoordTEST()
    {
        points[0] = new Vector3(0, 0, 1);
        points[1] = new Vector3(1, 1, 1);
        points[2] = new Vector3(3, 1, 1);
        points[3] = new Vector3(4, 1, 2);
    }*/

    public void GetCoord()
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


    public (Vector3[], Vector3[]) CalcRightEquidistant(Vector3[] pointCoord)//переделать с параметрами
    {
        /*Vector3[] avec = new Vector3[lines.Length];
        double[] a = new double[lines.Length];
        for (int i = 1; i < lines.Length; i++)
        {
            avec[i] = new Vector3(pointCoordinates[i].x - pointCoordinates[i - 1].x, pointCoordinates[i].y - pointCoordinates[i - 1].y, pointCoordinates[i].z - pointCoordinates[i - 1].z);
            //Debug.Log(avec);
            a[i] = Math.Sqrt(Math.Pow((double)avec[i].x, 2) + Math.Pow((double)avec[i].y, 2) + Math.Pow((double)avec[i].z, 2));
        }
        for (int i = 1; i < lines.Length; i++)
        {
            Vector3 nvecR = new Vector3((float)(avec[i].z / a[i]), (float)(avec[i].y / a[i]), -(float)(avec[i].x / a[i]));
            Vector3 nvecL = new Vector3(-(float)(avec[i].z / a[i]), (float)(avec[i].y / a[i]), (float)(avec[i].x / a[i]));

            Vector3 vecAR = new Vector3(pointCoordinates[i - 1].x + 2 * nvecR.x, pointCoordinates[i - 1].y, pointCoordinates[i - 1].z - 2);
            Vector3 vecBR = new Vector3(pointCoordinates[i].x + 2 * nvecR.x, pointCoordinates[i].y, pointCoordinates[i].z - 2);

            Vector3 vecAL = new Vector3(pointCoordinates[i - 1].x + 2 * nvecL.x, pointCoordinates[i - 1].y, pointCoordinates[i - 1].z + 2);
            Vector3 vecBL = new Vector3(pointCoordinates[i].x + 2 * nvecL.x, pointCoordinates[i].y, pointCoordinates[i].z + 2);
            nAR[i - 1] = vecAR;
            nAL[i - 1] = vecAL;
            nBR[i - 1] = vecBR;
            nBL[i - 1] = vecBL;
        }*/

        /*for (int i = 1; i < lines.Length; i++)
        {
            Vector3 avec = new Vector3(pointCoordinates[i].x - pointCoordinates[i - 1].x, pointCoordinates[i].y - pointCoordinates[i - 1].y, pointCoordinates[i].z - pointCoordinates[i - 1].z);
            //Debug.Log(avec);
            double a = Math.Sqrt(Math.Pow((double)avec.x, 2) + Math.Pow((double)avec.y, 2) + Math.Pow((double)avec.z, 2));


            Vector3 nvecR = new Vector3((float)(avec.z / a), (float)(avec.y / a), -(float)(avec.x / a));
            Vector3 nvecL = new Vector3(-(float)(avec.z / a), (float)(avec.y / a), (float)(avec.x / a));

            Vector3 vecAR = new Vector3(pointCoordinates[i - 1].x + 2 * nvecR.x, pointCoordinates[i - 1].y, pointCoordinates[i - 1].z - 2);
            Vector3 vecBR = new Vector3(pointCoordinates[i].x + 2 * nvecR.x, pointCoordinates[i].y, pointCoordinates[i].z - 2);

            Vector3 vecAL = new Vector3(pointCoordinates[i - 1].x + 2 * nvecL.x, pointCoordinates[i - 1].y, pointCoordinates[i - 1].z + 2);
            Vector3 vecBL = new Vector3(pointCoordinates[i].x + 2 * nvecL.x, pointCoordinates[i].y, pointCoordinates[i].z + 2);
            nAR[i - 1] = vecAR;
            nAL[i - 1] = vecAL;
            nBR[i - 1] = vecBR;
            nBL[i - 1] = vecBL;
        }*/

        /*for (int i = 1; i < lines.Length; i++)
        {
            Vector3 avec = new Vector3(pointCoordinates[i].x - pointCoordinates[i - 1].x, pointCoordinates[i].y - pointCoordinates[i - 1].y, pointCoordinates[i].z - pointCoordinates[i - 1].z);
            //Debug.Log(avec);
            double a = Math.Sqrt(Math.Pow(avec.x, 2) + Math.Pow(avec.y, 2) + Math.Pow(avec.z, 2));


            Vector3 nvecR = new Vector3((float)(avec.z / a), (float)(avec.y / a), -(float)(avec.x / a));
            Vector3 nvecL = new Vector3(-(float)(avec.z / a), (float)(avec.y / a), (float)(avec.x / a));

            Vector3 vecAR = new Vector3(pointCoordinates[i - 1].x + 2 * nvecR.x, (pointCoordinates[i - 1].y + pointCoordinates[i].y) / 2, pointCoordinates[i - 1].z - 2);
            Vector3 vecBR = new Vector3(pointCoordinates[i].x + 2 * nvecR.x, (pointCoordinates[i - 1].y + pointCoordinates[i].y) / 2, pointCoordinates[i].z - 2);

            Vector3 vecAL = new Vector3(pointCoordinates[i - 1].x + 2 * nvecL.x, (pointCoordinates[i - 1].y + pointCoordinates[i].y) / 2, pointCoordinates[i - 1].z + 2);
            Vector3 vecBL = new Vector3(pointCoordinates[i].x + 2 * nvecL.x, (pointCoordinates[i - 1].y + pointCoordinates[i].y) / 2, pointCoordinates[i].z + 2);
            nAR[i - 1] = vecAR;
            nAL[i - 1] = vecAL;
            nBR[i - 1] = vecBR;
            nBL[i - 1] = vecBL;
        }*/


        Vector3[] nARight = new Vector3[pointCoord.Length];
        Vector3[] nBRight = new Vector3[pointCoord.Length];

        for (int i = 1; i < pointCoord.Length; i++)
        {
            Vector3 avec = new Vector3(pointCoord[i].x - pointCoord[i - 1].x, pointCoord[i].y - pointCoord[i - 1].y, pointCoord[i].z - pointCoord[i - 1].z);

            double a = Math.Sqrt(Math.Pow(avec.x, 2) + Math.Pow(avec.y, 2) + Math.Pow(avec.z, 2));


            Vector3 nvecR = new Vector3((float)(avec.z / a), (float)(avec.y / a), -(float)(avec.x / a));

            //3.5 это ширина полосы дороги
            Vector3 vecAR = new Vector3(pointCoord[i - 1].x + (float)3.5 * nvecR.x, pointCoord[i - 1].y, pointCoord[i - 1].z + (float)3.5 * nvecR.z);
            Vector3 vecBR = new Vector3(pointCoord[i].x + (float)3.5 * nvecR.x, pointCoord[i].y, pointCoord[i].z + (float)3.5 * nvecR.z);

            nARight[i - 1] = vecAR;
            nBRight[i - 1] = vecBR;
           
        }

        return (nARight, nBRight);
    }

    public (Vector3[], Vector3[]) CalcLeftEquidistant(Vector3[] pointCoord)//переделать с параметрами
    {
        Vector3[] nALeft = new Vector3[pointCoord.Length];
        Vector3[] nBLeft = new Vector3[pointCoord.Length];
        for (int i = 1; i < pointCoord.Length; i++)
        {
            Vector3 avec = new Vector3(pointCoord[i].x - pointCoord[i - 1].x, pointCoord[i].y - pointCoord[i - 1].y, pointCoord[i].z - pointCoord[i - 1].z);
            //Debug.Log(avec);
            double a = Math.Sqrt(Math.Pow(avec.x, 2) + Math.Pow(avec.y, 2) + Math.Pow(avec.z, 2));

            Vector3 nvecL = new Vector3(-(float)(avec.z / a), (float)(avec.y / a), (float)(avec.x / a));

            //3.5 это ширина полосы дороги
            Vector3 vecAL = new Vector3(pointCoord[i - 1].x + (float)3.5 * nvecL.x, pointCoord[i - 1].y, pointCoord[i - 1].z + (float)3.5 * nvecL.z);
            Vector3 vecBL = new Vector3(pointCoord[i].x + (float)3.5 * nvecL.x, pointCoord[i].y, pointCoord[i].z + (float)3.5 * nvecL.z);

            nALeft[i - 1] = vecAL;
            nBLeft[i - 1] = vecBL;

            
        }
        return (nALeft, nBLeft);

    }


    public Vector3[] MeshNearPoints(Vector3[] nA, Vector3[] nB, int count, Vector3[] mainLine)
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
                    if (nA[i].x < nB[i - 1].x) //по иксу, т.к. х это путь вперЄд(сгиб внутрь)
                    {
                        
                        Vector3 mid;
                        float midx, midy, midz;

                        midx = (nA[i].x + nB[i - 1].x) / 2;
                        midy = (nA[i].y + nB[i - 1].y) / 2;
                        midz = (nA[i].z + nB[i - 1].z) / 2;

                        mid = new Vector3(midx, midy, midz);

                        Vector3 avec = new Vector3(mid.x - mainLine[i].x, mid.y - mainLine[i].y, mid.z - mainLine[i].z);//разница между сереждиной и осью
                        Vector3 normalizedDirection = avec.normalized;
                        Vector3 C = mainLine[i] + normalizedDirection * (float)3.5;

                        M[i] = C;
                        
                    }
                    else
                    {
                        if (nA[i].x > nB[i - 1].x)//по иксу, т.к. х это путь вперЄд(сгиб наружу)
                        {
                            
                            Vector3 mid;
                            float midx, midy, midz;

                            midx = (nA[i].x + nB[i - 1].x) / 2;
                            midy = (nA[i].y + nB[i - 1].y) / 2;
                            midz = (nA[i].z + nB[i - 1].z) / 2;

                            mid = new Vector3(midx, midy, midz);

                            Vector3 avec = new Vector3(mid.x - mainLine[i].x, mid.y - mainLine[i].y, mid.z - mainLine[i].z);//разница между сереждиной и осью
                            Vector3 normalizedDirection = avec.normalized;
                            Vector3 C = mainLine[i] + normalizedDirection * (float)3.5;

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

    public void MeshCreate()//creating mesh
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Road";

        int meshCount = RMR.Length + RML.Length + pointCoordinates.Length;
        Vector3[] MeshNearPoints = new Vector3[meshCount];

        for (int i = 0; i < RMR.Length; i++)
        {
            MeshNearPoints[i] = RMR[i];
        }

        for (int i = 0; i < pointCoordinates.Length; i++)
        {
            MeshNearPoints[i + RMR.Length] = pointCoordinates[i];
        }

        for (int i = 0; i < RML.Length; i++)
        {
            MeshNearPoints[i + RMR.Length + pointCoordinates.Length] = RML[i];
        }

        int[] triangle = new int[2 * (RMR.Length - 1) * 6];//количество квадов
        for (int ti = 0, vi = 0, i = 0; i < 2; i++, vi++)
        {
            for (int k = 0; k < RMR.Length - 1; k++, ti+=6, vi++)
            {
                triangle[ti] = vi;
                triangle[ti + 1] = vi + RMR.Length;
                triangle[ti + 2] = vi + 1;
                triangle[ti + 3] = vi + RMR.Length;
                triangle[ti + 4] = vi + RMR.Length + 1;
                triangle[ti + 5] = vi + 1;
            }
        }

        mesh.RecalculateNormals();  
        mesh.vertices = MeshNearPoints;
        mesh.triangles = triangle;

        
    }

    /*public Vector3 CalcIntersection(Vector3 st1, Vector3 end1, Vector3 st2, Vector3 end2)
    {
        float a1, a2, b1, b2, c1, c2, t1, t2, del;

        a1 = end1.z - st1.z;
        b1 = end1.x - st1.x;
        c1 = end1.y - st1.y;

        


        a2 = end2.z - st2.z;
        b2 = end2.x - st2.x;
        c2 = end2.y - st2.y;
        
        del = a1 * b2 - a2 * b1;

        t1= (st2.x - st1.x) * a2 - (st2.z - st1.z) * b2;
        t2 = (st2.x - st1.x) * a1 - (st2.z - st1.z) * b1; ;

       

        float x1, y1, z1;

        x1 = (b1 * d2 - b2 * d1) / det;
        z1 = (a2 * d1 - a1 * d2) / det;
        y1= (st1.y + c1 * (x1 - st1.x)) / a1;

        Vector3 Inter= new Vector3(x1, y1, z1);
        return Inter;
    }*/

    private void OnDrawGizmos()
    {
        //координаты оси дороги
        //Gizmos.color = Color.yellow;
        //for (int i = 0; i < pointCoordinates.Length; i++)
        //{
            
        //    Gizmos.DrawSphere(pointCoordinates[i], 0.05f);
        //}
        
        //ось дороги
        Gizmos.color = Color.white;
        for (int i = 1; i < pointCoordinates.Length; i++)
        {
            
            Gizmos.DrawLine(pointCoordinates[i-1], pointCoordinates[i]);
        }

        /*тест
        for (int i = 0; i < 4; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(points[i], 0.05f);

        }
        for (int i = 1; i < 4; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(points[i - 1], points[i]);
        }
        for (int i = 1; i < 4; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(nAR[i - 1], 0.05f);
            Gizmos.DrawSphere(nBR[i - 1], 0.05f);
            Gizmos.DrawSphere(nAL[i - 1], 0.05f);
            Gizmos.DrawSphere(nBL[i - 1], 0.05f);
        }
        for (int i = 1; i < 4; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(nAR[i - 1], nBR[i - 1]);
            Gizmos.DrawLine(nAL[i - 1], nBL[i - 1]);
        }
        for (int i = 1; i < 3; i++)
        {
            Gizmos.DrawLine(nAR[i], nBR[i - 1]);
            Gizmos.DrawLine(nAL[i], nBL[i - 1]);
        }*/
        
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

        //Ё ¬»ƒ»—“јЌ“џ
        Gizmos.color = Color.white;
        for (int i = 1; i < pointCoordinates.Length; i++)
        {

            Gizmos.DrawLine(nAR[i - 1], nBR[i - 1]);//right
            Gizmos.DrawLine(nAL[i - 1], nBL[i - 1]);//left
        }

        //пр€мые между кусками AiBi
        /*for (int i = 1; i < pointCoordinates.Length - 1; i++)
        {
            Gizmos.DrawLine(nAR[i], nBR[i - 1]);//right
            Gizmos.DrawLine(nAL[i], nBL[i - 1]);//left
        }*/

        //перпендикул€ры
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

        //проверка на расстро€ние перпендикул€ров
        //Gizmos.color = Color.white;
        //Gizmos.DrawSphere(nAL[10], 0.05f);
        //Gizmos.DrawSphere(nBL[9], 0.05f);
        //Gizmos.DrawSphere(pointCoordinates[10], 0.05f);
        //Gizmos.DrawLine(nAL[10], pointCoordinates[10]);
        //Gizmos.DrawLine(nBL[9], pointCoordinates[10]);
        //Debug.Log("YesYes");
        //Gizmos.color = Color.white;
        //for (int i = 0; i < pointCoordinates.Length; i++)
        //    {
        //        Gizmos.DrawLine(RML[i], pointCoordinates[i]);//right

        //    }


        //“очки середины между перпендикул€рами
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

        //Gizmos.color = Color.red;
        //for (int i = 1; i < RMR.Length; i++)
        //{
        //    Gizmos.DrawLine(RMR[i - 1], RMR[i]);
        //}
        //for (int i = 1; i < RML.Length; i++)
        //{
        //    Gizmos.DrawLine(RML[i - 1], RML[i]);
        //}
    }

    private void Start()
    {
        GetCoord();
        //GetCoordTEST();
        var resultLeft = CalcLeftEquidistant(pointCoordinates);
        nAL = resultLeft.Item1;
        nBL = resultLeft.Item2;

        Debug.Log(pointCoordinates.Length);
        Debug.Log(nAL.Length);

        var resultRight = CalcRightEquidistant(pointCoordinates);
        nAR = resultRight.Item1;
        nBR = resultRight.Item2;

        RML = MeshNearPoints(nAL, nBL, nAL.Length, pointCoordinates);
        RMR = MeshNearPoints(nAR, nBR, nAR.Length, pointCoordinates);

        MeshCreate();

        //проверка на расстро€ние от точек оси до меш
        //for (int i = 1; i < pointCoordinates.Length; i++)
        //{
        //    Vector3 evecA = new Vector3(RML[i].x - pointCoordinates[i].x, RML[i].y - pointCoordinates[i].y, RML[i].z - pointCoordinates[i].z);
        //    double eA = Math.Sqrt(Math.Pow((double)evecA.x, 2) + Math.Pow((double)evecA.y, 2) + Math.Pow((double)evecA.z, 2));

        //    //Vector3 evecB = new Vector3(nBL[i-1].x - pointCoordinates[i].x, nBL[i-1].y - pointCoordinates[i].y, nBL[i-1].z - pointCoordinates[i].z);
        //    //double eB = Math.Sqrt(Math.Pow((double)evecB.x, 2) + Math.Pow((double)evecB.y, 2) + Math.Pow((double)evecB.z, 2));

        //    Debug.Log(eA);
        //    //Debug.Log(eB);
        //}


        Debug.Log("Yes");
    }
}
