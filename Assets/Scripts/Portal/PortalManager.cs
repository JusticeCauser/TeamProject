using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static PortalManager instance;

    [Header("Performance")]
    [SerializeField] int maxRecursionDepth = 2;
    [SerializeField] int maxPortalsRenderedPerFrame = 4;
    [SerializeField] float portalCullDistance = 100f;
    [SerializeField] bool useFrustumCulling = true;

    List<Portal> allPortals = new List<Portal>();
    Camera mainCamera;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void RegisterPortal(Portal portal)
    {
        if (!allPortals.Contains(portal))
            allPortals.Add(portal);
    }

    public void UnregisterPortal(Portal portal)
    {
        allPortals.Remove(portal);
    }

    void LateUpdate()
    {
        RenderVisiblePortals();
    }

    void RenderVisiblePortals()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        List<Portal> visiblePortals = new List<Portal>();

        foreach (Portal portal in allPortals)
        {
            if (!portal.gameObject.activeInHierarchy)
                continue;

            if (portal.LinkedPortal == null)
                continue;

            float distance = Vector3.Distance(portal.transform.position, mainCamera.transform.position);
            if (distance > portalCullDistance)
                continue;

            if (useFrustumCulling && !portal.IsVisibleFrom(mainCamera))
                continue;

            visiblePortals.Add(portal);
        }

        visiblePortals.Sort((a, b) =>
        {
            float distA = Vector3.Distance(a.transform.position, mainCamera.transform.position);
            float distB = Vector3.Distance(b.transform.position, mainCamera.transform.position);
            return distB.CompareTo(distA);
        });

        int rendered = 0;
        foreach (Portal portal in visiblePortals)
        {
            if (rendered >= maxPortalsRenderedPerFrame)
                break;

            RenderPortalRecursive(portal, maxRecursionDepth);
            rendered++;
        }
    }

    void RenderPortalRecursive(Portal portal, int depth)
    {
        if (depth <= 0 || portal.LinkedPortal == null)
            return;

        if (depth > 1)
        {
            foreach (Portal otherPortal in allPortals)
            {
                if (otherPortal == portal || otherPortal == portal.LinkedPortal)
                    continue;

                if (otherPortal.LinkedPortal == null)
                    continue;

                if (otherPortal.IsVisibleFrom(portal.PortalCamera))
                {
                    RenderPortalRecursive(otherPortal, depth - 1);
                }
            }
        }

        portal.Render();
    }

    public List<Portal> GetAllPortals()
    {
        return allPortals;
    }
}
