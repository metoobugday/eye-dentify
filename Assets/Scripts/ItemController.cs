using UnityEngine;

public class ItemController : MonoBehaviour
{
    private LevelState levelState;
    private SpriteRenderer sr;

    void Awake()
    {
        // FindFirstObjectByType Unity'nin yeni versiyonlarında daha performanslıdır, 
        // ama eski sürüm kullanıyorsan FindObjectOfType yapabilirsin.
        levelState = Object.FindFirstObjectByType<LevelState>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Sadece Player ile etkileşime girsin
        if (!collision.CompareTag("Player")) return;

        // Level State'e haber ver (varsa)
        if (levelState != null)
            levelState.AddItem();

        // UI Manager'a haber ver ve Sprite'ı gönder
        if (CollectUI.Instance != null && sr != null)
        {
            CollectUI.Instance.OnItemCollected(sr.sprite);
        }
        else
        {
            Debug.LogWarning("CollectUI veya SpriteRenderer bulunamadı!");
        }

        // Objeyi sahneden kaldır (UI'da kopyası oluştuğu için sorun yok)
        Destroy(gameObject);
    }
}