using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class RoadMaker : MonoBehaviour
{
    public float roadWidth = 2f;
    public Material roadMaterial;
    public List<Vector3> points = new List<Vector3>();

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();

        mesh = new Mesh();
        meshFilter.mesh = mesh;
        meshRenderer.material = roadMaterial;
    }

    void Update()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            GenerateRoadMesh();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var point in points)
        {
            Gizmos.DrawSphere(point, 0.2f);
        }

        if (points.Count > 1)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector3 direction = (points[i + 1] - points[i]).normalized;
                Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x);
                Gizmos.DrawLine(points[i] + perpendicular * roadWidth, points[i] - perpendicular * roadWidth);
                Gizmos.DrawLine(points[i + 1] + perpendicular * roadWidth, points[i + 1] - perpendicular * roadWidth);
            }
        }
    }

    void GenerateRoadMesh()
    {
        if (points.Count < 2) return;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < points.Count; i++)
        {
            if (i < points.Count - 1)
            {
                Vector3 direction = (points[i + 1] - points[i]).normalized;
                Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x);

                vertices.Add(points[i] + perpendicular * roadWidth);
                vertices.Add(points[i] - perpendicular * roadWidth);

                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));

                if (i > 0)
                {
                    int index = i * 2;
                    triangles.Add(index - 2);
                    triangles.Add(index - 1);
                    triangles.Add(index);

                    triangles.Add(index - 1);
                    triangles.Add(index + 1);
                    triangles.Add(index);
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    void OnGUI()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                points.Add(hit.point);
                GenerateRoadMesh();
            }
        }
    }
}

