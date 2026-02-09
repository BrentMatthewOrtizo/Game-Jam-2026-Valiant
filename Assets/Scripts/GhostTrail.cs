using UnityEngine;
using DG.Tweening;

public class GhostTrail : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform ghostsParent;          
    [SerializeField] private Transform target;                
    [SerializeField] private SpriteRenderer sourceRenderer; 

    [Header("Look")]
    [SerializeField] private Color trailColor = new Color(1f, 1f, 1f, 0.6f);
    [SerializeField] private Color fadeColor  = new Color(1f, 1f, 1f, 0f);

    [Header("Timing")]
    [SerializeField] private float ghostInterval = 0.05f;
    [SerializeField] private float fadeTime = 0.15f;

    private Sequence seq;

    private void Awake()
    {
        if (ghostsParent == null) ghostsParent = transform;
        
        for (int i = 0; i < ghostsParent.childCount; i++)
        {
            var gsr = ghostsParent.GetChild(i).GetComponent<SpriteRenderer>();
            if (gsr != null) gsr.color = fadeColor;
        }
    }

    public void ShowGhost()
    {
        if (ghostsParent == null || target == null || sourceRenderer == null) return;

        if (seq != null && seq.IsActive()) seq.Kill();
        seq = DOTween.Sequence();

        for (int i = 0; i < ghostsParent.childCount; i++)
        {
            Transform currentGhost = ghostsParent.GetChild(i);
            SpriteRenderer gsr = currentGhost.GetComponent<SpriteRenderer>();
            if (gsr == null) continue;

            seq.AppendCallback(() => currentGhost.position = target.position);
            seq.AppendCallback(() => gsr.flipX = sourceRenderer.flipX);
            seq.AppendCallback(() => gsr.sprite = sourceRenderer.sprite);
            
            seq.AppendCallback(() =>
            {
                gsr.DOKill();
                gsr.color = trailColor;
                gsr.DOColor(fadeColor, fadeTime);
            });

            seq.AppendInterval(ghostInterval);
        }
    }
}