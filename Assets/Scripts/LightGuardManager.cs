using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random; // UnityEngine.Random ile System.Random karışıklığını önler

public class LightGuardManager : MonoBehaviour
{
    private Animator warningAnimator;

    public enum LightPhase
    {
        Safe = 0,
        Warning = 1,
        Check = 2
    }

    // --- Inspector'da ATAMANIZ GEREKENLER ---
    [Header("Gerekli Referanslar (Inspector'da Atayın!)")]
    public Light2D globalLight;
    public Light2D playerLight;
    public GameObject warningEye; // Bu objede Animator bileşeni olmalı

    [Header("Süreler (saniye)")]
    public float minSafeTime = 5f;
    public float maxSafeTime = 12f;
    public float warningDuration = 2f;
    public float checkDuration = 3f;

    [Header("Debug")]
    [SerializeField] private LightPhase currentPhase = LightPhase.Safe;
    [SerializeField] private float phaseTimer;

    public bool IsDangerActive => currentPhase == LightPhase.Check;

    void Start()
    {
        // 1. Animator referansını güvenli şekilde al
        if (warningEye != null)
        {
            warningAnimator = warningEye.GetComponent<Animator>();

            if (warningAnimator == null)
            {
                Debug.LogError("LightGuardManager: WarningEye objesinde **Animator** bileşeni bulunamadı.");
            }
            else if (warningAnimator.runtimeAnimatorController == null)
            {
                // Aldığınız "Animator is not playing an AnimatorController" hatasının kod tarafında yakalanması.
                Debug.LogError("LightGuardManager: WarningEye **Animator Controller**'ı atanmamış! Lütfen Inspector'da atama yapın.");
            }
        }
        else
        {
            Debug.LogError("LightGuardManager: **WarningEye** referansı **Inspector'da ATANMAMIŞ**. Faz geçişleri çalışmayacak.");
        }

        // Diğer referansların kontrolü
        if (globalLight == null || playerLight == null)
        {
            Debug.LogError("LightGuardManager: Işık (Global/Player) referansları Inspector'da atanmamış.");
        }

        // Oyuna SAFE ile başla
        EnterSafePhase();
    }

    void Update()
    {
        // Eğer global/player ışıklar atanmamışsa faz yönetimini durdur
        if (globalLight == null || playerLight == null) return;

        phaseTimer -= Time.deltaTime;

        if (phaseTimer <= 0f)
            SwitchPhase();
    }

    void SwitchPhase()
    {
        switch (currentPhase)
        {
            case LightPhase.Safe:
                EnterWarningPhase();
                break;

            case LightPhase.Warning:
                EnterCheckPhase();
                break;

            case LightPhase.Check:
                EnterSafePhase();
                break;
        }
    }

    // Merkezi Animator Tetikleme Fonksiyonu
    void SetAnimatorPhase(LightPhase phase)
    {
        // Yalnızca Animator geçerli ve Controller atanmışsa SetInteger'ı çağır.
        if (warningAnimator != null && warningAnimator.runtimeAnimatorController != null)
        {
            warningAnimator.SetInteger("Phase", (int)phase);
        }
    }

    //---------------------------------------------
    // SAFE (Phase = 0)
    //---------------------------------------------
    void EnterSafePhase()
    {
        currentPhase = LightPhase.Safe;
        phaseTimer = Random.Range(minSafeTime, maxSafeTime);

        // Animator Phase = 0
        SetAnimatorPhase(LightPhase.Safe);

        // Işık ayarları
        globalLight.intensity = 0f;
        playerLight.intensity = 1f;

        Debug.Log("[LightGuard] SAFE fazı. Hareket Serbest.");
    }

    //---------------------------------------------
    // WARNING (Phase = 1)
    //---------------------------------------------
    void EnterWarningPhase()
    {
        currentPhase = LightPhase.Warning;
        phaseTimer = warningDuration;

        // Animator Phase = 1
        SetAnimatorPhase(LightPhase.Warning);

        globalLight.intensity = 0f;
        playerLight.intensity = 1f;

        Debug.Log("[LightGuard] WARNING fazı. Göz Yanıp Sönüyor.");
    }

    //---------------------------------------------
    // CHECK (Phase = 2)
    //---------------------------------------------
    void EnterCheckPhase()
    {
        currentPhase = LightPhase.Check;
        phaseTimer = checkDuration;

        // Animator Phase = 2
        SetAnimatorPhase(LightPhase.Check);

        // Global ışık açılır
        globalLight.intensity = 1f;
        playerLight.intensity = 1f;

        Debug.Log("[LightGuard] CHECK fazı. Işık AÇIK, HAREKET YASAK!");
    }

    public LightPhase GetCurrentPhase()
    {
        return currentPhase;
    }
}