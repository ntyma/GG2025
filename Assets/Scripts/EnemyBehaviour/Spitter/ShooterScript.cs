using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterScript : StateMachineBehaviour
{
    private EnemyOverhead overhead;

    public Transform playerPos;

    public float fireRate;
    private float fireTimer;

    public float bulletLifetime;
    public float bulletSpeed;

    public LayerMask obstacleMask;

    public GameObject projectile;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        overhead = animator.GetComponent<EnemyOverhead>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer < 0)
        {
            fireTimer = fireRate;
            Vector2 firingDirection = (playerPos.position - animator.transform.position).normalized;
            GameObject bullet = Instantiate(projectile, animator.transform.position + new Vector3(firingDirection.x * animator.transform.localScale.x, firingDirection.y * animator.transform.localScale.y, 0f), Quaternion.identity);
            BulletScript bulletScript = bullet.GetComponent<BulletScript>();
            bulletScript.velocity = firingDirection * bulletSpeed;
            bulletScript.lifeTime = bulletLifetime;
            bulletScript.destroyOnWall = true;
        }


        //RaycastHit2D hit = Physics2D.Raycast(animator.transform.position, firingDirection, overhead.viewDst, obstacleMask);

        //if(hit) Debug.Log(hit.transform.position);
    }
}
