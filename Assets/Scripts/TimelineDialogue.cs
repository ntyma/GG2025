using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineAfterDialogue : MonoBehaviour
{
    [SerializeField] private PlayableDirector nextTimeline;
    [SerializeField] private Animator animator;
    [SerializeField] private TimelineAfterDialogue nextLink;
    [SerializeField] private bool startActive;

    private bool isActiveListener = false;

    private void OnEnable()
    {
        if (startActive)
            isActiveListener = true;

        StartCoroutine(WaitForDialogueManager());
    }

    private void OnDisable()
    {
        DialogueManager.Instance.onDialogueEnd -= PlayNextTimeline;
    }

    private IEnumerator WaitForDialogueManager()
    {
        while (DialogueManager.Instance == null)
            yield return null;

        if (isActiveListener)
            DialogueManager.Instance.onDialogueEnd += PlayNextTimeline;
    }

    public void PlayNextTimeline()
    {
        UnityEngine.Debug.Log("playing next timeline");

        if (!isActiveListener) return;

        if (nextTimeline != null)
            nextTimeline.Play();

        if (animator != null)
            animator.SetBool("isTransformed", true);

        DialogueManager.Instance.onDialogueEnd -= PlayNextTimeline;

        isActiveListener = false;

        if (nextLink != null)
            nextLink.ActivateListener();
    }

    public void ActivateListener() 
    { 
        isActiveListener = true; 
        DialogueManager.Instance.onDialogueEnd += PlayNextTimeline; 
    }
}