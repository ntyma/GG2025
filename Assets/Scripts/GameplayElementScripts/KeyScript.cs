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
    [SerializeField] private SpriteRenderer lockIlluminatedSpriteRenderer;

    // Reset Component Variable
    private bool lockIsTriggerInitial = false;
    private Color lockColorInitial;
    private Color lockIlluminatedColorInitial;
    // Awake is called before the first frame update and before Start
    void Awake()
    {
        // Record Instantiation Variables
        lockIsTriggerInitial = lockCollider.isTrigger;
        lockColorInitial = lockSpriteRenderer.color;
        lockIlluminatedColorInitial = lockIlluminatedSpriteRenderer.color;
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
        lockSpriteRenderer.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
        lockIlluminatedSpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    public override void ResetToInstantiation()
    {
        lockCollider.isTrigger = lockIsTriggerInitial;
        keySpriteRenderer.sprite = keyLocked;
        keyIllumSpriteRenderer.sprite = keyLocked;
        lockSpriteRenderer.color = lockColorInitial;
        lockIlluminatedSpriteRenderer.color = lockIlluminatedColorInitial;
    }
}
