using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadInteractable : MonoBehaviour, IInteractable
{
    [Header("Scene")]
    public string sceneToLoad;

    public string promptText = "Enter";

    public string PromptText => promptText;

    public bool CanInteract => !string.IsNullOrEmpty(sceneToLoad);

    public void Interact()
    {
        if (!CanInteract)
            return;

        SceneManager.LoadScene(sceneToLoad);
    }
}
