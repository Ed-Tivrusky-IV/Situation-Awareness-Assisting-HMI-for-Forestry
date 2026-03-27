using System;

[Serializable]
public class DrivingDistanceData : CommonData
{
    public string partID;
    public string taskID;
    public float TotalDriveDistance; // Renamed from Drive_distance
    public float End_pos_x;
    public float End_pos_y;
    public float End_pos_z;
    public float Targ_x;
    public float Targ_y;
    public float Targ_z;
}

