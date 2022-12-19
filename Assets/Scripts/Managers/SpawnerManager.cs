using Sahil;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] Spawner_sequence m_sequence_spawner;
    [SerializeField]Material m_rampMaterial;
    public void Initialize()
    {
        m_sequence_spawner.Initialize();
    }

    public void PopulateArea(Vector3[] aSpawnPoints)
    {
        float surfaceAngle;
        for (int i = 0; i < aSpawnPoints.Length - 1; i++)
        {
            Vector3 slopDirection = (aSpawnPoints[i + 1] - aSpawnPoints[i]).normalized;

            SpawnLandscapes(aSpawnPoints[i], (aSpawnPoints[aSpawnPoints.Length - 1] - aSpawnPoints[0]).normalized);

            if (Random.Range(0, 10) < 3)
            {
                SpawnTrees(aSpawnPoints[i], slopDirection);
            }
            SpawnObstacles(aSpawnPoints[i], slopDirection);

           surfaceAngle = GetSurfaceAngle(slopDirection);
        }
        Vector3[] rampCurvePoints=new Vector3[(aSpawnPoints.Length/4)];
        Array.Copy(aSpawnPoints, aSpawnPoints.Length / 4, rampCurvePoints, 0, (aSpawnPoints.Length / 4) - 1);
        float width = 70 * Random.Range(0.1f, 0.5f);

        //GenerateIceRamp(rampCurvePoints, width);
    }

    public void SpawnLandscapes(Vector3 aPosition, Vector3 aRotation)
    {
        m_sequence_spawner.SpawnProp("MountainPeak_L", aPosition, Quaternion.LookRotation(aRotation));
        m_sequence_spawner.SpawnProp("MountainPeak_R", aPosition, Quaternion.LookRotation(aRotation));
    }
    public void SpawnTrees(Vector3 aPosition, Vector3 aRotation)
    {
        m_sequence_spawner.SpawnProp("Tree_right", aPosition, Quaternion.LookRotation(aRotation + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3))));
        m_sequence_spawner.SpawnProp("Tree_left", aPosition, Quaternion.LookRotation(aRotation + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3))));
    }

    public void SpawnObstacles(Vector3 aPosition, Vector3 aRotation)
    {
        float surfaceAngle = GetSurfaceAngle(aRotation);
        if (surfaceAngle >10 )
            m_sequence_spawner.SpawnProp("obst_mountainArch", aPosition, Quaternion.LookRotation(aRotation));

       // if (surfaceAngle < 20 && Random.Range(0, 10) > 5)
       //     m_sequence_spawner.SpawnProp("obst_med_rock", aPosition + Vector3.forward * Random.Range(-5, 5.0f), Quaternion.LookRotation(aRotation));
    }

    float GetSurfaceAngle(Vector3 aDirection)
    {
        return Vector3.SignedAngle(Vector3.right, aDirection,Vector3.forward);
    }

    void GenerateIceRamp(Vector3[] aCurvePoint,float aWidth)
    {
        GameObject ramp = (new GameObject("ramp", typeof(MeshRenderer), typeof(MeshFilter)));
        MeshFilter ramp_MeshFilter = ramp.GetComponent<MeshFilter>();
        MeshCollider ramp_MeshCollider = ramp.GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        for (int k = 0; k < aCurvePoint.Length; k++)
        {
            vertices.Add(aCurvePoint[k] + Vector3.forward * aWidth);
            vertices.Add(aCurvePoint[k] + Vector3.back * aWidth);
        }

        int[] triangles = new int[6 * (aCurvePoint.Length - 1)];

        for (int i = 0, vertInd = 0; i < triangles.Length - 6; i += 6, vertInd += 2)
        {
            triangles[i] = vertInd;
            triangles[i + 1] = vertInd + 2;
            triangles[i + 2] = vertInd + 3;
            triangles[i + 3] = vertInd;
            triangles[i + 4] = vertInd + 3;
            triangles[i + 5] = vertInd + 1;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        ramp_MeshFilter.mesh = mesh;
        ramp_MeshFilter.mesh.RecalculateNormals();
        ramp.GetComponent<Renderer>().material = m_rampMaterial;
        ramp.transform.position += Vector3.up * 0.1f;
    }
}
