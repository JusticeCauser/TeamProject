using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PortalTravelerRigidbody : MonoBehaviour, IPortalTraveler
{
    Rigidbody rb;
    bool hasTeleportedRecently;
    float teleportCooldown = 0.1f;
    float lastTeleportTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (hasTeleportedRecently && Time.time - lastTeleportTime > teleportCooldown)
            hasTeleportedRecently = false;
    }

    public void OnPortalEnter(Portal portal) { }

    public void OnPortalExit(Portal portal) { }

    public void Teleport(Vector3 newPosition, Quaternion newRotation, Vector3 newVelocity, Portal fromPortal, Portal toPortal)
    {
        if (hasTeleportedRecently) return;

        rb.position = newPosition;
        rb.rotation = newRotation;
        rb.linearVelocity = newVelocity;
        rb.angularVelocity = fromPortal.GetTransformMatrix().MultiplyVector(rb.angularVelocity);

        hasTeleportedRecently = true;
        lastTeleportTime = Time.time;
    }

    public Vector3 GetVelocity()
    {
        return rb.linearVelocity;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
