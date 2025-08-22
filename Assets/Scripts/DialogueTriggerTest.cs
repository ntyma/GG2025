using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerTest : MonoBehaviour
{
    bool isPlaying = false;
    private AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other) // untether key when light hits enemy
    {
        if (other.CompareTag("Player") && !isPlaying)
        {
            isPlaying = true;
            audioManager.Pause("Theme");
            audioManager.PlayIntroThenLoop("GuideIntro", "GuideLoop");
        } else
            return;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPlaying)
        {
            isPlaying = false;
            audioManager.StopIntroThenLoop("GuideIntro", "GuideLoop");
            audioManager.Unpause("Theme");
        }
        else
            return;
    }
}
