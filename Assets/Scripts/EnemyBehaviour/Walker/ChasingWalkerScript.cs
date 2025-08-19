using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingWalkerScript : StateMachineBehaviour
{
    private Rigidbody2D rb;

    public Transform playerPos;

    public float speed;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float xDiff = animator.transform.position.x - playerPos.position.x;
        if(Mathf.Abs(xDiff) > 0.1)
        {
            float movement = -Mathf.Sign(xDiff) * speed;
            rb.velocity = new Vector2(movement, 0);
        }
    }
}
