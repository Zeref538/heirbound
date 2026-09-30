using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Hit points, damage, flinch and death. One component for BOTH the player
/// and enemies on purpose - damage has a single code path, so a bug fixed
/// here is fixed everywhere.
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Tuning")]
    public int maxHealth = 10;
    [Tooltip("Seconds where further hits are ignored after taking one.")]
    public float invulnerableAfterHit = 0.3f;
    [Tooltip("How hard a hit shoves this thing away from the attacker.")]
    public float knockbackForce = 8f;
    public float knockbackDuration = 0.12f;

    [Header("Feedback (works with no art at all)")]
    public Color flashColour = Color.white;
    public float flashDuration = 0.08f;

    public int Current { get; private set; }
    public bool IsDead => Current <= 0;

    /// <summary>True while i-frames are active - a dodge sets this too.</summary>
    public bool Invulnerable { get; set; }

    public event Action Died;
    public event Action Damaged;

    Rigidbody2D body;
    SpriteRenderer sprite;
    Animator animator;
    Color baseColour;

    void Awake()
    {
        Current = maxHealth;
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        if (sprite) baseColour = sprite.color;
    }

    /// <param name="from">Where the hit came from, for knockback direction.</param>
    public void TakeDamage(int amount, Vector2 from)
    {
        if (IsDead || Invulnerable) return;

        Current -= amount;
        Damaged?.Invoke();

        if (IsDead)
        {
            StartCoroutine(Die());
            return;
        }

        if (animator) animator.SetTrigger("Hurt");
        StartCoroutine(Flash());
        StartCoroutine(Knockback(((Vector2)transform.position - from).normalized));
        StartCoroutine(Ignore(invulnerableAfterHit));
    }

    IEnumerator Flash()
    {
        if (!sprite) yield break;
        sprite.color = flashColour;
        yield return new WaitForSeconds(flashDuration);
        if (sprite) sprite.color = baseColour;
    }

    IEnumerator Knockback(Vector2 direction)
    {
        if (!body) yield break;
        body.linearVelocity = direction * knockbackForce;
        yield return new WaitForSeconds(knockbackDuration);
        if (body) body.linearVelocity = Vector2.zero;
    }

    IEnumerator Ignore(float seconds)
    {
        Invulnerable = true;
        yield return new WaitForSeconds(seconds);
        Invulnerable = false;
    }

    IEnumerator Die()
    {
        if (animator) animator.SetTrigger("Death");

        // Stop it moving and stop it blocking the player, but leave the body
        // on the floor - the room should remember what happened in it.
        if (body) body.linearVelocity = Vector2.zero;
        foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = false;
        foreach (var b in GetComponentsInChildren<MonoBehaviour>())
            if (b != this) b.enabled = false;

        Died?.Invoke();

        // ponytail: fixed wait. Read the real death clip length once there is
        // one, if the timing ever looks off.
        yield return new WaitForSeconds(1f);
    }
}
