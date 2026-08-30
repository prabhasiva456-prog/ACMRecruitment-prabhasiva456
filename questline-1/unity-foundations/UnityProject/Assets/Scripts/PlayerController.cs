using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 7f, jumpForce = 11f;
    Rigidbody2D body; int jumpsRemaining; float attackCooldown; Vector3 baseScale;
    void Awake() { body = GetComponent<Rigidbody2D>(); baseScale = transform.localScale; }
    void Update()
    {
        if (GameManager.Instance.IsGameOver) return;
        float x = Input.GetAxisRaw("Horizontal");
        body.linearVelocity = new Vector2(x * speed, body.linearVelocity.y);
        if (Mathf.Abs(x) > .01f) transform.localScale = new Vector3(Mathf.Sign(x) * Mathf.Abs(baseScale.x), baseScale.y, 1);
        transform.rotation = Quaternion.Euler(0, 0, -body.linearVelocity.x * 1.2f);
        bool grounded = Physics2D.Raycast(transform.position, Vector2.down, .72f, GameBootstrap.GroundMask);
        if (grounded && body.linearVelocity.y <= .1f) jumpsRemaining = 2;
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0) { body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce); jumpsRemaining--; }
        attackCooldown -= Time.deltaTime;
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)) && attackCooldown <= 0f) Attack();
    }
    void Attack()
    {
        attackCooldown = .38f;
        Vector2 center = (Vector2)transform.position + Vector2.right * Mathf.Sign(transform.localScale.x) * 1.15f;
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(center, 1.25f)) { EnemyAI enemy = hit.GetComponent<EnemyAI>(); if (enemy) enemy.TakeDamage(1, transform.position); }
        GameBootstrap.SpawnPulse(center);
    }
}
