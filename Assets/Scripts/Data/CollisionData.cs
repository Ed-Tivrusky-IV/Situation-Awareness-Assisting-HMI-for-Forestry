using UnityEngine;

public class CollisionData : CommonData
{
    public string partID;         // Participant ID
    public string taskID;         // Task ID
    public string collidedObject; // Name of the object collided with
    public float playerPosX;      // Player's X position at collision
    public float playerPosY;      // Player's Y position at collision
    public float playerPosZ;      // Player's Z position at collision
}
