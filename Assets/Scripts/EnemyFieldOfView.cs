using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EnemyFieldOfView : MonoBehaviour
{
    public float viewRadius = 5f;
    [Range(0, 360)] public float viewAngle = 90f;
    public int rayCount = 40;
    public LayerMask obstacleMask;

    Mesh viewMesh;
    MeshFilter meshFilter;

    private GameOverManager gm;


    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        meshFilter.mesh = viewMesh;

        gm = FindFirstObjectByType<GameOverManager>();
    }

    void LateUpdate()
    {
        DrawFOV();
        
        if (CanSeePlayer())
        {
            if (CanSeePlayer() && gm != null)
            {
                gm.GameOver();
            }

        }


    }

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
            Vector3 localDir = AngleToDir(angle);          // local direction
            Vector3 worldDir = transform.TransformDirection(localDir); // rotate with object

            RaycastHit2D hit = Physics2D.Raycast(origin, worldDir, viewRadius, obstacleMask);

            Vector3 endPoint = hit ? (Vector3)hit.point : origin + worldDir * viewRadius;

            // convert world point to local mesh space
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
        viewMesh.RecalculateBounds();
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

        // 1) Açı kontrolü
        float angleBetween = Vector3.Angle(transform.right, dirToPlayer);
        if (angleBetween > viewAngle / 2f)
            return false;

        // 2) Menzil kontrolü
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > viewRadius)
            return false;

        // 3) Duvar engel kontrolü (raycast)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask);
        if (hit)
            return false;

        return true;
    }

}
