using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviour
{
    [Header("Win Condition")]
    [SerializeField] private string endSceneName = "EndScreen";

    private int totalCoins;
    private int collectedCoins;

    private void Awake()
    {
        totalCoins = FindObjectsOfType<CoinCollectible>(includeInactive: false).Length;
        collectedCoins = 0;

        
        if (totalCoins <= 0)
        {
            LoadEndScene();
        }
    }

    public void OnCoinCollected(int amount = 1)
    {
        collectedCoins += amount;

        if (collectedCoins >= totalCoins)
        {
            LoadEndScene();
        }
    }

    private void LoadEndScene()
    {
        SceneManager.LoadScene(endSceneName);
    }
}