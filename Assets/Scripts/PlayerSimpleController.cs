using UnityEngine;

public class PlayerSimpleController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;
    public float speed = 5f;

    [Header("Sprites")]
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;
    public Sprite idleSprite;

    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>(); // Eğer SpriteRenderer child objede ise -> GetComponentInChildren<SpriteRenderer>()
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        direction = new Vector2(x, y).normalized;

        // Sprite değişimi
        if (y > 0)
            sr.sprite = spriteUp;
        else if (y < 0)
            sr.sprite = spriteDown;
        else if (x > 0)
            sr.sprite = spriteRight;
        else if (x < 0)
            sr.sprite = spriteLeft;
        else
            sr.sprite = idleSprite;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }
}
