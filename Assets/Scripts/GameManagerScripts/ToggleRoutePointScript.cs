using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleRoutePointScript : MonoBehaviour
{
    [SerializeField] private GameManagerScript gameManagerScript;
    [SerializeField] private GameObject[] togglePoints = new GameObject[2];
    [SerializeField] private int togglePointsCurrentIndex = 1;

    private void Start()
    {
        this.transform.position = togglePoints[togglePointsCurrentIndex].transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        togglePointsCurrentIndex = (togglePointsCurrentIndex + 1) % 2;

        this.transform.position = togglePoints[togglePointsCurrentIndex].transform.position;
        gameManagerScript.SwapRoute(false);
    }
}
