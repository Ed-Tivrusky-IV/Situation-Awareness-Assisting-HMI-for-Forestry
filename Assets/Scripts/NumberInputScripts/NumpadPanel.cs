using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumpadPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField targetField;
    [SerializeField] private int maxLength = 2;

    public void PressDigit(int digit)
    {
        if (!targetField) return;
        if (targetField.text.Length >= maxLength) return;

        targetField.text += digit.ToString();
        targetField.caretPosition = targetField.text.Length;
        //Debug.Log($"TargetField GO: {targetField.name} id={targetField.gameObject.GetInstanceID()} text='{targetField.text}' " +
        //  $"textComp='{targetField.textComponent?.name}' tc_id={targetField.textComponent?.GetInstanceID()}");


        RefocusTarget();
    }

    public void PressBackspace()
    {
        if (!targetField) return;

        var t = targetField.text;
        if (t.Length == 0) return;

        targetField.text = t.Substring(0, t.Length - 1);
        targetField.caretPosition = targetField.text.Length;

        RefocusTarget();
    }

    void RefocusTarget()
    {
        if (EventSystem.current == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(targetField.gameObject);
        targetField.ActivateInputField();
    }
}
