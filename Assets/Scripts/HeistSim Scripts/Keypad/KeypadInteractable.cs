using UnityEngine;

public class KeypadInteractable : MonoBehaviour, IInteractable
{
    [Header("Wiring")]
    public KeypadUI manualKeypadPanel;
    public KeypadTarget keypadTarget;

    public string PromptText => "Use Keypad";
    public bool CanInteract => manualKeypadPanel != null && keypadTarget != null;

    public void Interact()
    {
        if (!CanInteract)
            return;
        manualKeypadPanel.Open(keypadTarget);
    }
}
