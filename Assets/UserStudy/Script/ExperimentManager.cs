using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Study;
using System;
using System.Linq;
using TMPro;

public class ExperimentManager : MonoBehaviour
{
    [SerializeField] private List<TaskConfigSO> taskConfigs = new List<TaskConfigSO>();
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private EnvironmentController environmentController; // Reference to the EnvironmentController script to manage lighting presets
    [SerializeField] private QuestPointer questPointer; // Reference to the QuestPointer script to manage the quest pointer on the minimap
    [SerializeField] private TaskTimer taskTimer; // Reference to the TaskTimer script to manage the task timer
    [SerializeField] private MinimapView minimapView;
    [SerializeField] private GameObject player; // Reference to the player GameObject, to be used in the question script
    [SerializeField] private AudioManager audioManager; // Reference to the AudioManager script to manage audio settings based on lighting conditions

    [SerializeField] private LightingPresetSO dayPreset; // Reference to the day lighting preset
    [SerializeField] private LightingPresetSO nightPreset; // Reference to the night lighting preset

    [SerializeField] private List<GameObject> easyTargets = new List<GameObject>(); // List to store the tasks to spawn
    [SerializeField] private List<GameObject> mediumTargets = new List<GameObject>();

    private GameObject currentTarget; // Reference to the current target object, to be used in the question script
    public int currentTargetNum;//to be handed over to the question script
    public bool autoSwitch = false;
    public int ConditionOrder;

    private int currentTaskIndex = 0;
    private int currentEasyTargetIndex = 0;
    private int currentMediumTargetIndex = 0;
    private int prevTaskIndex = 0;
    private int _cbIndex;
    public int CBIndex
    {
        get { return _cbIndex + 1; }
        set {
            if (value < 1 || value > 12)
            {
                Debug.LogError("Invalid CBIndex. Must be between 1 and 12.");
                return;
            }
            else _cbIndex = value - 1; 
        } 
    }

    //private Quaternion[] orientations = new Quaternion[12];

    //private void InitializeOrientations()
    //{
    //    for (int i = 0; i < 12; i++)
    //    {
    //        float angle = i * 30f; // 0, 30, 60, ..., 330
    //        orientations[i] = Quaternion.Euler(0, angle, 0);
    //    }
    //}
    private Quaternion[] fourOrientations = new Quaternion[4];
    private int[] orientationOrder = new int[4];
    private int orientationOrderIndex = 0;

    private void InitializeFourOrientations()
    {
        fourOrientations[0] = Quaternion.Euler(0, 45f, 0);
        fourOrientations[1] = Quaternion.Euler(0, -45f, 0);
        fourOrientations[2] = Quaternion.Euler(0, 135f, 0);
        fourOrientations[3] = Quaternion.Euler(0, -135f, 0);

        // Initial order
        for (int i = 0; i < 4; i++)
            orientationOrder[i] = i;

        ShuffleOrientationOrder();
        orientationOrderIndex = 0;
    }

    private void ShuffleOrientationOrder()
    {
        System.Random rng = new System.Random();
        for (int i = orientationOrder.Length - 1; i > 0; i--)
        {
            int swapIndex = rng.Next(i + 1);
            int temp = orientationOrder[i];
            orientationOrder[i] = orientationOrder[swapIndex];
            orientationOrder[swapIndex] = temp;
        }
    }

    void Start()
    {
        //autoSwitch = true;
        DisableAllTargets();
        //InitializeOrientations();
        InitializeFourOrientations();
    }

    void Update()
    {
        CalculateAndFaceTheTargetTowardsPlayer();
    }

    public void StartNextTask()
    {
        if (currentTaskIndex >= taskConfigs.Count)
        {
            Debug.Log("All tasks completed!");
            ResetExperiment();
            // You can add logic here to end the experiment, show results, or reset for another round
            return;
        }

        prevTaskIndex = currentTaskIndex; // <-- Update previous index here

        if (currentTaskIndex % 4 == 0)
        {
            ShuffleOrientationOrder();
            orientationOrderIndex = 0;
        }

        ApplyTask(taskConfigs[currentTaskIndex]);

        //SetPlayerOrientation(_cbIndex);

        currentTaskIndex++;

        taskTimer.OnTimeChanged += minimapView.UpdateTimerDisplay;
        taskTimer.OnTimerFinished += minimapView.OnTaskTimerFinished;

        taskTimer.StartTimer();
    }

    private void ApplyTask(TaskConfigSO taskConfig)
    {
        ApplyCondition(taskConfig.conditionType);
        ApplyTaskLevel(taskConfig.taskLevel);
        ApplyTaskLighting(taskConfig.taskLighting);

    }

    private void ApplyCondition(ConditionType conditionType)
    {
        switch (conditionType)
        {
            case ConditionType.Baseline:
                DisableTakeOff();
                autoSwitch = false;
                break;
            case ConditionType.AutoUpdate:
                DisableTakeOff();
                autoSwitch = true;
                break;
            case ConditionType.ManualUpdate:
                EnableTakeOff();
                autoSwitch = false;
                break;
        }
    }
    //To be implemented in the TaskSpawner script: list of tasks to spawn, spawn logic, and task completion logic
    private void ApplyTaskLevel(TaskLevel taskLevel)
    {
        switch (taskLevel)
        {
            case TaskLevel.Easy:
                // Apply easy task level settings
                if (currentEasyTargetIndex < easyTargets.Count)
                {
                    // TODO: Set up the question and answer logic for the current easy target here
                    //currentTargetNum = UnityEngine.Random.Range(10, 100);
                    easyTargets[currentEasyTargetIndex].SetActive(true);
                    currentTarget = easyTargets[currentEasyTargetIndex]; // Set the current target reference
                    TextMeshPro tmp = easyTargets[currentEasyTargetIndex].GetComponentInChildren<TextMeshPro>();
                    if (tmp != null)
                    {
                        //tmp.text = currentTargetNum.ToString(); // Set the text to the current target number
                        currentTargetNum = int.Parse(tmp.text); // Update currentTargetNum based on the text of the current target
                    }
                    questPointer.SetTarget(easyTargets[currentEasyTargetIndex]);
                    if (currentEasyTargetIndex > 0)
                    {
                        easyTargets[currentEasyTargetIndex - 1].SetActive(false);
                    }
                    currentEasyTargetIndex++;
                    if (currentEasyTargetIndex == easyTargets.Count)
                    {
                        currentEasyTargetIndex = 0; // Reset index if we exceed the list
                    }
                }
                break;
            case TaskLevel.Medium:
                // Apply medium task level settings
                if (currentMediumTargetIndex < mediumTargets.Count)
                {
                    // TODO: Set up the question and answer logic for the current medium target here
                    //currentTargetNum = UnityEngine.Random.Range(10, 100);
                    mediumTargets[currentMediumTargetIndex].SetActive(true);
                    currentTarget = mediumTargets[currentMediumTargetIndex]; // Set the current target reference
                    TextMeshPro tmp = mediumTargets[currentMediumTargetIndex].GetComponentInChildren<TextMeshPro>();
                    if (tmp != null)
                    {
                        //tmp.text = currentTargetNum.ToString(); // Set the text to the current target number
                        currentTargetNum = int.Parse(tmp.text); // Update currentTargetNum based on the text of the current target
                    }
                    questPointer.SetTarget(mediumTargets[currentMediumTargetIndex]);
                    if (currentMediumTargetIndex > 0)
                    {
                        mediumTargets[currentMediumTargetIndex - 1].SetActive(false);
                    }
                    currentMediumTargetIndex++;
                    if (currentMediumTargetIndex == mediumTargets.Count)
                    {
                        currentMediumTargetIndex = 0; // Reset index if we exceed the list
                    }
                }
                break;
        }
    }

    private void ApplyTaskLighting(TaskLighting taskLighting)
    {
        switch (taskLighting)
        {
            case TaskLighting.Day:
                // Apply day lighting settings
                audioManager.ApplyAudioSettings(true); // Apply daytime audio settings
                environmentController.ApplyPreset(dayPreset);
                TextMeshPro tmp = currentTarget.GetComponentInChildren<TextMeshPro>();
                if (tmp != null)
                {
                    environmentController.ApplyTextMaterial(dayPreset, tmp);
                }
                break;
            case TaskLighting.Night:
                // Apply night lighting settings
                audioManager.ApplyAudioSettings(false); // Apply nighttime audio settings
                environmentController.ApplyPreset(nightPreset);
                TextMeshPro tmpNight = currentTarget.GetComponentInChildren<TextMeshPro>();
                if (tmpNight != null)
                {
                    environmentController.ApplyTextMaterial(nightPreset, tmpNight);
                }
                break;
        }
    }

    private void ResetExperiment()
    {
        currentTaskIndex = 0;
    }

    private void DisableTakeOff()
    {
        playerInput.actions["TakeOff"].Disable();
    }

    private void EnableTakeOff() //has control problem, need to be updated
    {
        playerInput.actions["TakeOff"].Enable();
    }

    private void DisableAllTargets()
    {
        foreach (var target in easyTargets)
        {
            target.SetActive(false);
        }
        foreach (var target in mediumTargets)
        {
            target.SetActive(false);
        }
    }

    public void SetConditionOrder(int permutationIndex)
    {
        ConditionOrder = permutationIndex;

        // Define all 6 permutations of 3 conditions
        var allPermutations = new List<ConditionType[]>
    {
        new[] { ConditionType.Baseline, ConditionType.AutoUpdate, ConditionType.ManualUpdate },
        new[] { ConditionType.Baseline, ConditionType.ManualUpdate, ConditionType.AutoUpdate },
        new[] { ConditionType.AutoUpdate, ConditionType.Baseline, ConditionType.ManualUpdate },
        new[] { ConditionType.AutoUpdate, ConditionType.ManualUpdate, ConditionType.Baseline },
        new[] { ConditionType.ManualUpdate, ConditionType.Baseline, ConditionType.AutoUpdate },
        new[] { ConditionType.ManualUpdate, ConditionType.AutoUpdate, ConditionType.Baseline }
    };

        if (permutationIndex < 1 || permutationIndex > 6)
        {
            Debug.LogError("Invalid permutation index. Must be 1-6.");
            return;
        }

        var permutation = allPermutations[permutationIndex - 1];

        // Create a lookup for the order of each condition
        var orderLookup = permutation
            .Select((condition, index) => new { condition, index })
            .ToDictionary(x => x.condition, x => x.index);

        // Stable sort using OrderBy
        taskConfigs = taskConfigs
            .OrderBy(tc => orderLookup[tc.conditionType])
            .ToList();
    }

    public int GetCurrentIndex()
    {
        return currentTaskIndex;
    }

    public string GetCurrentConditionTaskID()
    {
        if (currentTaskIndex < taskConfigs.Count)
        {
            var currentTask = taskConfigs[currentTaskIndex];
            return currentTask.taskID;
        }
        return "No more tasks";
    }

    public int GetPreviousTaskIndex()
    {
        if(currentTaskIndex > 0)
        {
            return currentTaskIndex - 1;

        }
            
        return 0;
    }

    public string GetPreviousConditionTaskID()
    {
        if (prevTaskIndex >= 0 && prevTaskIndex < taskConfigs.Count)
        {
            var prevTask = taskConfigs[prevTaskIndex];
            return prevTask.taskID;
        }
        return "No previous task";
    }


    public Vector3 GetCurrentTargetPosition()
    {
        if (currentTarget != null)
        {
            Vector3 pos = currentTarget.transform.position;
            return new Vector3(pos.x, 0f, pos.z);
        }
        return Vector3.zero; // Return a default value if no target is active
    }

    private void CalculateAndFaceTheTargetTowardsPlayer()
    {
        if (currentTarget != null)
        {
            Vector3 targetPosition = currentTarget.transform.position;
            Vector3 playerPosition = player.transform.position;

            playerPosition.y = targetPosition.y; // Keep the y-coordinate the same for horizontal rotation
            // Calculate the direction from the target to the player
            Vector3 directionToPlayer = (playerPosition - targetPosition).normalized;
            // Calculate the rotation needed to face the player
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            targetRotation *= Quaternion.Euler(0f, 180f, 0f); // Rotate 180 degrees to face the player
            // Apply the rotation to the target
            currentTarget.transform.rotation = targetRotation;
        }
    }

    //public void SetPlayerOrientation(int CBIndex) //CBIndex from 0 to 11, corresponding to the start index of the orientation array
    //{
    //    player.transform.rotation = orientations[(CBIndex + currentTaskIndex) % orientations.Length];
    //    Debug.Log("Player orientation set to: " + orientations[(CBIndex + currentTaskIndex) % orientations.Length].eulerAngles);
    //}

    public void SetPlayerOrientationByOrder()
    {
        if (player != null)
        {
            int orientationIdx = orientationOrder[orientationOrderIndex % 4];
            FaceThePlayerTowardsTheTarget(); // Ensure the player is facing the target before applying the orientation offset
            player.transform.rotation *= fourOrientations[orientationIdx];
            Debug.Log("Player orientation set to: " + fourOrientations[orientationIdx].eulerAngles);
            orientationOrderIndex++;
        }
    }

    public void FaceThePlayerTowardsTheTarget()
    {
        if (currentTarget != null)
        {
            Vector3 targetPosition = currentTarget.transform.position;
            Vector3 playerPosition = player.transform.position;
            playerPosition.y = targetPosition.y; // Keep the y-coordinate the same for horizontal rotation
            // Calculate the direction from the player to the target
            Vector3 directionToTarget = (targetPosition - playerPosition).normalized;
            // Calculate the rotation needed to face the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            // Apply the rotation to the player
            player.transform.rotation = targetRotation;
        }
    }
}