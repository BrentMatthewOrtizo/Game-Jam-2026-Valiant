using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDeathRespawn : MonoBehaviour
{
    [Header("Hazard Detection")]
    [SerializeField] private LayerMask hazardLayer;

    [Header("Timing")]
    [SerializeField] private float cameraHoldTime = 2f;
    [SerializeField] private float respawnDelayAfterHold = 0.1f;

    [Header("Respawn")]
    [SerializeField] private Vector2 respawnOffset = new Vector2(0f, 0.1f);

    [Header("References (optional)")]
    [SerializeField] private GreedMeter greedMeter;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement2D movement;
    [SerializeField] private PlayerDash2D dash;

    [Header("Animator")]
    [SerializeField] private string isDeadBoolName = "IsDead";
    [SerializeField] private string deathStateName = "V_Death";

    private Rigidbody2D rb;
    private bool isDying;

    private Vector2 checkpointPos;
    private bool hasCheckpoint;

    private int isDeadHash;

    private float savedGravity;
    private RigidbodyConstraints2D savedConstraints;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (movement == null) movement = GetComponent<PlayerMovement2D>();
        if (dash == null) dash = GetComponent<PlayerDash2D>();
        if (animator == null) animator = GetComponentInChildren<Animator>(true);

        isDeadHash = Animator.StringToHash(isDeadBoolName);

        checkpointPos = transform.position;
        hasCheckpoint = true;

        savedGravity = rb.gravityScale;
        savedConstraints = rb.constraints;
    }

    public void SetCheckpoint(Vector2 worldPos)
    {
        checkpointPos = worldPos;
        hasCheckpoint = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsHazard(other.gameObject)) DieAndRespawn();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (IsHazard(other.gameObject)) DieAndRespawn();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsHazard(collision.gameObject)) DieAndRespawn();
    }

    private bool IsHazard(GameObject go)
    {
        return (hazardLayer.value & (1 << go.layer)) != 0;
    }

    public void DieAndRespawn()
    {
        if (isDying) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayHitHurt();

        isDying = true;
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        if (movement != null) movement.enabled = false;
        if (dash != null) dash.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (greedMeter != null) greedMeter.ResetToZero();

        if (animator != null)
        {
            animator.enabled = true;
            animator.SetBool(isDeadHash, true);
            animator.Play(deathStateName, 0, 0f);
            animator.Update(0f);
        }

        yield return new WaitForSeconds(cameraHoldTime);
        yield return new WaitForSeconds(respawnDelayAfterHold);

        if (hasCheckpoint)
            transform.position = checkpointPos + respawnOffset;

        rb.constraints = savedConstraints;
        rb.gravityScale = savedGravity;

        if (animator != null)
            animator.SetBool(isDeadHash, false);

        if (movement != null) movement.enabled = true;
        if (dash != null) dash.enabled = true;

        isDying = false;
    }
}