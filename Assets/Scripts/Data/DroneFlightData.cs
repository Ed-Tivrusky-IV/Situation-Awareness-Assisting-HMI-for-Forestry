using System;
using UnityEngine;

[Serializable]
public class DroneFlightData : CommonData
{
    public string partID;      // Participant ID
    public string taskID;      // Task ID

    // Player position when drone is flying
    public float Pos_x;
    public float Pos_y;
    public float Pos_z;

    // Drone state (e.g., true if flying, false if landed)
    public bool IsFlying;
}
