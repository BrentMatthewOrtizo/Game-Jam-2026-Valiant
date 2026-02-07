using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerCollision2D))]
public class PlayerAnimator2D : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private PlayerCollision2D coll;
    private PlayerDash2D dash;
    private PlayerMovement2D move;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<PlayerCollision2D>();
        dash = GetComponent<PlayerDash2D>();
        move = GetComponent<PlayerMovement2D>();
    }

    private void Update()
    {
        float vx = rb.linearVelocity.x;
        float vy = rb.linearVelocity.y;

        bool isDashing = dash != null && dash.IsDashing;
        bool onGround = coll.OnGround;

        bool wallSliding = !isDashing && move != null && move.IsWallSliding;
        bool wallGrabbing = false;

        animator.SetFloat("Speed", Mathf.Abs(vx));
        animator.SetFloat("YVelocity", vy);
        animator.SetBool("OnGround", onGround);
        animator.SetBool("Dashing", isDashing);
        animator.SetBool("WallSliding", wallSliding);
        animator.SetBool("WallGrabbing", wallGrabbing);

        if (Mathf.Abs(vx) > 0.01f)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Sign(vx);
            transform.localScale = s;
        }
    }
}