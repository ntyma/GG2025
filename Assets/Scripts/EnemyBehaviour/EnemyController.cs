using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    public Transform playerPos;
    public float speed;
    public float viewDst = 5f;

    public bool enemyIsInLight = false;

    /*[Range(0,360)]
    public float fov = 90f;
    public float angle;
    public float angleToPlayer;

    private float distance;

    public LayerMask targetMask;
    public LayerMask obstacleMask;*/

    public Vector3 DirFromAngle(float angleInDeg)
    {
        return new Vector3(Mathf.Sin(Mathf.Deg2Rad * angleInDeg), Mathf.Cos(Mathf.Deg2Rad * angleInDeg), 0f);
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyIsInLight = false;
    }
    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, playerPos.position);

        if(Mathf.Abs(distance) < viewDst && !enemyIsInLight)
        {
            float movement = -Mathf.Sign(transform.position.x - playerPos.position.x) * speed;
            Debug.Log("moving with the following velocity: " + movement);
            rb.velocity = new Vector2(movement, 0);
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
        {
            Debug.Log("collided with " + collision.gameObject.name);
            enemyIsInLight = true;
            rb.velocity = Vector2.zero;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
            enemyIsInLight = false;
    }
    private void OnCollisionEnter2D(Collision2D collision) // Take damage when colliding with the player
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject.transform.position.y < (transform.position.y + transform.localScale.y))
            {
                var healthComponent = collision.gameObject.GetComponent<Health>();
                if (healthComponent != null)
                {
                    healthComponent.TakeDamage(1);
                }
            }
            
        }
    }
}
