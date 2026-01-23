using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(playerController))]
[RequireComponent(typeof(CharacterController))]
public class PortalTraveler : MonoBehaviour, IPortalTraveler
{
    playerController player;
    CharacterController controller;
    cameraController camController;
    GrapplingHook grappleHook;
    wallRun wallRunScript;

    HashSet<Portal> activePortals = new HashSet<Portal>();
    bool isTeleporting;

    void Awake()
    {
        player = GetComponent<playerController>();
        controller = GetComponent<CharacterController>();
        grappleHook = GetComponent<GrapplingHook>();
        wallRunScript = GetComponent<wallRun>();
    }

    void Start()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
            camController = mainCam.GetComponent<cameraController>();
    }

    public void OnPortalEnter(Portal portal)
    {
        activePortals.Add(portal);
    }

    public void OnPortalExit(Portal portal)
    {
        activePortals.Remove(portal);
        isTeleporting = false;
    }

    public void Teleport(Vector3 newPosition, Quaternion newRotation, Vector3 newVelocity, Portal fromPortal, Portal toPortal)
    {
        if (isTeleporting) return;
        isTeleporting = true;

        if (grappleHook != null && grappleHook.IsGrappling())
            grappleHook.CancelGrapple();

        if (wallRunScript != null && wallRunScript.IsWallRunning)
            wallRunScript.ForceExitWallRun();

        controller.enabled = false;

        transform.position = newPosition;

        float yRotation = newRotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        controller.enabled = true;

        player.SetVelocity(newVelocity);
    }

    public Vector3 GetVelocity()
    {
        return player.GetVelocity();
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
