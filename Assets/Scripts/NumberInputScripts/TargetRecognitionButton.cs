using UnityEngine;
using System;
using System.Diagnostics;

public class TargetRecognitionButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject currentPanel;   // Panel or canvas that contains the button
    [SerializeField] private GameObject numpad;          // Reference to the numpad GameObject to activate when the button is pressed
    [SerializeField] private DataLoggerManager dataLoggerManager; // Reference to the DataLoggerManager script

    public void OnButtonPressed()
    {
        dataLoggerManager.LogEvent(TaskEvent.target_rec);
        dataLoggerManager.LogTargetRecognition(); // driveDistance defaults to 0

  

 

        // Deactivate current panel
        if (currentPanel != null)
            currentPanel.SetActive(false);

        // Activate numpad
        if (numpad != null)
            numpad.SetActive(true);
    }

}