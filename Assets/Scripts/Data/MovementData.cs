using System;
using UnityEngine;

[Serializable]
public class MovementData : CommonData
{
    public float Pos_x;
    public float Pos_y;
    public float Pos_z;
    public float Dir_x;
    public float Dir_y;
    public float Dir_z;
    public bool DroneCapt;
    public float TotalTravelledDistance;
}