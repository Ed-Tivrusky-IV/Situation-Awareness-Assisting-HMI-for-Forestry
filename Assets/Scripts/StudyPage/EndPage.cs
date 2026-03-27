using UnityEngine;
using UnityEngine.UI;

public class EndPage : StudyPage
{
    [SerializeField] private Button nextButton; // Reference to the Next button

    private void Awake()
    {
        nextButton.onClick.AddListener(OnEndClicked); // Add listener to the Next button
    }

    private void OnEndClicked()
    {
        //End the experiment or return to the main menu
        Application.Quit(); // Quit the application (this will work in a built version, not in the editor)
    }
}
