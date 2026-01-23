using UnityEngine;

public interface IPortalTraveler
{
    void OnPortalEnter(Portal portal);
    void OnPortalExit(Portal portal);
    void Teleport(Vector3 newPosition, Quaternion newRotation, Vector3 newVelocity, Portal fromPortal, Portal toPortal);
    Vector3 GetVelocity();
    Transform GetTransform();
}
