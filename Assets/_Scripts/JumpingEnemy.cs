using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class JumpingEnemy : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Vector2 relativeLandingPosition = new Vector2(2, 0);
    [SerializeField] private float dir = 1;
    [SerializeField] private float h = 1;//height
    [SerializeField] private float cooldownCounter;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundMask;
    private float prevYVel = 0;
    private float gravity;
    private Vector2 newPos;
    private bool wasOnGround;
    private bool canTurn = true;
    private string currentState = "FrogIdle";

    void Awake()
    {
        newPos = transform.position + new Vector3(relativeLandingPosition.x * dir, relativeLandingPosition.y, 0);
        gravity = Physics2D.gravity.y * rb.gravityScale;
    }

    private void Update()
    {
        bool isOnGround = GroundCheck();
        if (!wasOnGround && isOnGround)
        {
            transform.localPosition = new Vector3(Mathf.RoundToInt(transform.localPosition.x), transform.localPosition.y, 0);
            canTurn = true;
            Turn();
        }

        if(cooldownCounter <= 0)
        {
            Jump();
            cooldownCounter = jumpCooldown;
        }
        else if (wasOnGround && isOnGround)
        {
            cooldownCounter -= Time.deltaTime;
        }

        Animate(isOnGround);

        prevYVel = rb.velocity.y;
        wasOnGround = isOnGround;
    }

    Vector2 CalculateJumpVel()
    {
        Vector2 yVel = Vector2.up * Mathf.Sqrt(-2 * gravity * h);
        Vector2 xVel = new Vector2(relativeLandingPosition.x * dir, 0) / (Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (relativeLandingPosition.y - h) / gravity));
        return xVel + yVel;
    }

    void Jump()
    {
        newPos = transform.position + new Vector3(relativeLandingPosition.x * dir, relativeLandingPosition.y, 0);
        rb.velocity = CalculateJumpVel();
    }

    void Turn()
    {
        ChangeAnimationState("FrogTurn");
        dir *= -1;
        cooldownCounter = jumpCooldown;
    }

    #region Animation
    private void ChangeAnimationState(string newState)
    {
        //Return if it's the same animation so it doesn't interrupt itself
        if (currentState == newState) return;

        //Play the new animation
        if (currentState != "FrogTurn")
        {
            animator.Play(newState);
            currentState = newState;
        }
        else
        {
            if (HasAnimationFinished())
            {
                animator.Play(newState);
                currentState = newState;
            }
            else if (IsAnimationAtHalf() && canTurn)
            {
                GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
                canTurn = false;
            }
        }
    }

    private void Animate(bool isGrounded)
    {
        if (!isGrounded)
        {
            //Air animations
            if (rb.velocity.y > 2f)
            {
                ChangeAnimationState("FrogAirUp");
            }
            else if (rb.velocity.y < -2f)
            {
                ChangeAnimationState("FrogAirDown");
            }
            else
            {
                ChangeAnimationState("FrogAirMid");
            }
        }
        else if (cooldownCounter <= jumpCooldown / 1.05f)
        {
            ChangeAnimationState("FrogIdle");
        }
    }

    bool HasAnimationFinished()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f;
    }

    bool IsAnimationAtHalf()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f;
    }
    #endregion

    private bool GroundCheck()
    {
        return Physics2D.BoxCast(transform.position + groundCheckOffset, groundCheckSize, 0f, -Vector2.up, 0f, groundMask);
    }
}
