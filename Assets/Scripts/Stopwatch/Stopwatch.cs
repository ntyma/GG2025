using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stopwatch : MonoBehaviour
{
    public TextMeshProUGUI stopwatchText;
    public bool stopwatchRunning = true;
    private bool displayingTime = true;
    public float time = 0f;
    // Start is called before the first frame update
    void Start()
    {
        stopwatchText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (displayingTime) UpdateDisplay(time);
        if (stopwatchRunning)
        {
            time += Time.deltaTime;
        }
        if(Input.GetKeyDown(KeyCode.Z)) {
            Debug.Log("pressed Z");
            if (displayingTime)
            {
                stopwatchText.text = "";
                displayingTime = false;
            }
            else displayingTime = true;
        }
    }

    void UpdateDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 100F) % 100F);
        stopwatchText.text = $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }
}
