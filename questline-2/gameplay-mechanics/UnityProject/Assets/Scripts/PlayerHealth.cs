using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int CurrentHealth { get; private set; }
    bool invulnerable;
    SpriteRenderer sprite;
    PlayerController controller;

    void Start()
    {
        CurrentHealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        controller = GetComponent<PlayerController>();
        GameManager.Instance.SetHealth(CurrentHealth, maxHealth);
    }

    public void Damage(int amount, Vector2 source)
    {
        if (amount <= 0 || invulnerable || GameManager.Instance.IsGameOver) return;
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        GameManager.Instance.SetHealth(CurrentHealth, maxHealth);
        Vector2 direction = ((Vector2)transform.position - source).normalized;
        if (controller) controller.ApplyKnockback(new Vector2(direction.x * 7f, 6f));
        if (CurrentHealth == 0) Defeat();
        else StartCoroutine(Invulnerability());
    }

    void Defeat()
    {
        CurrentHealth = 0;
        GameManager.Instance.SetHealth(0, maxHealth);
        GameManager.Instance.GameOver();
        gameObject.SetActive(false);
    }

    IEnumerator Invulnerability()
    {
        invulnerable = true;
        for (int i = 0; i < 6; i++)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(.1f);
        }
        sprite.enabled = true;
        invulnerable = false;
    }

    void Update()
    {
        // Falling out ends the run even during the damage cooldown.
        if (transform.position.y < -8f && !GameManager.Instance.IsGameOver) Defeat();
    }
}
