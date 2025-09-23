using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionScript : MonoBehaviour
{
    [SerializeField] private BoxCollider2D transitionBoxCollider;
    [SerializeField] private SpriteRenderer transitionSpriteRenderer;
    [SerializeField] private PlayerRespawnCameraAnimationScript playerRespawnAnimationScript;

    enum Scenes {MainHouse, Lighthouse, Cutscene}
    [SerializeField] private Scenes nextScene = Scenes.MainHouse;
    private void Awake()
    {
        transitionSpriteRenderer.enabled = false;
        playerRespawnAnimationScript =
            GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerRespawnCameraAnimationScript>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;

        playerRespawnAnimationScript.PlayPlayerDeathCameraAnimation();
        Invoke("TransitionToScene", playerRespawnAnimationScript.initialDelay + playerRespawnAnimationScript.deathFadeoutTime+0.1f);
        // Stop Player Controls

        SaveData data = new SaveData
        {
            isForwardRoute = false
        };
        SaveManager.SaveGame(data);
    }

    private void TransitionToScene()
    {
        switch (nextScene)
        {
            case (Scenes.MainHouse):
                SceneManager.LoadScene("MainHouse");
                break;
            case (Scenes.Lighthouse):
                SceneManager.LoadScene("Lighthouse");
                break;
            case (Scenes.Cutscene):
                SceneManager.LoadScene("Cutscene");
                break;
            default:
                Debug.LogWarning("No Scene is Selected! - from TransitionToScene() in SceneTransitionScript.cs");
                break;
        }
    }
}
