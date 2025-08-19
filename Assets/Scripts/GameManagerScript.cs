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
    public int totalLevelCount = 0;
    [SerializeField] private GameObject[] levelObstaclesCollection;
    [SerializeField] private GameObject[] levelRespawnPointsCollection;
    [SerializeField] private bool[] levelHasObstacles;
    [SerializeField] private int levelObstacleGenerationFrameSize = 2;

    public bool isForwardRoute = true;

    // Start is called before the first frame update
    void Start()
    {
        if (!cameraGameObject.TryGetComponent<MainCameraScript>(out mainCameraScript))
            Debug.Log("camera Game Object DOES NOT HAVE a MainCameraScript Component! - from Start() in GameManagerScript");

        totalLevelCount = levelObstaclesGameObject.transform.childCount;

        // Load in ALL Per-Level Obstacles
        levelObstaclesCollection = new GameObject[totalLevelCount];
        int i = 0;
        foreach (Transform childTransform in levelObstaclesGameObject.transform)
        {
            levelObstaclesCollection[i++] = childTransform.gameObject;
        }
        // Generate a list of booleans that track whether not level i has obstacles
        levelHasObstacles = new bool[totalLevelCount];
        // Load in ALL Per-Level Respawn Points
        levelRespawnPointsCollection = new GameObject[totalLevelCount];
        i = 0;
        foreach (Transform childTransform in levelRespawnPointsGameObject.transform)
        {
            levelRespawnPointsCollection[i++] = childTransform.gameObject;
        }

        SelectLevel(currentGameLevel);
    }

    public void ProgressLevel (bool isProgressing)
    {
        currentGameLevel = currentGameLevel + (isProgressing ? 1 : -1);

        // Index out of bounds protection
        if (currentGameLevel < 0)
            currentGameLevel = 0;
        else if (currentGameLevel >= totalLevelCount)
            currentGameLevel = totalLevelCount - 1;

        FillLevelWithObstacles();
        SetRespawnPoint();
        mainCameraScript.ProgressCamera(isProgressing);
    }
    // Fill levels
    // currentGameLevel - levelObstacleGenerationFrameSize
    //          to
    // currentGameLevel + levelObstacleGenerationFrameSize
    // with Obstacles
    public void FillLevelWithObstacles ()
    {
        for (int i = currentGameLevel - levelObstacleGenerationFrameSize; i < currentGameLevel + levelObstacleGenerationFrameSize + 1; i ++)
        {
            // Index out of bounds
            if (i < 0 || i >= totalLevelCount)
                continue;
            // Level already has Obstacles
            if (levelHasObstacles[i])
                continue;

            levelHasObstacles[i] = true;
            // Generate Obstacles
            // levelObstaclesCollection[i];
        }
        // Check Levels just outside of levelObstacleGenerationFrame boundary
        int indexLowerThanFrame = currentGameLevel - levelObstacleGenerationFrameSize - 1;
        int indexHigherThanFrame = currentGameLevel + levelObstacleGenerationFrameSize + 1;
        if (indexLowerThanFrame >= 0 && levelHasObstacles[indexLowerThanFrame])
            CleanLevelOfObstacles(indexLowerThanFrame);
        if (indexHigherThanFrame < totalLevelCount && levelHasObstacles[indexHigherThanFrame])
            CleanLevelOfObstacles(indexHigherThanFrame);
    }
    public void CleanLevelOfObstacles (int Index)
    {
        levelHasObstacles[Index] = false;
        // Clear all Obstacles in Level Index

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
        if (Index < 0 || Index >= totalLevelCount)
        {
            Debug.Log("Index is OUT OF BOUNDS - in SelectLevel() from GameManagerScript");
            return;
        }

        // Spawn Player at appropriate respawn position
        Vector3 spawnPosition = levelRespawnPointsCollection[currentGameLevel].transform.GetChild((isForwardRoute ? 0 : 1)).transform.position;
        playerGameObject.transform.position = spawnPosition;
        guideGameObject.transform.position = spawnPosition;

        FillLevelWithObstacles();
        SetRespawnPoint();
        mainCameraScript.SetCameraPosition(Index);
    }
}
