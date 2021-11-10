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

    private Rigidbody2D playerRB;

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
        CheckIsGrounded();
    }

    private void Move()
    {
        if (isGrounded)
        {
            // keyboard input
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            float jump = Input.GetAxisRaw("Jump");

            // touch input
            Vector2 worldTouch = new Vector2();

            if (x != 0)
            {
                x = FlipAnimation(x);
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