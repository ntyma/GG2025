using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBarrierScript : MonoBehaviourWithReset
{
    [SerializeField] private BoxCollider2D deathBarrierBoxCollider;
    [SerializeField] private SpriteRenderer deathBarrierSpriteRenderer;

    [SerializeField] private int Damage = 99;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        Health playerHealthScript;
        if (!(collision.TryGetComponent<Health>(out playerHealthScript)))
        {
            Debug.Log("Player DOES NOT have Health Component!  -  from OnTriggerEnter2D in DeathBarrierScript");
            return;
        }

        playerHealthScript.TakeDamage(Damage);
    }

    public override void ResetToInstantiation()
    {
        return;
    }
}
