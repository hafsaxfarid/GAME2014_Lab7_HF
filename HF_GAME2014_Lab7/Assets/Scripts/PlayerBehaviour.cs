using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Movement")]
    public float horizontalForce;
    public float verticalForce;
    public bool isGrounded;
    public Transform groundOrigin;
    public float groundRadius;
    public LayerMask groundLayerMask;
    
    [Range(0.1f, 0.9f)]
    public float airControlFactor;

    [Header("Animation")]
    public PlayerAnimationState state;

    private Rigidbody2D playerRB;
    private Animator playerAnimationController;
    private string animationState = "AnimationState";

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerAnimationController = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Move();
        CheckIsGrounded();
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");

        if (isGrounded)
        {
            // keyboard input    
            float y = Input.GetAxisRaw("Vertical");
            float jump = Input.GetAxisRaw("Jump");

            // touch input
            Vector2 worldTouch = new Vector2();

            if (x != 0)
            {
                x = FlipAnimation(x);
                playerAnimationController.SetInteger(animationState, (int) PlayerAnimationState.RUN); // run state
                state = PlayerAnimationState.RUN;
            }
            else
            {
                playerAnimationController.SetInteger(animationState, (int)PlayerAnimationState.IDLE); // idle state
                state = PlayerAnimationState.IDLE;
            }

            foreach (var touch in Input.touches)
            {
                worldTouch = Camera.main.ScreenToWorldPoint(touch.position);
            }

            float horizontalMoveForce = x * horizontalForce;
            float jumpMoveForce = y * verticalForce;

            float mass = playerRB.mass * playerRB.gravityScale;

            playerRB.AddForce(new Vector2(horizontalMoveForce, jumpMoveForce) * mass);
            playerRB.velocity *= 0.99f;
        }
        else
        {
            playerAnimationController.SetInteger(animationState, (int) PlayerAnimationState.JUMP); // jump state
            state = PlayerAnimationState.JUMP;

            if (x != 0)
            {
                x = FlipAnimation(x);

                float horizontalMoveForce = x * horizontalForce * airControlFactor;
                float mass = playerRB.mass * playerRB.gravityScale;

                playerRB.AddForce(new Vector2(horizontalMoveForce, 0.0f) * mass);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Platform"))
        {
            transform.SetParent(other.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            transform.SetParent(null);
        }
    }

    public void CheckIsGrounded()
    {
        RaycastHit2D hit = Physics2D.CircleCast(groundOrigin.position, groundRadius, Vector2.down, groundRadius, groundLayerMask);
        isGrounded = (hit) ? true : false;
    }


    private float FlipAnimation(float x)
    {
        x = (x > 0) ? 1 : -1;

        transform.localScale = new Vector2 (x, 1.0f);
        return x;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundOrigin.position, groundRadius);
    }
}