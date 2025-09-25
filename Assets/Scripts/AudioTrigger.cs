using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public void TriggerAudio()
    {
        AudioManager.instance.Play("GuideTransform");
    }

    public void StopAudio()
    {
        AudioManager.instance.StopAllAudio();
    }
}
