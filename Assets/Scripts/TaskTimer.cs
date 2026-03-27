using UnityEngine;
using System;

public class TaskTimer : MonoBehaviour
{
    public float Duration = 20f; // 2 minutes

    public float timeRemaining { get; private set; }
    private bool isRunning;

    public Action<float> OnTimeChanged;
    public Action OnTimerFinished;

    public void StartTimer()
    {
        timeRemaining = Duration;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;
        OnTimeChanged?.Invoke(timeRemaining);

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;
            OnTimerFinished?.Invoke();
        }
    }
}