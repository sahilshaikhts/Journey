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

    public Vector3 P1 = new Vector3();
    public Vector3 P2 = new Vector3();
    public Vector3 P3 = new Vector3();
    public Vector3 P4 = new Vector3();
    public Vector3 P5 = new Vector3();

    public Material material;

    // Start is called before the first frame update
    void Start()
    {
        bezier = GameObject.FindGameObjectWithTag("Bezier").GetComponent<Bezier>();
        bezier.GenerateBezeirCurve(P1, P2, P3);
        bezier.GizmosOn = true;
        GenerateTubeMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            bezier.GenerateBezeirCurveFromLast(P4, P5);
        }
       GenerateTubeMesh();
    }

    void GenerateTubeMesh()
    {
        tubeVerts.Clear();

        Mesh mesh = new Mesh();

        for (int k = 0; k < bezier.points.Count; k++)
        {
            for (float i = 0; i < 2 * 3.14; i += (Mathf.PI * 2) / segments)
            {
                Vector3 pos = bezier.points[k] + new Vector3(0, Mathf.Sin(i), Mathf.Cos(i))*radius;
                tubeVerts.Add(pos);
            }

        }
        int[] triangles = new int[6*(segments*(bezier.segments-1))*2];

        for (int i = 0, vertInd = 0; i < triangles.Length; i += 6, vertInd++)
        {
            if (vertInd==0 || (vertInd+1) % segments != 0)
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
                triangles[i + 3] = vertInd -(segments-1);
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

    
}
