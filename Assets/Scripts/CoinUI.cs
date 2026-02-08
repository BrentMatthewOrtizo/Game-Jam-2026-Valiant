using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private Image coinImage;

    [Header("UI Coin Animation")]
    [SerializeField] private Sprite[] coinFrames;
    [SerializeField] private float fps = 12f;

    private int coins = 0;
    private float timer = 0f;
    private int frameIndex = 0;

    private void Start()
    {
        Refresh();
        if (coinImage != null && coinFrames != null && coinFrames.Length > 0)
            coinImage.sprite = coinFrames[0];
    }

    private void Update()
    {
        if (coinImage == null || coinFrames == null || coinFrames.Length == 0) return;

        timer += Time.deltaTime;
        float frameTime = 1f / Mathf.Max(1f, fps);

        while (timer >= frameTime)
        {
            timer -= frameTime;
            frameIndex = (frameIndex + 1) % coinFrames.Length;
            coinImage.sprite = coinFrames[frameIndex];
        }
    }

    public void AddCoin(int amount = 1)
    {
        coins += amount;
        Refresh();
    }

    private void Refresh()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }
}