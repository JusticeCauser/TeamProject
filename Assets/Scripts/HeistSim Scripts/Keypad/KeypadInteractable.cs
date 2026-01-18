using UnityEngine;

public class KeypadInteractable : MonoBehaviour, IInteractable
{
    [Header("Wiring")]
    public KeypadUI manualKeypadPanel;
    public HackMinigameUI hackPanel;
    public KeypadTarget keypadTarget;

    public string PromptText => "[E] Enter Code\n[H] Hack";
    public bool CanInteract => manualKeypadPanel != null && hackPanel != null && keypadTarget != null;

    public void Interact()
    {
        if (!CanInteract)
            return;
        manualKeypadPanel.Open(keypadTarget);
    }

    public void Hack()
    {
        if (!CanInteract)
            return;
        hackPanel.Open(keypadTarget);
    }
}
