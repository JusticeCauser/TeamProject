using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PortalTriggerZone : MonoBehaviour
{
    Portal parentPortal;

    void Awake()
    {
        parentPortal = GetComponentInParent<Portal>();
        if (parentPortal == null)
            Debug.LogError("PortalTriggerZone: No Portal found in parent!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (parentPortal != null)
            parentPortal.OnTravelerEnterTrigger(other);
    }

    void OnTriggerStay(Collider other)
    {
        if (parentPortal != null)
            parentPortal.OnTravelerStayTrigger(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (parentPortal != null)
            parentPortal.OnTravelerExitTrigger(other);
    }
}
