using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DashDasherScript : StateMachineBehaviour
{
    private Rigidbody2D rb;
    private float windupTime = 1f;
    private float dashDst = 1f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        windupTime -= Time.deltaTime;
        if (windupTime < 0f)
        {
            Debug.Log("winded up");
        }
    }
}
