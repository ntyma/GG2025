using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LighthouseSceneTransitionScript : MonoBehaviour
{
    [SerializeField] private BoxCollider2D transitionBoxCollider;
    [SerializeField] private SpriteRenderer transitionSpriteRenderer;
    [SerializeField] private PlayerRespawnCameraAnimationScript playerRespawnAnimationScript;
    private void Awake()
    {
        transitionSpriteRenderer.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;

        playerRespawnAnimationScript.PlayPlayerDeathCameraAnimation();
        Invoke("TransitionToLighthouse", playerRespawnAnimationScript.initialDelay + playerRespawnAnimationScript.deathFadeoutTime+0.1f);
        // Stop Player Controls
    }

    private void TransitionToLighthouse()
    {
        SceneManager.LoadScene("Lighthouse");
    }
}
