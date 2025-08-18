using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject playerGameObject;
    [SerializeField] private GameObject guideGameObject;
    [SerializeField] private GameObject cameraGameObject;
    [SerializeField] private GameObject levelObstaclesGameObject;
    [SerializeField] private GameObject levelRespawnPointsGameObject;
    private MainCameraScript mainCameraScript;

    [Header("Level Statistics")]
    public int currentGameLevel = 0;
    public int gameLevelCount = 0;
    [SerializeField] private GameObject[] levelObstaclesCollection;
    [SerializeField] private GameObject[] levelRespawnPointsCollection;
    [SerializeField] private bool[] levelHasObstacles;

    public bool isForwardRoute = true;

    // Start is called before the first frame update
    void Start()
    {
        if (!cameraGameObject.TryGetComponent<MainCameraScript>(out mainCameraScript))
            Debug.Log("camera Game Object DOES NOT HAVE a MainCameraScript Component! - from Start() in GameManagerScript");

        gameLevelCount = levelObstaclesGameObject.transform.childCount;

        // Load in ALL Per-Level Obstacles
        levelObstaclesCollection = new GameObject[gameLevelCount];
        int i = 0;
        foreach (Transform childTransform in levelObstaclesGameObject.transform)
        {
            levelObstaclesCollection[i++] = childTransform.gameObject;
        }
        // Generate a list of booleans that track whether not level i has obstacles
        levelHasObstacles = new bool[gameLevelCount];
        // Load in ALL Per-Level Respawn Points
        levelRespawnPointsCollection = new GameObject[gameLevelCount];
        i = 0;
        foreach (Transform childTransform in levelRespawnPointsGameObject.transform)
        {
            levelRespawnPointsCollection[i++] = childTransform.gameObject;
        }

        // Spawn Player at appropriate respawn position
        Vector3 spawnPosition = levelRespawnPointsCollection[currentGameLevel].transform.GetChild((isForwardRoute ? 0 : 1)).transform.position;
        playerGameObject.transform.position = spawnPosition;
        guideGameObject.transform.position = spawnPosition;

        // Level Setup
        FillLevelWithObstacles();
        SetRespawnPoint();
        mainCameraScript.SetCameraPosition(currentGameLevel);
    }

    public void ProgressLevel (bool isProgressing)
    {
        currentGameLevel = currentGameLevel + (isProgressing ? 1 : -1);

        // Index out of bounds protection
        if (currentGameLevel < 0)
            currentGameLevel = 0;
        else if (currentGameLevel >= gameLevelCount)
            currentGameLevel = gameLevelCount - 1;

        FillLevelWithObstacles();
        SetRespawnPoint();
        mainCameraScript.ProgressCamera(isProgressing);
    }
    public void FillLevelWithObstacles ()
    {
        // levelObstaclesCollection[currentGameLevel];
        levelHasObstacles[currentGameLevel] = true;
    }
    public void CleanLevelOfObstacles ()
    {
        levelHasObstacles[currentGameLevel] = false;
    }
    public void SetRespawnPoint ()
    {
        Health playerHealthScript;
        if (!playerGameObject.TryGetComponent<Health>(out playerHealthScript))
        {
            Debug.Log("Health script not found within Player Game Object - from SetRespawnPoint() in GameManagerScript");
            return;
        }

        playerHealthScript.respawnPoint =
            isForwardRoute ?
            levelRespawnPointsCollection[currentGameLevel].transform.GetChild(0).transform:
            levelRespawnPointsCollection[currentGameLevel].transform.GetChild(1).transform;
    }
    public void SelectLevel (int Index)
    {
        // If Index is out of bounds
        if (Index < 0 || Index >= gameLevelCount)
        {
            Debug.Log("Index is OUT OF BOUNDS - in SelectLevel() from GameManagerScript");
            return;
        }

        FillLevelWithObstacles();
        SetRespawnPoint();
        mainCameraScript.SetCameraPosition(Index);
    }
}
