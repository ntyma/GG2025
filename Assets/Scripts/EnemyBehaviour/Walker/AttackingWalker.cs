using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingWalker : StateMachineBehaviour
{
    private SpriteRenderer renderer;
    public Color hitColor;
    public Color defaultColor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        animator.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        renderer = animator.GetComponent<SpriteRenderer>();
        defaultColor = Color.white;

        renderer.color = hitColor;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        renderer.color = defaultColor;
    }


    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
