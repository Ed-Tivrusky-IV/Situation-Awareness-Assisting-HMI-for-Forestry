using UnityEngine;

public class UIPageController : MonoBehaviour
{
    [SerializeField] private StudyPage startPage;
    [SerializeField] private StudyPage taskPausePage;
    [SerializeField] private StudyPage conditionPausePage;
    [SerializeField] private StudyPage endPage;
    [SerializeField] private StudyPage minimapView;

    void Start()
    {
        ShowStart(); // Show the start page at the beginning of the experiment
    }

    public void ShowStart()
    {
        HideAll();
        startPage.Show();
    }

    public void ShowTaskPause()
    {
        HideAll();
        taskPausePage.Show();
    }

    public void ShowConditionPause()
    {
        HideAll();
        conditionPausePage.Show();
    }

    public void ShowEnd()
    {
        HideAll();
        endPage.Show();
    }

    public void ShowMinimap()
    {
        HideAll();
        minimapView.Show();
    }

    private void HideAll()
    {
        startPage.Hide();
        taskPausePage.Hide();
        conditionPausePage.Hide();
        endPage.Hide();
        minimapView.Hide();
    }
}

