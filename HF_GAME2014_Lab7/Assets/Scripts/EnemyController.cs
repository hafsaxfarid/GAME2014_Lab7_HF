using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float runForce;
    public Transform lookAheadPoint;
    public Transform lookInFrontPoint;
    public LayerMask groundLayerMask;
    public LayerMask wallLayerMask;
    public bool isGroundAhead;

    private Rigidbody2D enemyRB;

    void Start()
    {
        enemyRB = GetComponent<Rigidbody2D>();        
    }

    void FixedUpdate()
    {
        LookAhead();
        LookInFront();
        MoveEnemy();
    }

    private void LookAhead()
    {
        var hit = Physics2D.Linecast(transform.position, lookAheadPoint.position, groundLayerMask);

        if(hit)
        {
            isGroundAhead = true;
        }
        else
        {
            isGroundAhead = false;
        }
    }

    private void LookInFront()
    {
        var hit = Physics2D.Linecast(transform.position, lookInFrontPoint.position, wallLayerMask);

        if (hit)
        {
            Flip();
        }
    }


    private void MoveEnemy()
    {
        if(isGroundAhead)
        {
            enemyRB.AddForce(Vector2.left * runForce * transform.localScale.x);
            enemyRB.velocity *= 0.90f;
        }
        else
        {
            Flip();
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1.0f, transform.localScale.y, transform.localScale.z);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform"))
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, lookAheadPoint.position);
        Gizmos.DrawLine(transform.position, lookInFrontPoint.position);
    }

}