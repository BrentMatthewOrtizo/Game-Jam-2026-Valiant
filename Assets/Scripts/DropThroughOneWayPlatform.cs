using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class DropThroughOneWayPlatform : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string platformLayerName = "Platform";

    [Header("Drop Settings")]
    [SerializeField] private float dropTime = 0.20f;     
    [SerializeField] private float minDownInput = -0.6f; 

    [Header("Ground Check (optional override)")]
    [SerializeField] private PlayerCollision2D collision2D; 

    private int _playerLayer;
    private int _platformLayer;

    private bool _dropQueued;
    private float _lastDownY;

    private void Awake()
    {
        _playerLayer = LayerMask.NameToLayer(playerLayerName);
        _platformLayer = LayerMask.NameToLayer(platformLayerName);

        if (_playerLayer < 0) Debug.LogError($"DropThroughOneWayPlatform: Layer '{playerLayerName}' not found.");
        if (_platformLayer < 0) Debug.LogError($"DropThroughOneWayPlatform: Layer '{platformLayerName}' not found.");

        if (collision2D == null) collision2D = GetComponent<PlayerCollision2D>();
    }
    
    public void OnMove(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();
        _lastDownY = v.y;

        
        if (_lastDownY <= minDownInput)
            _dropQueued = true;
    }

    private void Update()
    {
        if (!_dropQueued) return;

        
        if (collision2D != null && collision2D.OnGround)
        {
            _dropQueued = false;
            StartCoroutine(DropRoutine());
        }
        else
        {
            _dropQueued = false;
        }
    }

    private IEnumerator DropRoutine()
    {
        Physics2D.IgnoreLayerCollision(_playerLayer, _platformLayer, true);
        yield return new WaitForSeconds(dropTime);
        Physics2D.IgnoreLayerCollision(_playerLayer, _platformLayer, false);
    }
}