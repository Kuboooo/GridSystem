using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CircleMesh : MonoBehaviour {
    public int segments = 50; // Number of segments in the circle
    public Material circleMaterial; // The material to apply to the mesh

    public Vector3 worldPosition; 
    public float radius = 5f; // Radius of the circle

   public void GenerateMesh() {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];
        Color[] colors = new Color[vertices.Length];

        vertices[0] = new Vector3(0, 0, 0); // Center of the circle
        colors[0] = new Color(0, 0, 1, 0); // Blue with full transparency in the middle
        float angleStep = 360f / segments;
        
        for (int i = 1; i <= segments; i++) {
            float angle = Mathf.Deg2Rad * angleStep * i;
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;
            vertices[i] = new Vector3(x, y, 0);

            // Calculate the color for this vertex
            float alpha = Mathf.Lerp(0, 1, (float)i / segments); // Alpha increases with distance from center
            colors[i] = new Color(0, 0, 1, alpha); // Blue with increasing alpha
        }  
        Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = rotation * vertices[i];
        }
        // Add the worldPosition offset to each vertex
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] += new Vector3(0, 5, 0) + worldPosition;
        }


        for (int i = 0; i < segments; i++) {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 1) % segments + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors; // Assign the colors to the mesh
        mesh.RecalculateNormals();

        // Assign the mesh to the Mesh Filter component
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // Assign the material to the Mesh Renderer component
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = circleMaterial;
    }
}