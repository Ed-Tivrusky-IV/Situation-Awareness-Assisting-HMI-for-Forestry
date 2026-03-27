using UnityEngine;
using UnityEngine.UI;

public class TaskPausePage : StudyPage
{
    [SerializeField] private TaskSpawner taskSpawner; // Reference to the TaskSpawner script
    [SerializeField] private UIPageController uiPageController; // Reference to the UIPageController script
    [SerializeField] private Button nextButton; // Reference to the Next button
    [SerializeField] private DataLoggerManager dataLoggerManager; // Reference to the DataLoggerManager script


    private void Awake()
    {
        nextButton.onClick.AddListener(OnNextClicked); // Add listener to the Next button
    }

    private void OnNextClicked()
    {
        taskSpawner.NextTask(); // Call the NextTask method in the TaskSpawner script to start the next task
        uiPageController.ShowMinimap(); // Show the minimap page after starting the next task
        dataLoggerManager.LogEvent(TaskEvent.task_start); // Log the start of the next task in the DataLoggerManager
        dataLoggerManager.StartMovementLogging(); // Start logging at 60 Hz



    }
}
