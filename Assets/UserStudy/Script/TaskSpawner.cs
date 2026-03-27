using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class TaskSpawner : MonoBehaviour
{
    [SerializeField] private ExperimentManager experimentManager; // Reference to the ExperimentManager script
    [SerializeField] private GameObject player;
    [SerializeField] private FollowPlayerDrone followPlayerDrone; // Reference to the FollowPlayerDrone script to manage the drone's behavior
    [SerializeField] private Renderer shadow;
    [SerializeField] private Panoramic panoramic; // Reference to the Panoramic script to manage the list of take-off positions

    public void NextTask()
    {
        Debug.Log("Starting next task...");
        
        experimentManager.StartNextTask(); // Call the StartNextTask method in the ExperimentManager script to start the next task
        ResetStartPosition();
        followPlayerDrone.ResetShadowMaterial(); // Reset the shadow material to ensure it is visible for the next task
        ClearPanoramicList();
    }

    private void ResetStartPosition()
    {
        // Logic to reset the player's position to the starting point
        player.transform.position = new Vector3(0, player.transform.position.y, 0); // Example: Reset the player's position to the origin (0, 0, 0)
        //player.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0); // Example: Reset the player's rotation to a random orientation
        //experimentManager.FaceThePlayerTowardsTheTarget(); // Call the method in the ExperimentManager to face the player towards the target for the next task
        experimentManager.SetPlayerOrientationByOrder(); // Set the player's orientation based on the order defined in the ExperimentManager for the next task
        followPlayerDrone.ResetTravelledDistance(); // Reset the travelled distance of the FollowPlayerDrone to ensure it starts fresh for the next task
        followPlayerDrone.ResetAutoTakeOff(); // Reset the auto take-off state of the FollowPlayerDrone to ensure it starts fresh for the next task
    }

    private void ClearPanoramicList()
    {
        panoramic.ClearTakeOffPositions(); // Clear the list of take-off positions in the Panoramic script to prepare for the next task
    }
}
