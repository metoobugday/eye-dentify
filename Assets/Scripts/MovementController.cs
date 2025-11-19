using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction = Vector2.down;
    public float speed = 5f;

    private LightGuardManager lightGuard;
    public System.Action OnMoveDuringCheck;   // Game Over için event
    
    private GameOverManager gameOver;


    [Header("Input (WASD)")]
    public KeyCode inputUp = KeyCode.W;
    public KeyCode inputDown = KeyCode.S;
    public KeyCode inputLeft = KeyCode.A;
    public KeyCode inputRight = KeyCode.D;

    [Header("Sprites")]
    public AnimatedSpriteRenderer spriteRendererUp;
    public AnimatedSpriteRenderer spriteRendererDown;
    public AnimatedSpriteRenderer spriteRendererLeft;
    public AnimatedSpriteRenderer spriteRendererRight;
    public AnimatedSpriteRenderer spriteRendererDeath;
    private AnimatedSpriteRenderer activeSpriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        activeSpriteRenderer = spriteRendererDown;

        lightGuard = FindAnyObjectByType<LightGuardManager>();

        gameOver = FindAnyObjectByType<GameOverManager>();
        if (gameOver != null)
        {
            OnMoveDuringCheck += gameOver.GameOver;
        }
    }

    private void Update()
    {
        // Eğer CHECK fazındaysa hareket girişini kontrol edip Game Over tetikleyelim
        if (lightGuard != null && lightGuard.IsDangerActive)
        {
            bool isPressing =
                Input.GetKey(inputUp) ||
                Input.GetKey(KeyCode.UpArrow) ||
                Input.GetKey(inputDown) ||
                Input.GetKey(KeyCode.DownArrow) ||
                Input.GetKey(inputLeft) ||
                Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(inputRight) ||
                Input.GetKey(KeyCode.RightArrow);

            if (isPressing)
            {
                Debug.Log("[PLAYER] CHECK fazında hareket etmeye çalıştı → GAME OVER tetiklenmeli.");
                OnMoveDuringCheck?.Invoke();
            }

            // direction'ı sıfırla (tamamen dur)
            direction = Vector2.zero;

            // Idle sprite’da kalmasını sağla
            activeSpriteRenderer.idle = true;
            return;    // Aşağıdaki kontrolleri çalıştırma
        }

        // --- NORMAL HAREKET BÖLÜMÜ ---
        // YUKARI (W + UpArrow)
        if (Input.GetKey(inputUp) || Input.GetKey(KeyCode.UpArrow))
        {
            SetDirection(Vector2.up, spriteRendererUp);
        }
        // AŞAĞI (S + DownArrow)
        else if (Input.GetKey(inputDown) || Input.GetKey(KeyCode.DownArrow))
        {
            SetDirection(Vector2.down, spriteRendererDown);
        }
        // SOL (A + LeftArrow)
        else if (Input.GetKey(inputLeft) || Input.GetKey(KeyCode.LeftArrow))
        {
            SetDirection(Vector2.left, spriteRendererLeft);
        }
        // SAĞ (D + RightArrow)
        else if (Input.GetKey(inputRight) || Input.GetKey(KeyCode.RightArrow))
        {
            SetDirection(Vector2.right, spriteRendererRight);
        }
        else
        {
            SetDirection(Vector2.zero, activeSpriteRenderer);
        }
    }

    private void FixedUpdate()
    {
        // CHECK fazında fizik hareketini tamamen engelle
        if (lightGuard != null && lightGuard.IsDangerActive)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 position = rb.position;
        Vector2 translation = speed * Time.fixedDeltaTime * direction;
        rb.MovePosition(position + translation);
    }

    private void SetDirection(Vector2 newDirection, AnimatedSpriteRenderer spriteRenderer)
    {
        direction = newDirection;

        spriteRendererUp.enabled = spriteRenderer == spriteRendererUp;
        spriteRendererDown.enabled = spriteRenderer == spriteRendererDown;
        spriteRendererLeft.enabled = spriteRenderer == spriteRendererLeft;
        spriteRendererRight.enabled = spriteRenderer == spriteRendererRight;

        activeSpriteRenderer = spriteRenderer;
        activeSpriteRenderer.idle = direction == Vector2.zero;
    }
}
