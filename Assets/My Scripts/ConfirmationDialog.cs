using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationDialog : MonoBehaviour
{
    public GameObject dialogPanel; // Reference to the panel GameObject
    public TMP_Text dialogMessage; // Reference to the Text element
    public Button confirmButton; // Reference to the confirm button
    public Button cancelButton; // Reference to the cancel button

    private System.Action onConfirmAction;

    public void Show(string message, System.Action onConfirm)
    {
        dialogMessage.text = message; // Set the message
        onConfirmAction = onConfirm; // Set the action to execute on confirmation

        dialogPanel.SetActive(true); // Show the dialog
    }

    public void Confirm()
    {
        onConfirmAction?.Invoke(); // Call the confirm action
        dialogPanel.SetActive(false); // Hide the dialog
    }

    public void Cancel()
    {
        dialogPanel.SetActive(false); // Hide the dialog
    }
}
