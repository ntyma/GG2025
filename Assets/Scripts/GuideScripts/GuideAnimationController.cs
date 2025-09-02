using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GuideAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private FollowMouse followMouse;
    [SerializeField] private float clipSpeedMultiplier;
    private bool isOverEnemy;

    private void Start()
    {
        animator.SetFloat("ClipSpeedMultiplier", clipSpeedMultiplier);
        followMouse.onGoingRight += UpdateAnimationDirection;
    }

    private void UpdateAnimationDirection(bool isGoingRight)
    {
        spriteRenderer.flipX = !isGoingRight;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Player") && !isOverEnemy)
        {
            animator.SetBool("isOverMC", true);
            animator.SetBool("isAway", false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Enemy"))
        {
            isOverEnemy = true;
            animator.SetBool("isOverEnemy", true);
            animator.SetBool("isOverMC", false);
            animator.SetBool("isAway", false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isOverMC", false);
        } 
        else if (other.CompareTag("Enemy"))
        {
            isOverEnemy = false;
            animator.SetBool("isOverEnemy", false);
        }

        animator.SetBool("isAway", true);
    }
}
