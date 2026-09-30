using UnityEngine;

/// <summary>
/// Walks at the player and damages by touching. No attack animation exists
/// because there is no attack - contact is the whole threat in stage 1.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
public class EnemyChase : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2.2f;
    [Tooltip("Closer than this and it stops shuffling into you.")]
    public float stopDistance = 0.6f;
    [Tooltip("Stands still until the player is this close.")]
    public float aggroRange = 8f;

    [Header("Contact damage")]
    public int touchDamage = 1;
    public float damageInterval = 0.8f;

    Rigidbody2D body;
    Health health;
    Animator animator;
    Transform player;
    float nextDamageAt;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        animator = GetComponentInChildren<Animator>();
        body.gravityScale = 0f;
        body.freezeRotation = true;

        var p = FindFirstObjectByType<PlayerMovement>();
        if (p) player = p.transform;
    }

    void FixedUpdate()
    {
        if (health.IsDead || player == null)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        var toPlayer = (Vector2)(player.position - transform.position);
        var distance = toPlayer.magnitude;
        var chasing = distance < aggroRange && distance > stopDistance;

        body.linearVelocity = chasing ? toPlayer.normalized * moveSpeed : Vector2.zero;

        if (animator) animator.SetFloat("Speed", chasing ? 1f : 0f);
        if (Mathf.Abs(toPlayer.x) > 0.01f)
        {
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (toPlayer.x < 0 ? -1f : 1f);
            transform.localScale = s;
        }
    }

    void OnCollisionStay2D(Collision2D other) => TryTouch(other.collider);
    void OnTriggerStay2D(Collider2D other) => TryTouch(other);

    void TryTouch(Collider2D other)
    {
        if (health.IsDead || Time.time < nextDamageAt) return;

        var target = other.GetComponentInParent<Health>();
        if (target == null || target == health || target.IsDead) return;
        if (other.GetComponentInParent<PlayerMovement>() == null) return;

        target.TakeDamage(touchDamage, transform.position);
        nextDamageAt = Time.time + damageInterval;
    }
}
