using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingWalker : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        animator.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
    }
}
