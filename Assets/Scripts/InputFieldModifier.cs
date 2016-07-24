using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class TextSubmitEvent : UnityEvent<string> { }

/// <summary>
/// Listen for Returns.
/// Note that this makes desktop-centric assumptions.
/// This is kinda hackish. Maybe revisit.
/// </summary>
[RequireComponent(typeof(InputField))]
public class InputFieldModifier : MonoBehaviour
{
    public InputField inputField;

    public TextSubmitEvent onReturn;

    /// <summary>
    /// Refocus on text field after submitting.
    /// </summary>
    public bool refocus;

    void Reset()
    {
        inputField = GetComponent<InputField>();
        inputField.lineType = InputField.LineType.MultiLineNewline;
    }

    void OnEnable()
    {
        inputField.onValueChanged.AddListener(this.OnValueChanged);
    }

    void Update()
    {
    }

    void OnDisable()
    {
        inputField.onValueChanged.RemoveListener(this.OnValueChanged);
    }

    public void OnValueChanged(string value)
    {
        // contains instead of just checking the last one,
        // to handle return being hit in the middle
        if (value.Contains("\n"))
        {
            value = value.Replace("\n", "");
            inputField.text = "";

            if (value.Length == 0) return; // empty enter

            inputField.DeactivateInputField();
            Debug.Log(value, this);
            onReturn.Invoke(value);
            if (refocus) inputField.ActivateInputField();
        }
    }
}
