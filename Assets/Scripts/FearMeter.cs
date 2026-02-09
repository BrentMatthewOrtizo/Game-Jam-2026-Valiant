using UnityEngine;
using UnityEngine.UI;

public class FearMeter : MonoBehaviour
{
    [Header("Fear Values")]
    [SerializeField] private float maxFear = 100f;
    [SerializeField] private float currentFear = 0f;

    [Header("Gain Rates")]
    [Tooltip("How fast fear rises per second normally.")]
    [SerializeField] private float fearGainPerSecond = 6f;

    [Tooltip("Optional: fear rises slower/faster while greed is activated.")]
    [SerializeField] private float fearGainPerSecondWhileGreed = 0f; 

    [Header("Coin Fear Reduction")]
    [SerializeField] private float fearReducedPerCoin = 10f;

    [Header("UI")]
    [Tooltip("11 sprites from empty -> full (0..10)")]
    [SerializeField] private Sprite[] fillStages;
    [SerializeField] private Image fillImage;

    [Header("Optional Links")]
    [SerializeField] private GreedMeter greedMeter; 
    
    public float Fear01 => (maxFear <= 0f) ? 0f : Mathf.Clamp01(currentFear / maxFear);
    public float CurrentFear => currentFear;

    private void Awake()
    {
        if (greedMeter == null) greedMeter = FindObjectOfType<GreedMeter>();
        UpdateUI();
    }

    private void Update()
    {
        float rate = fearGainPerSecond;
        
        if (greedMeter != null && greedMeter.IsActivated)
            rate = fearGainPerSecondWhileGreed;

        currentFear += rate * Time.deltaTime;
        currentFear = Mathf.Clamp(currentFear, 0f, maxFear);

        UpdateUI();
    }

    public void ReduceFearFromCoin()
    {
        ReduceFear(fearReducedPerCoin);
    }

    public void ReduceFear(float amount)
    {
        currentFear = Mathf.Clamp(currentFear - amount, 0f, maxFear);
        UpdateUI();
    }

    public void AddFear(float amount)
    {
        currentFear = Mathf.Clamp(currentFear + amount, 0f, maxFear);
        UpdateUI();
    }

    public void ResetToZero()
    {
        currentFear = 0f;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (fillImage == null) return;
        if (fillStages == null || fillStages.Length < 11) return;

        float t = Fear01;
        int index = Mathf.Clamp(Mathf.RoundToInt(t * 10f), 0, 10);

        fillImage.sprite = fillStages[index];
    }
}