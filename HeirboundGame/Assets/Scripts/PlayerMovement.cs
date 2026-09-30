using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Eight-direction top-down movement, plus the dodge.
///
/// Input is read straight off the devices (Keyboard.current, Gamepad.current)
/// instead of an .inputactions asset. Fewer files, nothing to drag into the
/// Inspector, and one less thing that can be silently mis-wired while the
/// only question that matters is whether this feels good.
/// ponytail: swap to an .inputactions asset when rebinding is wanted.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Dodge")]
    public float dodgeSpeed = 16f;
    public float dodgeDuration = 0.18f;
    [Tooltip("Seconds of the dodge where damage passes through you.")]
    public float dodgeInvulnerability = 0.22f;
    public float dodgeCooldown = 0.45f;

    /// <summary>Last non-zero direction. The attack hitbox goes here.</summary>
    public Vector2 Facing { get; private set; } = Vector2.right;

    public bool Busy { get; set; }      // set by PlayerCombat during a swing
    public bool Dodging { get; private set; }

    Rigidbody2D body;
    Animator animator;
    Health health;
    Vector2 input;
    float dodgeEndsAt, dodgeReadyAt;
    Vector2 dodgeDirection;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();

        // Top-down has no gravity and things should not spin when bumped.
        body.gravityScale = 0f;
        body.freezeRotation = true;
    }

    void Update()
    {
        input = ReadMove();

        if (input.sqrMagnitude > 0.01f) Facing = input.normalized;

        if (DodgePressed() && Time.time >= dodgeReadyAt && !Dodging)
            StartDodge();

        UpdateAnimator();
        FaceRightWay();
    }

    void FixedUpdate()
    {
        // Physics goes in FixedUpdate so movement does not change speed with
        // frame rate. Update is for input, FixedUpdate is for moving.
        if (Dodging)
        {
            body.linearVelocity = dodgeDirection * dodgeSpeed;
            if (Time.time >= dodgeEndsAt) Dodging = false;
            return;
        }

        body.linearVelocity = Busy ? Vector2.zero : input.normalized * moveSpeed;
    }

    void StartDodge()
    {
        Dodging = true;
        Busy = false;                       // a dodge cancels a swing
        dodgeDirection = input.sqrMagnitude > 0.01f ? input.normalized : Facing;
        dodgeEndsAt = Time.time + dodgeDuration;
        dodgeReadyAt = Time.time + dodgeCooldown;

        if (animator) animator.SetTrigger("Dodge");
        if (health) Invoke(nameof(EndInvulnerability), dodgeInvulnerability);
        if (health) health.Invulnerable = true;
    }

    void EndInvulnerability()
    {
        if (health) health.Invulnerable = false;
    }

    void UpdateAnimator()
    {
        if (!animator) return;
        animator.SetFloat("Speed", input.magnitude);
        animator.SetFloat("MoveX", Facing.x);
        animator.SetFloat("MoveY", Facing.y);
    }

    void FaceRightWay()
    {
        // Art is only ever drawn facing right. Flipping the scale is why
        // three directions of art covers eight directions of movement.
        if (Mathf.Abs(Facing.x) < 0.01f) return;
        var s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (Facing.x < 0 ? -1f : 1f);
        transform.localScale = s;
    }

    static Vector2 ReadMove()
    {
        var pad = Gamepad.current;
        if (pad != null)
        {
            var stick = pad.leftStick.ReadValue();
            if (stick.sqrMagnitude > 0.04f) return stick;   // past the dead zone
        }

        var k = Keyboard.current;
        if (k == null) return Vector2.zero;

        var v = Vector2.zero;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed) v.x -= 1;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) v.x += 1;
        if (k.sKey.isPressed || k.downArrowKey.isPressed) v.y -= 1;
        if (k.wKey.isPressed || k.upArrowKey.isPressed) v.y += 1;
        return v;
    }

    static bool DodgePressed() =>
        (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
        (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame);
}
