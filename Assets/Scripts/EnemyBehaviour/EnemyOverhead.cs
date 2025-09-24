using System.Collections;
using System.Collections.Generic;
//using UnityEditor.UI;
using UnityEngine;

public class EnemyOverhead : MonoBehaviourWithReset
{

    private Animator animator;
    private Rigidbody2D rb;
    private Transform playerPos;
    private PlayerController playerController;


    public float viewDst = 5;
    public float speed = 5;
    public float hitDst = 1;

    public bool midAttack = false;
    public bool isFrozen = false;
    public bool contactDamage = true;

    public float knockbackForce = 10f;
    public float knockbackUpwardForce = 10f;

    public Vector2 initialpos;

    //for hitbox
    public Vector2 boxSize = new Vector2(2f, 2f);   // Width & height of the box
    private float boxDistance = -1f;                  // How far in front of enemy
    public LayerMask playerLayer;                   // Assign "Player" layer in Inspector

    private bool isPlayingFreeze;
    // Awake is called before all Start() functions and the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

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
    private int lightCount = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
        {
            lightCount = lightCount + 1;
            if (lightCount == 1)
            {
                isFrozen = true;
                animator.SetTrigger("EnemyInLight");
                rb.velocity = Vector2.zero;
                if (!isPlayingFreeze)
                {
                    isPlayingFreeze = true;
                    if (Vector3.SqrMagnitude(playerController.transform.position - this.transform.position) < 150.0f)
                        AudioManager.instance.Play("EnemyFreeze");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
        {
            lightCount = lightCount - 1;
            if (lightCount == 0)
            {
                isFrozen = false;
                animator.ResetTrigger("EnemyInLight");
                isPlayingFreeze = false;

                if (Vector3.SqrMagnitude(playerController.transform.position - this.transform.position) < 150.0f)
                    AudioManager.instance.Play("EnemyUnfreeze");
            }
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (contactDamage && !midAttack && !isFrozen)
        {
            if(collision.gameObject.layer == 8)
            {
                AudioManager.instance.Play("MonsterAttack");
                animator.SetTrigger("HittingPlayer");
                midAttack = true;
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

                Health playerHealth = collision.gameObject.GetComponent<Health>();
                playerHealth.TakeDamage(1f);

                playerController.move.Disable();
                playerController.jump.Disable();
                playerRb.velocity = Vector2.zero; // reset before applying

                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                knockbackDir.y = 0; // keep horizontal if you want
                knockbackDir = knockbackDir.normalized;
                playerRb.AddForce(new Vector2(knockbackDir.x * knockbackForce, knockbackUpwardForce), ForceMode2D.Impulse);

                Invoke("reactivateMovement", 1f);
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

    private void reactivateMovement()
    {
        playerController.move.Enable();
        playerController.jump.Enable();
    }

    public override void ResetToInstantiation()
    {
        transform.position = initialpos;
    }
}
