using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite activeSprite;

    [Header("Optional")]
    [SerializeField] private AudioSource sfx;

    private SpriteRenderer sr;
    private bool activated;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null && inactiveSprite != null)
            sr.sprite = inactiveSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        PlayerDeathRespawn respawn = other.GetComponent<PlayerDeathRespawn>();
        if (respawn != null)
            respawn.SetCheckpoint(transform.position);

        activated = true;
        if (sr != null && activeSprite != null) sr.sprite = activeSprite;
    }
}