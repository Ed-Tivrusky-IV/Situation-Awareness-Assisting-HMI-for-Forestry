using UnityEngine;
using UnityEngine.UI;

public class NumpadKey : MonoBehaviour
{
    [SerializeField] private int digit = 0; // set to -1 for backspace
    [SerializeField] private NumpadPanel target;

    private void Awake()
    {
        if (target == null)
            target = GetComponentInParent<NumpadPanel>();

        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        if (digit >= 0) target.PressDigit(digit);
        else target.PressBackspace();
    }
}
