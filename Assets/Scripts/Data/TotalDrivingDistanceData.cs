using System;

[Serializable]
public class TotalDrivingDistanceData : CommonData
{
    public float Duration;          // Task duration in seconds
    public float Total_distance;    // Total driving distance
    public float Final_pos_x;      // Final player position X
    public float Final_pos_y;      // Final player position Y
    public float Final_pos_z;      // Final player position Z
}
