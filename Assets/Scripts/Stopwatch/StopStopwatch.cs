using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopStopwatch : MonoBehaviour
{
    public Stopwatch stopwatch;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only consider collisions from Player
        if (collision.gameObject.tag == "Player")
        {
            stopwatch.stopwatchRunning = false;
            stopwatch.stopwatchText.color = Color.yellow;

            SaveManager.UpdateSaveData(data => data.beatenGame = true);
        }  
    }
}
