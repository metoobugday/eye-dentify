using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CollectUI : MonoBehaviour
{
    public static CollectUI Instance { get; private set; }

    [Header("Ayarlar")]
    public bool clearSlotsOnSceneLoad = true;

    [Header("Center Popup (Orta Ekran)")]
    public CanvasGroup centerGroup;
    public Image centerImage;
    public float centerShowTime = 0.8f;
    public float moveTime = 0.5f; // Uçuş süresi

    [Header("Inventory Icons (Sürükle-Bırak)")]
    // BURASI ÖNEMLİ: Buraya Slot'ları değil, içindeki 'Icon' objelerini atacağız.
    public List<Image> itemIcons = new List<Image>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (clearSlotsOnSceneLoad)
            ClearAllSlots();
        ResetCenterPopup();
    }

    public void OnItemCollected(Sprite sprite)
    {
        if (sprite == null) return;
        StartCoroutine(AnimatePickup(sprite));
    }

    private IEnumerator AnimatePickup(Sprite s)
    {
        // 1. Oyunu dondur
        Time.timeScale = 0f;

        // Ortadaki görseli ayarla ve göster
        centerImage.sprite = s;
        centerImage.SetNativeSize(); // Görselin orijinal boyutunu korur (veya silebilirsin)

        // Fade In
        yield return Fade(centerGroup, 0f, 1f, 0.15f);

        // Kullanıcı hemen geçmesin diye minik bekleme
        yield return new WaitForSecondsRealtime(0.2f);

        // Tuşa basılmasını bekle
        yield return new WaitUntil(() => Input.anyKeyDown);

        // Boş olan ilk İKONU bul (Slotu değil, ikonu arıyoruz)
        Image targetIcon = GetNextEmptyIcon();

        // Eğer yer yoksa kapat ve devam et
        if (targetIcon == null)
        {
            yield return Fade(centerGroup, 1f, 0f, 0.15f);
            Time.timeScale = 1f;
            yield break;
        }

        // 2. Oyunu devam ettir (Uçarken oyun aksın)
        Time.timeScale = 1f;

        // 3. Uçuş Animasyonu için kopya oluştur
        Image moving = Instantiate(centerImage, centerImage.transform.parent);
        moving.raycastTarget = false;

        // Ortadakini gizle
        centerGroup.alpha = 0f;

        RectTransform rt = moving.rectTransform;
        Vector3 startPos = rt.position;
        Vector3 endPos = targetIcon.rectTransform.position; // İkonun olduğu yere git

        float t = 0f;
        while (t < moveTime)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / moveTime);
            // Yumuşak geçiş (Ease-in-out)
            float smoothK = Mathf.SmoothStep(0f, 1f, k);

            rt.position = Vector3.Lerp(startPos, endPos, smoothK);

            // Opsiyonel: Giderken küçülsün istersen:
            // rt.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.5f, smoothK);

            yield return null;
        }

        // 4. Hedefe vardı, İkonu aç
        targetIcon.sprite = s;
        targetIcon.enabled = true; // Gizli olan ikonu görünür yap

        Destroy(moving.gameObject); // Uçan kopyayı yok et
    }

    // Boş (gizli) olan ilk ikonu bulur
    private Image GetNextEmptyIcon()
    {
        foreach (var img in itemIcons)
        {
            // Eğer image enable değilse (kapalıysa) boştur
            if (img != null && !img.enabled)
                return img;
        }
        return null;
    }

    private IEnumerator Fade(CanvasGroup g, float a, float b, float d)
    {
        float t = 0f; g.alpha = a;
        while (t < d)
        {
            t += Time.unscaledDeltaTime;
            g.alpha = Mathf.Lerp(a, b, t / d);
            yield return null;
        }
        g.alpha = b;
    }

    public void ClearAllSlots()
    {
        foreach (var img in itemIcons)
        {
            if (img != null)
            {
                img.sprite = null;
                img.enabled = false; // İkonu gizle (Kutu arkada görünmeye devam eder)
            }
        }
    }

    public void ResetCenterPopup()
    {
        if (centerGroup != null) centerGroup.alpha = 0f;
    }
}