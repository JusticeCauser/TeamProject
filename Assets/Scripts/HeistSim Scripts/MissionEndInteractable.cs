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
    [SerializeField] private string missingObjectiveText = "Find the primary objective first";

    [Header("Settings")]
    [SerializeField] private bool requirePrimaryObjective = true;

    private bool used;

    public string PromptText
    {
        get
        {
            //if we require objectigve and player doesnt have it, show different text
            if(requirePrimaryObjective && PlayerController.instance != null && !PlayerController.instance.hasPrimaryObjective)
            {
                return missingObjectiveText;
            }
            return promptText;
        }
    }
    public bool CanInteract
    {
        get
        {
            if (used)
                return false;

            // if we require objective, check if player has it
            if(requirePrimaryObjective && PlayerController.instance != null)
            {
                return PlayerController.instance.hasPrimaryObjective;
            }

            return true;
        }
    }

    public void Interact()
    {
        if (used)
            return;

        if (requirePrimaryObjective && PlayerController.instance != null && !PlayerController.instance.hasPrimaryObjective)
            return;

        used = true;

        if (GameManager.instance == null)
        {
            used = false;
            return;
        }

        switch (result)
        {
            case Result.Win:
                if (ObjectiveManager.instance != null)
                    ObjectiveManager.instance.objectivesCompleted();

                SceneManager.LoadScene("GetawayScene");
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
