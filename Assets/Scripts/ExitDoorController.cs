using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoorController : MonoBehaviour
{
    [Header("Refs")]
    public LevelState levelState;          // Boş bırakırsan otomatik bulur
    public SpriteRenderer doorSprite;      // (opsiyonel) kapı görseli
    public Sprite closedSprite;            // (opsiyonel)
    public Sprite openSprite;              // (opsiyonel)

    [Header("Scene Load")]
    public bool loadByBuildIndex = true;   // true: aktif sahne +1
    public string nextSceneName;           // false ise buraya sahne adı gir

    void Awake()
    {
        if (levelState == null)
            levelState = Object.FindFirstObjectByType<LevelState>();
    }

    void Update()
    {
        // Görsel durumu canlı güncelle (opsiyonel)
        if (doorSprite != null)
            doorSprite.sprite = IsOpen() ? openSprite : closedSprite;
    }

    private bool IsOpen()
    {
        return levelState != null &&
               levelState.collectedItemCount >= levelState.requiredItemCount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (IsOpen())
        {
            if (loadByBuildIndex)
            {
                int i = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(i + 1);
            }
            else
            {
                if (!string.IsNullOrEmpty(nextSceneName))
                    SceneManager.LoadScene(nextSceneName);
                else
                    Debug.LogError("nextSceneName boş! Inspector'da sahne adını gir.");
            }
        }
        else
        {
            Debug.Log("Kapı kilitli: Gerekli item sayısı henüz tamamlanmadı.");
        }
    }
}
