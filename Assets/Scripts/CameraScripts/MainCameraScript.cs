using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScript : MonoBehaviour
{
    [SerializeField] private GameObject cameraPositionsGameObject;
    [SerializeField] private int cameraPositionsCount;
    [SerializeField] private int cameraPositionIndex = 0;
    [SerializeField] private GameObject playerGameObject;

    public bool isActive = true;

    private Vector3[] cameraPositions;
    [SerializeField] private enum CameraMovementStates
    {
        Static, followPlayer, lockedToHorizontal, lockedToVertical
    }
    [SerializeField] private CameraMovementStates currentCameraState;
    [SerializeField] private CameraMovementStates[] individualSceneSettings;

    [SerializeField] private float cameraMoveSpeedCoefficient = 5.0f;

    private float cameraCorrectionDistance = 0.01f;
    private bool stateStatic_isPanning = true;

    // Start is called before the first frame update
    void Start()
    {
        cameraPositionsCount = cameraPositionsGameObject.transform.childCount;
        cameraPositions = new Vector3[cameraPositionsCount];

        int i = 0;
        foreach (Transform childTransform in cameraPositionsGameObject.transform)
        {
            cameraPositions[i++] = childTransform.position;
        }

        // Defaults everything to the Static state
        individualSceneSettings = new CameraMovementStates[cameraPositionsCount];
        // Specific overrides
        individualSceneSettings[1] = CameraMovementStates.followPlayer;
        individualSceneSettings[5] = CameraMovementStates.followPlayer;

        // Initialize Camera to position
        SetCameraPosition(cameraPositionIndex);
    }

    // Update is responsible for the actual smooth movement of the Camera
    void Update()
    {
        if (!isActive)
            return;
        // Pan Camera to new pre-set Camera Position
        if (currentCameraState == CameraMovementStates.Static && stateStatic_isPanning)
        {
            // Pans towards new camera position
            this.transform.position = 
                Vector3.Lerp
                (
                    this.transform.position, 
                    cameraPositions[cameraPositionIndex], 
                    cameraMoveSpeedCoefficient * Time.deltaTime
                );
            // Snap camera position to the target position if it's close enough, determined by cameraCorrectionDistance
            if (Vector3.Distance(this.transform.position, cameraPositions[cameraPositionIndex]) <= cameraCorrectionDistance)
            {
                this.transform.position = cameraPositions[cameraPositionIndex];
                stateStatic_isPanning = false;
            }
        }
        // Camera follows Player
        if (currentCameraState == CameraMovementStates.followPlayer)
        {
            this.transform.position = Vector3.Lerp
                (
                    this.transform.position, 
                    new Vector3
                    (
                        playerGameObject.transform.position.x, 
                        playerGameObject.transform.position.y+1.0f, 
                        this.transform.position.z
                    ), 
                    cameraMoveSpeedCoefficient * Time.deltaTime
                );
        }   
    }

    // Increments or Decrements Camera Index and protect it from being out of bounds,
    // then updates camera to new position
    public void ProgressCamera(bool isProgressing)
    {
        // Increment Index if Increment is true, Decrement otherwise
        cameraPositionIndex = cameraPositionIndex + (isProgressing ? 1 : -1);

        // Index out of bounds protection
        if (cameraPositionIndex < 0)
            cameraPositionIndex = 0;
        else if (cameraPositionIndex >= cameraPositionsCount)
            cameraPositionIndex = cameraPositionsCount - 1;

        RefreshCamera();
    }
    [ContextMenu("RefreshCamera()")]
    public void RefreshCamera()
    {
        if (cameraPositionIndex < 0 || cameraPositionIndex >= cameraPositionsCount)
        {
            Debug.Log("cameraPositionIndex is OUT OF BOUNDS - in RefreshCamera() from MainCameraScript");
            return;
        }
        currentCameraState = individualSceneSettings[cameraPositionIndex];
        if (currentCameraState == CameraMovementStates.Static)
            stateStatic_isPanning = true;
    }

    // Sets Camera to the position at Index, if Out of Bounds, then function does nothing
    public void SetCameraPosition(int Index)
    {
        // If Index is out of bounds
        if (Index < 0 || Index >= cameraPositionsCount)
        {
            Debug.Log("Index is OUT OF BOUNDS - in SetCameraPosition() from MainCameraScript");
            return;
        }

        cameraPositionIndex = Index;
        this.transform.position = cameraPositions[cameraPositionIndex];
        RefreshCamera();
    }
}
