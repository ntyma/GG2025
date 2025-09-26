using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenColorChanger : StateMachineBehaviour
{
    public Sprite table;
    private SpriteRenderer renderer;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        renderer = animator.GetComponent<SpriteRenderer>();
        renderer.sprite = table;
    }
}
