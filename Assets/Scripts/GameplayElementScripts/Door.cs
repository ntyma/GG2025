using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Door : MonoBehaviourWithReset
{
    public bool locked;
    public Sprite doorLocked;
    public Sprite doorUnlocked;
    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer lockSpriteRenderer;
    [SerializeField] private SpriteRenderer illumSpriteRenderer;

    // Reset Component Variable
    private bool lockedInitial;
    private Color lockSpriteRendererColorInitial;
    private bool lockColliderIsTriggerInitial;

    // Awake is called before the first frame update and before Start
    void Awake()
    {
        locked = true;
        col = GetComponent<Collider2D>();
        col.isTrigger = false;

        lockSpriteRenderer = GetComponent<SpriteRenderer>();

        // Record Instantiation Variables
        lockedInitial = locked;
        lockSpriteRendererColorInitial = lockSpriteRenderer.color;
        lockColliderIsTriggerInitial = col.isTrigger;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("BURH");
        if (other.gameObject.CompareTag("Key") && locked)
        {
            locked = false;
            col.isTrigger = true;
            lockSpriteRenderer.sprite = doorUnlocked;
            illumSpriteRenderer.sprite = doorUnlocked;
            FindObjectOfType<AudioManager>().Play("DoorOpen");
            //Destroy(other.gameObject);

            // Reset and Disable Key GameObject
            MonoBehaviourWithReset keyScript;
            if (!(other.gameObject.TryGetComponent<MonoBehaviourWithReset>(out keyScript)))
                return;
            keyScript.ResetToInstantiation();
            other.gameObject.SetActive(false);
        }
    }

    public override void ResetToInstantiation()
    {
        locked = lockedInitial;
        lockSpriteRenderer.color = lockSpriteRendererColorInitial;
        lockSpriteRenderer.sprite = doorLocked;
        illumSpriteRenderer.sprite = doorLocked;
        col.isTrigger = lockColliderIsTriggerInitial;
    }
}
