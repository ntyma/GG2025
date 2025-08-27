using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject playerGameObject;
    [SerializeField] private GameObject guideGameObject;
    [SerializeField] private GameObject cameraGameObject;
    [SerializeField] private GameObject levelProgressionTriggersGameObject;
    [SerializeField] private GameObject cameraPositionsGameObject;
    [SerializeField] private GameObject levelObstaclesGameObject;
    [SerializeField] private GameObject levelRespawnPointsGameObject;
    [SerializeField] private GameObject levelForegroundTilemapsGameObject;
    [SerializeField] private GameObject levelPlayerMemoryTilemapsGameObject;
    private MainCameraScript mainCameraScript;

    [SerializeField] private PlayerScript playerScript;
    private Health playerHealthScript;
    [Header("Level Statistics")]
    private int currentGameLevel = 0;
    public int furthestGameLevel = 0;
    public int totalLevelCount = 0;
    [SerializeField] private GameObject[] levelObstaclesCollection;
    [SerializeField] private GameObject[] levelRespawnPointsCollection;
    [SerializeField] private bool[] levelHasObstacles;
    [SerializeField] private int levelObstacleGenerationFrameSize = 2;
    [SerializeField] private GameObject[] levelPlayerMemoryTilemapsCollection;
    [SerializeField] private GameObject[] levelForegroundTilemapsCollection;

    public bool isForwardRoute = true;

    [SerializeField] private GameObject backwardRouteScrollingLightWall;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("starting game");
        // Check if every Game Manager Collection is consistent with the number of levels
        if  (
                (levelObstaclesGameObject.transform.childCount != levelRespawnPointsGameObject.transform.childCount) ||
                (levelObstaclesGameObject.transform.childCount != levelForegroundTilemapsGameObject.transform.childCount) ||
                (levelObstaclesGameObject.transform.childCount != levelPlayerMemoryTilemapsGameObject.transform.childCount) ||
                (levelObstaclesGameObject.transform.childCount != levelProgressionTriggersGameObject.transform.childCount+1) ||
                (levelObstaclesGameObject.transform.childCount != cameraPositionsGameObject.transform.childCount)
            )
        {
            Debug.Log("A Level Component is Missing! - from Start() in GameManagerScript\n" +
                "There are " + levelObstaclesGameObject.transform.childCount + " sets of Level Obstacles... (CLICK FOR MORE INFO)\n" +
                "There are " + levelRespawnPointsGameObject.transform.childCount + " sets of Level Respawn Points\n" +
                "There are " + levelForegroundTilemapsGameObject.transform.childCount + " sets of Player Foreground Tilemaps\n" +
                "There are " + levelPlayerMemoryTilemapsGameObject.transform.childCount + " sets of Player Memory Tilemaps\n" +
                "There are " + cameraPositionsGameObject.transform.childCount + " sets of Level Camera Positions" +
                "There are " + levelProgressionTriggersGameObject.transform.childCount + " sets of Level Progression Triggers (this should be 1 less than other Values)\n");
        }

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
        // Load in ALL level specific Foreground Tilemaps
        levelForegroundTilemapsCollection = new GameObject[totalLevelCount];
        i = 0;
        foreach (Transform childTransform in levelForegroundTilemapsGameObject.transform)
        {
            levelForegroundTilemapsCollection[i++] = childTransform.gameObject;
        }
        // Load in ALL level specific Player Memory Tilemaps
        levelPlayerMemoryTilemapsCollection = new GameObject[totalLevelCount];
        i = 0;
        foreach (Transform childTransform in levelPlayerMemoryTilemapsGameObject.transform)
        {
            levelPlayerMemoryTilemapsCollection[i++] = childTransform.gameObject;
        }
        backwardRouteScrollingLightWall.SetActive(false);

        if (SaveManager.loadingData)
        {
            furthestGameLevel = SaveManager.levelLoading;
            currentGameLevel = SaveManager.levelLoading;
            Debug.Log("loaded save data! Current level: " + furthestGameLevel);
        }
        //to resume time in case it was stopped by a previous pause
        Time.timeScale = 1f;
        Debug.Log(furthestGameLevel);
        SelectLevel(furthestGameLevel);
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
        currentGameLevel = Mathf.Clamp((currentGameLevel + (isProgressing ? 1 : -1)), 0, totalLevelCount - 1);
        // Update Furthest Level the Player has gotten
        if (isForwardRoute && currentGameLevel > furthestGameLevel)
            furthestGameLevel = currentGameLevel;
        else if (!isForwardRoute && currentGameLevel < furthestGameLevel)
            furthestGameLevel = currentGameLevel;

        EnableObstaclesInLevelFrame();
        SetRespawnPoint();
        playerScript.SetPlayerMemoryTilemap(levelPlayerMemoryTilemapsCollection[currentGameLevel].GetComponent<Tilemap>());
        mainCameraScript.ProgressCamera(isProgressing);


        SaveData data = new SaveData
        {
            playerLevel = furthestGameLevel,
        };
        SaveManager.SaveGame(data);
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
        /*int indexLowerThanFrame = currentGameLevel - levelObstacleGenerationFrameSize - 1;
        int indexHigherThanFrame = currentGameLevel + levelObstacleGenerationFrameSize + 1;
        if (indexLowerThanFrame >= 0 && levelHasObstacles[indexLowerThanFrame])
            DisableObstaclesOfLevel(indexLowerThanFrame);
        if (indexHigherThanFrame < totalLevelCount && levelHasObstacles[indexHigherThanFrame])
            DisableObstaclesOfLevel(indexHigherThanFrame);*/

        for (int i = 0; i < currentGameLevel - levelObstacleGenerationFrameSize; i ++)
        {
            if (levelHasObstacles[i])
                DisableObstaclesOfLevel(i);
        }
        for (int i = currentGameLevel + levelObstacleGenerationFrameSize + 1; i < totalLevelCount; i++)
        {
            if (levelHasObstacles[i])
                DisableObstaclesOfLevel(i);
        }

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

        // Enable corresponding Player Memory Tilemap
        levelForegroundTilemapsCollection[Index].SetActive(true);
        levelPlayerMemoryTilemapsCollection[Index].SetActive(true);
    }
    // Clean Level Index of Pre-set Obstacles
    public void DisableObstaclesOfLevel (int Index)
    {
        levelHasObstacles[Index] = false;
        // Disable all ForwardRoute Obstacles in Level Index
        foreach (Transform childTransform in levelObstaclesCollection[Index].transform.GetChild(0))
        {
            // Reset Position of Obstacles to Instantiation State
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

        // Disable corresponding Player Memory Tilemap
        levelForegroundTilemapsCollection[Index].SetActive(false);
        levelPlayerMemoryTilemapsCollection[Index].SetActive(false);
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

        // Reset corresponding Player Memory Tilemap
        PlayerMemoryTilemapScript playerMemoryTilemapScript;
        if (!(levelPlayerMemoryTilemapsCollection[Index].TryGetComponent<PlayerMemoryTilemapScript>(out playerMemoryTilemapScript)))
        {
            Debug.Log("Player Memory Tilemap #" + Index + " DOES NOT have PlayerMemoryTilemapScript - from ResetLevelObstacles() in GameManagerScript");
            return;
        }
        playerMemoryTilemapScript.ResetToInstantiation();

        // Set Level to furthestGameLevel
        SelectLevel(furthestGameLevel);
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
            levelRespawnPointsCollection[furthestGameLevel].transform.GetChild(0).transform:
            levelRespawnPointsCollection[furthestGameLevel].transform.GetChild(1).transform;
    }

    // Set the Player to Level Index
    public void SelectLevel(int Index)
    {
        // If Index is out of bounds
        if (Index < 0 || Index >= totalLevelCount)
        {
            Debug.Log("Index is OUT OF BOUNDS - in SelectLevel() from GameManagerScript");
            return;
        }
        currentGameLevel = Index;
        // Spawn Player at appropriate respawn position
        Vector3 spawnPosition = levelRespawnPointsCollection[Index].transform.GetChild((isForwardRoute ? 0 : 1)).transform.position;
        playerGameObject.transform.position = spawnPosition;
        guideGameObject.transform.position = spawnPosition;

        EnableObstaclesInLevelFrame();
        guideGameObject.SetActive(isForwardRoute);
        SetRespawnPoint();
        playerScript.SetPlayerMemoryTilemap(levelPlayerMemoryTilemapsCollection[Index].GetComponent<Tilemap>());
        playerScript.SetRoute(isForwardRoute);
        mainCameraScript.SetCameraPosition(Index);

        backwardRouteScrollingLightWall.SetActive(!isForwardRoute);
        SetScrollingLightWallPosition(spawnPosition);
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

        guideGameObject.SetActive(isForwardRoute);
        EnableObstaclesInLevelFrame();
        SetRespawnPoint();
        playerScript.SetRoute(isForwardRoute);
        backwardRouteScrollingLightWall.SetActive(!isForwardRoute);

        SetScrollingLightWallPosition(playerGameObject.transform.position);

        // No need to update which Memory Tileset the Player is manipulating
        // SwapRoute would always place the player in the same room
        if (respawnPlayer)
        {
            playerHealthScript.Respawn();
            //mainCameraScript.SetCameraPosition(currentGameLevel);
        } 
    }

    public void SetScrollingLightWallPosition(Vector3 spawnPosition)
    {
        // Must Offset spawnPosition based on Scale of Light Wall
        backwardRouteScrollingLightWall.transform.position =
            (
                furthestGameLevel <= 16 ? 
                spawnPosition + Vector3.right * (backwardRouteScrollingLightWall.transform.localScale.x/2 + 4) 
                    :
                spawnPosition + Vector3.up * (backwardRouteScrollingLightWall.transform.localScale.y/2 + 4)
            );
    }
}
