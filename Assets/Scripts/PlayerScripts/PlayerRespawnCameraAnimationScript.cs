using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnCameraAnimationScript : MonoBehaviour
{
    [SerializeField] private GameObject playerSpotlightGameObject;
    [SerializeField] private SpriteMask playerSpotlightSpriteMask;
    [SerializeField] private SpriteRenderer blackBackgroundSpriteRenderer;

    [SerializeField] private float firstIncrementScale = 1.0f;
    [SerializeField] private float secondIncrementScale = 7.0f;

    // Start is called before the first frame update
    void Start()
    {
        SetAllSpriteComponents(true);
        PlayPlayerSpawnCameraAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            PlayPlayerSpawnCameraAnimation();
        if (Input.GetKeyDown(KeyCode.RightShift))
            PlayPlayerDeathCameraAnimation();
    }
    //  Enables or Disables Player Spotlight Sprite Mask and Black Background based on Input
    public void SetAllSpriteComponents (bool Input = true)
    {
        playerSpotlightSpriteMask.enabled = Input;
        blackBackgroundSpriteRenderer.enabled = Input;
    }
    public float firstIncrementTime = 0.6f;
    public float secondIncrementTime = 1.0f;
    private Coroutine PlayerSpawnAnimationCoroutine;
    public void PlayPlayerSpawnCameraAnimation(float firstIncrementTime = 0.6f, float secondIncrementTime = 1.0f)
    {
        if (PlayerSpawnAnimationCoroutine != null)
        {
            StopCoroutine(PlayerSpawnAnimationCoroutine);
            //SetAllSpriteComponents(false);
        }
        else if (PlayerDeathCameraAnimationCoroutine != null)
        {
            StopCoroutine(PlayerDeathCameraAnimationCoroutine);
            //SetAllSpriteComponents(true);
        }

        this.firstIncrementTime = firstIncrementTime;
        this.secondIncrementTime = secondIncrementTime;
        PlayerSpawnAnimationCoroutine = StartCoroutine(PlayPlayerSpawnCameraAnimationCoroutine());
    }
    private IEnumerator PlayPlayerSpawnCameraAnimationCoroutine()
    {
        //SetAllSpriteComponents(true);

        playerSpotlightGameObject.transform.localScale = Vector3.zero;
        float Timer = 0.0f;
        float currentScale = 0.0f;
        while (Timer <= firstIncrementTime)
        {
            yield return null;
            currentScale = Mathf.Lerp(currentScale, firstIncrementScale, 7.0f * Time.deltaTime);
            playerSpotlightGameObject.transform.localScale = new Vector3(1.0f, 1.0f, 0.0f) * currentScale;
            Timer = Timer + Time.deltaTime;
        }
        Timer = 0.0f;
        while (Timer <= secondIncrementTime)
        {
            yield return null;
            currentScale = Mathf.Lerp(currentScale, secondIncrementScale, 2.0f * Time.deltaTime);
            playerSpotlightGameObject.transform.localScale = new Vector3(1.0f, 1.0f, 0.0f) * currentScale;
            Timer = Timer + Time.deltaTime;
        }
        currentScale = secondIncrementScale;
        playerSpotlightGameObject.transform.localScale = new Vector3(1.0f, 1.0f, 0.0f) * currentScale;

        //SetAllSpriteComponents(false);
    }

    public float initialDelay = 0.5f;
    public float deathFadeoutTime = 1.0f;
    private Coroutine PlayerDeathCameraAnimationCoroutine;
    public void PlayPlayerDeathCameraAnimation(float initialDelay = 0.5f, float deathFadeoutTime = 1.0f)
    {
        if (PlayerDeathCameraAnimationCoroutine != null)
        {
            StopCoroutine(PlayerDeathCameraAnimationCoroutine);
            //SetAllSpriteComponents(false);
        }
        else if (PlayerSpawnAnimationCoroutine != null)
        {
            StopCoroutine(PlayerSpawnAnimationCoroutine);
            //SetAllSpriteComponents(false);
        }

        this.initialDelay = initialDelay;
        this.deathFadeoutTime = deathFadeoutTime;
        PlayerDeathCameraAnimationCoroutine = StartCoroutine(PlayPlayerDeathCameraAnimationCoroutine());
    }
    private IEnumerator PlayPlayerDeathCameraAnimationCoroutine()
    {
        //SetAllSpriteComponents(true);

        float currentScale = secondIncrementScale;
        playerSpotlightGameObject.transform.localScale = new Vector3(1.0f, 1.0f, 0.0f) * currentScale;
        float Timer = 0.0f;
        while (Timer <= initialDelay)
        {
            yield return null;
            Timer = Timer + Time.deltaTime;
        }
        Timer = 0.0f;
        while (Timer <= deathFadeoutTime)
        {
            yield return null;
            Timer = Timer + Time.deltaTime;
            currentScale = Mathf.Lerp(currentScale, 0.0f, 7.0f * Time.deltaTime);
            playerSpotlightGameObject.transform.localScale = new Vector3(1.0f, 1.0f, 0.0f) * currentScale;
        }
        playerSpotlightGameObject.transform.localScale = Vector3.zero;
        //SetAllSpriteComponents(false);
    }
}
