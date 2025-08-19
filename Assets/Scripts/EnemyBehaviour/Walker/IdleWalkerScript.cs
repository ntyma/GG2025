using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleWalkerScript : StateMachineBehaviour
{
    public float moveSpeed = 0.7f;       // speed when walking
    public float decisionInterval = 3f; // how often NPC decides what to do

    private float actionTimer;

    private float[] actionSeq = { 0f, 1f, 0f, -1f };
    private int currentAction;

    private Rigidbody2D rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // countdown to next decision
        actionTimer -= Time.deltaTime;

        if (actionTimer <= 0f)
        {
            currentAction = currentAction == 3 ? 0 : currentAction + 1;
            actionTimer = decisionInterval;
        }
        rb.velocity = (Vector2.right * actionSeq[currentAction] * moveSpeed);
    }
}
