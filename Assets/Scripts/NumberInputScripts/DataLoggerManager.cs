using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;


public class DataLoggerManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string participantId = "p0";
    private ExperimentManager experimentManager;
    [SerializeField] private List<TaskConfigSO> taskConfigs = new List<TaskConfigSO>();

    [Header("Movement Logging")]
    [SerializeField] private bool enableMovementLogging = true;
    private bool isDroneCapturing = false;

    [Header("References for Logging")]
    [SerializeField] private Transform playerTransform; // Reference to the player
    [SerializeField] private QuestPointer questPointer; // To get current target

    [SerializeField] private FollowPlayerDrone followPlayerDrone;
    [SerializeField] private TaskTimer taskTimer; // Reference to the TaskTimer



    // Cached player position - updated every frame
    private Vector3 currentPlayerPosition;
    private Vector3 lastPlayerPosition;
    private float totalDistanceTravelled = 0f; // Total distance travelled by the player

    private bool hasPlayerReference = false;

    public bool HasLoggedTaskEnd { get; set; } = false;

    private Coroutine movementLogCoroutine;





    void Awake()
    {
        experimentManager = FindObjectOfType<ExperimentManager>();
    }

    void Start()
    {
        // Try to find player reference if not assigned in inspector


        UpdatePlayerReference();
    }

    void Update()
    {
        // Update player position every frame if we have a valid reference
        UpdatePlayerPosition();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // For debug: log a custom event when spacebar is pressed
            int currentTaskIndex = experimentManager.GetPreviousTaskIndex();
            string currentTaskId = experimentManager.GetPreviousConditionTaskID();
            string partID = participantId;

            // Debug the values before logging
            Debug.Log($"Spacebar pressed - PartID: {partID}, TaskID: {currentTaskId}, TaskIndex: {currentTaskIndex}");
        }
    }

    void OnDestroy()
    {
        DataLogger.Instance?.CloseSession();
    }

    // Update the cached player position
    private void UpdatePlayerPosition()
    {
        if (hasPlayerReference && playerTransform != null)
        {
            currentPlayerPosition = playerTransform.position;
        }
        else
        {
            // Try to re-establish player reference if lost
            UpdatePlayerReference();
        }
    }

    // Establish or re-establish player reference
    private void UpdatePlayerReference()
    {
        if (playerTransform == null)
        {
            playerTransform = null; // Default to zero if no reference found
        }

        hasPlayerReference = (playerTransform != null);

        if (hasPlayerReference)
        {
            currentPlayerPosition = playerTransform.position;
            Debug.Log("Player reference established in DataLoggerManager");
        }
        else
        {
            currentPlayerPosition = Vector3.zero;
            Debug.LogWarning("DataLoggerManager: No player reference found");
        }
    }


    // Public method to manually set player reference
    public void SetPlayerReference(Transform player)
    {
        playerTransform = player;
        UpdatePlayerReference();
    }

    // Get current player position (always returns the cached position)
    public Vector3 GetCurrentPlayerPosition()
    {
        return currentPlayerPosition;
    }

    public void LogDrivingDistance(Vector3? endPos = null, Vector3? targetPos = null)
    {
        string currentTaskId = experimentManager.GetPreviousConditionTaskID();
        Vector3 endPosition = endPos ?? GetCurrentPlayerPosition();
        Vector3 targetPosition = targetPos ?? Vector3.zero;

        DataLogger.Instance.LogDrivingDistance(new DrivingDistanceData
        {
            partID = participantId,
            taskID = currentTaskId,
            TotalDriveDistance = totalDistanceTravelled, // Use the calculated total distance
            End_pos_x = endPosition.x,
            End_pos_y = endPosition.y,
            End_pos_z = endPosition.z,
            Targ_x = targetPosition.x,
            Targ_y = targetPosition.y,
            Targ_z = targetPosition.z
        });
    }






    public void LogDroneFlight()
    {
        string currentTaskId = experimentManager.GetPreviousConditionTaskID();
        Vector3 playerPos = GetCurrentPlayerPosition();

        DataLogger.Instance.LogDroneFlight(new DroneFlightData
        {
            partID = participantId,
            taskID = currentTaskId,
            Pos_x = playerPos.x,
            Pos_y = playerPos.y,
            Pos_z = playerPos.z,
            IsFlying = true,
        });
    }


    public void LogTargetRecognition(float driveDistance = 0f)
    {
        string currentTaskId = experimentManager.GetPreviousConditionTaskID();

        // Use cached player position
        Vector3 playerPos = GetCurrentPlayerPosition();

        // Get player's forward vector (direction they're facing)
        Vector3 forwardDirection = Vector3.forward; // Default
        if (playerTransform != null)
        {
            forwardDirection = playerTransform.forward;
        }

        // Get target position, we need to think how this is stored
        Vector3 targetPos = Vector3.zero;
        //if (questPointer != null && questPointer.GetTarget() != null)
        //{
        //    targetPos = questPointer.GetTarget().transform.position;
        //}

        DataLogger.Instance.LogTargetRecognition(new TargetRecognitionData
        {
            partID = participantId,
            taskID = currentTaskId,
            Drive_distance = driveDistance,
            Rec_pos_x = playerPos.x,
            Rec_pos_y = playerPos.y,
            Rec_pos_z = playerPos.z,
            Targ_x = targetPos.x,
            Targ_y = targetPos.y,
            Targ_z = targetPos.z,
            Forward_x = forwardDirection.x,  // Player forward vector X
            Forward_y = forwardDirection.y,  // Player forward vector Y
            Forward_z = forwardDirection.z   // Player forward vector Z
        });
    }

    private IEnumerator MovementLoggingRoutine(float frequencyHz)
    {
        float interval = 1f / frequencyHz;
        Vector3 lastPosition = currentPlayerPosition; // Cache the last position

        while (true)
        {
            Vector3 currentPosition = GetCurrentPlayerPosition();
            float distanceThisFrame = Vector3.Distance(currentPosition, lastPosition);

            totalDistanceTravelled += distanceThisFrame;
            lastPosition = currentPosition;

            LogMovement();

            yield return new WaitForSeconds(interval);
        }
    }




    public void StartMovementLogging(float frequencyHz = 60f)
    {
        if (movementLogCoroutine != null)
            StopCoroutine(movementLogCoroutine);

        movementLogCoroutine = StartCoroutine(MovementLoggingRoutine(frequencyHz));
    }

    public void StopMovementLogging()
    {
        if (movementLogCoroutine != null)
        {
            StopCoroutine(movementLogCoroutine);
            movementLogCoroutine = null;
        }

        totalDistanceTravelled = 0f; // Reset the total distance travelled
    }




    //public void LogTotalDrivingDistance(float duration, float totalDistance)
    //{
    //    string currentTaskId = experimentManager.GetPreviousConditionTaskID();

    //    // Use cached player position as final position
    //    Vector3 finalPos = currentPlayerPosition;

    //    DataLogger.Instance.LogTotalDrivingDistance(new TotalDrivingDistanceData
    //    {
    //        partID = participantId,
    //        taskID = currentTaskId,
    //        Duration = duration,
    //        Total_distance = totalDistance,
    //        Final_pos_x = finalPos.x,
    //        Final_pos_y = finalPos.y,
    //        Final_pos_z = finalPos.z
    //    });
    //}

    public void LogUserInput(int? input = null, bool isCorrect = false, bool isCompleted = false, int targetNumber = 0)
    {
        string currentTaskId = experimentManager.GetPreviousConditionTaskID();

        // Use int.MinValue for NaN equivalent when input is not provided
        int finalInput = input ?? int.MinValue;

        DataLogger.Instance?.LogUserInput(new UserInputData
        {
            partID = participantId,
            taskID = currentTaskId,
            Input = finalInput,
            Correct = isCorrect,
            Completed = isCompleted,
            TargetNumber = targetNumber
        });
    }


    public void LogEvent(TaskEvent eventType)
    {
        int currentTaskIndex = experimentManager.GetPreviousTaskIndex();
        string currentTaskId = experimentManager.GetPreviousConditionTaskID();
        string partID = participantId;

        // Access timeRemaining from the referenced TaskTimer
        float timeLeft = taskTimer != null ? taskTimer.timeRemaining : -1f; // -1 if TaskTimer is not assigned

        //Debug.Log($"Function called: {nameof(LogEvent)}\nCall Stack:\n{Environment.StackTrace}");



        DataLogger.Instance.LogEvent(new EventTimeData
        {
            partID = partID,
            taskID = currentTaskId,
            eventType = eventType,
            timeRemaining = timeLeft
        });
    }


    public void LogMovement()
    {
        if (!hasPlayerReference) return;

        string currentTaskId = experimentManager.GetPreviousConditionTaskID();
        Vector3 currentPos = GetCurrentPlayerPosition();

        Vector3 forwardDirection = Vector3.forward;
        if (playerTransform != null)
        {
            forwardDirection = playerTransform.forward;
        }

        // Get the drone capturing state from FollowPlayerDrone
        bool droneCapturing = followPlayerDrone != null && followPlayerDrone.IsDroneCapturing;

        DataLogger.Instance?.LogMovement(new MovementData
        {
            partID = participantId,
            taskID = currentTaskId,
            Pos_x = currentPos.x,
            Pos_y = currentPos.y,
            Pos_z = currentPos.z,
            Dir_x = forwardDirection.x,
            Dir_y = forwardDirection.y,
            Dir_z = forwardDirection.z,
            DroneCapt = droneCapturing,
            TotalTravelledDistance = totalDistanceTravelled
        });

        lastPlayerPosition = currentPos;
    }




    // Method to enable/disable automatic movement logging
    public void SetMovementLogging(bool enabled)
    {
        enableMovementLogging = enabled;
    }

    public void InitializeSession()
    {
        DataLogger.Instance.InitializeSession(participantId);
        Debug.Log($"Persistent Data Path: {Application.persistentDataPath}");
    }

    public void SetParticipantId(string id)
    {
        participantId = id;
    }

    public string GetParticipantId()
    {
        return participantId;
    }

    // Property to access the logger directly if needed
    public IDataLogger Logger => DataLogger.Instance;
}
