using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;

    private Rigidbody2D rb;

    public Transform playerPos;

    public float viewDst = 5;

    public float speed = 5;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
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
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
            animator.ResetTrigger("EnemyInLight");
    }
}
