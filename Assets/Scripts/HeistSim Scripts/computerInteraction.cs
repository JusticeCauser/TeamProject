using UnityEngine;
using TMPro;

public class computerInteraction : MonoBehaviour
{
    private hubManager hubManager;
    private bool playerInRange = false;

    [SerializeField] TMP_Text promptText;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject pcPanel;
    // later, grayed out gear, purchase options (maybe from PC, need to discuss)

    void Start()
    {
        hubManager = FindFirstObjectByType<hubManager>();

        if (promptText != null)
            promptText.gameObject.SetActive(false);
        if (pcPanel != null)
            pcPanel.SetActive(false);
    }

    private void Update()
    {
        // bug fix? ESC to exit PC view 
        if(pcPanel != null && pcPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            //only exit if settings menu isn't open
            if(SettingsManager.instance != null && !SettingsManager.instance.isActive)
            {
                hubManager.GoBack();
            }
        }

        // hide loadout panel if player exits 'back'
        if (!hubManager.IsInInteraction() && pcPanel != null && pcPanel.activeSelf)
        {
            pcPanel.SetActive(false);
        }

        // hide prompt if in interaction
        if (hubManager.IsInInteraction())
        {
            if (promptText != null)
                promptText.gameObject.SetActive(false);
            return;
        }

        //// dont check for new interactions if in one
        //if (hubManager.IsInInteraction())
        //    return;

        bool isLookingAt = false;

        if (playerInRange)
        {
            Vector3 toBoard = (transform.position - Camera.main.transform.position).normalized;
            float dot = Vector3.Dot(Camera.main.transform.forward, toBoard);
            isLookingAt = dot > 0.7f;
        }

        if (promptText != null)
            promptText.gameObject.SetActive(isLookingAt);

        if (isLookingAt && Input.GetKeyDown(KeyCode.E))
        {
            // PC view only if not in process
            hubManager.EnterInteraction(hubManager.computerTarget, backButton, null, null);

            //if (promptText != null)
            //    promptText.gameObject.SetActive(true);

            // show pc panel separately
            if (pcPanel != null)
                pcPanel.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
    }
}
