using UnityEngine;

public class PlayerCollision2D : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Collision Checks")]
    [SerializeField] private float collisionRadius = 0.25f;
    [SerializeField] private Vector2 bottomOffset = new Vector2(0f, -0.5f);
    [SerializeField] private Vector2 rightOffset = new Vector2(0.5f, 0f);
    [SerializeField] private Vector2 leftOffset = new Vector2(-0.5f, 0f);

    public bool OnGround { get; private set; }
    public bool OnWall { get; private set; }
    public bool OnRightWall { get; private set; }
    public bool OnLeftWall { get; private set; }
    public int WallSide { get; private set; } = 1;

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        OnGround = Physics2D.OverlapCircle(pos + bottomOffset, collisionRadius, groundLayer);

        OnRightWall = Physics2D.OverlapCircle(pos + rightOffset, collisionRadius, groundLayer);
        OnLeftWall = Physics2D.OverlapCircle(pos + leftOffset, collisionRadius, groundLayer);

        OnWall = OnRightWall || OnLeftWall;

        WallSide = OnRightWall ? -1 : 1;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 pos = transform.position;

        Gizmos.DrawWireSphere(pos + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere(pos + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere(pos + leftOffset, collisionRadius);
    }
}