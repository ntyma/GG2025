using System.Collections;
using System.Collections.Generic;
//using UnityEditor.UI;
using UnityEngine;

public class EnemyOverhead : MonoBehaviourWithReset
{

    private Animator animator;
    private Rigidbody2D rb;
    public Transform playerPos;


    public float viewDst = 5;
    public float speed = 5;
    public float hitDst = 1;


    public Vector2 initialpos;

    //for hitbox
    public Vector2 boxSize = new Vector2(2f, 1f);   // Width & height of the box
    public float boxDistance = 1f;                  // How far in front of enemy
    public LayerMask playerLayer;                   // Assign "Player" layer in Inspector

    private bool isPlayingFreeze;
    // Awake is called before all Start() functions and the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();

        initialpos = transform.position;

        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        float distance = Vector2.Distance(transform.position, playerPos.position);

        if (Mathf.Abs(distance) < viewDst)
        {
            animator.SetTrigger("PlayerInRange");
        }

        else animator.ResetTrigger("PlayerInRange");

        if (Mathf.Abs(distance) < hitDst)
        {
            animator.SetTrigger("HittingPlayer");
        }

        else animator.ResetTrigger("HittingPlayer");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
        {
            animator.SetTrigger("EnemyInLight");
            rb.velocity = Vector2.zero;
            if (!isPlayingFreeze)
            {
                isPlayingFreeze = true;
                AudioManager.instance.Play("EnemyFreeze");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
        {
            animator.ResetTrigger("EnemyInLight");
            isPlayingFreeze = false;
            AudioManager.instance.Play("EnemyUnfreeze");
        }
    }
    public void hitPlayer()
    {
        // Calculate the center of the box in front of enemy
        Vector2 boxCenter = (Vector2)transform.position + (Vector2)(transform.right * boxDistance);

        // Check if player is inside the box
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0f, playerLayer);

        if (hit != null)
        {
            Rigidbody2D playerRb = hit.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Determine knockback direction
                Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                knockbackDir.y = 0; // keep horizontal if you want
                knockbackDir = knockbackDir.normalized;

                // Apply knockback
                playerRb.velocity = Vector2.zero; // reset before applying
                //playerRb.AddForce(new Vector2(knockbackDir.x * knockbackForce, knockbackUpwardForce), ForceMode2D.Impulse);
            }
        }
    }

    // Draw the box in Scene view for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 boxCenter = (Vector2)transform.position + (Vector2)(transform.right * boxDistance);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.ResetTrigger("HittingPlayer");
        }
        }
    public override void ResetToInstantiation()
    {
        transform.position = initialpos;
    }
}
