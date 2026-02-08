using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private float greedAmountOverride = -1f; 

    private static CoinUI cachedCoinUI;
    private static GreedMeter cachedGreed;

    private void Awake()
    {
        if (cachedCoinUI == null) cachedCoinUI = FindObjectOfType<CoinUI>();
        if (cachedGreed == null) cachedGreed = FindObjectOfType<GreedMeter>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (cachedCoinUI != null)
            cachedCoinUI.AddCoin(value);

        if (cachedGreed != null)
        {
            if (greedAmountOverride > 0f) cachedGreed.AddGreed(greedAmountOverride);
            else cachedGreed.AddCoinGreed();
        }

        Destroy(gameObject);
    }
}