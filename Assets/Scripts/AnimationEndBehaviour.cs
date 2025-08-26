using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEndBehaviour : StateMachineBehaviour
{
    public Action OnAnimationEnded;
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnAnimationEnded.Invoke();
        
    }
}
