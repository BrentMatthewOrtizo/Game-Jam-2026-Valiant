using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerCollision2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 14f;

    [Header("Jump Forgiveness")]
    [SerializeField] private float coyoteTime = 0.10f;
    [SerializeField] private float jumpBufferTime = 0.10f;

    [Header("Wall Slide")]
    [SerializeField] private float wallSlideSpeed = 3.5f;
    [SerializeField] private float wallStickInput = 0.1f;
    [SerializeField] private float wallSlideGravityScale = 0.6f;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 wallJumpForce = new Vector2(12f, 14f);
    [SerializeField] private float wallJumpLockTime = 0.12f;
    [SerializeField] private float wallJumpLerp = 10f;

    public bool IsWallSliding => isWallSliding;
    public float WallSlideSpeed => wallSlideSpeed;
    public Vector2 MoveInput => moveInput;

    private Rigidbody2D rb;
    private PlayerCollision2D coll;
    private PlayerDash2D dash;

    private Vector2 moveInput;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private float defaultGravityScale;

    private bool isWallSliding;
    private float wallJumpLockTimer;
    private float wallControlLockTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<PlayerCollision2D>();
        dash = GetComponent<PlayerDash2D>();
        defaultGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        if (coll.OnGround) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;

        if (jumpBufferTimer > 0f) jumpBufferTimer -= Time.deltaTime;
        if (wallJumpLockTimer > 0f) wallJumpLockTimer -= Time.deltaTime;
        if (wallControlLockTimer > 0f) wallControlLockTimer -= Time.deltaTime;

        if (dash != null && dash.IsDashing)
        {
            isWallSliding = false;
            return;
        }

        isWallSliding = (wallJumpLockTimer <= 0f) && ShouldWallSlide();
    }

    private void FixedUpdate()
    {
        if (dash != null && dash.IsDashing) return;

        bool wantsJump = jumpBufferTimer > 0f;

        if (wantsJump && !coll.OnGround && coll.OnWall)
        {
            jumpBufferTimer = 0f;
            DoWallJump();
        }
        else if (wantsJump && coyoteTimer > 0f)
        {
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            DoGroundJump();
        }

        if (isWallSliding)
        {
            rb.gravityScale = wallSlideGravityScale;

            float y = rb.linearVelocity.y;
            if (y > 0f) y = 0f;
            y = Mathf.Max(y, -wallSlideSpeed);

            rb.linearVelocity = new Vector2(0f, y);
            return;
        }

        rb.gravityScale = defaultGravityScale;

        float targetX = moveInput.x * moveSpeed;

        if (wallControlLockTimer > 0f)
        {
            float newX = Mathf.Lerp(rb.linearVelocity.x, targetX, wallJumpLerp * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);
        }
    }

    private void DoGroundJump()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayJump();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.linearVelocity += Vector2.up * jumpForce;
    }

    private void DoWallJump()
    {
        float awayX;
        if (coll.OnRightWall) awayX = -1f;
        else if (coll.OnLeftWall) awayX = 1f;
        else awayX = (moveInput.x == 0f) ? 1f : -Mathf.Sign(moveInput.x);

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = new Vector2(awayX * wallJumpForce.x, wallJumpForce.y);

        wallJumpLockTimer = wallJumpLockTime;
        wallControlLockTimer = wallJumpLockTime;
        coyoteTimer = 0f;
    }

    private bool ShouldWallSlide()
    {
        if (coll.OnGround) return false;
        if (!coll.OnWall) return false;
        if (rb.linearVelocity.y > 0.1f) return false;

        if (coll.OnRightWall && moveInput.x > wallStickInput) return true;
        if (coll.OnLeftWall && moveInput.x < -wallStickInput) return true;

        return false;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (dash != null) dash.SetMoveInput(moveInput);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed) jumpBufferTimer = jumpBufferTime;
    }

    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}