using UnityEngine;

public class Sentinel : MonoBehaviour
{
    public float left, right;
    float direction = 1;
    int health = 2;
    Rigidbody2D body;
    bool defeated;
    void Awake() { body = GetComponent<Rigidbody2D>(); }
    void FixedUpdate()
    {
        if (GameSession.Instance.State != RunState.Playing || defeated) return;
        if (transform.position.x >= right) direction = -1;
        if (transform.position.x <= left) direction = 1;
        body.linearVelocity = new Vector2(direction * 2f, body.linearVelocity.y);
    }
    public void Hit()
    {
        if (defeated) return;
        health--;
        if (health > 0) return;
        defeated = true;
        GameSession.Instance.EnemyDefeated();
        LevelBuilder.Pulse(transform.position);
        Destroy(gameObject);
    }
}

public enum TriggerKind { Core, Hazard, Exit }
public class LevelTrigger : MonoBehaviour
{
    public TriggerKind kind;
    Vector3 origin;
    bool consumed;
    void Start() { origin = transform.position; }
    void Update()
    {
        if (kind == TriggerKind.Core && GameSession.Instance.State == RunState.Playing)
        {
            transform.position = origin + Vector3.up * Mathf.Sin(Time.time * 3f + origin.x) * .12f;
            transform.Rotate(0, 0, 65f * Time.deltaTime);
        }
        if (kind == TriggerKind.Exit)
            GetComponent<SpriteRenderer>().color = GameSession.Instance.Collected == GameSession.CrystalGoal ? LevelBuilder.Teal : new Color(.35f,.4f,.55f);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<Runner>() || GameSession.Instance.State != RunState.Playing) return;
        if (kind == TriggerKind.Core && !consumed)
        {
            consumed = true;
            GameSession.Instance.Collect();
            LevelBuilder.Pulse(transform.position);
            Destroy(gameObject);
        }
        else if (kind == TriggerKind.Exit) GameSession.Instance.EnterExit();
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (kind == TriggerKind.Hazard && other.GetComponent<Runner>()) GameSession.Instance.Hurt(transform.position);
    }
}
