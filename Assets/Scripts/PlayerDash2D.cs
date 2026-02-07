using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerCollision2D))]
public class PlayerDash2D : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 22f;
    [SerializeField] private float dashTime = 0.12f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashEndDamping = 8f;
    [SerializeField] private bool allowAirDash = true;

    [Header("Safety")]
    [SerializeField] private float dashRefreshLockTime = 0.08f;
    [SerializeField] private float groundedRefreshMinTime = 0.04f;

    [Header("Dash Wall Unstick")]
    [SerializeField] private float wallUnstickDownSpeed = 2.5f;
    [SerializeField] private float wallUnstickTime = 0.06f;

    public bool IsDashing { get; private set; }

    private Rigidbody2D rb;
    private PlayerCollision2D coll;

    private Vector2 moveInput;
    private bool dashQueued;

    private bool hasAirDashed;
    private float dashCooldownTimer;
    private float dashRefreshLockTimer;
    private float groundedTimer;

    private Coroutine dashRoutine;
    private Coroutine wallUnstickRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<PlayerCollision2D>();
    }

    private void Update()
    {
        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;
        if (dashRefreshLockTimer > 0f) dashRefreshLockTimer -= Time.deltaTime;

        if (coll.OnGround) groundedTimer += Time.deltaTime;
        else groundedTimer = 0f;

        if (!IsDashing &&
            dashRefreshLockTimer <= 0f &&
            groundedTimer >= groundedRefreshMinTime &&
            rb.linearVelocity.y <= 0.01f)
        {
            hasAirDashed = false;
        }

        if (dashQueued)
        {
            dashQueued = false;
            TryDash();
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed) dashQueued = true;
    }

    private void TryDash()
    {
        if (IsDashing) return;
        if (dashCooldownTimer > 0f) return;
        if (!coll.OnGround && allowAirDash && hasAirDashed) return;

        Vector2 dir = moveInput;

        if (dir.sqrMagnitude < 0.01f)
        {
            float facing = transform.localScale.x >= 0 ? 1f : -1f;
            dir = new Vector2(facing, 0f);
        }

        dir.Normalize();

        if (!coll.OnGround && allowAirDash)
            hasAirDashed = true;

        if (dashRoutine != null) StopCoroutine(dashRoutine);
        if (wallUnstickRoutine != null) StopCoroutine(wallUnstickRoutine);

        dashRoutine = StartCoroutine(DashRoutine(dir));
    }

    private IEnumerator DashRoutine(Vector2 dir)
    {
        IsDashing = true;
        dashCooldownTimer = dashCooldown;
        dashRefreshLockTimer = dashRefreshLockTime;

        float prevGravity = rb.gravityScale;
        float prevDamping = rb.linearDamping;

        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = dir * dashSpeed;

        float t = 0f;
        while (t < dashTime)
        {
            if (coll.OnWall && !coll.OnGround)
                break;

            t += Time.deltaTime;
            yield return null;
        }
        
        rb.linearDamping = dashEndDamping;
        rb.gravityScale = prevGravity;

        
        if (coll.OnWall && !coll.OnGround)
        {
            float y = rb.linearVelocity.y;
            if (y > 0f) y = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, y - 2f);
        }

        yield return new WaitForSeconds(0.03f);
        rb.linearDamping = prevDamping;

        IsDashing = false;
    }

    private IEnumerator WallUnstickRoutine()
    {
        float t = wallUnstickTime;

        while (t > 0f && coll.OnWall && !coll.OnGround)
        {
            if (rb.linearVelocity.y > -wallUnstickDownSpeed)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallUnstickDownSpeed);

            t -= Time.deltaTime;
            yield return null;
        }
    }
}