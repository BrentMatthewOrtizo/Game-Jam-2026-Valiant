using UnityEngine;

public class PlayerAnimator2D : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private PlayerCollision2D coll;
    private PlayerDash2D dash;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
        coll = GetComponentInParent<PlayerCollision2D>();
        dash = GetComponentInParent<PlayerDash2D>();
    }

    private void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("OnGround", coll.OnGround);
        animator.SetBool("Dashing", dash != null && dash.IsDashing);

        if (Mathf.Abs(rb.linearVelocity.x) > 0.01f)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Sign(rb.linearVelocity.x);
            transform.localScale = s;
        }
    }
}