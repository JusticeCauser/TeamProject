using UnityEngine;

public class hubManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera playerCamera;
    public float transitionSpeed = 3f;

    [Header("Camera Targets")]
    public Transform whiteboardTarget;
    public Transform pegboardTarget;
    public Transform trunkTarget;
    public Transform computerTarget;

    private Transform originalCameraParent;
    private Vector3 originalLocalPosiiton;
    private Quaternion originalLocalRotation;

    private bool isInInteraction = false;
    private Transform currentTarget = null;

    private GameObject currentBackButton = null;
    private GameObject currentMission1Button = null;
    private GameObject currentMission2Button = null;

    // remmeber previous state for back button
    private Transform previousTarget = null;
    private GameObject previousBackButton = null;
    private GameObject previousMission1Button = null;
    private GameObject previousMission2Button = null;

    [Header("UI References")]
    public GameObject missionConfirmPanel;
    public GameObject hubBackButton;
    
    public MonoBehaviour playerController;

    private void Start()
    {
        originalCameraParent = playerCamera.transform.parent;
        originalLocalPosiiton = playerCamera.transform.localPosition;
        originalLocalRotation = playerCamera.transform.localRotation;
    }

    // making exit a button
    //private void Update()
    //{
    //    // ESC to exit interactable menu
    //    if(isInInteraction && Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        // only exit if settings menu isn't open
    //        if(SettingsManager.instance != null && !SettingsManager.instance.isActive)
    //        {
    //            ExitInteraction();
    //        }
    //    }
    //}

    private void LateUpdate()
    {
        // smoothly transition cam to target
        if(isInInteraction && currentTarget != null)
        {
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, currentTarget.position, Time.deltaTime * transitionSpeed);

            playerCamera.transform.rotation = Quaternion.Lerp(playerCamera.transform.rotation, currentTarget.rotation, Time.deltaTime * transitionSpeed);
        }
    }

    public void EnterInteraction(Transform target, GameObject  backButton, GameObject mission1Button, GameObject mission2Button)
    {
        // save current state as previous for back
        if(isInInteraction)
        {
            previousTarget = currentTarget;
            previousBackButton = currentBackButton;
            previousMission1Button = currentMission1Button; 
            previousMission2Button = currentMission2Button;

            // hide current buttons
            if (currentBackButton != null) currentBackButton.SetActive(false);
            if (currentMission1Button != null) currentMission1Button.SetActive(false);
            if (currentMission2Button != null) currentMission2Button.SetActive(false);
        }
        else
        {
            // first interaction disables player
            isInInteraction = true;

            if(playerController != null)
                playerController.enabled = false;

            Renderer playerRenderer = playerController.GetComponent<Renderer>();
            if (playerRenderer != null)
                playerRenderer.enabled = false;

            cameraController camScript = playerCamera.GetComponent<cameraController>();
            if (camScript != null)
                camScript.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // set new state
        currentTarget = target;
        currentBackButton = backButton;
        currentMission1Button = mission1Button;
        currentMission2Button = mission2Button;

        // show new buttons
        if (currentBackButton != null)
            currentBackButton.SetActive(true);
        if (currentMission1Button != null) currentMission1Button.SetActive(true);
        if (currentMission2Button != null) currentMission2Button.SetActive(true);

        //isInInteraction = true;
        //currentTarget = target;
        //currentBackButton = backButton;
        //currentMission1Button = mission1Button;
        //currentMission2Button = mission2Button;

        //// disable player controller
        //if (playerController != null)
        //    playerController.enabled = false;

        //// hide player renderer when viewing interactive
        //Renderer playerRenderer = playerController.GetComponent<Renderer>();
        //if (playerRenderer != null)
        //    playerRenderer.enabled = false;

        //// disable cam 
        //cameraController camScript = playerCamera.GetComponent<cameraController>();
        //if (camScript != null)
        //    camScript.enabled = false;

        //// show cursor
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    public void GoBack()
    {
        // hide current buttons
        if(currentBackButton != null) currentBackButton.SetActive(false);
        if (currentMission1Button != null) currentMission1Button.SetActive(false);
        if (currentMission2Button != null) currentMission2Button.SetActive(false);

        // go back to previous/exit
        if(previousTarget != null)
        {
            currentTarget = previousTarget;
            currentBackButton = previousBackButton;
            currentMission1Button = previousMission1Button;
            currentMission2Button = previousMission2Button;

            // show previous buttons
            if (currentBackButton != null) currentBackButton.SetActive(true);
            if (currentMission1Button != null) currentMission1Button.SetActive(true);
            if (currentMission2Button != null) currentMission2Button.SetActive(true);

            // clear previosu
            previousTarget = null;
            previousBackButton = null;
            previousMission1Button = null;
            previousMission2Button = null;
        }
        else
        {
            ExitInteraction();
        }
    }

    public void ExitInteraction()
    {
        isInInteraction = false;
        currentTarget = null;

        // show player renderer when viewing interactive
        Renderer playerRenderer = playerController.GetComponent<Renderer>();
        if (playerRenderer != null)
            playerRenderer.enabled = true;

        // reenable player control
        if (playerController != null)
            playerController.enabled = true;

        // reenable cam control?
        cameraController camScript = playerCamera.GetComponent<cameraController>();
        if (camScript != null)
            camScript.enabled = true;

        // return cam to player
        playerCamera.transform.SetParent(originalCameraParent);
        playerCamera.transform.localPosition = originalLocalPosiiton;
        playerCamera.transform.localRotation = originalLocalRotation;

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentBackButton != null)
            currentBackButton.SetActive(false);

        if (currentMission1Button != null)
            currentMission1Button.SetActive(false);

        if (currentMission2Button != null)
            currentMission2Button.SetActive(false);
    }

    public bool IsInInteraction()
    {
        return isInInteraction;
    }

    // menu procession
    public void ProceedToLoadout()
    {
        if (missionConfirmPanel != null)
            missionConfirmPanel.SetActive(false);

        EnterInteraction(pegboardTarget, hubBackButton, null, null);

        //currentTarget = pegboardTarget;

        //// hide confirmation panel
        //GameObject confirmPanel = GameObject.Find("MissionConfirmPanel");
        //if(confirmPanel != null)
        //{
        //    confirmPanel.SetActive(false);
        //}
    }
}