using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerCollision2D))]
public class PlayerAnimator2D : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private PlayerCollision2D coll;
    private PlayerDash2D dash;
    private PlayerMovement2D move;

    private static readonly int SpeedHash        = Animator.StringToHash("Speed");
    private static readonly int YVelHash         = Animator.StringToHash("YVelocity");
    private static readonly int OnGroundHash     = Animator.StringToHash("OnGround");
    private static readonly int DashingHash      = Animator.StringToHash("Dashing");
    private static readonly int WallSlidingHash  = Animator.StringToHash("WallSliding");
    private static readonly int WallGrabbingHash = Animator.StringToHash("WallGrabbing");
    private static readonly int IsDeadHash       = Animator.StringToHash("IsDead");

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);

        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<PlayerCollision2D>();
        dash = GetComponent<PlayerDash2D>();
        move = GetComponent<PlayerMovement2D>();

        if (animator == null)
            Debug.LogError("PlayerAnimator2D: No Animator found (assign it or make sure one exists on a child).", this);
    }

    private void Update()
    {
        if (animator == null) return;
        
        if (animator.GetBool(IsDeadHash))
        {
            animator.SetFloat(SpeedHash, 0f);
            animator.SetFloat(YVelHash, 0f);
            animator.SetBool(DashingHash, false);
            animator.SetBool(WallSlidingHash, false);
            animator.SetBool(WallGrabbingHash, false);
            return;
        }

        float vx = rb.linearVelocity.x;
        float vy = rb.linearVelocity.y;

        bool isDashing = dash != null && dash.IsDashing;
        bool onGround = coll != null && coll.OnGround;

        bool wallSliding = !isDashing && move != null && move.IsWallSliding;
        bool onWall = coll != null && coll.OnWall;
        bool wallGrabbing = !isDashing && !onGround && onWall && !wallSliding;

        animator.SetFloat(SpeedHash, Mathf.Abs(vx));
        animator.SetFloat(YVelHash, vy);
        animator.SetBool(OnGroundHash, onGround);
        animator.SetBool(DashingHash, isDashing);
        animator.SetBool(WallSlidingHash, wallSliding);
        animator.SetBool(WallGrabbingHash, wallGrabbing);

        
        if (Mathf.Abs(vx) > 0.01f)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Sign(vx);
            transform.localScale = s;
        }
    }
}