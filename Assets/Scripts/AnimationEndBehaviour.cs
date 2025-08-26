using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEndBehaviour : MonoBehaviour
{
    public Action OnLandAnimationEnded;
    public Action OnStartJumpAnimationEnded;

    public void LandAnimationEnded()
    {
        OnLandAnimationEnded.Invoke();
    }

    public void StartJumpAnimationEnded()
    {
        OnStartJumpAnimationEnded.Invoke();
    }
}
