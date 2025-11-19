using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightGuardManager : MonoBehaviour
{
    public enum LightPhase
    {
        Safe,       // Oyuncu özgürce hareket edebilir, sadece oyuncu ışığı açık
        Warning,    // Göz UI uyarısı (yakında denetim ışığı açılacak)
        Check       // Tüm map aydınlık, hareket yasak (Green Light / Red Light anı)
    }

    [Header("Lights")]
    public Light2D globalLight;
    public Light2D playerLight;

    [Header("Warning UI")]
    public GameObject warningEye;


    [Header("Süreler (saniye)")]
    public float minSafeTime = 5f;      // Bir sonraki denetime kadar minimum bekleme
    public float maxSafeTime = 12f;     // Bir sonraki denetime kadar maksimum bekleme
    public float warningDuration = 2f;  // Uyarı (göz) süresi
    public float checkDuration = 3f;    // Denetim ışığı açık kalma süresi

    [Header("Debug (sahnede görmek için)")]
    [SerializeField] private LightPhase currentPhase = LightPhase.Safe;
    [SerializeField] private float phaseTimer;   // O anki fazın kalan süresi

    // Diğer scriptlerin kullanacağı: Şu an hareket yasak mı?
    public bool IsDangerActive => currentPhase == LightPhase.Check;

    void Start()
    {
        // Oyuna Safe durumunda başla
        EnterSafePhase();
    }

    void Update()
    {
        phaseTimer -= Time.deltaTime;

        if (phaseTimer <= 0f)
        {
            SwitchPhase();
        }
    }

    private void Awake()
    {
        // WARNING EYE referansı inspector'da boşsa otomatik bul
        if (warningEye == null)
        {
            warningEye = GameObject.Find("WarningEye");
        }

        // Işıklar da restart sonrası kaçarsa onları da otomatik bulalım
        if (globalLight == null)
        {
            globalLight = FindAnyObjectByType<UnityEngine.Rendering.Universal.Light2D>();
        }

        if (playerLight == null)
        {
            playerLight = GameObject.Find("Player1.1")?.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
        }
    }


    void SwitchPhase()
    {
        switch (currentPhase)
        {
            case LightPhase.Safe:
                // Safe bittiyse Warning'e geç
                EnterWarningPhase();
                break;

            case LightPhase.Warning:
                // Warning bittiyse Check'e geç
                EnterCheckPhase();
                break;

            case LightPhase.Check:
                // Check bittiyse tekrar Safe'e dön
                EnterSafePhase();
                break;
        }
    }

    void EnterSafePhase()
    {
        currentPhase = LightPhase.Safe;

        // Bir sonraki Warning’e kadar rastgele süre seç
        phaseTimer = Random.Range(minSafeTime, maxSafeTime);

        Debug.Log("[LightGuard] SAFE fazına geçildi. Bir sonraki WARNING " + phaseTimer.ToString("F1") + " sn sonra.");
        // İLERİDE: burada "sadece oyuncu ışığını aç, map ışığını kapat" diyeceğiz.

        // SAFE → Sadece player light açık, global kapalı
        globalLight.intensity = 0f;
        playerLight.intensity = 1f;

        // Göz uyarısı kapalı
        if (warningEye != null)
            warningEye.SetActive(false);

    }

    void EnterWarningPhase()
    {
        currentPhase = LightPhase.Warning;
        phaseTimer = warningDuration;

        Debug.Log("[LightGuard] WARNING! Göz uyarısı göster. " + warningDuration + " sn sonra CHECK başlayacak.");
        // İLERİDE: burada "göz UI aç, belki ufak ses" diyeceğiz.

        // WARNING → Işıklar Safe ile aynı, sadece UI göz uyarısı aktif
        globalLight.intensity = 0f;
        playerLight.intensity = 1f;

        if (warningEye != null)
            warningEye.SetActive(true);

    }

    void EnterCheckPhase()
    {
        currentPhase = LightPhase.Check;
        phaseTimer = checkDuration;

        Debug.Log("[LightGuard] CHECK! Tüm map ışığı açıldı, hareket yasak. Süre: " + checkDuration + " sn");
        // İLERİDE: burada "tüm map ışığı aç, oyuncu ışığını ayarla" diyeceğiz.

        // CHECK → Hem global light hem player light açık
        globalLight.intensity = 1f;
        playerLight.intensity = 1f;

        // Göz uyarısı kapalı
        if (warningEye != null)
            warningEye.SetActive(true);

    }

    // İstersen başka scriptler de hangi fazda olduğumuzu sorabilsin:
    public LightPhase GetCurrentPhase()
    {
        return currentPhase;
    }

}
