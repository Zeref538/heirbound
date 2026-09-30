using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The swing. Three phases, because that is what makes a hit feel like a hit:
///
///   windup   - committed, cannot turn or move. This is what makes it weighty.
///   active   - the one moment the game checks what the sword touched.
///   recovery - the cost of missing.
///
/// Every number here is meant to be changed in the Inspector while playing.
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Timing (seconds)")]
    public float windup = 0.08f;
    public float active = 0.06f;
    public float recovery = 0.18f;

    [Header("Hitbox")]
    [Tooltip("How far in front of the player the swing reaches.")]
    public float reach = 0.8f;
    public float radius = 0.7f;
    public LayerMask hits;

    [Header("Damage")]
    public int damage = 3;

    [Header("Juice")]
    [Tooltip("Freeze frame on a connect. Tiny numbers. 0 turns it off.")]
    public float hitStop = 0.05f;

    [Header("Debug")]
    public bool drawHitbox = true;

    PlayerMovement movement;
    Animator animator;
    bool swinging;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!swinging && !movement.Dodging && AttackPressed())
            StartCoroutine(Swing());
    }

    IEnumerator Swing()
    {
        swinging = true;
        movement.Busy = true;
        if (animator) animator.SetTrigger("Attack");

        yield return new WaitForSeconds(windup);

        // Checked once, at the start of the active window. A single check is
        // enough at these speeds and it cannot double-hit the same enemy.
        var landed = Strike();

        if (landed && hitStop > 0f) yield return HitStop();

        yield return new WaitForSeconds(active);
        movement.Busy = false;              // free to move during recovery
        yield return new WaitForSeconds(recovery);

        swinging = false;
    }

    bool Strike()
    {
        var centre = (Vector2)transform.position + movement.Facing * reach;
        var found = Physics2D.OverlapCircleAll(centre, radius, hits);
        var landed = false;

        foreach (var c in found)
        {
            if (c.transform.root == transform.root) continue;   // never hit self
            var h = c.GetComponentInParent<Health>();
            if (h == null || h.IsDead) continue;
            h.TakeDamage(damage, transform.position);
            landed = true;
        }
        return landed;
    }

    IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        // Unscaled, or this waits forever - scaled time is not moving.
        yield return new WaitForSecondsRealtime(hitStop);
        Time.timeScale = 1f;
    }

    static bool AttackPressed() =>
        (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
        (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame) ||
        (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame);

    void OnDrawGizmosSelected()
    {
        if (!drawHitbox) return;
        var facing = Application.isPlaying && movement != null
            ? movement.Facing
            : Vector2.right;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + facing * reach, radius);
    }
}
