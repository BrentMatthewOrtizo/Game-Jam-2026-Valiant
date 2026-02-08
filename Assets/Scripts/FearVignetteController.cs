using UnityEngine;
using UnityEngine.UI;

public class FearVignetteController : MonoBehaviour
{
    [SerializeField] private FearMeter fearMeter;
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;
    [SerializeField] private Image img;

    [Header("Radius Tuning")]
    [SerializeField] private float radiusLowFear = 0.70f;
    [SerializeField] private float radiusHighFear = 0.18f;
    [SerializeField] private float softness = 0.08f;

    [Header("Pixelation")]
    [SerializeField] private float pixelSizeLowFear = 1200f;
    [SerializeField] private float pixelSizeHighFear = 250f;

    [Header("Darkness")]
    [SerializeField] private float darknessLowFear = 0.35f;
    [SerializeField] private float darknessHighFear = 0.95f;

    private Material mat;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (img == null) img = GetComponent<Image>();
        if (fearMeter == null) fearMeter = FindObjectOfType<FearMeter>();
        
        mat = Instantiate(img.material);
        img.material = mat;
    }

    void LateUpdate()
    {
        if (mat == null || cam == null || player == null || fearMeter == null)
            return;

        float t = fearMeter.Fear01;
        
        Vector3 vp = cam.WorldToViewportPoint(player.position);
        Vector2 center = new Vector2(vp.x, vp.y);

        float radius = Mathf.Lerp(radiusLowFear, radiusHighFear, t);
        float px = Mathf.Lerp(pixelSizeLowFear, pixelSizeHighFear, t);
        float dark = Mathf.Lerp(darknessLowFear, darknessHighFear, t);

        mat.SetVector("_Center", new Vector4(center.x, center.y, 0, 0));
        mat.SetFloat("_Radius", radius);
        mat.SetFloat("_Softness", softness);
        mat.SetFloat("_PixelSize", px);
        mat.SetFloat("_Darkness", dark);
    }
}