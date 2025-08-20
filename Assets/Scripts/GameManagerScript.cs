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

    private Health playerHealthScript;
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
            Debug.Log("Camera Game Object DOES NOT HAVE a MainCameraScript Component! - from Start() in GameManagerScript");

        totalLevelCount = levelObstaclesGameObject.transform.childCount;
        if (!(playerGameObject.TryGetComponent<Health>(out playerHealthScript)))
            Debug.Log("Player GameObject does not have a Health Component! - from Start() in GameManagerScript");

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
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            SwapRoute();
        if (Input.GetKeyDown(KeyCode.R))
            playerHealthScript.Respawn();
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
        // Enable ForwardRoute or BackwardRoute Obstacles in Level Index
        foreach (Transform childTransform in levelObstaclesCollection[Index].transform.GetChild((isForwardRoute ? 0 : 1)))
        {
            // Enables current Obstacle
            childTransform.gameObject.SetActive(true);
        }
        // Enable and then Disable Obstacles of the OTHER Route to record Instatiation Variables
        // Because a Disabled GameObject's is set to (0,0,0) upon Game Start, we need to temporarily
        // activate them to record Starting Positions of Game Objects
        foreach (Transform childTransform in levelObstaclesCollection[Index].transform.GetChild((isForwardRoute ? 1 : 0)))
        {
            // Enables current Obstacle
            childTransform.gameObject.SetActive(true);
            // Awake() of the current Game Object will be called between these two SetActive() calls
            // enabling the Recording of Position Instantiation Variables
            childTransform.gameObject.SetActive(false);
        }
    }
    // Clean Level Index of Pre-set Obstacles
    public void DisableObstaclesOfLevel (int Index)
    {
        levelHasObstacles[Index] = false;
        // Disable all ForwardRoute Obstacles in Level Index
        foreach (Transform childTransform in levelObstaclesCollection[Index].transform.GetChild(0))
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
        // Disable all BackwardRoute Obstacles in Level Index
        foreach (Transform childTransform in levelObstaclesCollection[Index].transform.GetChild(1))
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
    // Reset Level Index's ForwardRoute or BackwardRoute Obstacles to their Original State upon Instantiation
    public void ResetLevelObstacles (int Index)
    {
        // Reset all Obstacles in Level Index
        foreach (Transform childTransform in levelObstaclesCollection[Index].transform.GetChild((isForwardRoute ? 0 : 1)))
        {
            MonoBehaviourWithReset monoBehaviourWithReset;
            if (!childTransform.gameObject.TryGetComponent<MonoBehaviourWithReset>(out monoBehaviourWithReset))
            {
                Debug.Log("childTransform DOES NOT have MonoBehaviourWithReset component! - from ResetLevelObstacles() in GameManagerScript");
                continue;
            }
            monoBehaviourWithReset.ResetToInstantiation();
            childTransform.gameObject.SetActive(true);
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

    [ContextMenu("SwapRoute()")]
    // Switches the Route between ForwardRoute and BackwardRoute
    public void SwapRoute (bool respawnPlayer = true)
    {
        isForwardRoute = !isForwardRoute;

        for (int i = 0; i < totalLevelCount; i ++)
        {
            if (!levelHasObstacles[i])
                continue;
            // Level has Obstacles, so Disable them
            DisableObstaclesOfLevel(i);
        }

        EnableObstaclesInLevelFrame();
        SetRespawnPoint();

        if (respawnPlayer)
            playerHealthScript.Respawn();

        mainCameraScript.SetCameraPosition(currentGameLevel);
    }
}
