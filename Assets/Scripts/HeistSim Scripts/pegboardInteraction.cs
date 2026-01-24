using UnityEngine;
using TMPro;

public class pegboardInteraction : MonoBehaviour
{
    private hubManager hubManager;
    private bool playerInRange = false;

    [SerializeField] TMP_Text promptText;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject loadoutPanel;
    // later, grayed out gear, purchase options (maybe from PC, need to discuss)

    void Start()
    {
        hubManager = FindFirstObjectByType<hubManager>();

        if (promptText != null)
            promptText.gameObject.SetActive(false);
        if (loadoutPanel != null)
            loadoutPanel.SetActive(false);
    }

    private void Update()
    {
        //// hide loadout panel if player exits 'back'
        //if(!hubManager.IsInInteraction() && loadoutPanel != null && loadoutPanel.activeSelf)
        //{
        //    loadoutPanel.SetActive(false);
        //}

        // hide prompt if in interaction
        if(hubManager.IsInInteraction())
        {
            if (promptText != null)
                promptText.gameObject.SetActive(false);
            return;
        }

        //// dont check for new interactions if in one
        //if (hubManager.IsInInteraction())
        //    return;

        bool isLookingAt = false;

        if(playerInRange)
        {
            Vector3 toBoard = (transform.position - Camera.main.transform.position).normalized;
            float dot = Vector3.Dot(Camera.main.transform.forward, toBoard);
            isLookingAt = dot > 0.7f;
        }

        if (promptText != null)
            promptText.gameObject.SetActive(isLookingAt);

        if(isLookingAt && InputManager.instance.interactKeyPressed())
            {
            // pegboard view only if not in process
            hubManager.EnterInteraction(hubManager.pegboardTarget, backButton, null, null);

            //if (promptText != null)
            //    promptText.gameObject.SetActive(true);

            // show loadout panel separately
            if (loadoutPanel != null)
                loadoutPanel.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;

            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
    }
}
