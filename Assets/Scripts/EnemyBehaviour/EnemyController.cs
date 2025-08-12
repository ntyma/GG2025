using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform playerPos;
    public float speed;
    public float viewDst = 5f;
    [Range(0,360)]
    public float fov = 90f;
    public float angle;
    public float angleToPlayer;

    private float distance;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public Vector3 DirFromAngle(float angleInDeg)
    {
        return new Vector3(Mathf.Sin(Mathf.Deg2Rad * angleInDeg), Mathf.Cos(Mathf.Deg2Rad * angleInDeg), 0f);
    }



    // Update is called once per frame
    void Update()
    {
        distance = Vector2.Distance(transform.position, playerPos.position);
        Vector2 direction = playerPos.transform.position - transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angleToPlayer = angle - transform.rotation.eulerAngles.z;

        if(/*Mathf.Abs(angleToPlayer) < fov && */Mathf.Abs(distance) < viewDst)
        {
            Vector2 newPos = Vector2.MoveTowards(transform.position, playerPos.position, speed * Time.deltaTime);
            transform.position = newPos;
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        }
    }
}
