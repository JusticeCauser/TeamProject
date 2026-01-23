using UnityEngine;

public interface IInteractable
{
    // text shown in the on screen prompt
    string PromptText { get; }

    // whether interaciton is currently allowed
    bool CanInteract {  get; }

    // called when the player presses the interact key
    void Interact();
}
