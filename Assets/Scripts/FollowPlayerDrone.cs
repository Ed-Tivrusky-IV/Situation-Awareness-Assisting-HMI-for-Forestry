using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FollowPlayerDrone : MonoBehaviour
{
    private GameObject player; // Reference to the player GameObject
    private PlayerController playerController; // Reference to the PlayerController script
    [SerializeField] private Panoramic panoramic; // Reference to the Panoramic script
    [SerializeField] private ExperimentManager experimentManager; // Reference to the ExperimentManager script
    [SerializeField] private Image loadingProgressBar;
    [SerializeField] private GameObject bar;
    [SerializeField] private DataLoggerManager dataLoggerManager; // Reference to the DataLoggerManager script for logging data

    [SerializeField] private Image AutoProgressBar;
    
    private float autoThresholdOne = 66f; // The first distance threshold for auto take-off, used to change the color of the auto progress bar
    private float autoThresholdTwo = 33f; // The second distance threshold for auto take-off, used to change the color of the auto progress bar
    private float autoThresholdThree = 7f; // The third distance threshold for auto take-off, used to change the color of the auto progress bar
    private float distanceFromBeginningToTheTarget = 132f; // The distance from the starting point to the target, used for auto take-off progress calculation

    private bool ifFirstThresholdReached = false; // Flag to check if the first threshold has been reached for auto take-off
    private bool ifSecondThresholdReached = false; // Flag to check if the second threshold has been reached for auto take-off
    private bool ifThirdThresholdReached = false; // Flag to check if the third threshold has been reached for auto take-off

    [SerializeField] private Image CheckpointOne; // Reference to the first checkpoint image on the auto progress bar
    [SerializeField] private Image CheckpointTwo; // Reference to the second checkpoint image on the auto progress bar
    [SerializeField] private Image CheckpointThree; // Reference to the third checkpoint image on the auto progress bar

    [SerializeField] private Renderer shadow;

    private Animator[] fanAnimator; // Reference to the Animator for the fan animation

    private Vector3 offset = new(-0.7f, 0.8f, 1f); // Offset from the player position 
    private Vector3 lastPosition; // Last position of the player for distance calculation

    private bool isTakingOff = false; // Flag to check if the drone is taking off
    private bool isAscending = true; // Flag to check if the drone is ascending

    private float takeOffHeight = 2f; // Height value to take off
    private float takeOffSpeed = 2f; // Speed of the take-off
    private float pauseDuration = 1f; // Duration to pause at the top
    private Vector3 rotationSpeed = new(0, 360f, 0);
    private float rotationAngle = 360f; // Current rotation angle of the drone
    private float rotatedAngle = 0f; // Total rotated angle
    private float totalDistanceTravelled = 0f; // Total distance travelled by the player
    private float autoTakeOffDistanceThreshold = 50f; // Distance threshold for auto take-off

    private float takeOffTimer = 0f; // Timer for the take-off process
    private float takeOffDuration = 2.9f; // Duration required to complete the take-off

    private Vector4[] circles = new Vector4[50];
    private int circleCount = 0;

    //iqbal's
    private bool isDroneCapturing = false;
    private bool isBarLoaded = true;
    public bool IsDroneCapturing => isDroneCapturing;



    private PlayerControls controls;

    private Material shadowMaterial;

    void Awake()
    {
        shadowMaterial = shadow.material; // Get the material from the Renderer component
    }

    void Start()
    {
        controls = new PlayerControls();
        player = GameObject.FindWithTag("Player"); // Find the player by tag
        playerController = player.GetComponent<PlayerController>();

        fanAnimator = GetComponentsInChildren<Animator>(); // Get all Animator components in children

        lastPosition = player.transform.position; // Initialize last position for distance calculation

        DeactivatePanoBar(); // Ensure the loading progress bar is hidden at the start
    }

    private void OnTakeOff()
    {
        Debug.Log("Manual TakeOff action performed!");
        // Show the bar
        ActivatePanoBar();
        isBarLoaded = false; // Set the flag to indicate the bar is now loading
        isTakingOff = true; // Set the flag to true when space is pressed
        isDroneCapturing = true; // iqbal's variable for movement logging
        ChangeFanState(); // Change the fan state animation
        playerController.StopPlayer(); // Stop the player when taking off
        dataLoggerManager.LogDroneFlight(); // Log the take-off event in the DataLoggerManager
    }

    private void OnMenu()
    {
        Debug.Log("Menu action performed!");
        //Calculate the distance from the player to each take-off point and determine which one is the closest,
        //if that point is within the range, then enable the panoramic view with the closest take-off point's information,
        //and highlight the closest take-off point on the minimap. If the player is out of range, disable the panoramic view.
        panoramic.EnableMenu(new Vector2(player.transform.position.x, player.transform.position.z)); // Pass the player's position to the panoramic view
    }

    private void OnExitMenu()
    {
        Debug.Log("Cancel action performed!");
        panoramic.DisableChildren(); // Disable the panoramic view
    }

    // Update is called once per frame
    void Update()
    {

        //UpdateDistance();

        if (experimentManager.autoSwitch)
        {
            //UpdateDistance(); // Update the distance travelled by the player
            UpdateAutoBar(); // Update the auto take-off progress bar based on the distance travelled
        }

        if (!isBarLoaded)
        {
            UpdateLoadingProgressBar(); // Update the loading progress bar during take-off
        }

        if (isTakingOff)
        {
            TakeOff();
            panoramic.AddTakeOffPosition(new Vector2(player.transform.position.x, player.transform.position.z));
            // Add the player's position to the list of take-off positions
        }
    }

    private void ActivatePanoBar()
    {
        bar.SetActive(true);
    }

    private void DeactivatePanoBar()
    {
        bar.SetActive(false);
    }

    private void UpdateLoadingProgressBar()
    {

        // Update timer
        takeOffTimer += Time.deltaTime;

        // Update loading bar fill
        loadingProgressBar.fillAmount = Mathf.Clamp01(takeOffTimer / takeOffDuration);

        // When takeoff is complete
        if (takeOffTimer >= takeOffDuration)
        {
            takeOffTimer = 0f;
            loadingProgressBar.fillAmount = 0f;
            isBarLoaded = true; // Set the flag to indicate the bar has finished loading
            DeactivatePanoBar();
            panoramic.EnableMenu(new Vector2(player.transform.position.x, player.transform.position.z));
        }
    }

    void TakeOff()
    {
        // Logic for taking off, if needed
        float originalY = player.transform.position.y + offset.y;
        if (isAscending)
        {
            // Ascend to the take-off height
            if (transform.position.y < originalY + takeOffHeight)
            {
                //Debug.Log("current height: " + transform.position.y);
                transform.Translate(Vector3.up * takeOffSpeed * Time.deltaTime);
            }
            else if(rotatedAngle < rotationAngle)
            {
                // Pause at the top for a while
                //StartCoroutine(PauseAtTop());
                rotatedAngle += rotationSpeed.y * Time.deltaTime;
                transform.Rotate(rotationSpeed * Time.deltaTime); // Rotate the drone
            }
            else
            {
                rotatedAngle = 0f; // Reset the rotated angle
                isAscending = false; // Stop ascending after reaching the height
            }
        }
        else
        {
            if (transform.position.y > originalY)
            {
                // Descend back to the player's height
                transform.Translate(Vector3.down * takeOffSpeed * Time.deltaTime);
                //Debug.Log("current height: " + transform.position.y);
            }
            else
            {
                // Reset the drone's position and stop taking off
                isTakingOff = false;
                isAscending = true;
                ChangeFanState(); // Change the fan state animation back
                playerController.StartPlayer(); // Restart the player movement
                UpdateMinimap(); // Update the minimap with the drone's position
                isDroneCapturing = false; // Set capturing to false IQBAL

            }
        }
    }

    public float GetTotalDistanceTravelled()
{
    return totalDistanceTravelled;
}



    public void ResetTravelledDistance()
    {
        totalDistanceTravelled = 0f;
        lastPosition = player.transform.position; // Reset last position to the current position
    }

    private void UpdateDistance()
    {
        float distanceThisFrame = (player.transform.position - lastPosition).magnitude;
        totalDistanceTravelled += distanceThisFrame;
        lastPosition = player.transform.position;

        if (autoTakeOffDistanceThreshold > 0 && totalDistanceTravelled >= autoTakeOffDistanceThreshold)
        {
            ActivatePanoBar(); // Show the bar when auto take-off is triggered
            isBarLoaded = false; // Set the flag to indicate the bar is now loading
            isTakingOff = true; // Set the flag to true when space is pressed
            ChangeFanState(); // Change the fan state animation
            playerController.StopPlayer(); // Stop the player when taking off

            totalDistanceTravelled = 0f; // Reset distance after take-off
        }
        //Debug.Log($"Distance travelled: {totalDistanceTravelled}");
    }

    public void ResetAutoTakeOff()
    {
        ifFirstThresholdReached = false;
        ifSecondThresholdReached = false;
        ifThirdThresholdReached = false;

        if (CheckpointOne != null)
        {
            CheckpointOne.color = new Color32(255, 255, 255, 255); // Reset the color of the first checkpoint to white
        }
        if (CheckpointTwo != null)
        {
            CheckpointTwo.color = new Color32(255, 255, 255, 255); // Reset the color of the second checkpoint to white
        }
        if (CheckpointThree != null)
        {
            CheckpointThree.color = new Color32(255, 255, 255, 255); // Reset the color of the third checkpoint to white
        }

        // Reset markers on the auto progress bar if necessary
    }

    private void UpdateAutoBar()
    {
        // Update the auto take-off progress bar based on the distance travelled
        float distanceFromTarget = (experimentManager.GetCurrentTargetPosition() - player.transform.position).magnitude;
        float progress;
        if (distanceFromTarget < distanceFromBeginningToTheTarget)
        {
            progress = 1f - (distanceFromTarget / distanceFromBeginningToTheTarget);
        }
        else
        {
            progress = 0f;
        }

        if (AutoProgressBar != null)
            AutoProgressBar.fillAmount = progress;

        if (distanceFromTarget <= autoThresholdOne && !ifFirstThresholdReached)
        {
            ActivatePanoBar();
            isBarLoaded = false; // Set the flag to indicate the bar is now loading
            isTakingOff = true; // Set the flag to true when space is pressed
            ChangeFanState(); // Change the fan state animation
            playerController.StopPlayer(); // Stop the player when taking off

            // Log automatic takeoff here IQBAL
            dataLoggerManager.LogDroneFlight();

            //TODO: light up the first check mark on top of the bar
            if (CheckpointOne != null)
                CheckpointOne.color = new Color32(149, 255, 0, 255); // Example: Change the color of the first checkpoint to green to indicate it has been reached

            ifFirstThresholdReached = true;
        }
        else if (distanceFromTarget <= autoThresholdTwo && !ifSecondThresholdReached)
        {
            ActivatePanoBar();
            isBarLoaded = false; // Set the flag to indicate the bar is now loading
            isTakingOff = true; // Set the flag to true when space is pressed
            ChangeFanState(); // Change the fan state animation
            playerController.StopPlayer(); // Stop the player when taking off

            // Log automatic takeoff here IQBAL
            dataLoggerManager.LogDroneFlight();

            //TODO: light up the second check mark on top of the bar
            if (CheckpointTwo != null)
                CheckpointTwo.color = new Color32(149, 255, 0, 255); // Example: Change the color of the second checkpoint to green to indicate it has been reached

            ifSecondThresholdReached = true;
        }
        else if (distanceFromTarget > autoThresholdThree && distanceFromTarget < autoThresholdThree + 5f) // Reset the third threshold if the player moves away from the target after reaching the third threshold
        {
            ifThirdThresholdReached = false;
            if (CheckpointThree != null)
                CheckpointThree.color = new Color32(255, 255, 255, 255); // Reset the color of the third checkpoint to white
        }
        else if (distanceFromTarget <= autoThresholdThree && !ifThirdThresholdReached)
        {
            ActivatePanoBar();
            isBarLoaded = false; // Set the flag to indicate the bar is now loading
            isTakingOff = true; // Set the flag to true when space is pressed
            ChangeFanState(); // Change the fan state animation
            playerController.StopPlayer(); // Stop the player when taking off

            // Log automatic takeoff here IQBAL
            dataLoggerManager.LogDroneFlight();

            //TODO: light up the third check mark on top of the bar
            if (CheckpointThree != null)
                CheckpointThree.color = new Color32(149, 255, 0, 255); // Example: Change the color of the third checkpoint to green to indicate it has been reached

            ifThirdThresholdReached = true;
        }
    }

    private void ChangeFanState()
    {
        // Change the state of the fan animation
        foreach (var animator in fanAnimator)
        {
            if (animator != null)
            {
                animator.SetBool("IsTakingOff", isTakingOff);
            }
        }
    }

    private void UpdateMinimap() //Need to be debugged
    {
        if (circleCount < circles.Length)
        {
            float playerXParam = (player.transform.position.x + 500) / 1000;
            float playerZParam = (player.transform.position.z + 500) / 1000;
            Debug.Log($"Player Position: {player.transform.position}, UV: ({playerXParam}, {playerZParam})");
            Vector2 uv = new(playerXParam, playerZParam);
            //Vector2 uv = new(0f, 0f);
            circles[circleCount] = new Vector4(uv.x, uv.y, 0.04f, 0);
            circleCount++;
            Debug.Log($"Added circle at UV: ({uv.x}, {uv.y}), Circle Count: {circleCount}");

            //mat.SetInt("_CircleCount", circleCount);
            //mat.SetVectorArray("_CircleData", circles);
            shadowMaterial.SetInt("_CircleCount", circleCount);
            shadowMaterial.SetVectorArray("_CircleData", circles);
        }
    }

    public void ResetShadowMaterial()
    {
        circleCount = 0;
        Array.Clear(circles, 0, circles.Length); // Clear the circles array
        shadowMaterial.SetInt("_CircleCount", circleCount);
        shadowMaterial.SetVectorArray("_CircleData", circles);
    }

    private IEnumerator PauseAtTop()
    {
        yield return new WaitForSeconds(pauseDuration); // Wait for the specified duration
        isAscending = false; // Stop ascending after reaching the height
        //UpdateMinimap(); // Update the minimap with the drone's position
    }

}
