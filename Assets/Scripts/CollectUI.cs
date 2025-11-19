using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CollectUI : MonoBehaviour
{
    // ==== Singleton & Persistence ====
    public static CollectUI Instance { get; private set; }

    [Header("Persistence")]
    public bool clearSlotsOnSceneLoad = true; // Level değişince slotları temizle

    void Awake()
    {
        // Tek kopya kuralı
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject); // Sahne değişse de yok olma
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (clearSlotsOnSceneLoad)
            ClearAllSlots(true);
        ResetCenterPopup();
    }

    // ==== Center Popup & Move-to-Slot (önceki adımda yaptık) ====
    [Header("Center Popup")]
    public CanvasGroup centerGroup;
    public Image centerImage;
    public float centerShowTime = 0.8f;
    public float moveTime = 0.4f;

    [Header("Inventory Slots")]
    public List<Image> slots = new List<Image>();

    public void OnItemCollected(Sprite sprite)
    {
        if (sprite == null) return;
        StartCoroutine(AnimatePickup(sprite));
    }

    private IEnumerator AnimatePickup(Sprite s)
    {
        centerImage.sprite = s;
        centerImage.SetNativeSize();
        yield return Fade(centerGroup, 0f, 1f, 0.15f);
        
        // Küçük bir gecikme: itemi topladığın anda basılı tuşlarla kapanmasın diye
        yield return new WaitForSeconds(0.1f);

        // Oyuncu herhangi bir tuşa basana kadar bekle
        yield return new WaitUntil(() => Input.anyKeyDown);

        Image target = GetNextEmptySlot();
        if (target == null)
        {
            yield return Fade(centerGroup, 1f, 0f, 0.15f);
            yield break;
        }

        Image moving = Instantiate(centerImage, centerImage.transform.parent);
        moving.raycastTarget = false;

        yield return Fade(centerGroup, 1f, 0f, 0.1f);

        RectTransform rt = moving.rectTransform;
        Vector3 start = rt.position;
        Vector3 end = target.rectTransform.position;

        float t = 0f;
        while (t < moveTime)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / moveTime);
            rt.position = Vector3.Lerp(start, end, k);
            yield return null;
        }

        target.sprite = s;
        target.enabled = true;
        Destroy(moving.gameObject);
    }

    private Image GetNextEmptySlot()
    {
        foreach (var img in slots)
        {
            if (img == null) continue;
            if (!img.enabled || img.sprite == null)
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

    // ==== Yardımcılar ====
    public void ClearAllSlots(bool disableImages)
    {
        foreach (var img in slots)
        {
            if (img == null) continue;
            img.sprite = null;
            if (disableImages) img.enabled = false;
        }
    }

    public void ResetCenterPopup()
    {
        if (centerGroup != null) centerGroup.alpha = 0f;
        if (centerImage != null) centerImage.sprite = null;
    }
}


