using UnityEngine;

public class ItemController : MonoBehaviour
{
    private LevelState levelState;
    private SpriteRenderer sr;

    void Awake()
    {
        levelState = Object.FindFirstObjectByType<LevelState>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (levelState != null)
            levelState.AddItem();

        if (CollectUI.Instance != null && sr != null)
            CollectUI.Instance.OnItemCollected(sr.sprite);

        Destroy(gameObject);
    }
}




