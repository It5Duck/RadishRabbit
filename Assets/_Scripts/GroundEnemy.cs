using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float dir = 1;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector3 wallCheckStart;
    [SerializeField] private Vector3 edgeCheckStart;
    [SerializeField] private LayerMask ground;
    private void Awake()
    {
        edgeCheckStart = new Vector3(dir * 0.4f, -0.4f, 0);
    }

    void FixedUpdate()
    {
        if (EdgeWallCheck())
        {
            dir *= -1;
            
        }
        edgeCheckStart = new Vector3(dir * 0.4f, -0.4f, 0);
        transform.localScale = new Vector3(dir, 1, 1); //Make the enemy face the direction it's headed
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
    }

    bool EdgeWallCheck()
    {
        bool hitWall = Physics2D.Linecast(wallCheckStart + transform.position, wallCheckStart + transform.position + new Vector3(dir * 0.05f, 0, 0), ground);
        bool hitEdge = !Physics2D.Linecast(edgeCheckStart + transform.position, edgeCheckStart + transform.position + Vector3.down, ground);
        return hitWall || hitEdge;
    }
}
