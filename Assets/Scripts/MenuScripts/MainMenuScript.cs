using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
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
            SaveManager.loadingData = true;
            SaveManager.levelLoading = continuingData.playerLevel;
            Debug.Log("Game Loaded: Level " + continuingData.playerLevel);
            SceneManager.LoadScene("MainHouse");
        }
        else
        {
            Debug.Log("error - no player data found");
        }
    }

    public void NewGame()
    {
        SaveData data = new SaveData
        {
            playerLevel = 0
        };
        SaveManager.SaveGame(data);
        SaveManager.loadingData = false;
        SceneManager.LoadScene("MainHouse");
        AudioManager.instance.Stop("Title");
        AudioManager.instance.Play("House");
    }

    public void QuitGame()
    {
        UnityEngine.Debug.Log("Quitting game...");
        Application.Quit();
    }
}
