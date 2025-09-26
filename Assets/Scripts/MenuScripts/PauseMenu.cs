using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenuUI;

    [SerializeField] private Health playerHealthScript;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // unfreeze the game
        isPaused = false;
        AudioManager.instance.Play("MenuBackwards");
        AudioManager.instance.UnpauseAllAudio();
    }

    void Pause()
    {
        AudioManager.instance.PauseAllAudio();
        AudioManager.instance.Play("MenuForwards");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // freeze the game
        isPaused = true;
    }

    public void Reset()
    {
        UnityEngine.Debug.Log("Resetting game...");
        playerHealthScript.Respawn();
        Resume();
    }

    /* to go back to main menu, enable when that is ready
    public void QuitGame()
    {
        UnityEngine.Debug.Log("Quitting game...");
        Application.Quit();
    }
    */

    public void LoadMenu()
    {
        Debug.Log("Saving and returning to title");
        AudioManager.instance.StopAllAudio();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
