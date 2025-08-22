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
    }

    void Pause()
    {
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
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    */

    public void QuitGame()
    {
        //Sorry! Ben here doing some changes
        //
        //UnityEngine.Debug.Log("Quitting game...");
        //Application.Quit();

        Debug.Log("Saving and returning to title");
        SceneManager.LoadScene("testMainMenu");
    }
}
