using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SocialPlatforms;
using System.Reflection;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;

public class RoadPoints : MonoBehaviour
{
    private static string Path = "Resources/XYZpoints.txt";

    private static string[] lines = File.ReadAllLines(Path);
    public Vector3[] pointCoordinates = new Vector3[lines.Length];//��� ������

    //Points for road
    private Vector3[] nARightEquidistantRoad = new Vector3[lines.Length];
    private Vector3[] nBRightEquidistantRoad = new Vector3[lines.Length];
    private Vector3[] nALeftEquidistantRoad = new Vector3[lines.Length];
    private Vector3[] nBLeftEquidistantRoad = new Vector3[lines.Length];

    //Mesh for road
    public Vector3[] RoadMeshL = new Vector3[1];//����� ������� ������
    public Vector3[] RoadMeshR = new Vector3[1];//������ ������� ������
    
    //�������
    //Mesh for roadside
    private Vector3[] RoadsideMeshL = new Vector3[1];//����� ������� �������
    private Vector3[] RoadsideMeshR = new Vector3[1];//������ ������� �������
    
    //�����
    private Vector3[] RoadSlopeMeshL = new Vector3[1];//����� ������� ������
    private Vector3[] RoadSlopeMeshR = new Vector3[1];//������ ������� ������

    //��������
    private Vector3[] RoadBottomMeshL = new Vector3[1];//����� ������� ��� ������
    private Vector3[] RoadBottomMeshR = new Vector3[1];//������ ������� ��� ������

    public Vector3[] RoadUpSlopeMeshL = new Vector3[1];//����� ������� ��� ������
    public Vector3[] RoadUpSlopeMeshR = new Vector3[1];//������ ������� ��� ������

    public Mesh[] roadMeshes = new Mesh[9];

    private void GetCoord()
    {
        string[] coordinates = new string[3];
        float[] start = new float[3];
        float[] coord = new float[3];
        //float x, y, z;

        coordinates = lines[0].Split(' ');//��������� �������� ���������� x z y
        
        for (int i = 0; i < 3; i++)
        {
            start[i] = float.Parse(coordinates[i]);//���������� ��� ���� x z y
            //Debug.Log(start[i]);
        }

        for (int i = 0; i < lines.Length; i++)
        {
            coordinates = lines[i].Split(' ');//��� �������� ���������� x z y

            for (int j = 0; j < 3; j++)//�� ���[0], �� ���[1], �� �����[2] ���������� ����� �� ������� �����
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
            pointCoordinates[i] = new Vector3(coord[0] + 13391, coord[2], coord[1] + 13391);// x y z           

            //Debug.Log(pointCoordinates[i]);
        }
    }//���������� ��� ������

    //probnic is GOOD
    private (Vector3[], Vector3[]) CalcRightRoadEquidistant(Vector3[] pointCoord, float width, float high, int left, int right)//���������� � �����������
    {
        Vector3[] nARight = new Vector3[pointCoord.Length];
        Vector3[] nBRight = new Vector3[pointCoord.Length];

        for (int i = 1; i < pointCoord.Length; i++)
        {
            Vector3 avec = new Vector3(pointCoord[i].x - pointCoord[i - 1].x, pointCoord[i].y - pointCoord[i - 1].y, pointCoord[i].z - pointCoord[i - 1].z);

            double a = Math.Sqrt(Math.Pow(avec.x, 2) + Math.Pow(avec.y, 2) + Math.Pow(avec.z, 2));


            Vector3 nvecR = new Vector3(left*(float)(avec.z / a), (float)(avec.y / a), right*(float)(avec.x / a));

            //widht ��� ������ ������ ������
            //high ��� ������ � ������
            Vector3 vecAR = new Vector3(pointCoord[i - 1].x + width * nvecR.x, pointCoord[i - 1].y +  high, pointCoord[i - 1].z + width * nvecR.z);
            Vector3 vecBR = new Vector3(pointCoord[i].x + width * nvecR.x, pointCoord[i].y +  high, pointCoord[i].z + width * nvecR.z);

            nARight[i - 1] = vecAR;
            nBRight[i - 1] = vecBR;

        }

        return (nARight, nBRight);
    }
    private Vector3[] MeshNearPoints(Vector3[] nA, Vector3[] nB, int count, Vector3[] mainLine, float width)
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
            if (i == count - 1)
            {
                M[i] = nB[i-1];
            }
            else
            if (nA[i].x == nB[i - 1].x)//���� � �� �� �����
            {
                M[i] = nA[i];
            }
            else
            {
                Vector3 mid;
                float midx, midy, midz;

                midx = (nA[i].x + nB[i - 1].x) / 2;
                midy = (nA[i].y + nB[i - 1].y) / 2;
                midz = (nA[i].z + nB[i - 1].z) / 2;

                mid = new Vector3(midx, midy, midz);

                Vector3 avec = new Vector3(mid.x - mainLine[i].x, mid.y - mainLine[i].y, mid.z - mainLine[i].z);//������� ����� ���������� � ����
                Vector3 normalizedDirection = avec.normalized;
                Vector3 C = mainLine[i] + normalizedDirection * width;

                M[i] = C;
            }
        }

        return M;
    }
    private Mesh RoadMeshCreate(int index)//0, 1, 2, 3, 4, 5, 6, 7, 8
    {
        Mesh mesh = new Mesh();
        int meshCount;
        Vector3[] MeshNearPoints;
        int[] triangle;
        Vector2[] uvs;

        if (index == 0)
        {
            meshCount = RoadMeshR.Length + RoadMeshL.Length + pointCoordinates.Length;
            MeshNearPoints = new Vector3[meshCount];
            uvs = new Vector2[meshCount];
            for (int i = 0; i < RoadMeshR.Length; i++)
            {
                MeshNearPoints[i] = RoadMeshR[i];
                uvs[i] = new Vector2((float)RoadMeshR[i].x / RoadMeshR.Length, (float)RoadMeshR[i].y / RoadMeshR.Length);
            }

            for (int i = 0; i < pointCoordinates.Length; i++)
            {
                MeshNearPoints[i + RoadMeshR.Length] = pointCoordinates[i];
                uvs[i + RoadMeshR.Length] = new Vector2((float)pointCoordinates[i].x / RoadMeshR.Length, (float)pointCoordinates[i].y / RoadMeshR.Length);
            }

            for (int i = 0; i < RoadMeshL.Length; i++)
            {
                MeshNearPoints[i + RoadMeshR.Length + pointCoordinates.Length] = RoadMeshL[i];
                uvs[i + RoadMeshR.Length + pointCoordinates.Length] = new Vector2((float)RoadMeshL[i].x / RoadMeshR.Length, (float)RoadMeshL[i].y / RoadMeshR.Length);
            }

            triangle = new int[2 * (RoadMeshR.Length - 1) * 6];//���������� ������
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
            mesh.uv = uvs;
            mesh.RecalculateNormals();
        }//����������� ������
        else
            if (index == 1)
        {
            meshCount = RoadsideMeshL.Length + RoadMeshL.Length;//�������  ����� ����������, ����� ������(�������)
            MeshNearPoints = new Vector3[meshCount];
            
            //������� ������� ����������
            for (int i = 0; i < RoadMeshL.Length; i++)
            {
                MeshNearPoints[i] = RoadMeshL[i];
            }

            //����� ����� ����������� ����������
            for (int i = 0; i < RoadsideMeshL.Length; i++)
            {
                MeshNearPoints[i + RoadMeshL.Length] = RoadsideMeshL[i];
            }

            //���������� ������� ��������� 
            triangle = new int[1 * (RoadMeshL.Length - 1) * 6];//���������� ������ !!!!//1 ��� 2 ��� ���������� �����(�� ���� ���� 2 ������ ������ ���� 1
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
        }//����� ������� �������
        else
        if (index == 2)
        {
            meshCount = RoadMeshR.Length + RoadsideMeshR.Length;//������� ������(�������) ����� ����� ����������
            MeshNearPoints = new Vector3[meshCount];

            //��� ������� ������ �� ��� ������ ���������� ���� �� ���������, �� ����� ������ ��� �������� ������������� �� ����� ������(������) �.�. ���������� ������������� �� �������� ���������� ������ �� ����

            //������� ����� ����������� ����������
            for (int i = 0; i < RoadsideMeshR.Length; i++)
            {
                MeshNearPoints[i] = RoadsideMeshR[i];
            }

            //����� ������� ����������
            for (int i = 0; i < RoadMeshR.Length; i++)
            {
                MeshNearPoints[i + RoadsideMeshR.Length] = RoadMeshR[i];
            }

            //���������� ������� ���������
            triangle = new int[1 * (RoadMeshR.Length - 1) * 6];//���������� ������ !!!!//1 ��� 2 ��� ���������� �����(�� ���� ���� 2 ������ ������ ���� 1
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
        }//������ ������� �������(� �������)
        else
        if (index == 3)
        {
            meshCount = RoadSlopeMeshL.Length + RoadsideMeshL.Length;//������� ����� ����������, ����� ������(�������)
            MeshNearPoints = new Vector3[meshCount];

            //������� ������� ����������
            for (int i = 0; i < RoadsideMeshL.Length; i++)
            {
                MeshNearPoints[i] = RoadsideMeshL[i];
            }

            //����� ����� ����������� ����������
            for (int i = 0; i < RoadSlopeMeshL.Length; i++)
            {
                MeshNearPoints[i + RoadsideMeshL.Length] = RoadSlopeMeshL[i];
            }

            //���������� ������� ���������
            triangle = new int[1 * (RoadsideMeshL.Length - 1) * 6];//���������� ������ !!!!//1 ��� 2 ��� ���������� �����(�� ���� ���� 2 ������ ������ ���� 1
            for (int ti = 0, vi = 0, i = 0; i < 1; i++, vi++)
            {
                for (int k = 0; k < RoadsideMeshL.Length - 1; k++, ti += 6, vi++)
                {
                    triangle[ti] = vi;
                    triangle[ti + 1] = vi + RoadsideMeshL.Length;
                    triangle[ti + 2] = vi + 1;
                    triangle[ti + 3] = vi + RoadsideMeshL.Length;
                    triangle[ti + 4] = vi + RoadsideMeshL.Length + 1;
                    triangle[ti + 5] = vi + 1;
                }
            }
            mesh.vertices = MeshNearPoints;
            mesh.triangles = triangle;
            mesh.RecalculateNormals();
        }//����� ������� ������
        else
        if (index == 4)
        {
            meshCount = RoadsideMeshR.Length + RoadSlopeMeshR.Length;//������� ������(�������) ����� ����� ����������
            MeshNearPoints = new Vector3[meshCount];

            //��� ������� ������ �� ��� ������ ���������� ���� �� ���������, �� ����� ������ ��� �������� ������������� �� ����� ������(������) �.�. ���������� ������������� �� �������� ���������� ������ �� ����

            //������� ����� ����������� ����������
            for (int i = 0; i < RoadSlopeMeshR.Length; i++)
            {
                MeshNearPoints[i] = RoadSlopeMeshR[i];
            }

            //����� ������� ����������
            for (int i = 0; i < RoadsideMeshR.Length; i++)
            {
                MeshNearPoints[i + RoadSlopeMeshR.Length] = RoadsideMeshR[i];
            }
            
            //���������� ������� ���������
            triangle = new int[1 * (RoadsideMeshR.Length - 1) * 6];//���������� ������ !!!!//1 ��� 2 ��� ���������� �����(�� ���� ���� 2 ������ ������ ���� 1
            for (int ti = 0, vi = 0, i = 0; i < 1; i++, vi++)
            {
                for (int k = 0; k < RoadsideMeshR.Length - 1; k++, ti += 6, vi++)
                {
                    triangle[ti] = vi;
                    triangle[ti + 1] = vi + RoadsideMeshR.Length;
                    triangle[ti + 2] = vi + 1;
                    triangle[ti + 3] = vi + RoadsideMeshR.Length;
                    triangle[ti + 4] = vi + RoadsideMeshR.Length + 1;
                    triangle[ti + 5] = vi + 1;
                }
            }

            mesh.vertices = MeshNearPoints;
            mesh.triangles = triangle;
            mesh.RecalculateNormals();
        }//������ ������� ������(� �������)
        else
        if (index == 5)
        {
            meshCount = RoadBottomMeshL.Length + RoadSlopeMeshL.Length;//������� ����� ����������, ����� ������(�������)
            MeshNearPoints = new Vector3[meshCount];

            //������� ������� ����������
            for (int i = 0; i < RoadSlopeMeshL.Length; i++)
            {
                MeshNearPoints[i] = RoadSlopeMeshL[i];
            }

            //����� ����� ����������� ����������
            for (int i = 0; i < RoadBottomMeshL.Length; i++)
            {
                MeshNearPoints[i + RoadSlopeMeshL.Length] = RoadBottomMeshL[i];
            }
            
            //���������� ������� ���������
            triangle = new int[1 * (RoadSlopeMeshL.Length - 1) * 6];//���������� ������ !!!!//1 ��� 2 ��� ���������� �����(�� ���� ���� 2 ������ ������ ���� 1
            for (int ti = 0, vi = 0, i = 0; i < 1; i++, vi++)
            {
                for (int k = 0; k < RoadSlopeMeshL.Length - 1; k++, ti += 6, vi++)
                {
                    triangle[ti] = vi;
                    triangle[ti + 1] = vi + RoadSlopeMeshL.Length;
                    triangle[ti + 2] = vi + 1;
                    triangle[ti + 3] = vi + RoadSlopeMeshL.Length;
                    triangle[ti + 4] = vi + RoadSlopeMeshL.Length + 1;
                    triangle[ti + 5] = vi + 1;
                }
            }
            mesh.vertices = MeshNearPoints;
            mesh.triangles = triangle;
            mesh.RecalculateNormals();
        }//!!//����� ������� ��� ������
        else
        if (index == 6)
        {
            meshCount = RoadSlopeMeshR.Length + RoadBottomMeshR.Length;//������� ������(�������) ����� ����� ����������
            MeshNearPoints = new Vector3[meshCount];

            //��� ������� ������ �� ��� ������ ���������� ���� �� ���������, �� ����� ������ ��� �������� ������������� �� ����� ������(������) �.�. ���������� ������������� �� �������� ���������� ������ �� ����

            //������� ����� ����������� ����������
            for (int i = 0; i < RoadBottomMeshR.Length; i++)
            {
                MeshNearPoints[i] = RoadBottomMeshR[i];
            }

            //����� ������� ����������
            for (int i = 0; i < RoadSlopeMeshR.Length; i++)
            {
                MeshNearPoints[i + RoadBottomMeshR.Length] = RoadSlopeMeshR[i];
            }

            //���������� ������� ���������
            triangle = new int[1 * (RoadSlopeMeshR.Length - 1) * 6];//���������� ������ !!!!//1 ��� 2 ��� ���������� �����(�� ���� ���� 2 ������ ������ ���� 1
            for (int ti = 0, vi = 0, i = 0; i < 1; i++, vi++)
            {
                for (int k = 0; k < RoadSlopeMeshR.Length - 1; k++, ti += 6, vi++)
                {
                    triangle[ti] = vi;
                    triangle[ti + 1] = vi + RoadSlopeMeshR.Length;
                    triangle[ti + 2] = vi + 1;
                    triangle[ti + 3] = vi + RoadSlopeMeshR.Length;
                    triangle[ti + 4] = vi + RoadSlopeMeshR.Length + 1;
                    triangle[ti + 5] = vi + 1;
                }
            }

            mesh.vertices = MeshNearPoints;
            mesh.triangles = triangle;
            mesh.RecalculateNormals();
        }//!!//������ ������� ��� ������(� �������)
        else
        if (index == 7)
        {
            meshCount = RoadUpSlopeMeshL.Length + RoadBottomMeshL.Length;//������� ����� ����������, ����� ������(�������)
            MeshNearPoints = new Vector3[meshCount];

            //������� ������� ����������
            for (int i = 0; i < RoadBottomMeshL.Length; i++)
            {
                MeshNearPoints[i] = RoadBottomMeshL[i];
            }

            //����� ����� ����������� ����������
            for (int i = 0; i < RoadUpSlopeMeshL.Length; i++)
            {
                MeshNearPoints[i + RoadBottomMeshL.Length] = RoadUpSlopeMeshL[i];
            }

            //���������� ������� ���������
            triangle = new int[1 * (RoadBottomMeshL.Length - 1) * 6];//���������� ������ !!!!//1 ��� 2 ��� ���������� �����(�� ���� ���� 2 ������ ������ ���� 1
            for (int ti = 0, vi = 0, i = 0; i < 1; i++, vi++)
            {
                for (int k = 0; k < RoadBottomMeshL.Length - 1; k++, ti += 6, vi++)
                {
                    triangle[ti] = vi;
                    triangle[ti + 1] = vi + RoadBottomMeshL.Length;
                    triangle[ti + 2] = vi + 1;
                    triangle[ti + 3] = vi + RoadBottomMeshL.Length;
                    triangle[ti + 4] = vi + RoadBottomMeshL.Length + 1;
                    triangle[ti + 5] = vi + 1;
                }
            }
            mesh.vertices = MeshNearPoints;
            mesh.triangles = triangle;
            mesh.RecalculateNormals();
        }//!!//����� ������� �������
        else
        if (index == 8)
        {
            meshCount = RoadBottomMeshR.Length + RoadUpSlopeMeshR.Length;//������� ������(�������) ����� ����� ����������
            MeshNearPoints = new Vector3[meshCount];

            //��� ������� ������ �� ��� ������ ���������� ���� �� ���������, �� ����� ������ ��� �������� ������������� �� ����� ������(������) �.�. ���������� ������������� �� �������� ���������� ������ �� ����

            //������� ����� ����������� ����������
            for (int i = 0; i < RoadUpSlopeMeshR.Length; i++)
            {
                MeshNearPoints[i] = RoadUpSlopeMeshR[i];
            }

            //����� ������� ����������
            for (int i = 0; i < RoadBottomMeshR.Length; i++)
            {
                MeshNearPoints[i + RoadUpSlopeMeshR.Length] = RoadBottomMeshR[i];
            }

            //���������� ������� ���������
            triangle = new int[1 * (RoadBottomMeshR.Length - 1) * 6];//���������� ������ !!!!//1 ��� 2 ��� ���������� �����(�� ���� ���� 2 ������ ������ ���� 1
            for (int ti = 0, vi = 0, i = 0; i < 1; i++, vi++)
            {
                for (int k = 0; k < RoadBottomMeshR.Length - 1; k++, ti += 6, vi++)
                {
                    triangle[ti] = vi;
                    triangle[ti + 1] = vi + RoadBottomMeshR.Length;
                    triangle[ti + 2] = vi + 1;
                    triangle[ti + 3] = vi + RoadBottomMeshR.Length;
                    triangle[ti + 4] = vi + RoadBottomMeshR.Length + 1;
                    triangle[ti + 5] = vi + 1;
                }
            }

            mesh.vertices = MeshNearPoints;
            mesh.triangles = triangle;
            mesh.RecalculateNormals();
        }//!!//������ ������� �������

        return mesh;
    }
    private void AssignMeshesToObjects(int index)//0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
    {
        // ������������ ����� ������� ���������� �������
        if (index == 0)
        {
            GameObject roadObject = GameObject.Find("Road");

            MeshFilter meshFilter = roadObject.GetComponent<MeshFilter>();
            //MeshRenderer meshRenderer = roadObject.GetComponent<MeshRenderer>();

            //int SideRoadL;
            if (meshFilter != null)
            {
                meshFilter.mesh = roadMeshes[index]; // ������������ ����
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
                meshFilter.mesh = roadMeshes[index]; // ������������ ����
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
                meshFilter.mesh = roadMeshes[index]; // ������������ ����
            }
            else
            {
                Debug.LogWarning("MeshFilter component not found on RoadObject.");
            }
        }
        else
            if (index == 3)
        {
            GameObject roadObject = GameObject.Find("SopeRoadL");
            MeshFilter meshFilter = roadObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = roadObject.GetComponent<MeshRenderer>();

            if (meshFilter != null)
            {
                meshFilter.mesh = roadMeshes[index]; // ������������ ����
            }
            else
            {
                Debug.LogWarning("MeshFilter component not found on RoadObject.");
            }
        }
        else
            if (index == 4)
        {
            GameObject roadObject = GameObject.Find("SopeRoadR");
            MeshFilter meshFilter = roadObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = roadObject.GetComponent<MeshRenderer>();

            if (meshFilter != null)
            {
                meshFilter.mesh = roadMeshes[index]; // ������������ ����
            }
            else
            {
                Debug.LogWarning("MeshFilter component not found on RoadObject.");
            }
        }
        else
            if (index == 5)
        {
            GameObject roadObject = GameObject.Find("BottomRoadL");
            MeshFilter meshFilter = roadObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = roadObject.GetComponent<MeshRenderer>();

            if (meshFilter != null)
            {
                meshFilter.mesh = roadMeshes[index]; // ������������ ����
            }
            else
            {
                Debug.LogWarning("MeshFilter component not found on RoadObject.");
            }
        }
        else
            if (index == 6)
        {
            GameObject roadObject = GameObject.Find("BottomRoadR");
            MeshFilter meshFilter = roadObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = roadObject.GetComponent<MeshRenderer>();

            if (meshFilter != null)
            {
                meshFilter.mesh = roadMeshes[index]; // ������������ ����
            }
            else
            {
                Debug.LogWarning("MeshFilter component not found on RoadObject.");
            }
        }
        else
            if (index == 7)
        {
            GameObject roadObject = GameObject.Find("UpSopeRoadL");
            MeshFilter meshFilter = roadObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = roadObject.GetComponent<MeshRenderer>();

            if (meshFilter != null)
            {
                meshFilter.mesh = roadMeshes[index]; // ������������ ����
            }
            else
            {
                Debug.LogWarning("MeshFilter component not found on RoadObject.");
            }
        }
        else
            if (index == 8)
        {
            GameObject roadObject = GameObject.Find("UpSopeRoadR");
            MeshFilter meshFilter = roadObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = roadObject.GetComponent<MeshRenderer>();

            if (meshFilter != null)
            {
                meshFilter.mesh = roadMeshes[index]; // ������������ ����
            }
            else
            {
                Debug.LogWarning("MeshFilter component not found on RoadObject.");
            }
        }
    }

    private void OnDrawGizmos()
    {
        //���������� ��� ������
        //Gizmos.color = Color.yellow;
        //for (int i = 0; i < pointCoordinates.Length; i++)
        //{

        //    Gizmos.DrawSphere(pointCoordinates[i], 0.05f);
        //}

        ////��� ������
        Gizmos.color = Color.white;

        for (int i = 1; i < pointCoordinates.Length; i++)
        {

            Gizmos.DrawLine(pointCoordinates[i - 1], pointCoordinates[i]);
        }

        //����� �����������
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

        ////������������
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

        ////����� �����������
        //Gizmos.color = Color.black;
        //for (int i = 1; i < RoadSlopeMeshL.Length; i++)
        //{
        //    //right
        //    Gizmos.DrawSphere(RoadSlopeMeshL[i - 1], 0.05f);
        //    Gizmos.DrawSphere(RoadSlopeMeshR[i - 1], 0.05f);
        //}

        //������ ����� ������� AiBi
        /*for (int i = 1; i < pointCoordinates.Length - 1; i++)
        {
            Gizmos.DrawLine(nAR[i], nBR[i - 1]);//right
            Gizmos.DrawLine(nAL[i], nBL[i - 1]);//left
        }*/

        //��������������
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

        //����� �������� ����� ����������������
        //Gizmos.color = Color.black;
        //for (int i = 0; i < 15; i++)
        //{
        //    //left
        //    Gizmos.DrawSphere(nALeftEquidistantRoad[i], 0.05f);
        //}
        //for (int i = 0; i < nBLeftEquidistantRoad.Length; i++)
        //{
        //    //right
        //    Gizmos.DrawSphere(nBLeftEquidistantRoad[i], 0.05f);
        //}

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

    private void PointMesh(Vector3[] RoadMeshLeft, Vector3[] RoadMeshRight, float high, float width, int index)//0, 1, 2, 3, 4, 5, 6 , 7, 8
    {
        var resultSideLeft = CalcRightRoadEquidistant(RoadMeshLeft, width, high, -1, 1);
        var resultSideRight = CalcRightRoadEquidistant(RoadMeshRight, width, high, 1, -1);
        
        nALeftEquidistantRoad = resultSideLeft.Item1;
        nBLeftEquidistantRoad = resultSideLeft.Item2;

        nARightEquidistantRoad = resultSideRight.Item1;
        nBRightEquidistantRoad = resultSideRight.Item2; 

        if(index == 0)
        {      
            RoadMeshL = MeshNearPoints(nALeftEquidistantRoad, nBLeftEquidistantRoad, nALeftEquidistantRoad.Length, RoadMeshLeft, width);
            RoadMeshR = MeshNearPoints(nARightEquidistantRoad, nBRightEquidistantRoad, nARightEquidistantRoad.Length, RoadMeshRight, width);
        }
        else 
            if(index == 1)
        {        
            RoadsideMeshL = MeshNearPoints(nALeftEquidistantRoad, nBLeftEquidistantRoad, nALeftEquidistantRoad.Length, RoadMeshLeft, width);
        }
        else
            if (index == 2)
        {
            RoadsideMeshR = MeshNearPoints(nARightEquidistantRoad, nBRightEquidistantRoad, nARightEquidistantRoad.Length, RoadMeshRight, width);
        }
        else
            if (index == 3)
        {
            RoadSlopeMeshL = MeshNearPoints(nALeftEquidistantRoad, nBLeftEquidistantRoad, nALeftEquidistantRoad.Length, RoadMeshLeft, width);
        }
        else
            if (index == 4)
        {
            RoadSlopeMeshR = MeshNearPoints(nARightEquidistantRoad, nBRightEquidistantRoad, nARightEquidistantRoad.Length, RoadMeshRight, width);
        }
        else
            if (index == 5)
        {
            RoadBottomMeshL = MeshNearPoints(nALeftEquidistantRoad, nBLeftEquidistantRoad, nALeftEquidistantRoad.Length, RoadMeshLeft, width);
        }
        else
            if (index == 6)
        {
            RoadBottomMeshR = MeshNearPoints(nARightEquidistantRoad, nBRightEquidistantRoad, nARightEquidistantRoad.Length, RoadMeshRight, width);
        }
        else
            if (index == 7)
        {
            RoadUpSlopeMeshL = MeshNearPoints(nALeftEquidistantRoad, nBLeftEquidistantRoad, nALeftEquidistantRoad.Length, RoadMeshLeft, width);
        }
        else
            if (index == 8)
        {
            RoadUpSlopeMeshR = MeshNearPoints(nARightEquidistantRoad, nBRightEquidistantRoad, nARightEquidistantRoad.Length, RoadMeshRight, width);
        }



    }   

    public static event System.Action OnRoadMeshInitialized;
    private void Start()
    {        
        //ROAD
        GetCoord();
        //roadObject1 = new GameObject[9];
        

        PointMesh(pointCoordinates, pointCoordinates, (float)-0.02, (float)3.5, 0);
        roadMeshes[0] = RoadMeshCreate(0);
        AssignMeshesToObjects(0);



        PointMesh(RoadMeshL, RoadMeshR, (float)-0.04, 2, 1);
        roadMeshes[1] = RoadMeshCreate(1);
        AssignMeshesToObjects(1);

        PointMesh(RoadMeshL, RoadMeshR, (float)-0.04, 2, 2);
        roadMeshes[2] = RoadMeshCreate(2);
        AssignMeshesToObjects(2);

        

        PointMesh(RoadsideMeshL, RoadsideMeshR, (float)-1.2, (float)Math.Sqrt(Math.Pow(1.2, 2) + Math.Pow(3.6, 2)), 3);//����� �����
        roadMeshes[3] = RoadMeshCreate(3);
        AssignMeshesToObjects(3);

        PointMesh(RoadsideMeshL, RoadsideMeshR, (float)-1.2, (float)Math.Sqrt(Math.Pow(1.2, 2) + Math.Pow(3.6, 2)), 4);//����� ������
        roadMeshes[4] = RoadMeshCreate(4);
        AssignMeshesToObjects(4);



        PointMesh(RoadSlopeMeshL, RoadSlopeMeshR, 0, (float)0.4, 5);//��� �����(�����)
        roadMeshes[5] = RoadMeshCreate(5);
        AssignMeshesToObjects(5);

        PointMesh(RoadSlopeMeshL, RoadSlopeMeshR, 0, (float)0.4, 6);//��� ������(�����)
        roadMeshes[6] = RoadMeshCreate(6);
        AssignMeshesToObjects(6);



        PointMesh(RoadSlopeMeshL, RoadSlopeMeshR, (float)0.4, (float)Math.Sqrt(Math.Pow(0.4, 2) + Math.Pow(0.6, 2)), 7);//������ �����
        roadMeshes[7] = RoadMeshCreate(7);
        AssignMeshesToObjects(7);

        PointMesh(RoadSlopeMeshL, RoadSlopeMeshR, (float)0.4, (float)Math.Sqrt(Math.Pow(0.4, 2) + Math.Pow(0.6, 2)), 8);//������ ������
        roadMeshes[8] = RoadMeshCreate(8);
        AssignMeshesToObjects(8);



        //roadObject1[0] = GameObject.Find("Road");
        //roadObject1[1] = GameObject.Find("SideRoadL");
        //roadObject1[2] = GameObject.Find("SideRoadR");
        //roadObject1[3] = GameObject.Find("SopeRoadL");
        //roadObject1[4] = GameObject.Find("SopeRoadR");
        //roadObject1[5] = GameObject.Find("BottomRoadL");
        //roadObject1[6] = GameObject.Find("BottomRoadR");
        //roadObject1[7] = GameObject.Find("UpSopeRoadL");
        //roadObject1[8] = GameObject.Find("UpSopeRoadR");



        OnRoadMeshInitialized?.Invoke();


    }


}
