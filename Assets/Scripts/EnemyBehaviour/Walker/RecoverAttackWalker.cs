using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverAttackWalker : StateMachineBehaviour
{

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("HittingPlayer");
        animator.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        animator.GetComponent<EnemyOverhead>().midAttack = false;
    }
}
