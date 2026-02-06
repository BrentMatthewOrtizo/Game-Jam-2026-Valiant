using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 8f;
    public float jumpVelocity = 12f;

    private Rigidbody2D rb;
    private PlayerCollision coll;

    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<PlayerCollision>();
    }

    // Called automatically by PlayerInput (Send Messages)
    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Called automatically by PlayerInput (Send Messages)
    private void OnJump()
    {
        if (coll != null && coll.onGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
    }
}