using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionEndInteractable : MonoBehaviour, IInteractable
{
    public enum Result
    {
        Win,
        LoseCaptured,
        LoseHeatExpired
    }

    [Header("Result")]
    public Result result = Result.Win;

    [Header("Prompt")]
    [SerializeField] private string promptText = "[E] Finish";

    private bool used;

    public string PromptText => promptText;
    public bool CanInteract => !used;

    public void Interact()
    {
        if (used)
            return;
        used = true;

        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager.instance is NULL in this scene.");
            used = false;
            return;
        }

        switch (result)
        {
            case Result.Win:
                GameManager.instance.missionComplete();
                break;
            case Result.LoseCaptured:
                GameManager.instance.missionFail(GameManager.fail.captured);
                break;
            case Result.LoseHeatExpired:
                GameManager.instance.missionFail(GameManager.fail.heatTimeExpired);
                break;
        }
    }
}
