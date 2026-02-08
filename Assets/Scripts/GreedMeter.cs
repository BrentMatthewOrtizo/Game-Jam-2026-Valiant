using UnityEngine;
using UnityEngine.UI;

public class GreedMeter : MonoBehaviour
{
    [Header("Greed Values")]
    [SerializeField] private float maxGreed = 100f;
    [SerializeField] private float currentGreed = 0f;

    [Header("Drain Rates")]
    [SerializeField] private float normalDrainPerSecond = 8f;
    [SerializeField] private float activatedDrainPerSecond = 20f;

    [Header("Coin Gain")]
    [SerializeField] private float greedPerCoin = 12f;

    [Header("UI")]
    [Tooltip("11 sprites from empty -> full")]
    [SerializeField] private Sprite[] normalFillStages;      
    [Tooltip("11 sprites from empty -> full")]
    [SerializeField] private Sprite[] activatedFillStages;  
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject normalFrame;
    [SerializeField] private GameObject activatedFrame;

    [Header("Activated Effect")]
    [SerializeField] private PlayerMovement2D movement;
    [SerializeField] private float normalMoveSpeed = 10f;
    [SerializeField] private float activatedMoveSpeed = 14f;

    public bool IsActivated { get; private set; }

    private void Awake()
    {
        if (movement == null)
            movement = FindObjectOfType<PlayerMovement2D>();

        SetActivated(false, force:true);
        UpdateUI();
    }

    private void Update()
    {
        float drain = IsActivated ? activatedDrainPerSecond : normalDrainPerSecond;

        currentGreed -= drain * Time.deltaTime;
        currentGreed = Mathf.Clamp(currentGreed, 0f, maxGreed);
        
        if (!IsActivated && currentGreed >= maxGreed)
        {
            currentGreed = maxGreed;
            SetActivated(true);
        }
        
        if (IsActivated && currentGreed <= 0f)
        {
            currentGreed = 0f;
            SetActivated(false);
        }

        UpdateUI();
    }

    public void AddCoinGreed()
    {
        AddGreed(greedPerCoin);
    }

    public void AddGreed(float amount)
    {
        currentGreed = Mathf.Clamp(currentGreed + amount, 0f, maxGreed);

        if (!IsActivated && currentGreed >= maxGreed)
        {
            currentGreed = maxGreed;
            SetActivated(true);
        }

        UpdateUI();
    }

    private void SetActivated(bool active, bool force = false)
    {
        if (!force && IsActivated == active) return;

        IsActivated = active;

        if (normalFrame != null) normalFrame.SetActive(!active);
        if (activatedFrame != null) activatedFrame.SetActive(active);

        if (movement != null)
        {
            movement.SetMoveSpeed(active ? activatedMoveSpeed : normalMoveSpeed);
        }
    }

    private void UpdateUI()
    {
        if (fillImage == null) return;

        float t = (maxGreed <= 0f) ? 0f : currentGreed / maxGreed;
        int index = Mathf.Clamp(Mathf.RoundToInt(t * 10f), 0, 10);

        Sprite[] stages = IsActivated ? activatedFillStages : normalFillStages;

        if (stages == null || stages.Length < 11) return;

        fillImage.sprite = stages[index];
    }
}