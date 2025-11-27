using UnityEngine;
// Light 2D'ye erişmek için bu kütüphane şart:
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EnemyFieldOfView : MonoBehaviour
{
    public float viewRadius = 5f;
    [Range(0, 360)] public float viewAngle = 90f;
    public int rayCount = 40;
    public LayerMask obstacleMask;

    // --- YENİ EKLENEN ---
    // Sahnedeki Global Light objesini buraya sürükleyeceğiz
    public Light2D sceneGlobalLight;
    // --------------------

    Mesh viewMesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer; // Renderer'ı global yaptık ki Update'te erişelim

    private GameOverManager gm;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>(); // Referansı al

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        meshFilter.mesh = viewMesh;

        // Katman ayarları (Önceki adımdan)
        if (meshRenderer != null)
        {
            meshRenderer.sortingLayerName = "Grounds";
            meshRenderer.sortingOrder = 10;
        }

        gm = FindFirstObjectByType<GameOverManager>();
    }

    void LateUpdate()
    {
        DrawFOV();

        
        if (CanSeePlayer())
        {
            if (gm != null)
            {
                gm.GameOver();
            }
        }
    }

    // ... (DrawFOV, AngleToDir ve CanSeePlayer fonksiyonları aynen kalacak) ...

    void DrawFOV()
    {
        float stepAngle = viewAngle / rayCount;
        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];
        vertices[0] = Vector3.zero;
        int v = 1;
        int t = 0;
        Vector3 origin = transform.position;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = -viewAngle / 2 + stepAngle * i;
            Vector3 localDir = AngleToDir(angle);
            Vector3 worldDir = transform.TransformDirection(localDir);

            RaycastHit2D hit = Physics2D.Raycast(origin, worldDir, viewRadius, obstacleMask);
            Vector3 endPoint = hit ? (Vector3)hit.point : origin + worldDir * viewRadius;
            vertices[v] = transform.InverseTransformPoint(endPoint);

            if (i > 0)
            {
                triangles[t++] = 0;
                triangles[t++] = v - 1;
                triangles[t++] = v;
            }
            v++;
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
        viewMesh.RecalculateBounds(); // Bounds hatasını önler
    }

    Vector3 AngleToDir(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    bool CanSeePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
        float angleBetween = Vector3.Angle(transform.right, dirToPlayer);
        if (angleBetween > viewAngle / 2f) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > viewRadius) return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask);
        if (hit) return false;

        return true;
    }
}