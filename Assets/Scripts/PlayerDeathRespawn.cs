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

    [Header("Animator Params")]
    [SerializeField] private string deathTriggerName = "Die";
    [SerializeField] private string isDeadBoolName = "IsDead";

    private Rigidbody2D rb;
    private bool isDying;

    private Vector2 checkpointPos;
    private bool hasCheckpoint;

    private int deathTriggerHash;
    private int isDeadHash;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (movement == null) movement = GetComponent<PlayerMovement2D>();
        if (dash == null) dash = GetComponent<PlayerDash2D>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        deathTriggerHash = Animator.StringToHash(deathTriggerName);
        isDeadHash = Animator.StringToHash(isDeadBoolName);

        checkpointPos = transform.position;
        hasCheckpoint = true;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsHazard(collision.gameObject)) DieAndRespawn();
    }

    private bool IsHazard(GameObject go)
    {
        int mask = 1 << go.layer;
        return (hazardLayer.value & mask) != 0;
    }

    public void DieAndRespawn()
    {
        if (isDying) return;
        isDying = true;
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if (movement != null) movement.enabled = false;
        if (dash != null) dash.enabled = false;

        if (greedMeter != null) greedMeter.ResetToZero();

        if (animator != null)
        {
            animator.SetBool(isDeadHash, true);
            animator.SetTrigger(deathTriggerHash);
        }

        rb.simulated = false;

        yield return new WaitForSeconds(cameraHoldTime);
        yield return new WaitForSeconds(respawnDelayAfterHold);

        if (hasCheckpoint)
            transform.position = checkpointPos + respawnOffset;

        rb.simulated = true;

        if (animator != null)
            animator.SetBool(isDeadHash, false);

        if (movement != null) movement.enabled = true;
        if (dash != null) dash.enabled = true;

        isDying = false;
    }
}