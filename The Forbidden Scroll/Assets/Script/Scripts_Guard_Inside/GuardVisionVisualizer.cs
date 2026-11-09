using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GuardVisionVisualizer : MonoBehaviour
{
    public float viewDistance = 12.0f;
    public float viewAngle = 90.0f;
    public int segments = 20; // Feinheit des Bogens
    public Color visionColor = new Color(1f, 1f, 0f, 0.3f); // Halbtransparentes Gelb

    private Mesh mesh;
    private Material mat;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Renderer renderer = GetComponent<MeshRenderer>();
        
        // Sicherer Shader-Suchlauf mit mehreren Fallbacks für verschiedene Unity-Pipelines
        Shader shader = Shader.Find("Unlit/Color");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Standard");

        if (shader != null)
        {
            mat = new Material(shader);
            mat.color = visionColor;
            renderer.material = mat;
        }
        else
        {
            Debug.LogWarning("Kein Shader gefunden, nutze Standard-Renderer-Material.");
        }
    }

    void Update()
    {
        DrawVisionCone();
    }

    void DrawVisionCone()
    {
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero; // Startpunkt an der Wache (lokal)

        float angleStep = viewAngle / segments;
        float halfAngle = viewAngle / 2f;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -halfAngle + (angleStep * i);
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 dir = rotation * Vector3.forward;
            
            // Punkt auf dem Boden berechnen (leicht angehoben, damit es nicht flackert)
            vertices[i + 1] = (dir * viewDistance) + new Vector3(0, 0.05f, 0);

            if (i < segments)
            {
                triangles[i * 3] = 0;
                triangles[(i * 3) + 1] = i + 1;
                triangles[(i * 3) + 2] = i + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}