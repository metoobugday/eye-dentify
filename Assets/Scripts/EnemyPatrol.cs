using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float waitTime = 1f;

    [Header("FOV")]
    public Transform fovTransform;   // FOV child buraya atanacak

    private Vector3 targetPoint;
    private bool isWaiting;

    void Start()
    {
        targetPoint = pointB.position;
    }

    void Update()
    {
        if (isWaiting) return;

        // Hedefe doğru hareket
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        Vector2 moveDir = (targetPoint - transform.position).normalized;

        if (moveDir.y > 0.01f)
        {
            fovTransform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (moveDir.y < -0.01f)
        {
            fovTransform.localRotation = Quaternion.Euler(0, 0, -90);
        }

        // Hedefe ulaştık mı?
        if (Vector3.Distance(transform.position, targetPoint) < 0.05f)
        {
            StartCoroutine(WaitAndSwitch());
        }
    }

    IEnumerator WaitAndSwitch()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        // Hedefi değiştir
        if (targetPoint == pointA.position)
            targetPoint = pointB.position;
        else
            targetPoint = pointA.position;

        isWaiting = false;
    }

    void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
