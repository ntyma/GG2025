
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenuScript : MonoBehaviour
{
    private Animator transition;
    private Canvas canvasComponent;

    void Awake()
    {
        transition = GameObject.Find("light_main").GetComponent<Animator>();
        canvasComponent = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    // Start is called before the first frame update
    public void Start()
    {
        SaveData continuingData = SaveManager.LoadGame();
        if (continuingData != null && continuingData.playEndingCutscene == true)
        {
            // Play Allen's Cutscene
            PlayEndingCutscene();
        }
        else
        {
            AudioManager.instance.Play("Title");
        }
    }

    public void ContinueGame()
    {
        SaveData continuingData = SaveManager.LoadGame();
        if (continuingData != null)
        {
            AudioManager.instance.Play("MenuForwards");
            SaveManager.loadingData = true;
            SaveManager.levelLoading = continuingData.playerLevel;
            SaveManager.timeLoading = continuingData.time;
            SaveManager.isForwardRoute = continuingData.isForwardRoute;
            SaveManager.isHouseLevels = continuingData.isHouseLevels;

            Debug.Log("Game Loaded: Level " + continuingData.playerLevel);
            //SceneManager.LoadScene("MainHouse");
            transition.SetTrigger("playFlashbang"); // trigger the transition animation

            if (continuingData.isHouseLevels)
                StartCoroutine(LoadAfterTransition("MainHouse"));
            else
                StartCoroutine(LoadAfterTransition("Lighthouse"));

            //LevelManager.Instance.LoadScene("MainHouse", "CrossFade");
            AudioManager.instance.Stop("Title");
            //AudioManager.instance.Play("House");
        }
        else
        {
            AudioManager.instance.Play("DebrisFall");
        }
    }

    public void NewGame()
    {
        AudioManager.instance.Play("NewGame");
        SaveData data = new SaveData
        {
            playerLevel = 0,
            time = 0,
            beatenGame = false,
            isHouseLevels = true,
            isForwardRoute = true,

            playEndingCutscene = false,

            playerMemory = new bool[10]
        };
        SaveManager.SaveGame(data);
        SaveManager.loadingData = false;

        transition.SetTrigger("playFlashbang"); // trigger the transition animation
        StartCoroutine(LoadAfterTransition("MainHouse"));
        //LevelManager.Instance.LoadScene("MainHouse", "CrossFade");

        AudioManager.instance.Stop("Title");
        //AudioManager.instance.Play("House");
        Debug.Log(data.playerMemory.Length);
    }

    private IEnumerator LoadAfterTransition(string sceneName)
    {
        yield return null; // wait for the flashbang to start

        float flashbangDuration = transition.GetCurrentAnimatorStateInfo(0).length;
        // wait for the transition animation to finish
        yield return new WaitForSeconds(flashbangDuration);

        LevelManager.Instance.LoadScene(sceneName, "CrossFade");
    }

    public void QuitGame()
    {
        AudioManager.instance.Play("MenuBackwards");
        UnityEngine.Debug.Log("Quitting game...");
        Application.Quit();
    }

    private void PlayEndingCutscene()
    {
        Debug.Log("Ending Cutscene begins!");
        // Do not play the cutscene again upon Game Restart
        SaveManager.UpdateSaveData(data => data.playEndingCutscene = false);
        canvasComponent.enabled = false;
        GameObject.Find("BlackScreen").GetComponent<SpriteRenderer>().enabled = true;
        VideoPlayer endingCutsceneVideoPlayer = GameObject.Find("EndingCutscene").GetComponent<VideoPlayer>();
        endingCutsceneVideoPlayer.loopPointReached += EndingCutsceneEnd;

        endingCutsceneVideoPlayer.Play();
    }
    private void EndingCutsceneEnd(VideoPlayer vp)
    {
        Debug.Log("Ending Cutscene has finished playing!");
        vp.enabled = false;

        AudioManager.instance.Play("Title");
        StartCoroutine(FadeBlackScreen());
    }
    private IEnumerator FadeBlackScreen()
    {
        yield return new WaitForSeconds(1.0f);

        SpriteRenderer blackScreen = GameObject.Find("BlackScreen").GetComponent<SpriteRenderer>();
        float timer = 1.0f;
        while (timer >= 0.0f)
        {
            yield return null;
            timer = timer - Time.deltaTime;
            blackScreen.color = blackScreen.color - new Color(0.0f, 0.0f, 0.0f, Time.deltaTime);
        }

        blackScreen.enabled = false;
        canvasComponent.enabled = true;
    }
}
