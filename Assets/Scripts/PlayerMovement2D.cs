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

    private Rigidbody2D rb;
    private PlayerCollision2D coll;

    private Vector2 moveInput;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private float defaultGravityScale;
    private bool isWallSliding;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<PlayerCollision2D>();
        defaultGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        if (coll.OnGround)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        jumpBufferTimer -= Time.deltaTime;
        
        isWallSliding = ShouldWallSlide();
    }

    private void FixedUpdate()
    {
        if (!isWallSliding)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
            rb.gravityScale = defaultGravityScale;
        }
        else
        {
            rb.gravityScale = wallSlideGravityScale;

            float clampedY = rb.linearVelocity.y;
            if (clampedY < -wallSlideSpeed)
                clampedY = -wallSlideSpeed;

            rb.linearVelocity = new Vector2(0f, clampedY);
        }
        
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.linearVelocity += Vector2.up * jumpForce;
        }
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
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpBufferTimer = jumpBufferTime;
        }
    }
}