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

        EnableObstaclesInLevelFrame();
        SetRespawnPoint();
        mainCameraScript.ProgressCamera(isProgressing);
    }
    // Enables the Obstacles of levels
    // currentGameLevel - levelObstacleGenerationFrameSize
    //          to
    // currentGameLevel + levelObstacleGenerationFrameSize
    public void EnableObstaclesInLevelFrame ()
    {
        for (int i = currentGameLevel - levelObstacleGenerationFrameSize; i < currentGameLevel + levelObstacleGenerationFrameSize + 1; i ++)
        {
            // Index out of bounds
            if (i < 0 || i >= totalLevelCount)
                continue;
            // Level already has Obstacles
            if (levelHasObstacles[i])
                continue;

            EnableObstaclesOfLevel(i);
        }
        // Check Levels just outside of levelObstacleGenerationFrame boundary
        int indexLowerThanFrame = currentGameLevel - levelObstacleGenerationFrameSize - 1;
        int indexHigherThanFrame = currentGameLevel + levelObstacleGenerationFrameSize + 1;
        if (indexLowerThanFrame >= 0 && levelHasObstacles[indexLowerThanFrame])
            DisableObstaclesOfLevel(indexLowerThanFrame);
        if (indexHigherThanFrame < totalLevelCount && levelHasObstacles[indexHigherThanFrame])
            DisableObstaclesOfLevel(indexHigherThanFrame);
    }
    // Fill Level Index with Pre-set Obstacles
    public void EnableObstaclesOfLevel (int Index)
    {
        levelHasObstacles[Index] = true;
        // Enable Obstacles in Level Index
        foreach (Transform childTransform in levelObstaclesCollection[Index].transform)
        {
            // Enables current Obstacle
            childTransform.gameObject.SetActive(true);
        }
    }
    // Clean Level Index of Pre-set Obstacles
    public void DisableObstaclesOfLevel (int Index)
    {
        levelHasObstacles[Index] = false;
        // Disable all Obstacles in Level Index
        foreach (Transform childTransform in levelObstaclesCollection[Index].transform)
        {
            // Reset Position of Obstacles to Instation State
            MonoBehaviourWithReset monoBehaviourWithReset;
            if (!childTransform.gameObject.TryGetComponent<MonoBehaviourWithReset>(out monoBehaviourWithReset))
            {
                Debug.Log("childTransform DOES NOT have MonoBehaviourWithReset component! - from DisableObstaclesOfLevel() in GameManagerScript");
                continue;
            }
            monoBehaviourWithReset.ResetToInstantiation();

            // Disable current Obstacle
            childTransform.gameObject.SetActive(false);
        }
    }
    // Reset Level Index Obstacles to their Original State upon Instantiation
    public void ResetLevelObstacles (int Index)
    {
        // Reset all Obstacles in Level Index
        foreach (Transform childTransform in levelObstaclesCollection[Index].transform)
        {
            MonoBehaviourWithReset monoBehaviourWithReset;
            if (!childTransform.gameObject.TryGetComponent<MonoBehaviourWithReset>(out monoBehaviourWithReset))
            {
                Debug.Log("childTransform DOES NOT have MonoBehaviourWithReset component! - from ResetLevelObstacles() in GameManagerScript");
                continue;
            }
            monoBehaviourWithReset.ResetToInstantiation();
        }
    }
    // Set the Player's respawn point to one of the points in the Level
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

    // Set the Player to Level Index
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

        EnableObstaclesInLevelFrame();
        SetRespawnPoint();
        mainCameraScript.SetCameraPosition(Index);
    }
}
