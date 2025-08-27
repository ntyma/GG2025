using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingLightWallScript : MonoBehaviourWithReset
{
    [SerializeField] private GameObject lightWallGameObject;
    [SerializeField] private SpriteRenderer lightWallSpriteRenderer;
    [SerializeField] private BoxCollider2D lightWallBoxCollider;
    [SerializeField] private GameObject lightWallScrollingPoints;

    [SerializeField]  private Vector3 lightWallStartPoint;
    [SerializeField]  private Vector3 lightWallEndPoint;

    [SerializeField] private float scrollingSpeed = 2.0f;
    private void Awake()
    {
        lightWallStartPoint = lightWallScrollingPoints.transform.GetChild(0).transform.position;
        lightWallEndPoint = lightWallScrollingPoints.transform.GetChild(1).transform.position;

        lightWallGameObject.transform.position = lightWallStartPoint;
    }
    private void Update()
    {
        lightWallGameObject.transform.position = Vector3.MoveTowards
            (
            lightWallGameObject.transform.position,
            lightWallEndPoint,
            scrollingSpeed * Time.deltaTime
            );
    }
    public override void ResetToInstantiation()
    {
        lightWallGameObject.transform.position = lightWallStartPoint;
    }
}
