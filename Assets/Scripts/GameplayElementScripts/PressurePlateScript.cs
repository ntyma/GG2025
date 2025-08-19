using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateScript : MonoBehaviourWithReset
{
    [SerializeField] private GameObject doorGameObject;
    private Vector3 doorStartingPosition;
    private Vector3 doorEndingPosition;
    [SerializeField] private Vector3 doorEndingPositionOffset = new Vector3(0.0f, 5.0f, 0.0f);
    public float pressureStatus = 0.0f;
    [SerializeField] private float pressureTarget = 100.0f;
    [SerializeField] private float pressureFillRate = 50.0f;
    [SerializeField] private float pressureDrainRate = 10.0f;

    public bool pressureIsActivated = false;

    // Reset Component Variable
    private float pressureStatusInitial;
    private float pressureTargetInitial;
    private float pressureFillRateInitial;
    private float pressureDrainRateInitial;
    private bool pressureIsActivatedInitial;

    // Start is called before the first frame update
    void Start()
    {
        doorStartingPosition = doorGameObject.transform.position;
        doorEndingPosition = doorGameObject.transform.position + doorEndingPositionOffset;

        // Record Instantiation Variables
        pressureStatusInitial = pressureStatus;
        pressureTargetInitial = pressureTarget;
        pressureFillRateInitial = pressureFillRate;
        pressureDrainRateInitial = pressureDrainRate;
        pressureIsActivatedInitial = pressureIsActivated;
}

    // Update is called once per frame
    void Update()
    {
        if (pressureIsActivated)
            return;

        pressureStatus = Mathf.Clamp(pressureStatus - pressureDrainRate * Time.deltaTime, 0.0f, pressureTarget);

        doorGameObject.transform.position = Vector3.Lerp(doorStartingPosition, doorEndingPosition, pressureStatus*0.01f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!(collision.gameObject.tag == "Player") && !(collision.gameObject.tag == "Enemy"))
            return;

        pressureStatus = Mathf.Clamp(pressureStatus + pressureFillRate * Time.deltaTime, 0.0f, pressureTarget);
    }
   
    public override void ResetToInstantiation()
    {
        // Debug.Log("ResetToInstantiation() OVERRIDE - from ResetToInstantiation() in PressurePlateScript");

        pressureStatus = pressureStatusInitial;
        pressureTarget = pressureTargetInitial;
        pressureFillRate = pressureFillRateInitial;
        pressureDrainRate = pressureDrainRateInitial;
        pressureIsActivated = pressureIsActivatedInitial;
    }
}
