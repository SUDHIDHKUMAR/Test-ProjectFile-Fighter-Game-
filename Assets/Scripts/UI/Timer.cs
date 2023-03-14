using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Fusion;

public class Timer : MonoBehaviour
{
    [SerializeField] float timeRemaining = 3;
    [SerializeField] bool timerIsRunning = false;
    [SerializeField] TMP_Text timeText;
    [SerializeField] string prefixText;
    Action OnTimerTicked;
    public void StartTimer(float _time, Action onTimer)
    {
        timeRemaining = _time;
        OnTimerTicked = onTimer;
        timerIsRunning = true;

    }
    public void StartTimer(float _time)
    {
        timeRemaining = _time;
        timerIsRunning = true;
    }public void StartTimer()
    {
        timerIsRunning = true;
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                OnTimerTicked?.Invoke();
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = $"{prefixText} {seconds}...";
    }
}