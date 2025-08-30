using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScript : MonoBehaviour
{
    [SerializeField] private GameObject cameraPositionsGameObject;
    [SerializeField] private GameObject playerGameObject;
    private int cameraPositionsCount;
    public bool isActive = true;

    [SerializeField] public enum CameraMovementStates
    {
        lockedToArea, followPlayer
    }

    [SerializeField] private int cameraPositionIndex = 0;
    private float cameraMoveSpeedCoefficient = 5.0f;

    [SerializeField] private GameObject currentCameraPositionGameObject;
    private CameraPositionScript currentCameraPositionsScript;
    // Awake is called before all Start() functions
    void Awake()
    {
        cameraPositionsCount = cameraPositionsGameObject.transform.childCount;
    }

    // Update is responsible for the actual smooth movement of the Camera
    void Update()
    {
        if (!isActive)
            return;
        // Camera is locked to pre-set Camera Position
        if (currentCameraPositionsScript.levelCameraState == CameraMovementStates.lockedToArea)
        {
            Vector3 currentCameraPosition = currentCameraPositionsScript.transform.position;
            Vector3 targetCameraPosition = new Vector3
                (
                    Mathf.Clamp
                    (
                        playerGameObject.transform.position.x, 
                        currentCameraPosition.x,
                        currentCameraPosition.x + currentCameraPositionsScript.lockedToArea_xLength
                    ),
                    Mathf.Clamp
                    (
                        playerGameObject.transform.position.y, 
                        currentCameraPosition.y,
                        currentCameraPosition.y + currentCameraPositionsScript.lockedToArea_yLength
                    ),
                    this.transform.position.z
                );
            // Pans towards new camera position
            this.transform.position =
            Vector3.Lerp
            (
                this.transform.position,
                targetCameraPosition,
                cameraMoveSpeedCoefficient * Time.deltaTime
            );
        }
        // Camera follows Player
        if (currentCameraPositionsScript.levelCameraState == CameraMovementStates.followPlayer)
        {
            this.transform.position = Vector3.Lerp
                (
                    this.transform.position, 
                    new Vector3
                    (
                        playerGameObject.transform.position.x + currentCameraPositionsScript.followPlayer_xOffset,
                        playerGameObject.transform.position.y + currentCameraPositionsScript.followPlayer_yOffset,
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
        // Mathf.Clamp() is for Index Out of Bounds Protection
        cameraPositionIndex = Mathf.Clamp(cameraPositionIndex + (isProgressing ? 1 : -1), 0, cameraPositionsCount - 1);

        currentCameraPositionGameObject = cameraPositionsGameObject.transform.GetChild(cameraPositionIndex).gameObject;
        currentCameraPositionsScript = currentCameraPositionGameObject.GetComponent<CameraPositionScript>();
    }

    // Sets Camera to the position at Index, if Out of Bounds, then function does nothing
    public void SetCameraPosition(int Index)
    {
        // If Index is out of bounds
        if (Index < 0 || Index >= cameraPositionsCount)
        {
            Debug.Log("Index " + Index + " is OUT OF BOUNDS - in SetCameraPosition() from MainCameraScript");
            return;
        }

        cameraPositionIndex = Index;
        currentCameraPositionGameObject = cameraPositionsGameObject.transform.GetChild(Index).gameObject;
        currentCameraPositionsScript = currentCameraPositionGameObject.GetComponent<CameraPositionScript>();

        if (currentCameraPositionsScript.levelCameraState == CameraMovementStates.lockedToArea)
        {
            this.transform.position = new Vector3
                (
                    Mathf.Clamp
                    (
                        playerGameObject.transform.position.x,
                        currentCameraPositionsScript.transform.position.x,
                        currentCameraPositionsScript.transform.position.x + currentCameraPositionsScript.lockedToArea_xLength
                    ),
                    Mathf.Clamp
                    (
                        playerGameObject.transform.position.y,
                        currentCameraPositionsScript.transform.position.y,
                        currentCameraPositionsScript.transform.position.y + currentCameraPositionsScript.lockedToArea_yLength
                    ),
                    this.transform.position.z
                );
        }
        else if (currentCameraPositionsScript.levelCameraState == CameraMovementStates.followPlayer)
        {
            this.transform.position = new Vector3
                (
                    playerGameObject.transform.position.x + currentCameraPositionsScript.followPlayer_xOffset,
                    playerGameObject.transform.position.y + currentCameraPositionsScript.followPlayer_yOffset,
                    this.transform.position.z
                );
        }
        else
        {
            Debug.Log("WHAT DO YOU MEAN THERE'S ANOTHER CAMERA STATE?! - From SetCameraPosition() in MainCameraScript");
            this.transform.position = currentCameraPositionsScript.transform.position;
        }
        
    }

    // Camera Shake
    private float cameraShakeDuration = 0.1f;
    private float cameraShakeIntensity = 0.02f;
    private Coroutine shakeCoroutine;
    public void CameraShake(float cameraShakeDuration = 0.1f, float cameraShakeIntensity = 0.02f)
    {
        this.cameraShakeDuration = cameraShakeDuration;
        this.cameraShakeIntensity = cameraShakeIntensity;

        // Don't shake the Camera more than once at a time
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(CameraShakeCoroutine());
    }
    // Thanks for giving me a start Google AI, I can't believe I just typed  that
    private IEnumerator CameraShakeCoroutine()
    {
        float currentShakeDuration = cameraShakeDuration;
        float currentCameraShakeIntensity = cameraShakeIntensity;
        while (currentShakeDuration > 0)
        {
            // Calculate random offset
            Vector3 randomOffset = Random.insideUnitSphere * currentCameraShakeIntensity;

            // Apply offset to camera position
            this.transform.position = this.transform.position + randomOffset;

            // Reduce shake magnitude over time
            currentCameraShakeIntensity = Mathf.Lerp(currentCameraShakeIntensity, 0.0f, Time.deltaTime);

            // Reduce duration
            currentShakeDuration = currentShakeDuration - Time.deltaTime;

            yield return null; // Wait for the next frame
        }
    }
}
