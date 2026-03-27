using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

public class MinimapView : StudyPage
{
    [SerializeField] private ExperimentManager experimentManager; // Reference to the ExperimentManager script to manage the flow of the experiment
    [SerializeField] private UIPageController uiPageController; // Reference to the UIPageController script
    [SerializeField] private TMP_InputField numberInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject numpad;
    [SerializeField] private GameObject recpad;
    [SerializeField] private GameObject timeout;
    [SerializeField] private GameObject autoTakeOffBar;
    [SerializeField] private TextMeshProUGUI timerText; // Reference to the TextMeshProUGUI component to display the timer
    [SerializeField] private DataLoggerManager dataLoggerManager;
    [SerializeField] private TaskTimer taskTimer;
    [SerializeField] private FollowPlayerDrone followPlayerDrone; //reference to follow player drone script to get travelled distance;



    private int currentTaskIndex = 0; // Index to keep track of the current task
    private int currentTargetNum; // Variable to store the current target number for the question page, which = experimentManager.currentTargetNum
    private Coroutine currentRefocusCoroutine;



    void Awake()
    {
        submitButton.onClick.AddListener(OnSubmit);
        nextButton.onClick.AddListener(OnNext);
        numberInputField.onValueChanged.AddListener(OnInputValueChanged); // Add this line

    }

    void Start()
    {
        recpad.SetActive(true); // Hide the recording pad at the start
        numpad.SetActive(false); // Hide the numpad at the start
        submitButton.interactable = false;
    }

    public override void Show()
    {
        base.Show();

        if (dataLoggerManager != null)
            dataLoggerManager.HasLoggedTaskEnd = false;


        recpad.SetActive(true); // Hide the recording pad at the start
        numpad.SetActive(false); // Hide the numpad at the start
        timeout.SetActive(false); // Hide the timeout message at the start
        if (experimentManager.autoSwitch)
        {
            autoTakeOffBar.SetActive(true); // Show the auto take-off bar if auto-switching is enabled
        }
        else {
            autoTakeOffBar.SetActive(false); // Hide the auto take-off bar if auto-switching is not enabled
        }
        RefocusNextFrame(); // Refocus the input field on the next frame to ensure it's active and ready for input
    }

    private void GetCurrentTaskIndex() //Need to be called when submit button is pressed
    {
        currentTaskIndex = experimentManager.GetCurrentIndex();
    }

    private void ShowPausePage() //Need to be called when submit button is pressed
    {
        if (currentTaskIndex % 4 == 0 && currentTaskIndex != 12) // Check if the current task index is even
        {
            uiPageController.ShowConditionPause();
        }
        else if (currentTaskIndex == 12) // Check if the current task index is 12 (end of the experiment)
        {
            uiPageController.ShowEnd();
        }
        else
        {
            uiPageController.ShowTaskPause();
        }
    }

    private void OnInputValueChanged(string value)
    {
        // Enable the button only if the input is exactly two digits
        submitButton.interactable = System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d{2}$");
    }


    public void OnSubmit()
    {
        if (!numberInputField) return;
        string input = numberInputField.text.Trim();
        numberInputField.text = "";
        RefocusNextFrame();

        GetCurrentTaskIndex();
        currentTargetNum = experimentManager.currentTargetNum;

        if (taskTimer != null && taskTimer.timeRemaining > 0 && dataLoggerManager != null && !dataLoggerManager.HasLoggedTaskEnd)
        {

            if (int.TryParse(input, out int parsedInput))
            {
                dataLoggerManager.LogUserInput(parsedInput, isCorrect: parsedInput == currentTargetNum, isCompleted: true, targetNumber: currentTargetNum);
            }
            else
            {
                // Log 'none' if input is empty or invalid
                dataLoggerManager.LogUserInput(null, isCorrect: false, isCompleted: false, targetNumber: currentTargetNum);
            }


            dataLoggerManager.LogDrivingDistance();

            dataLoggerManager.LogEvent(TaskEvent.task_end); //log task end event
            dataLoggerManager.StopMovementLogging(); // Stop logging
            dataLoggerManager.HasLoggedTaskEnd = true;
        }

        ShowPausePage();
    }



    public void OnNext()
    {
        GetCurrentTaskIndex();
        currentTargetNum = experimentManager.currentTargetNum;
        // Log the input value and the current target number for data collection
        ShowPausePage();
    }

    void RefocusNextFrame()
    {
        // Stop only the refocus coroutine, not the feedback coroutine
        if (currentRefocusCoroutine != null)
        {
            StopCoroutine(currentRefocusCoroutine);
        }
        currentRefocusCoroutine = StartCoroutine(RefocusRoutine());
    }

    IEnumerator RefocusRoutine()
    {
        yield return new WaitForEndOfFrame();

        if (!numberInputField || !numberInputField.gameObject.activeInHierarchy) yield break;
        if (EventSystem.current == null) yield break;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(numberInputField.gameObject);

        numberInputField.ActivateInputField();
        numberInputField.Select();
        numberInputField.caretPosition = numberInputField.text.Length;

        currentRefocusCoroutine = null; // Clear reference when done
    }

    public void UpdateTimerDisplay(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void OnTaskTimerFinished()
    {
        timerText.text = "00:00";
        recpad.SetActive(false);
        numpad.SetActive(false);
        timeout.SetActive(true);

        if (dataLoggerManager != null && !dataLoggerManager.HasLoggedTaskEnd) //handle case where participant runs out of time without submitting an answer
        {
            dataLoggerManager.LogUserInput(null, isCorrect: false, isCompleted: false, targetNumber: currentTargetNum);

            dataLoggerManager.LogDrivingDistance(); //distance travelled

            dataLoggerManager.LogEvent(TaskEvent.task_end);
            

            dataLoggerManager.StopMovementLogging(); // Stop logging
            dataLoggerManager.HasLoggedTaskEnd = true;
        }
    }


}