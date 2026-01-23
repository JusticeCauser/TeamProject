using UnityEngine;

public class PortalPair : MonoBehaviour
{
    [Header("Portal References")]
    [SerializeField] Portal portalA;
    [SerializeField] Portal portalB;

    [Header("Colors")]
    [SerializeField] Color portalAColor = new Color(0f, 0.5f, 1f, 1f);
    [SerializeField] Color portalBColor = new Color(1f, 0.5f, 0f, 1f);

    void OnValidate()
    {
        if (portalA != null && portalB != null)
        {
            portalA.SetLinkedPortal(portalB);
            portalB.SetLinkedPortal(portalA);
        }
    }

    void Start()
    {
        if (portalA != null && portalB != null)
        {
            portalA.SetLinkedPortal(portalB);
            portalB.SetLinkedPortal(portalA);
            portalA.SetBorderColor(portalAColor);
            portalB.SetBorderColor(portalBColor);
        }
    }

    public void SetPortals(Portal a, Portal b)
    {
        portalA = a;
        portalB = b;

        if (portalA != null && portalB != null)
        {
            portalA.SetLinkedPortal(portalB);
            portalB.SetLinkedPortal(portalA);
        }
    }
}
