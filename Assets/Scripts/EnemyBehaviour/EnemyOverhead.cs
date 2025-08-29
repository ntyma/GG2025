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

    public Vector2 initialpos;
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
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
        {
            animator.SetTrigger("EnemyInLight");
            rb.velocity = Vector2.zero;
            AudioManager.instance.Play("EnemyFreeze");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
        {
            animator.ResetTrigger("EnemyInLight");
            AudioManager.instance.Play("EnemyUnfreeze");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) // Take damage when colliding with the player
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.transform.position.y < (transform.position.y + transform.localScale.y))
            {
                var healthComponent = collision.gameObject.GetComponent<Health>();
                if (healthComponent != null)
                {
                    healthComponent.TakeDamage(1);
                    AudioManager.instance.Play("PlayerHurt");
                }
            }

        }
    }
    public override void ResetToInstantiation()
    {
        transform.position = initialpos;
    }
}
