using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 70f;
    [SerializeField] private float deceleration = 90f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float coyoteTime = 0.10f;
    [SerializeField] private float jumpBufferTime = 0.10f;
    [SerializeField] private float fallGravityMultiplier = 1.8f;
    [SerializeField] private float lowJumpGravityMultiplier = 2.4f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.6f, 0.12f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashTime = 0.15f;
    [SerializeField] private float dashCooldown = 0.35f;

    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private int facingDirection = 1;

    private Rigidbody2D rb;
    private float moveInput;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool jumpHeld;
    private float baseGravityScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseGravityScale = rb.gravityScale;
        rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpHeld = Input.GetButton("Jump");

        if (moveInput > 0)
        {
            facingDirection = 1;
        }
        else if (moveInput < 0)
        {
            facingDirection = -1;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && !isDashing)
        {
            isDashing = true;
            dashTimer = dashTime;
            dashCooldownTimer = dashCooldown;
        }

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private void FixedUpdate()
    {
        float targetSpeed = moveInput * moveSpeed;
        float speedDifference = targetSpeed - rb.linearVelocity.x;

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float movementForce = speedDifference * accelRate;

        if (isDashing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);

            dashTimer -= Time.fixedDeltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.gravityScale = baseGravityScale;
            }

            return;
        }

        rb.AddForce(Vector2.right * movementForce);

        if (rb.linearVelocity.y < -0.01f)
        {
            rb.gravityScale = baseGravityScale * fallGravityMultiplier;
        }
        else if (rb.linearVelocity.y > 0.01f && !jumpHeld)
        {
            rb.gravityScale = baseGravityScale * lowJumpGravityMultiplier;
        }
        else
        {
            rb.gravityScale = baseGravityScale;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}