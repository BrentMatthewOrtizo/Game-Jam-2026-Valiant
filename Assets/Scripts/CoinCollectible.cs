using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private float greedAmountOverride = -1f; 

    private static CoinUI cachedCoinUI;
    private static GreedMeter cachedGreed;
    private static FearMeter cachedFear;

    private void Awake()
    {
        if (cachedCoinUI == null) cachedCoinUI = FindObjectOfType<CoinUI>();
        if (cachedGreed == null) cachedGreed = FindObjectOfType<GreedMeter>();
        if (cachedFear == null) cachedFear = FindObjectOfType<FearMeter>();
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
        
        if (cachedFear != null)
        {
            cachedFear.ReduceFearFromCoin();
        }
        
        Destroy(gameObject);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayPickupCoin();
    }
}