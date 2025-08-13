using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScript : MonoBehaviour
{
    [SerializeField] private GameObject cameraPositionsGameObject;
    [SerializeField] private int cameraPositionsCount;
    [SerializeField] private int cameraPositionIndex = 0;

    [SerializeField] private Vector3[] cameraPositions;

    [SerializeField] private float cameraMoveSpeed = 5.0f;
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
    }

    // Update is called once per frame
    void Update()
    {
        // Pans towards new camera position
        this.transform.position = Vector3.Lerp(this.transform.position, cameraPositions[cameraPositionIndex], cameraMoveSpeed * Time.deltaTime);
    }

    // Updates Camera Index and protect it from being out of bounds
    public void UpdateCamera(bool isProgressing)
    {
        // Increment Index if Increment is true, Decrement otherwise
        cameraPositionIndex = cameraPositionIndex + (isProgressing ? 1 : -1);

        // Index out of bounds protection
        if (cameraPositionIndex < 0)
            cameraPositionIndex = 0;
        else if (cameraPositionIndex >= cameraPositionsCount)
            cameraPositionIndex = cameraPositionsCount - 1;
    }
    // Sets Camera to the position at Index, if Out of Bounds, then function does nothing
    public void SetCameraPosition(int Index)
    {
        // If Index is out of bounds
        if (Index < 0 || Index >= cameraPositionsCount)
            return;

        cameraPositionIndex = Index;
    }
}
