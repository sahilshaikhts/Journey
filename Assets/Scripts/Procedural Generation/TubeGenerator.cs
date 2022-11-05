using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeGenerator : MonoBehaviour
{
    [Header("Tube properties")]

    [SerializeField]
    GameObject tube;

    [SerializeField]
    float width;

    [SerializeField]
    int segments;

    [Header(" ")]
    [SerializeField]
    Bezier bezier;

    List<Vector3> tubeVerts = new List<Vector3>();

    public Vector3 lastPoint = new Vector3();
    public Vector2 perlinMapPosition;

    public Material material;

    public void Initialize(int meshInitLength)
    {
        perlinMapPosition = new Vector2(0.001f, 0.001f);

        bezier.GizmosOn = true;

        InitializeTube(meshInitLength);
        tube.GetComponent<Tube>().path = bezier.GetCurvePoints();

    }

    void InitializeTube(int length)
    {
        Vector3 P1 = transform.position;
        Vector3 P2 = P1 + new Vector3(5, Random.Range(-0.5f, 0.5f),0);
        Vector3 P3 = P2 + new Vector3(4, Random.Range(-0.5f, 0.5f),0);
        Vector3 P4 = P3 + new Vector3(4, Random.Range(-0.5f, 0.5f),0);

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

        perlinMapPosition += new Vector2(0.01f, 0.01f);

        tube.GetComponent<Tube>().path = bezier.GetCurvePoints();

        GenerateTubeMesh();
    }

    Vector3 GenerateNewPoint(Vector3 aLastPoint)
    {
        Vector3 newPoint;

        float displacement_y = 0;

        displacement_y = Mathf.PerlinNoise(perlinMapPosition.x*2.5f, perlinMapPosition.y*2.5f)- 0.5f;

        if(displacement_y <.15f)
        {
            displacement_y *= Random.Range(15, 22);
        }else
        {
            displacement_y = Mathf.Min(.1f,displacement_y);
            Debug.Log(displacement_y);
        }    

        //displacement_y *= Random.Range(0, 3) >= 1 ? -1 : 1;


        newPoint = aLastPoint + new Vector3(Random.Range(5, 8), displacement_y, 0);

        return newPoint;
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

        tube.GetComponent<MeshFilter>().mesh = mesh;
        tube.GetComponent<MeshRenderer>().material = material;
        tube.GetComponent<MeshCollider>().sharedMesh = mesh;

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

        tube.GetComponent<Tube>().path = bezier.GetCurvePoints();

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
        tube.GetComponent<Tube>().path = bezier.GetCurvePoints();

        GenerateTubeMesh();
        return PlayerDistToOrigin;
    }
}
