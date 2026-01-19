using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class hubManager : MonoBehaviour
{
    [Header("Mission Tracking")]
    private string selectedMission = "";
    private bool inMissionFlow = false;
   
    [Header("Camera Settings")]
    public Camera playerCamera;
    public float transitionSpeed = 3f;

    [Header("Camera Targets")]
    public Transform whiteboardTarget;
    public Transform pegboardTarget;
    public Transform trunkTarget;
    public Transform computerTarget;

    private Transform originalCameraParent;
    private Vector3 originalLocalPosition;
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
    public GameObject loadoutConfirmPanel;
    public GameObject loadoutPanel;
    public GameObject trunkPanel;
    public GameObject hubBackButton;
    public GameObject mission1Button;
    public GameObject mission2Button;

    public MonoBehaviour playerController;

    private void Start()
    {
        originalCameraParent = playerCamera.transform.parent;
        originalLocalPosition = playerCamera.transform.localPosition;
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

        // hide loadout panel when going back
        if (loadoutPanel != null) loadoutPanel.SetActive(false);

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
        inMissionFlow = false;

        // reset loadout selection when exiting flow
        loadoutManager lm = FindFirstObjectByType<loadoutManager>();
        if (lm != null)
            lm.ResetSelection();

        // then hide loadout panels? what the fuck
        if(loadoutPanel != null)
            loadoutPanel.SetActive(false);

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
        playerCamera.transform.localPosition = originalLocalPosition;
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

        // show loadout panel when entering through mission flow
        if (loadoutPanel != null)
            loadoutPanel.SetActive(true);

        //currentTarget = pegboardTarget;

        //// hide confirmation panel
        //GameObject confirmPanel = GameObject.Find("MissionConfirmPanel");
        //if(confirmPanel != null)
        //{
        //    confirmPanel.SetActive(false);
        //}
    }

    public void ProceedToTrunk()
    {
        // hide confirmation panel
        if (loadoutConfirmPanel != null)
            loadoutConfirmPanel.SetActive(false);

        // transition to trunk shot
        EnterInteraction(trunkTarget, null, null, null); // no back button here, handled by NO button

        // show trunk panel
        if (trunkPanel != null)
            trunkPanel.SetActive(true);
    }

    public void ReturnToPegboardFromTrunk()
    {
        // hide trunk panel
        if (trunkPanel != null)
            trunkPanel.SetActive(false);

        // set directly to pegboard view
        currentTarget = pegboardTarget;

        previousTarget = whiteboardTarget;
        previousBackButton = hubBackButton;
        previousMission1Button = mission1Button;
        previousMission2Button = mission2Button;

        //// transition cam back to pegboard
        //EnterInteraction(pegboardTarget, hubBackButton, null, null);

        // show back button
        if (hubBackButton != null)
            hubBackButton.SetActive(true);

        // show loadout panel
        if (loadoutPanel != null)
            loadoutPanel.SetActive(true);

        // tell loadout manager deselect last tool
        loadoutManager lm = FindFirstObjectByType<loadoutManager>();
        if (lm != null)
            lm.DeselectLastTool();
    }

    public void SelectMission(string missionName)
    {
        selectedMission = missionName;
        inMissionFlow = true;

        if (missionConfirmPanel != null)
            missionConfirmPanel.SetActive(true);
    }

    public bool IsInMissionFlow()
    {
        return inMissionFlow;
    }

    public void StartMission()
    {
        // get selected gadgets from loadout manager
        loadoutManager lm = FindFirstObjectByType<loadoutManager>();
        if(lm != null)
        {
            List<string> selectedGadgets = lm.GetSelectedToolNames();

            // store in playerprefs to transfer to next scene
            PlayerPrefs.SetString("SelectedGadgets", string.Join(",", selectedGadgets));
            PlayerPrefs.Save();
        }
        if (selectedMission == "mansion")
        {
            SceneManager.LoadScene("LoadingIntroMansion");
        }
        else if (selectedMission == "facility")
        {
            SceneManager.LoadScene("LoadingIntroAsylum");
        }
    }
}