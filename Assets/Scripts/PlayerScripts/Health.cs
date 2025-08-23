using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public Transform respawnPoint;
    public FollowMouse guide;

    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private GameManagerScript gameManagerScript;
    private bool isForwardRoute = true;

    [SerializeField] private float forwardRouteHealthRegen = 0.2f;
    [SerializeField] private float backwardRouteHealthDrain = 2.0f;
    [SerializeField] private float backwardRouteHealthRegen = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }
    private void Update()
    {
        // in ForwardRoute
        if (isForwardRoute)
        {
            if (playerScript.playerIsInLight)
                Heal(forwardRouteHealthRegen * Time.deltaTime);
        }
        // in BackwardRoute
        else
        {
            // Drain Health in Light AND not in Cover
            if (playerScript.playerIsInLight && !playerScript.playerIsInCover)
                TakeDamage(backwardRouteHealthDrain * Time.deltaTime);
            // Else Regen Health
            else
                Heal(backwardRouteHealthRegen * Time.deltaTime);
        }
    }
    public void TakeDamage(float amount)
    {
        // add sound here

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            // dead, we'll have to link to a death animation later
            UnityEngine.Debug.Log("dead");

            Respawn();
        }
    }
    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Clamp((currentHealth + healAmount), 0.0f, maxHealth);
    }

    public void Respawn()
    {
        transform.position = respawnPoint.position;
        guide.transform.position = respawnPoint.position;
        currentHealth = maxHealth; // restore full health on respawn

        gameManagerScript.ResetLevelObstacles(gameManagerScript.currentGameLevel);
    }
    public void SetRoute(bool isForwardRoute = true)
    {
        this.isForwardRoute = isForwardRoute;
    }
}
