using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float acceleration; //Also used for decceleration
    [SerializeField] private Vector3 groundCheckOffset;//The position relative to the player where the ground will be checked
    [SerializeField] private Vector2 groundCheckSize; //Size of the box in which we allow the player on the ground
    [SerializeField] private LayerMask groundMask; //Layer of the ground


    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallMultiplier; //Controls how much the velocity is being multiplied by when falling
    [SerializeField] private float jumpHoldMultiplier; //Controls the jump height based on the time that the jump button being held down
    private bool hasDoubleJump;
    private bool canControl = true; //This is true if you jump and didn't bounce from a mushroom so you can't get to an insanely high elevation
    private float coyoteCounter; //Gets decreased after the player leaves the ground, and the jump will be still valid as long as it's more than 0
    [SerializeField] private float coyoteTime; //The time that 'coyoteCounter' will start the countdown from

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private string currentState = "PlayerIdle";
    const string PLAYER_IDLE = "PlayerIdle";
    const string PLAYER_WALK = "PlayerWalk";
    const string PLAYER_JUMP = "PlayerJump";
    const string PLAYER_AIR_UP = "PlayerAirUp";
    const string PLAYER_AIR = "PlayerAir";
    const string PLAYER_AIR_DOWN = "PlayerAirDown";
    const string PLAYER_TURN = "PlayerTurn";
    //[SerializeField] private Animation anim;

    private float inputX;
    private bool wasOnGround; //Used for keeping track of the player being on the ground in he previous frame
    private float prevScale = 1f; // Used for knowing when the player turns
    #endregion

    private void Update()
    {
        bool isOnGround = GroundCheck();
        float xVel = rb.velocity.x;

        if (Mathf.Abs(xVel) > 0.05f)
        {
            transform.localScale = new Vector3(xVel / Mathf.Abs(xVel), 1f, 1f);
        }

        //Check if the player can double jump
        if (isOnGround && !wasOnGround)
        {
            hasDoubleJump = true;
            coyoteCounter = coyoteTime;
        }

        if (isOnGround)
        {

        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        //Call jump
        if (Input.GetButtonDown("Jump"))
        {
            canControl = true;
            if (isOnGround || (coyoteCounter >= 0f && rb.velocity.y <= 0f))
            {
                Jump(jumpForce);
                //ChangeAnimationState(PLAYER_JUMP);
            }
            else if (hasDoubleJump)
            {
                Jump(jumpForce * 0.75f);
                hasDoubleJump = false;
            }
        }

        //Dynamic falling
        if (!isOnGround)
        {
            if (rb.velocity.y < 0f)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime; //Calculating the additional gravity that should be applied to the player
            }
            else if (rb.velocity.y > 0f && !Input.GetButton("Jump"))
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (jumpHoldMultiplier - 1f) * Time.deltaTime; //Calculating the additional gravity that should be applied to the player
            }
            else if (!canControl)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (jumpHoldMultiplier - 1f) * Time.deltaTime;
            }
        }

        Animate(isOnGround);

        wasOnGround = isOnGround;
    }

    void FixedUpdate()
    {
        //Getting user input
        inputX = Input.GetAxisRaw("Horizontal");
        //Accelerate and deccelerate
        if (inputX != 0f)
        {
            Accelerate();
        }
        else
        {
            Decelerate();
        }
    }

    private void Accelerate()
    {
        Vector2 newVel = new Vector2(moveSpeed * Time.deltaTime * inputX, rb.velocity.y);
        rb.velocity = Vector2.Lerp(rb.velocity, newVel, acceleration);
    }

    private void Decelerate()
    {
        Vector2 newVel = new Vector2(0f, rb.velocity.y);
        rb.velocity = Vector2.Lerp(rb.velocity, newVel, acceleration);
    }

    private void Jump(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x, force);
    }

    //Returns true if the player is on the ground
    private bool GroundCheck()
    {
        return Physics2D.BoxCast(transform.position + groundCheckOffset, groundCheckSize, 0f, -Vector2.up, 0f, groundMask);
    }

    #region Animations

    private void ChangeAnimationState(string newState)
    {
        //Return if it's the same animation so it doesn't interrupt itself
        if (currentState == newState) return;

        //Play the new animation
        if (currentState != PLAYER_TURN)
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
        }
    }

    private void Animate(bool isGrounded)
    {
        float scale = transform.localScale.x;

        //Switch between walk and idle
        if (isGrounded)
        {
            if (Mathf.Abs(inputX) >= 0.2f)
            {
                ChangeAnimationState(PLAYER_WALK);
            }
            else if (Mathf.Abs(inputX) < 0.2f)
            {
                ChangeAnimationState(PLAYER_IDLE);
            }

            if (scale != prevScale)
            {
                ChangeAnimationState(PLAYER_TURN);
            }
        }
        else
        {
            //Air animations
            if (rb.velocity.y > 2f)
            {
                ChangeAnimationState(PLAYER_AIR_UP);
            }
            else if (rb.velocity.y < -2f)
            {
                ChangeAnimationState(PLAYER_AIR_DOWN);
            }
            else
            {
                ChangeAnimationState(PLAYER_AIR);
            }
        }

        prevScale = scale;
    }

    bool HasAnimationFinished()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f;
    }

    #endregion


    [SerializeField] private LayerMask enemyLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Player.GameOver();
        }
        else if (collision.gameObject.CompareTag("Shroom"))
        {
            if (transform.position.y > collision.transform.position.y)
            {
                collision.gameObject.GetComponent<MushroomAnim>().PlayAnimation();
                canControl = false;
                Jump(jumpForce * 2f);
            }
        }
    }
}
