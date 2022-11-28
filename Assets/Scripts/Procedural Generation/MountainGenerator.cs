using Sahil.Perlin;
using System.Collections.Generic;
using UnityEngine;

public class MountainGenerator : MonoBehaviour
{
    [Header("Mountain properties")]

    [SerializeField] MeshFilter m_mountain_MeshFilter;
    [SerializeField] MeshRenderer m_mountain_Renderer;
    [SerializeField] MeshCollider m_mountain_MeshCollider;
    [SerializeField] Material m_mountain_material;

    [SerializeField] float m_mountain_heightMultiplier;
    [SerializeField] float m_mountain_width;

    [SerializeField] int m_mesh_segments;

    [SerializeField] Bezier m_bezier;

    [SerializeField]PerlinNoiseGenerator m_noiseGenerator;
    Vector3 lastPoint = new Vector3();
    Vector2 m_perlinMapPosition=Vector2.zero;

    List<Vector3> m_vertices = new List<Vector3>();

    public void Initialize(int meshInitLength, float aExtendDistance)
    {
        m_noiseGenerator.SetSeed(Random.Range(0,1000));

        InitializeMountain(meshInitLength,aExtendDistance);
     
        m_mountain_Renderer.material = m_mountain_material;
    }

    void InitializeMountain(int length,float aExtendDistance)
    {
        float increment= aExtendDistance / m_mesh_segments;

        Vector3 P1 = GenerateNewPoint(transform.position, increment);
        Vector3 P2 = GenerateNewPoint(P1, increment);
        Vector3 P3 = GenerateNewPoint(P2, increment);
        Vector3 P4 = GenerateNewPoint(P3, increment);

        m_bezier.Intialize(P1, P2, P3, P4);

        lastPoint = P4;

        GenerateTubeMesh();
    }

    public Vector3 ExtendMesh(float aExtendDistance)
    {
        Vector3 newPoint;
        float increment = aExtendDistance / 5;

        for (float i = 0; i < increment; i++)
        {
            newPoint = GenerateNewPoint(lastPoint, increment);

            m_bezier.AddPoint(newPoint);
            lastPoint = newPoint;
        }

        GenerateTubeMesh();

        return lastPoint;
    }

    Vector3 GenerateNewPoint(Vector3 aLastPoint, float aIncrement)
    {
        float displacement_y= m_noiseGenerator.GenerateValue(m_perlinMapPosition);

       ////add "terrace" WIP ,figre it out
       //float W = 0.2f; // m_width of terracing bands
       //float k = Mathf.Floor(displacement_y / W);
       //float f = (displacement_y - k * W) / W;
       //float s = Mathf.Min(2 * f, 1.0f);
       // displacement_y = (k + s) * W;
       
        displacement_y *= m_mountain_heightMultiplier;

        m_perlinMapPosition += Vector2.one;
        
        return aLastPoint + new Vector3(5, displacement_y, 0);
    }

    //note: This function clears and calculates all the verts instead of just the new ones and creates a new mesh!!! tf?
    void GenerateTubeMesh()
    {
        m_vertices.Clear();

        Mesh mesh = new Mesh();

        int totalCurvePoints = m_bezier.GetCurvePoints().Length;
        Vector3[] curve = m_bezier.GetCurvePoints();

        for (int k = 0; k < totalCurvePoints; k++)
        {
            m_vertices.Add(curve[k] + Vector3.forward * m_mountain_width);
            m_vertices.Add(curve[k] + Vector3.back * m_mountain_width);
        }

        int[] triangles = new int[6 * (m_bezier.GetCurvePoints().Length - 1)];

        for (int i = 0, vertInd = 0; i < triangles.Length - 6; i += 6, vertInd += 2)
        {
            triangles[i] = vertInd;
            triangles[i + 1] = vertInd + 2;
            triangles[i + 2] = vertInd + 3;
            triangles[i + 3] = vertInd;
            triangles[i + 4] = vertInd + 3;
            triangles[i + 5] = vertInd + 1;
        }

        mesh.vertices = m_vertices.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        m_mountain_MeshFilter.mesh = mesh;
        m_mountain_MeshFilter.mesh.RecalculateNormals();
        m_mountain_MeshCollider.sharedMesh = mesh;

    }

    public int CropMesh(int points)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Vector3[] pointss = m_bezier.GetCurvePoints();

        //int ind = player.GetComponent<player>().index;
        int curvePointsRemoved= ((points + 1) / 4) *m_bezier.segments;

        m_bezier.RemovePoints(0, points);
        m_bezier.RegenerateCurvePoints();

        //player.GetComponent<player>().index -= curvePointsRemoved;

        return curvePointsRemoved;
    }

    public Vector3 RecenterMesh()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        List<Vector3> currentBezierPoints = new List<Vector3>(); //Temporarily store the points in an array
        currentBezierPoints.AddRange(m_bezier.GetPoints());

        Vector3 PlayerDistToOrigin = transform.position - (player.transform.position - player.GetComponent<player>().offsetFromCenter);

        //Add the distance from player to origin(center) to every control point(not curve points)
        for (int i = 0; i < currentBezierPoints.Count; i++)
        {
            currentBezierPoints[i] += PlayerDistToOrigin;
        }
        //Set new points to m_bezier
        m_bezier.SetPoints(currentBezierPoints);
        m_bezier.RegenerateCurvePoints();

        lastPoint = m_bezier.GetPoints()[m_bezier.GetPoints().Length - 1];

        player.transform.position += PlayerDistToOrigin;
        //m_mountain.GetComponent<Tube>().path = m_bezier.GetCurvePoints();

        GenerateTubeMesh();
        return PlayerDistToOrigin;
    } 
}

