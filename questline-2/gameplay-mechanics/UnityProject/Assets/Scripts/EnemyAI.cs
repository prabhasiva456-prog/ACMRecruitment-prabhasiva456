using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.8f;
    public float detectionRange = 7f;
    public int health = 2;

    Rigidbody2D body;
    Transform player;
    float direction = 1f;
    float jumpTimer;
    float knockbackUntil;
    bool defeated;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void FixedUpdate()
    {
        if (!player || GameManager.Instance.IsGameOver || defeated)
        {
            body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
            return;
        }
        if (Time.time < knockbackUntil) return;

        bool chasing = Vector2.Distance(transform.position, player.position) < detectionRange;
        float moveDirection = chasing ? Mathf.Sign(player.position.x - transform.position.x) : direction;
        float speed = (chasing ? chaseSpeed : patrolSpeed) * GameManager.Instance.DifficultyMultiplier;
        body.linearVelocity = new Vector2(moveDirection * speed, body.linearVelocity.y);
        transform.localScale = new Vector3(-moveDirection * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);

        bool floorAhead = Physics2D.Raycast((Vector2)transform.position + Vector2.right * moveDirection * .55f,
            Vector2.down, 1f, GameBootstrap.GroundMask);
        bool wallAhead = Physics2D.Raycast(transform.position, Vector2.right * moveDirection, .75f, GameBootstrap.GroundMask);
        if (!floorAhead && !chasing) direction *= -1f;

        jumpTimer -= Time.fixedDeltaTime;
        if (wallAhead && jumpTimer <= 0f)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, 8f);
            jumpTimer = .8f;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (defeated) return;
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth) playerHealth.Damage(1, transform.position);
    }

    public void TakeDamage(int amount, Vector2 source)
    {
        if (defeated || amount <= 0 || GameManager.Instance.IsGameOver) return;
        health -= amount;
        body.linearVelocity = new Vector2(Mathf.Sign(transform.position.x - source.x) * 7f, 5f);
        knockbackUntil = Time.time + .15f;
        if (health <= 0)
        {
            // Destroy is deferred until the end of the frame; guard the score award.
            defeated = true;
            GameManager.Instance.AddScore(25);
            Destroy(gameObject);
        }
    }
}
