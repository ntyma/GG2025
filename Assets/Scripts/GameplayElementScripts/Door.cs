using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Door : MonoBehaviourWithReset
{
    public bool locked;
    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer lockSpriteRenderer;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            locked = false;
            col.isTrigger = true;
            lockSpriteRenderer.color = new Color(0.0f, 1.0f, 1.0f, 0.3f);
            //Destroy(other.gameObject);

            // Reset and Disable Key GameObject
            MonoBehaviourWithReset keyScript;
            if (!(other.gameObject.TryGetComponent<MonoBehaviourWithReset>(out keyScript)))
            {
                Debug.Log("Key DOES NOT have KeyScript! - from OnTriggerEnter2D() in Door.cs");
                return;
            }
            keyScript.ResetToInstantiation();
            other.gameObject.SetActive(false);
        }
    }

    public override void ResetToInstantiation()
    {
        locked = lockedInitial;
        lockSpriteRenderer.color = lockSpriteRendererColorInitial;
        col.isTrigger = lockColliderIsTriggerInitial;
    }
}
