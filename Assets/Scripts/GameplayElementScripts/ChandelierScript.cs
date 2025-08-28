using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChandelierScript : MonoBehaviourWithReset
{
    [SerializeField] private PolygonCollider2D chandelierHitbox;
    [SerializeField] private MainCameraScript mainCameraScript;
    [SerializeField] private GameObject chandelierPositions;

    private Vector3 startingPosition;
    private Vector3 endingPosition;
    private bool isInLight = false;
    private bool isFalling = false;
    private bool isRetracting = false;
    private bool hasActivated = false;

    [SerializeField] private float fallingRate = 3.0f;
    [SerializeField] private float fallingAcceleration = 2.0f;
    [SerializeField] private float currentFallingAccerleration = 0.0f;
    [SerializeField] private float retractingRate = 2.0f;
    [SerializeField] private float Damage = 5.0f;
    // Awake is called before the first frame update and before Start
    void Awake()
    {
        isInLight = false;
        isFalling = false;
        isRetracting = false;
        startingPosition = chandelierPositions.transform.GetChild(0).position;
        endingPosition = chandelierPositions.transform.GetChild(1).position;
        this.transform.position = startingPosition;

        currentFallingAccerleration = 0.0f;

        if (mainCameraScript == null)
            Debug.Log("This Chandelier CAN'T Shake Camera! - from Awake() in ChandelierScript");
    }

    // Update is called once per frame
    void Update()
    {
        if (isInLight)
        {
            currentFallingAccerleration = 0.0f;
            return;
        }

        if (isFalling)
        {
            currentFallingAccerleration = Mathf.Lerp(currentFallingAccerleration, fallingAcceleration, 5.0f * Time.deltaTime);
            this.transform.position = Vector3.MoveTowards
                (
                    this.transform.position,
                    endingPosition,
                    (fallingRate + currentFallingAccerleration) * Time.deltaTime
                );

            if (this.transform.position == endingPosition)
                RetractChandelier();
        }
        else if (isRetracting)
        {
            this.transform.position = Vector3.MoveTowards
                (
                    this.transform.position,
                    startingPosition,
                    retractingRate * Time.deltaTime
                );

            if (this.transform.position == startingPosition)
            {
                hasActivated = false;
                isRetracting = false;
            }
        }
    }
    public void ActivateChandelier()
    {
        if (isRetracting || isInLight)
            return;

        hasActivated = true;
        isFalling = true;
    }

    private float postImpactRetractionDelay = 1.0f;
    private Coroutine SetIsRetractingCoroutine;
    public void RetractChandelier()
    {
        currentFallingAccerleration = 0.0f;
        isFalling = false;

        if (mainCameraScript != null)
            mainCameraScript.CameraShake();

        if (SetIsRetractingCoroutine != null)
            StopCoroutine(SetIsRetractingCoroutine);

        StartCoroutine(SetIsRetracting());
    }
    private IEnumerator SetIsRetracting()
    {
        float Timer = 0.0f;
        while (Timer <= postImpactRetractionDelay)
        {
            yield return null;

            Timer = Timer + Time.deltaTime;
        }
        isRetracting = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player" || !isFalling || isInLight)
            return;

        Health healthScript;
        if (!(collision.gameObject.TryGetComponent<Health>(out healthScript)))
        {
            Debug.Log("Player DOES NOT have a HealthScript Component! - From OnCollisionEnter2D in Chandelier Script");
            return;
        }
        if (Damage > 1)
            healthScript.TakeDamage(Damage);
    }
    public float activationDelay = 0.2f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !hasActivated)
        {
            Invoke("ActivateChandelier", activationDelay);
        }
        else if (collision.gameObject.tag == "Light")
        {
            isInLight = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
            isInLight = false;
    }
    public override void ResetToInstantiation()
    {
        // Set Chandelier to Starting Position
        this.transform.position = startingPosition;
        isFalling = false;
        isRetracting = false;
        isInLight = false;
        hasActivated = false;
    }
}
