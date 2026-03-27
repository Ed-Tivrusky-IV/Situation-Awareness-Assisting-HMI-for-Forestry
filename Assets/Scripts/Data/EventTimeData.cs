using System;

[Serializable]
public class EventTimeData : CommonData
{
    // "task_start", "target_rec", or "task_end"
    public TaskEvent eventType;
    public float timeRemaining; // New field for time left

}