using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineAfterDialogue : MonoBehaviour
{
    [SerializeField] private PlayableDirector nextTimeline;
    [SerializeField] private Animator animator;
    [SerializeField] private TimelineAfterDialogue nextLink;
    [SerializeField] private bool startActive;

    [SerializeField] private bool isActiveListener = false;

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

        if (this.transform.GetSiblingIndex() == 3)
        {
            DialogueManager.Instance.onDialogueEnd += TransitionToLighthouse;
        }
    }

    float transitionDelay = 5.0f;
    public void TransitionToLighthouse()
    {
        UnityEngine.Debug.Log("Transitioning back to Lighthouse Scene in " + transitionDelay + " Seconds");
        StartCoroutine(TransitionCoroutine());
    }
    private IEnumerator TransitionCoroutine()
    {
        float timer = transitionDelay;
        while (timer >= 0.0f)
        {
            yield return null;
            timer = timer - Time.deltaTime;
        }

        LevelManager.Instance.LoadScene("Lighthouse", "CrossFade");
    }
}