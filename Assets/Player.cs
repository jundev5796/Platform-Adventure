using System;
using System.Collections;
using Unity.VisualScripting;


// using System.Numerics;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;

    [Header("Buffer Jump")]
    [SerializeField] private float bufferJumpWindow = 0.25f;
    private float bufferJumpActivated = -1;

    [Header("Wall Interactions")]
    [SerializeField] private float wallJumpDuration = 0.6f;
    [SerializeField] private Vector2 wallJumpForce;
    private bool isWallJumping;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 1;
    [SerializeField] private Vector2 knockbackPower;
    private bool isKnocked;

    [Header("Collision")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;

    private float xInput;
    private float yInput;

    private bool facingRight = true;
    private int facingDir = 1;
    
    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        UpdateAirboneStatus();

        if (isKnocked)
            return;

        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimations();
    }

    private void Knockback()
    {
        if (isKnocked)
            return;

        StartCoroutine(KnockbackRoutine());
        anim.SetTrigger("Knockback");
        rb.linearVelocity = new Vector2(knockbackPower.x * -facingDir, knockbackPower.y);
    }

    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.linearVelocityY < 0; 
        float yModifier = yInput < 0 ? 1 : 0.05f;

        if (canWallSlide == false)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * yModifier);
    }

    private void UpdateAirboneStatus()
    {
        if (isGrounded && isAirborne)
            HandleLanding();

        if (!isGrounded && !isAirborne)
            BecomeAirborne();
    }

    private void BecomeAirborne()
    {
        isAirborne = true;
    }

    private void HandleLanding()
    {
        isAirborne = false;
        canDoubleJump = true;

        AttemptBufferJump();
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
            RequestBufferJump();
        }
    }

    private void RequestBufferJump()
    {
        if (isAirborne)
            bufferJumpActivated = Time.time;
    }

    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpActivated + bufferJumpWindow)
        {
            bufferJumpActivated = 0;
            Jump();
        }
    }

    private void JumpButton()
    {
        if (isGrounded)
        {
            Jump();
        }
        else if (isWallDetected && !isGrounded)
        {
            WallJump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
        }
    }

    private void Jump() => rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
    private void DoubleJump()
    {
        isWallJumping = false;
        canDoubleJump = false;
        rb.linearVelocity = new Vector2(rb.linearVelocityX, doubleJumpForce);
    }

    private void WallJump()
    {
        canDoubleJump = true;
        rb.linearVelocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);
        Flip();
        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
    }

    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.linearVelocityX);
        // anim.SetBool("isRunning", rb.linearVelocityX != 0);
        anim.SetFloat("yVelocity", rb.linearVelocityY);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    private void HandleMovement() 
    {
        if (isWallDetected)
            return;

        if (isWallJumping)
            return;

        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocityY);
    }

    private void HandleFlip()
    {
        if (xInput < 0 && facingRight || xInput > 0 && !facingRight)
            Flip();
    }

    private void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
    }
}
