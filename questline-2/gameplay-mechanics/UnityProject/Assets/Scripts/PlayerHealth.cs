using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int CurrentHealth { get; private set; }
    bool invulnerable;
    SpriteRenderer sprite;
    Rigidbody2D body;

    void Start()
    {
        CurrentHealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>(); body = GetComponent<Rigidbody2D>();
        GameManager.Instance.SetHealth(CurrentHealth, maxHealth);
    }

    public void Damage(int amount, Vector2 source)
    {
        if (invulnerable || GameManager.Instance.IsGameOver) return;
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        GameManager.Instance.SetHealth(CurrentHealth, maxHealth);
        Vector2 direction = ((Vector2)transform.position - source).normalized;
        body.linearVelocity = new Vector2(direction.x * 7f, 6f);
        if (CurrentHealth == 0) { GameManager.Instance.GameOver(); gameObject.SetActive(false); }
        else StartCoroutine(Invulnerability());
    }

    IEnumerator Invulnerability()
    {
        invulnerable = true;
        for (int i = 0; i < 6; i++) { sprite.enabled = !sprite.enabled; yield return new WaitForSeconds(.1f); }
        sprite.enabled = true; invulnerable = false;
    }

    void Update() { if (transform.position.y < -8f) Damage(maxHealth, transform.position + Vector3.up); }
}
