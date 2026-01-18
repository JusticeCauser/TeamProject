using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorDoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Scene")]
    public string sceneToLoad = "NextLevel";

    public string PromptText => (GameProgress.Instance != null && GameProgress.Instance.ElevatorUnlocked) ? "Use Elevator" : "Locked";

    public bool CanInteract => GameProgress.Instance != null && GameProgress.Instance.ElevatorUnlocked;
    
    public void Interact()
    {
        if (!CanInteract)
            return;
        SceneManager.LoadScene(sceneToLoad);
    }
}
