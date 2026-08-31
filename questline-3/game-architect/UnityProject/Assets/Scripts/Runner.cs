using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class Runner : MonoBehaviour
{
    Rigidbody2D body;
    CapsuleCollider2D capsule;
    SpriteRenderer sprite;
    float move;
    bool jumpQueued;
    int jumpsLeft = 2;
    float attackAt;
    float knockbackUntil;
    float flashUntil;
    float facing = 1;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        sprite.color = Time.time < flashUntil && Mathf.FloorToInt(Time.time * 14) % 2 == 0 ? Color.white : LevelBuilder.Teal;
        if (GameSession.Instance.State != RunState.Playing) { jumpQueued = false; return; }
        move = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(move) > .01f) facing = Mathf.Sign(move);
        transform.localScale = new Vector3(facing * .75f, 1.2f, 1f);
        jumpQueued |= Input.GetKeyDown(KeyCode.Space);
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)) && Time.time >= attackAt)
        {
            attackAt = Time.time + .3f;
            Vector2 center = (Vector2)transform.position + Vector2.right * facing * .9f;
            foreach (Collider2D hit in Physics2D.OverlapCircleAll(center, 1.2f, 1 << 7))
            {
                Sentinel enemy = hit.GetComponent<Sentinel>();
                if (enemy) enemy.Hit();
            }
            LevelBuilder.Pulse(center);
            GameSession.Instance.Sound.Play(Cue.Attack);
        }
    }

    void FixedUpdate()
    {
        if (GameSession.Instance.State != RunState.Playing) return;
        Bounds b = capsule.bounds;
        bool grounded = Physics2D.BoxCast(b.center, new Vector2(b.size.x * .8f, b.size.y * .92f), 0f, Vector2.down, .09f, 1 << 8);
        if (grounded && body.linearVelocity.y <= .1f) jumpsLeft = 2;
        if (Time.time >= knockbackUntil)
        {
            body.linearVelocity = new Vector2(move * 6.5f, body.linearVelocity.y);
            if (jumpQueued && jumpsLeft > 0)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, 10.8f);
                jumpsLeft--;
                GameSession.Instance.Sound.Play(Cue.Jump);
            }
        }
        jumpQueued = false;
    }

    public void Knockback(Vector2 source)
    {
        float away = transform.position.x >= source.x ? 1f : -1f;
        body.linearVelocity = new Vector2(away * 7f, 7f);
        knockbackUntil = Time.time + .2f;
        flashUntil = Time.time + .9f;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Sentinel>()) GameSession.Instance.Hurt(collision.transform.position);
    }
}
