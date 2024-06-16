using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SphereMeshGenerator : MonoBehaviour {
    private float radius = 15f; // Radius of the sphere
    [SerializeField] public int segments = 20; // Number of segments in each circle

    public Vector3 worldPosition;

    private Mesh mesh;


    public void GenerateMesh(float inpuRadius) {
        radius = inpuRadius;
        mesh = new Mesh();

        // Generate vertices and triangles
        Vector3[] vertices = GenerateVertices();
        int[] triangles = GenerateTriangles();

        // Assign vertices and triangles to mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // Recalculate normals for proper shading

        // Assign the created mesh to the MeshFilter component
        GetComponent<MeshFilter>().mesh = mesh;
    }

    Vector3[] GenerateVertices() {
        int vertexCount = (segments + 1) * (segments + 1);
        Vector3[] vertices = new Vector3[vertexCount];

        float deltaTheta = 2f * Mathf.PI / segments;
        float deltaPhi = Mathf.PI / segments;

        int vertexIndex = 0;

        // Generate vertices
        for (int i = 0; i <= segments; i++) {
            float theta = i * deltaTheta;
            for (int j = 0; j <= segments; j++) {
                float phi = j * deltaPhi;

                float x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
                float z = radius * Mathf.Cos(phi);

                vertices[vertexIndex++] = new Vector3(x, y, z) + worldPosition;
            }
        }

        return vertices;
    }

    int[] GenerateTriangles() {
        int triangleCount = segments * segments * 2 * 3; // 2 triangles per segment face, 3 vertices per triangle
        int[] triangles = new int[triangleCount];

        int triangleIndex = 0;
        int vertexCount = segments + 1;

        // Generate triangles
        for (int i = 0; i < segments; i++) {
            for (int j = 0; j < segments; j++) {
                int vertexIndex = i * vertexCount + j;
                int nextVertexIndex = vertexIndex + vertexCount;

                // First triangle
                triangles[triangleIndex++] = vertexIndex;
                triangles[triangleIndex++] = vertexIndex + 1;
                triangles[triangleIndex++] = nextVertexIndex;

                // Second triangle
                triangles[triangleIndex++] = vertexIndex + 1;
                triangles[triangleIndex++] = nextVertexIndex + 1;
                triangles[triangleIndex++] = nextVertexIndex;
            }
        }

        return triangles;
    }
}