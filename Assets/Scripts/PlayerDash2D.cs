using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerCollision2D))]
public class PlayerDash2D : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 22f;
    [SerializeField] private float dashDuration = 0.30f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private bool allowAirDash = true;

    [Header("Dash VFX (child on player)")]
    [SerializeField] private ParticleSystem dashParticle;
    [SerializeField] private Vector2 dashVfxLocalOffset = new Vector2(-0.5f, 0f);

    [Header("Ghost Trail")]
    [SerializeField] private GhostTrail ghostTrail;

    public bool IsDashing { get; private set; }

    private Rigidbody2D rb;
    private PlayerCollision2D coll;

    private Vector2 moveInput;
    private bool dashQueued;

    private bool hasAirDashed;
    private float cooldownTimer;
    private float defaultGravity;

    private Coroutine dashRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<PlayerCollision2D>();
        defaultGravity = rb.gravityScale;

        if (dashParticle != null)
            dashParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (coll.OnGround)
            hasAirDashed = false;

        if (dashQueued)
        {
            dashQueued = false;
            TryDash();
        }
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed) dashQueued = true;
    }

    private void TryDash()
    {
        if (IsDashing) return;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayDash();
        if (cooldownTimer > 0f) return;
        if (!coll.OnGround && allowAirDash && hasAirDashed) return;

        Vector2 dir = moveInput;

        if (dir.sqrMagnitude < 0.01f)
        {
            float facing = transform.localScale.x >= 0f ? 1f : -1f;
            dir = new Vector2(facing, 0f);
        }

        dir.Normalize();

        if (!coll.OnGround && allowAirDash)
            hasAirDashed = true;

        cooldownTimer = dashCooldown;

        if (dashRoutine != null)
            StopCoroutine(dashRoutine);

        dashRoutine = StartCoroutine(DashRoutine(dir));
    }

    private IEnumerator DashRoutine(Vector2 dir)
    {
        IsDashing = true;

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = dir * dashSpeed;

        PlayDashParticle(dir);

        if (ghostTrail != null)
            ghostTrail.ShowGhost();

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = defaultGravity;
        IsDashing = false;
        dashRoutine = null;

        if (dashParticle != null)
            dashParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void PlayDashParticle(Vector2 dir)
    {
        if (dashParticle == null) return;

        Vector2 off = dashVfxLocalOffset;
        if (dir.x < -0.01f) off.x = -Mathf.Abs(off.x);
        else if (dir.x > 0.01f) off.x = Mathf.Abs(off.x);

        dashParticle.transform.localPosition = off;

        dashParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        dashParticle.Play(true);
    }
}