using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public float speed = 7f;
    public float jumpForce = 11f;

    Rigidbody2D body;
    BoxCollider2D hitbox;
    float horizontalInput;
    bool jumpRequested;
    int jumpsRemaining;
    float nextAttackTime;
    float controlLockedUntil;
    Vector3 baseScale;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<BoxCollider2D>();
        baseScale = transform.localScale;
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver) return;
        horizontalInput = Input.GetAxisRaw("Horizontal");
        jumpRequested |= Input.GetKeyDown(KeyCode.Space);
        if (Mathf.Abs(horizontalInput) > .01f)
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput) * Mathf.Abs(baseScale.x), baseScale.y, 1f);
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)) && Time.time >= nextAttackTime)
            Attack();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOver) return;
        // A short box cast detects support at both feet. Only solid arena objects use GroundMask.
        Bounds bounds = hitbox.bounds;
        bool grounded = Physics2D.BoxCast(bounds.center,
            new Vector2(bounds.size.x * .85f, bounds.size.y * .95f),
            0f, Vector2.down, .08f, GameBootstrap.GroundMask);
        if (grounded && body.linearVelocity.y <= .1f) jumpsRemaining = 2;

        if (Time.time >= controlLockedUntil)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);
            if (jumpRequested && jumpsRemaining > 0)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
                jumpsRemaining--;
            }
        }
        jumpRequested = false;
    }

    public void ApplyKnockback(Vector2 velocity)
    {
        // Keep movement from immediately cancelling collision feedback.
        controlLockedUntil = Time.time + .18f;
        body.linearVelocity = velocity;
    }

    void Attack()
    {
        nextAttackTime = Time.time + .38f;
        Vector2 center = (Vector2)transform.position + Vector2.right * Mathf.Sign(transform.localScale.x) * 1.15f;
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(center, 1.25f, 1 << 7))
        {
            EnemyAI enemy = hit.GetComponent<EnemyAI>();
            if (enemy) enemy.TakeDamage(1, transform.position);
        }
        GameBootstrap.SpawnPulse(center);
    }
}
