using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeGenerator : MonoBehaviour
{
    [Header("Tube properties")]

    [SerializeField]
    GameObject tube;

    [SerializeField]
    float radius;

    [SerializeField]
    int segments;

    [Header(" ")]
    [SerializeField]
    Bezier bezier;

    List<Vector3> tubeVerts = new List<Vector3>();
    List<Vector2> tubseVerts = new List<Vector2>();

    public Vector3 lastPoint = new Vector3();
    public Vector2 perlinMapPosition;

    public Material material;

    bool gameRunning = true;

    void Start()
    {
        perlinMapPosition = new Vector2(0.001f, 0.001f);

        bezier.GizmosOn = true;

        InitializeTube();
        tube.GetComponent<Tube>().path = bezier.GetCurvePoints();

        StartCoroutine(ExtendMeshAtInterval(2));
        StartCoroutine(CropMesh());
        StartCoroutine(RecenterMesh());

    }

    void InitializeTube()
    {
        Vector3 P1 = transform.position;
        Vector3 P2 = P1 + new Vector3(5, Random.Range(-0.5f, 0.5f));
        Vector3 P3 = P2 + new Vector3(4, Random.Range(-0.5f, 0.5f));
        Vector3 P4 = P3 + new Vector3(4, Random.Range(-0.5f, 0.5f));

        bezier.Intialize(P1, P2, P3, P4);

        lastPoint = P4 + Vector3.right;

        for (int i = 0; i < 10; i++)
        {
            ExtendMesh();
        }

        GenerateTubeMesh();
    }

    void ExtendMesh()
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
        float displacement_z = 0;

        displacement_y = Mathf.PerlinNoise(perlinMapPosition.x, perlinMapPosition.y);
        displacement_y *= Random.Range(0, 2) >= 1 ? 1 : -1;

        if (Random.Range(0, 10) > 7)
        {
            displacement_z = Random.Range(-1.0f, 1.0f);
        }

        newPoint = aLastPoint + new Vector3(Random.Range(5, 8), displacement_y, 0);

        return newPoint;
    }

    void GenerateTubeMesh()
    {
        tubeVerts.Clear();

        Mesh mesh = new Mesh();

        int totalCurvePoints = bezier.GetCurvePoints().Length;
        Vector3[] curve = bezier.GetCurvePoints();

        for (int k = 0; k < totalCurvePoints; k++)
        {
            for (float i = 0; i < 2 * 3.14; i += (Mathf.PI * 2) / segments)
            {
                Vector3 pos = curve[k] + new Vector3(0, Mathf.Sin(i), Mathf.Cos(i)) * radius;
                tubeVerts.Add(pos);
            }

        }

        int[] triangles = new int[6 * segments * (bezier.GetCurvePoints().Length - 1)];

        for (int i = 0, vertInd = 0; i < triangles.Length; i += 6, vertInd++)
        {
            if (vertInd == 0 || (vertInd + 1) % segments != 0)
            {
                triangles[i] = vertInd;
                triangles[i + 1] = vertInd + segments + 1;
                triangles[i + 2] = vertInd + segments;
                triangles[i + 3] = vertInd + 1;
                triangles[i + 4] = triangles[i + 1];
                triangles[i + 5] = vertInd;
            }
            else
            {
                triangles[i] = vertInd;
                triangles[i + 1] = vertInd + 1;
                triangles[i + 2] = vertInd + segments;
                triangles[i + 3] = vertInd - (segments - 1);
                triangles[i + 4] = triangles[i + 1];
                triangles[i + 5] = vertInd;
            }
        }

        mesh.vertices = tubeVerts.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        tube.GetComponent<MeshFilter>().mesh = mesh;
        tube.GetComponent<MeshRenderer>().material = material;

    }
    

    
    IEnumerator ExtendMeshAtInterval(float interval)
    {
        while (gameRunning)
        {
            yield return new WaitForSeconds(interval);

            ExtendMesh();
        }
    }

    IEnumerator CropMesh()
    {
        while (gameRunning)
        {
            yield return new WaitForSeconds(20);
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            Vector3[] pointss = bezier.GetCurvePoints();
            int ind = player.GetComponent<player>().index;
            bezier.RemovePoints(0, 14);
            bezier.RegenerateCurvePoints();

            player.GetComponent<player>().index -= (15/4)*bezier.segments;
            
            Vector3[] pointss_1 = bezier.GetCurvePoints();
            ind = player.GetComponent<player>().index;
            tube.GetComponent<Tube>().path = bezier.GetCurvePoints();

            GenerateTubeMesh();
        }
    }

    IEnumerator RecenterMesh()
    {
        while (gameRunning)
        {
            yield return new WaitForSeconds(25);

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            Vector3[] currentBezierPoints = bezier.GetPoints();

            Vector3 PlayerDistToOrigin = -1 * (player.transform.position - player.GetComponent<player>().offsetFromCenter) - transform.position;

            Vector3 DistToNextTargetLeft = player.GetComponent<player>().GetTargetPosition() - player.transform.position;

            Vector3 PlayerTargetPos = player.GetComponent<player>().GetTargetPosition() + PlayerDistToOrigin;


            for (int i = 0; i < currentBezierPoints.Length; i++)
            {
                currentBezierPoints[i] += PlayerDistToOrigin;
            }

            bezier.Intialize(currentBezierPoints[0], currentBezierPoints[1], currentBezierPoints[2], currentBezierPoints[3]);

            for (int i = 4; i < currentBezierPoints.Length; i += 3)
            {
                bezier.AddPoint(currentBezierPoints[i]);
            }
            lastPoint = currentBezierPoints[currentBezierPoints.Length - 1];


            player.transform.position = transform.position;
            GenerateTubeMesh();
        }
    }
}
