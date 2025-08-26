using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviourWithReset
{
    public Sprite keyLocked;
    public Sprite keyUnlocked;

    // Start is called before the first frame update
    [SerializeField] private CircleCollider2D keyCollider;
    [SerializeField] private BoxCollider2D lockCollider;
    [SerializeField] private SpriteRenderer keySpriteRenderer;
    [SerializeField] private SpriteRenderer keyIllumSpriteRenderer;
    [SerializeField] private SpriteRenderer lockSpriteRenderer;

    // Reset Component Variable
    private bool lockIsTriggerInitial = false;
    private Color keyColorInitial;
    private Color lockColorInitial;

    // Awake is called before the first frame update and before Start
    void Awake()
    {
        // Record Instantiation Variables
        lockIsTriggerInitial = lockCollider.isTrigger;
        keyColorInitial = keySpriteRenderer.color;
        lockColorInitial = lockSpriteRenderer.color;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        // Player collects the Key
        // Unlock the the door
        if (!lockCollider.isTrigger)
            AudioManager.instance.Play("SwitchOn");
        lockCollider.isTrigger = true;
        keySpriteRenderer.sprite = keyUnlocked;
        keyIllumSpriteRenderer.sprite = keyUnlocked;
        //keySpriteRenderer.color = keySpriteRenderer.color - new Color(0.0f, 0.0f, 0.0f, 1.0f);
        lockSpriteRenderer.color = new Color(0.0f, 1.0f, 1.0f, 0.3f);
    }

    public override void ResetToInstantiation()
    {
        lockCollider.isTrigger = lockIsTriggerInitial;
        keySpriteRenderer.sprite = keyLocked;
        keyIllumSpriteRenderer.sprite = keyLocked;
        lockSpriteRenderer.color = lockColorInitial;
    }
}
