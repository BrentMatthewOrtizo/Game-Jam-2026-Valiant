using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    [SerializeField] private int value = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CoinUI ui = FindObjectOfType<CoinUI>();
        if (ui != null)
            ui.AddCoin(value);

        Destroy(gameObject);
    }
}