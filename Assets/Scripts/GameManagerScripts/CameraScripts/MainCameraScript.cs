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
    [SerializeField] private CameraMovementStates currentCameraState;
    private float cameraMoveSpeedCoefficient = 5.0f;

    [System.Serializable]
    public class CameraLevelSettings
    {
        public Vector3 levelCameraPosition;
        public CameraMovementStates levelCameraState;
        public float lockedToArea_xLength;
        public float lockedToArea_yLength;

        public CameraLevelSettings (Vector3 CameraPosition, CameraMovementStates CameraState = CameraMovementStates.lockedToArea, float xLength = 0.0f, float yLength = 0.0f)
        {
            this.levelCameraPosition = CameraPosition;
            this.levelCameraState = CameraState;
            this.lockedToArea_xLength = xLength;
            this.lockedToArea_yLength = yLength;
        }
    };
    [SerializeField] private CameraLevelSettings[] cameraLevelSettings;

    // Awake is called before all Start() functions
    void Awake()
    {
        cameraPositionsCount = cameraPositionsGameObject.transform.childCount;

        cameraLevelSettings = new CameraLevelSettings[cameraPositionsCount];

        // Initialize all Camera Data for Every Level
        int i = 0;
        foreach (Transform childTransform in cameraPositionsGameObject.transform)
        {
            if (!childTransform.gameObject.TryGetComponent<CameraPositionScript>(out CameraPositionScript currentCameraPositionScript))
            {
                Debug.Log("No CameraPositionScript component found! - from Start() in MainCameraScript");
                continue;
            }
            cameraLevelSettings[i++] = new CameraLevelSettings
                (
                    childTransform.position,
                    currentCameraPositionScript.levelCameraState,
                    currentCameraPositionScript.lockedToArea_xLength,
                    currentCameraPositionScript.lockedToArea_yLength
                );
        }

        // Initialize Camera to position
        //SetCameraPosition(cameraPositionIndex);
    }

    // Update is responsible for the actual smooth movement of the Camera
    void Update()
    {
        if (!isActive)
            return;
        // Camera is locked to pre-set Camera Position
        if (currentCameraState == CameraMovementStates.lockedToArea)
        {
            Vector3 currentCameraPosition = cameraLevelSettings[cameraPositionIndex].levelCameraPosition;
            Vector3 targetCameraPosition = new Vector3
                (
                    Mathf.Clamp
                    (
                        playerGameObject.transform.position.x, 
                        currentCameraPosition.x, 
                        currentCameraPosition.x + cameraLevelSettings[cameraPositionIndex].lockedToArea_xLength
                    ),
                    Mathf.Clamp
                    (
                        playerGameObject.transform.position.y, 
                        currentCameraPosition.y, 
                        currentCameraPosition.y + cameraLevelSettings[cameraPositionIndex].lockedToArea_yLength
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

        //RefreshCamera();
        currentCameraState = cameraLevelSettings[cameraPositionIndex].levelCameraState;
    }
    /*[ContextMenu("RefreshCamera()")]
    public void RefreshCamera()
    {
        if (cameraPositionIndex < 0 || cameraPositionIndex >= cameraPositionsCount)
        {
            Debug.Log("cameraPositionIndex is OUT OF BOUNDS - in RefreshCamera() from MainCameraScript");
            return;
        }
        currentCameraState = cameraLevelSettings[cameraPositionIndex].levelCameraState;
    }*/

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
        this.transform.position = cameraLevelSettings[cameraPositionIndex].levelCameraPosition;
        //RefreshCamera();
        currentCameraState = cameraLevelSettings[cameraPositionIndex].levelCameraState;
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
