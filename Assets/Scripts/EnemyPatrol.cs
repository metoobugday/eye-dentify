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
    // Düşmanın görüş yönünü gösteren child obje (Gerekirse görseli döndürmek için)
    public Transform fovTransform;

    private Animator anim;
    private Vector3 targetPoint;
    private bool isWaiting = false;

    void Start()
    {
        // 1. Animator bileşenini alıyoruz
        anim = GetComponent<Animator>();

        // Başlangıç hedefi
        targetPoint = pointB.position;

        // Düşmanın başlangıç yönünü animasyona bildiriyoruz (A'dan B'ye, yani aşağı yönde)
        // Eğer B, A'dan daha aşağıdaysa 'isMovingUp' = false olmalı.
        // Genellikle Unity'de Y ekseninde aşağı gitmek Y değerini azaltır, yani 'targetPoint.y < transform.position.y'
        bool initiallyMovingUp = (targetPoint.y > transform.position.y);
        anim.SetBool("isMovingUp", initiallyMovingUp);
    }

    void Update()
    {
        if (isWaiting)
        {
            // Beklerken hareket animasyonunu kapat
            anim.SetBool("isMoving", false);
            return;
        }

        // Hareket başladı, animasyonu aç
        if (!anim.GetBool("isMoving"))
        {
            anim.SetBool("isMoving", true);
        }

        // Hedefe doğru hareket et
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        // Hedef noktaya geldiysek bekleme rutini başlat
        if (Vector3.Distance(transform.position, targetPoint) < 0.05f)
        {
            StartCoroutine(WaitAndSwitch());
        }

        // Opsiyonel: FOV yönünü sadece Y ekseninde (kuşbakışı labirent varsayımıyla)
        // Düşman hangi noktaya gidiyorsa (targetPoint) o yöne bakar.
        HandleFOVRotation();
    }

    IEnumerator WaitAndSwitch()
    {
        isWaiting = true;

        // 1. Beklemeye geçtiğimiz için hareket animasyonunu kapatıyoruz
        anim.SetBool("isMoving", false);

        // 2. Belirtilen süre kadar bekliyoruz
        yield return new WaitForSeconds(waitTime);

        // 3. Hedef noktayı tersine çevir
        Vector3 previousTarget = targetPoint;
        if (targetPoint == pointA.position)
            targetPoint = pointB.position;
        else
            targetPoint = pointA.position;

        // 4. Animasyon Yönünü Ayarla
        // Yeni hedef (targetPoint), eski hedeften (previousTarget) daha yukarıda mı?
        // Bu, düşmanın yukarı mı (true) yoksa aşağı mı (false) hareket edeceğini belirler.
        bool isMovingUp = (targetPoint.y > previousTarget.y);
        anim.SetBool("isMovingUp", isMovingUp);

        // 5. Bekleme bitti, harekete devam
        isWaiting = false;
    }

    private void HandleFOVRotation()
    {
        if (fovTransform == null) return;

        // Mevcut hedef, mevcut pozisyondan daha yukarıda mı?
        bool isTargetUp = targetPoint.y > transform.position.y;

        // Eğer isTargetUp doğruysa yukarı (90 derece), yanlışsa aşağı (-90 derece)
        float targetAngle = isTargetUp ? 90f : -90f;

        fovTransform.localRotation = Quaternion.Euler(0, 0, targetAngle);
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}