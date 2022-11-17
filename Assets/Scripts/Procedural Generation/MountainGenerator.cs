using Sahil.Perlin;
using System.Collections.Generic;
using UnityEngine;

public class MountainGenerator : MonoBehaviour
{
    [Header("Mountain properties")]

    [SerializeField] GameObject m_mountain;

    List<Vector3> tubeVerts = new List<Vector3>();

    [SerializeField]float width;

    [SerializeField]int segments;

    [SerializeField]
    float m_terrainHeight;

    [Header(" ")]
    [SerializeField]
    Bezier bezier;


    [SerializeField] PerlinNoiseGenerator m_noiseGenerator;
    Vector3 lastPoint = new Vector3();
    Vector2 m_perlinMapPosition=Vector2.zero;

    public Material material;

    public void Initialize(int meshInitLength)
    {
        m_noiseGenerator.SetSeed(Random.Range(0,1000));

        bezier.GizmosOn = true;

        InitializeTube(meshInitLength);
        m_mountain.GetComponent<Tube>().path = bezier.GetCurvePoints();
    }

    void InitializeTube(int length)
    {
        Vector3 P1 = GenerateNewPoint(transform.position);
        Vector3 P2 = GenerateNewPoint(P1);
        Vector3 P3 = GenerateNewPoint(P2);
        Vector3 P4 = GenerateNewPoint(P3);

        bezier.Intialize(P1, P2, P3, P4);

        lastPoint = P4;

        for (int i = 0; i < length; i++)
        {
            ExtendMesh();
        }

        GenerateTubeMesh();
    }

    public void ExtendMesh()
    {
        Vector3 newPoint = GenerateNewPoint(lastPoint);
        bezier.AddPoint(newPoint);

        lastPoint = newPoint;

        m_mountain.GetComponent<Tube>().path = bezier.GetCurvePoints();

        GenerateTubeMesh();
    }

    Vector3 GenerateNewPoint(Vector3 aLastPoint)
    {
        float displacement_y= m_noiseGenerator.GenerateValue(m_perlinMapPosition);
        
        //offset value to be in the range -1 to 1.
        
        displacement_y *= m_terrainHeight;

        m_perlinMapPosition += Vector2.one;
        
        return aLastPoint + new Vector3(Random.Range(5, 8), displacement_y, 0);
    }

    //note: This function clears and calculates all the verts instead of just the new ones and creates a new mesh!!! tf?
    void GenerateTubeMesh()
    {
        tubeVerts.Clear();

        Mesh mesh = new Mesh();

        int totalCurvePoints = bezier.GetCurvePoints().Length;
        Vector3[] curve = bezier.GetCurvePoints();

        for (int k = 0; k < totalCurvePoints; k++)
        {
            tubeVerts.Add(curve[k] + Vector3.forward * width);
            tubeVerts.Add(curve[k] + Vector3.back * width);
        }

        int[] triangles = new int[6 * (bezier.GetCurvePoints().Length - 1)];

        for (int i = 0, vertInd = 0; i < triangles.Length - 6; i += 6, vertInd += 2)
        {
            triangles[i] = vertInd;
            triangles[i + 1] = vertInd + 2;
            triangles[i + 2] = vertInd + 3;
            triangles[i + 3] = vertInd;
            triangles[i + 4] = vertInd + 3;
            triangles[i + 5] = vertInd + 1;
        }

        mesh.vertices = tubeVerts.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        m_mountain.GetComponent<MeshFilter>().mesh = mesh;
        m_mountain.GetComponent<MeshRenderer>().material = material;
        m_mountain.GetComponent<MeshCollider>().sharedMesh = mesh;

    }

    public int CropMesh(int points)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Vector3[] pointss = bezier.GetCurvePoints();

        //int ind = player.GetComponent<player>().index;
        int curvePointsRemoved= ((points + 1) / 4) *bezier.segments;

        bezier.RemovePoints(0, points);
        bezier.RegenerateCurvePoints();

        //player.GetComponent<player>().index -= curvePointsRemoved;

        m_mountain.GetComponent<Tube>().path = bezier.GetCurvePoints();

        return curvePointsRemoved;
    }

    public Vector3 RecenterMesh()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        List<Vector3> currentBezierPoints = new List<Vector3>(); //Temporarily store the points in an array
        currentBezierPoints.AddRange(bezier.GetPoints());

        Vector3 PlayerDistToOrigin = transform.position - (player.transform.position - player.GetComponent<player>().offsetFromCenter);

        //Add the distance from player to origin(center) to every control point(not curve points)
        for (int i = 0; i < currentBezierPoints.Count; i++)
        {
            currentBezierPoints[i] += PlayerDistToOrigin;
        }
        //Set new points to bezier
        bezier.SetPoints(currentBezierPoints);
        bezier.RegenerateCurvePoints();

        lastPoint = bezier.GetPoints()[bezier.GetPoints().Length - 1];

        player.transform.position += PlayerDistToOrigin;
        m_mountain.GetComponent<Tube>().path = bezier.GetCurvePoints();

        GenerateTubeMesh();
        return PlayerDistToOrigin;
    }
}

