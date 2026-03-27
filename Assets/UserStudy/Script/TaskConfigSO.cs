using UnityEngine;
using Study;

[CreateAssetMenu(fileName = "TaskConfigSO", menuName = "Scriptable Objects/TaskConfigSO")]
public class TaskConfigSO : ScriptableObject
{
    public ConditionType conditionType;
    public TaskLevel taskLevel;
    public TaskLighting taskLighting;

    [Header("Optional Study Metadata")]
    public string taskID;
}
