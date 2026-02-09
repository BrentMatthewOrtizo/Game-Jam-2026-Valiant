using UnityEngine;

public class PlayerCollision2D : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private LayerMask solidGroundLayer;
    [SerializeField] private LayerMask oneWayPlatformLayer;

    [Header("Collision Checks")]
    [SerializeField] private float collisionRadius = 0.18f;
    [SerializeField] private Vector2 bottomOffset = new Vector2(0f, -0.55f);
    [SerializeField] private float footSeparation = 0.22f;

    [SerializeField] private Vector2 rightOffset = new Vector2(0.52f, 0f);
    [SerializeField] private Vector2 leftOffset  = new Vector2(-0.52f, 0f);

    public bool OnGround { get; private set; }
    public bool OnSolidGround { get; private set; }
    public bool OnOneWay { get; private set; }

    public bool OnWall { get; private set; }
    public bool OnRightWall { get; private set; }
    public bool OnLeftWall { get; private set; }
    public int WallSide { get; private set; }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        Vector2 footL = pos + bottomOffset + new Vector2(-footSeparation, 0f);
        Vector2 footR = pos + bottomOffset + new Vector2( footSeparation, 0f);

        bool solidL = Physics2D.OverlapCircle(footL, collisionRadius, solidGroundLayer);
        bool solidR = Physics2D.OverlapCircle(footR, collisionRadius, solidGroundLayer);

        bool oneWayL = Physics2D.OverlapCircle(footL, collisionRadius, oneWayPlatformLayer);
        bool oneWayR = Physics2D.OverlapCircle(footR, collisionRadius, oneWayPlatformLayer);

        OnSolidGround = solidL || solidR;
        OnOneWay = oneWayL || oneWayR;
        OnGround = OnSolidGround || OnOneWay;

        OnRightWall = !OnGround && Physics2D.OverlapCircle(pos + rightOffset, collisionRadius, solidGroundLayer);
        OnLeftWall  = !OnGround && Physics2D.OverlapCircle(pos + leftOffset,  collisionRadius, solidGroundLayer);

        OnWall = OnRightWall || OnLeftWall;

        if (OnRightWall) WallSide = -1;
        else if (OnLeftWall) WallSide = 1;
        else WallSide = 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector2 pos = transform.position;
        Vector2 footL = pos + bottomOffset + new Vector2(-footSeparation, 0f);
        Vector2 footR = pos + bottomOffset + new Vector2( footSeparation, 0f);

        Gizmos.DrawWireSphere(footL, collisionRadius);
        Gizmos.DrawWireSphere(footR, collisionRadius);

        Gizmos.DrawWireSphere(pos + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere(pos + leftOffset, collisionRadius);
    }
}