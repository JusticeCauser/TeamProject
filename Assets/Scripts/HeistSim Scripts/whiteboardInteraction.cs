using UnityEngine;
using TMPro;

public class whiteboardInteraction : MonoBehaviour
{
    private hubManager hubManager;
    private bool playerInRange = false;

    [SerializeField] TMP_Text promptText;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject mission1Button;
    [SerializeField] GameObject mission2Button;

    void Start()
    {
        // find hub manager
        hubManager = FindFirstObjectByType<hubManager>();

        //hide prompt on start
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // dont check if in interaction
        if (hubManager.IsInInteraction())
            return;

        // check if player is looking at whiteboard
        bool isLookingAt = false;

        if(playerInRange)
        {
            Vector3 toBoard = (transform.position - Camera.main.transform.position).normalized;
            float dot = Vector3.Dot(Camera.main.transform.forward, toBoard);
            isLookingAt = dot > 0.7f;
        }

        // show prompt only if looked at
        if (promptText != null)
            promptText.gameObject.SetActive(isLookingAt);

        // allow E press
        if(isLookingAt && Input.GetKeyDown(KeyCode.E))
        {
            hubManager.EnterInteraction(hubManager.whiteboardTarget, backButton, mission1Button, mission2Button);

            // hide prompt when viewing
            if (promptText != null)
                promptText.gameObject.SetActive(false);

            if (backButton != null)
                backButton.SetActive(true);

            // show mission buttons
            if (mission1Button != null)
                mission1Button.SetActive(true);
            if (mission2Button != null)
                mission2Button.SetActive(true);
        }
        //// only allow interaction if not already in another
        //if(playerInRange && !hubManager.IsInInteraction() && Input.GetKeyDown(KeyCode.E))
        //{
        //    // enter whiteboard view/interaction
        //    hubManager.EnterInteraction(hubManager.whiteboardTarget);

        //    // show back button
        //    if (backButton != null)
        //        backButton.SetActive(true);
        //    // show mission selection UI
        //}
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
            //// show interaction prompt
            //if (promptText != null)
            //    promptText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
            // hide prompt
            if (promptText != null)
                promptText.gameObject.SetActive(false);

            // hide back button 
            if (backButton != null)
                backButton.SetActive(false);
        }
    }
}