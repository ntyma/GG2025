using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionScript : MonoBehaviour
{
    [SerializeField] private BoxCollider2D transitionBoxCollider;
    [SerializeField] private SpriteRenderer transitionSpriteRenderer;
    [SerializeField] private PlayerRespawnCameraAnimationScript playerRespawnAnimationScript;

    enum Scenes {MainHouse, Lighthouse, Cutscene, MainMenuEnding}
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
    }

    private void TransitionToScene()
    {
        SaveData continuingData;
        switch (nextScene)
        {
            case (Scenes.MainHouse):
                SceneManager.LoadScene("MainHouse");
                break;
            case (Scenes.Lighthouse):
                SaveManager.UpdateSaveData(data => data.playerLevel = 18);
                SaveManager.UpdateSaveData(data => data.isHouseLevels = false);

                continuingData = SaveManager.LoadGame();
                if (continuingData != null)
                {
                    SaveManager.loadingData = true;
                    SaveManager.levelLoading = continuingData.playerLevel;
                    SaveManager.timeLoading = continuingData.time;
                    SaveManager.isForwardRoute = continuingData.isForwardRoute;
                    SaveManager.isHouseLevels = continuingData.isHouseLevels;
                }
                    LevelManager.Instance.LoadScene("Lighthouse", "CrossFade");
                //SceneManager.LoadScene("Lighthouse");
                break;
            case (Scenes.Cutscene):
                SaveManager.UpdateSaveData(data => data.playerLevel = 26);
                SaveManager.UpdateSaveData(data => data.isForwardRoute = false);

                continuingData = SaveManager.LoadGame();
                if (continuingData != null)
                {
                    SaveManager.loadingData = true;
                    SaveManager.levelLoading = continuingData.playerLevel;
                    SaveManager.timeLoading = continuingData.time;
                    SaveManager.isForwardRoute = continuingData.isForwardRoute;
                    SaveManager.isHouseLevels = continuingData.isHouseLevels;
                }
                AudioManager.instance.StopAllAudio();
                SceneManager.LoadScene("Cutscene");
                break;
            case (Scenes.MainMenuEnding):
                SaveManager.UpdateSaveData(data => data.playEndingCutscene = true);

                continuingData = SaveManager.LoadGame();
                if (continuingData != null)
                {
                    SaveManager.playEndingCutscene = continuingData.playEndingCutscene;
                }
                AudioManager.instance.StopAllAudio();
                SceneManager.LoadScene("MainMenu");
                break;
            default:
                Debug.LogWarning("No Scene is Selected! - from TransitionToScene() in SceneTransitionScript.cs");
                break;
        }
    }
}
