
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    private Animator transition;

    void Awake()
    {
        transition = GameObject.Find("light_main").GetComponent<Animator>();
    }

    // Start is called before the first frame update
    public void Start()
    {
        AudioManager.instance.Play("Title");
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
            Debug.Log("Game Loaded: Level " + continuingData.playerLevel);
            //SceneManager.LoadScene("MainHouse");
            transition.SetTrigger("playFlashbang"); // trigger the transition animation
            StartCoroutine(LoadAfterTransition("Lighthouse"));
            //LevelManager.Instance.LoadScene("MainHouse", "CrossFade");
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
            playerMemory = new bool[10]
        };
        SaveManager.SaveGame(data);
        SaveManager.loadingData = false;

        transition.SetTrigger("playFlashbang"); // trigger the transition animation
        StartCoroutine(LoadAfterTransition("Lighthouse"));
        //LevelManager.Instance.LoadScene("MainHouse", "CrossFade");

        AudioManager.instance.Stop("Title");
        AudioManager.instance.Play("House");
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
}
