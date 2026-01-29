using UnityEngine;

public class SpawnOnSceneLoad : MonoBehaviour, IInteractable
{
    public string setSpawnId = "Default";

    public string sceneToLoad;

    public string promptText = "Enter";

    public string PromptText => promptText;
    public bool CanInteract => !string.IsNullOrEmpty(sceneToLoad);

    public void Interact()
    {
        if (!CanInteract)
            return;

        if (SaveState.Instance != null)
            SaveState.Instance.lastSpawnId = setSpawnId;

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
}
