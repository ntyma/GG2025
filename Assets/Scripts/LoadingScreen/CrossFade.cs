using System.Collections;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class CrossFade : SceneTransition
{
    public CanvasGroup crossFade;

    public override IEnumerator AnimateTransitionIn()
    {
        var tweener = crossFade.DOFade(1f, 0.75f); // fade in over 0.75 seconds
        yield return tweener.WaitForCompletion();
    }

    public override IEnumerator AnimateTransitionOut()
    {
        var tweener = crossFade.DOFade(0f, 1f); // fade out over 1 seconds
        yield return tweener.WaitForCompletion();
    }
}
