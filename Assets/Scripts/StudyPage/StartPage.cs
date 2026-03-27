using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class StartPage : StudyPage
{
    [SerializeField] private TMP_InputField participantIdInput;
    [SerializeField] private TMP_InputField conditionOrderInput;
    [SerializeField] private Button startButton;
    [SerializeField] private ExperimentManager experimentManager; // Reference to the ExperimentManager script
    [SerializeField] private TaskSpawner taskSpawner; // Reference to the TaskSpawner script
    [SerializeField] private UIPageController uiPageController; // Reference to the UIPageController script
    [SerializeField] private DataLoggerManager dataLoggerManager; // Reference to the DataLoggerManager script
    [SerializeField] private GameObject tampTarget;

    private void Awake()
    {
        startButton.onClick.AddListener(OnStartClicked);
    }

    private void OnStartClicked()
    {
        string participantId = participantIdInput.text;
        string conditionOrder = conditionOrderInput.text;
        //Debug.Log("Participant: " + participantId);
        //Debug.Log("Condition Order: " + conditionOrder);

        bool isIDNumCorrect = SetCBIndex(); // Set the CB index based on the participant ID input

        // Check if participantId matches the format "p" followed by digits
        if (Regex.IsMatch(participantId, @"^p\d+$") && isIDNumCorrect)
        {
            Debug.Log("String matches the required format.");
            dataLoggerManager.SetParticipantId(participantId);
            dataLoggerManager.InitializeSession();

            experimentManager.SetConditionOrder(int.Parse(conditionOrder)); // Pass the condition order to the ExperimentManager

            tampTarget.SetActive(false); // Activate the tamp target before starting the first task

            taskSpawner.NextTask(); // Start the first task through the TaskSpawner
            uiPageController.ShowMinimap(); // Show the minimap page after starting the first task

            dataLoggerManager.LogEvent(TaskEvent.task_start);
            dataLoggerManager.StartMovementLogging(); // Start logging at 60 Hz
        }
        else
        {
            Debug.LogWarning("Participant ID must be in the format 'p' followed by digits, e.g., p12.");
            // Optionally, show a UI warning to the user here
        }

    }

    private bool SetCBIndex()
    {
        string input = participantIdInput.text; // e.g., "p12"
        int number = 0;
        Match match = Regex.Match(input, @"\d+");
        if (match.Success)
        {
            number = int.Parse(match.Value);
        }

        if (number > 0)
        {
            experimentManager.CBIndex = number;
            Debug.Log("CB Index set to: " + experimentManager.CBIndex);
            return true;
        }
        else
        {
            return false;
        }
    }
}