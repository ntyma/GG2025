using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionScript : MonoBehaviour
{
    public MainCameraScript.CameraMovementStates levelCameraState = MainCameraScript.CameraMovementStates.lockedToArea;
    public float lockedToArea_xLength = 0.0f;
    public float lockedToArea_yLength = 0.0f;
}
