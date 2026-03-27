using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Panoramic : MonoBehaviour
{
    private List<Vector2> TakeOffPositions = new List<Vector2>(); // List to store the positions of the take-off points
    private List<float> DistancesToEachTakeOffPoint = new List<float>(); // List to store the distances from the player to each take-off point
    public float Range = 5f; // Range within which the player can interact with the take-off points

    private GameObject player; // Reference to the player GameObject
    private bool isCameraOn = false; // Flag to check if the camera is on
    //public Canvas canvas; // Reference to the Canvas component for the panoramic view
    [SerializeField] private GameObject panoImage;


    void Start()
    {
        player = GameObject.FindWithTag("Player"); // Find the player by tag
        DisableChildren(); // Disable the panoramic view at the start
    }
    // Update is called once per frame
    void Update()
    {
        if (isCameraOn && player != null)
        {
            if (player != null)
            {
                transform.rotation = player.transform.rotation * Quaternion.Euler(0f, -135f, 0f); // Set the camera's rotation to the player's rotation
            }
        }
    }

    public void EnableMenu(Vector2 playerPosition)
    {
        if (TakeOffPositions.Count == 0)
            return;

        DistancesToEachTakeOffPoint.Clear();

        foreach (var pos in TakeOffPositions)
        {
            float distance = Vector2.Distance(playerPosition, pos);
            DistancesToEachTakeOffPoint.Add(distance);
        }

        float closestDistance = Mathf.Min(DistancesToEachTakeOffPoint.ToArray());

        if (closestDistance <= Range)
        {
            int closestIndex = DistancesToEachTakeOffPoint.IndexOf(closestDistance);

            EnableChildren();
            isCameraOn = true;
            SetupCameraPosition(TakeOffPositions[closestIndex]);
        }
    }


    public void DisableChildren()
    {
        isCameraOn = false; // Set the camera on flag to false
        //canvas.enabled = false; // Disable the canvas for the panoramic view
        panoImage.SetActive(false); // Disable the panoramic image
    }

    public void EnableChildren()
    {
        //canvas.enabled = true; // Enable the canvas for the panoramic view
        panoImage.SetActive(true); // Enable the panoramic image
    }

    public void SetupCameraPosition(Vector2 takeoffPosition)
    {
        // Set the camera's position to the player's position with an offset
        transform.position = new Vector3(takeoffPosition.x, transform.position.y, takeoffPosition.y);
    }

    public void AddTakeOffPosition(Vector2 position)
    {
        TakeOffPositions.Add(position); // Add a new take-off position to the list
    }

    public void ClearTakeOffPositions()
    {
        TakeOffPositions.Clear(); // Clear the list of take-off positions
        DistancesToEachTakeOffPoint.Clear(); // Clear the list of distances to each take-off point
        DisableChildren(); // Disable the panoramic view when clearing the take-off positions
    }

}
