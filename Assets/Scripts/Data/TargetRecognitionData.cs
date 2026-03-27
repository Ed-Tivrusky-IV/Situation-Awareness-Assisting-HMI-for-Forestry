using System;

[Serializable]
public class TargetRecognitionData : CommonData
{
    public float Drive_distance;
    public float Rec_pos_x;
    public float Rec_pos_y;
    public float Rec_pos_z;  // Z position for 3D coordinates
    public float Targ_x;
    public float Targ_y;
    public float Targ_z;     // Z position for 3D coordinates

    // Player forward vector at time of target recognition
    public float Forward_x;
    public float Forward_y;
    public float Forward_z;
}

