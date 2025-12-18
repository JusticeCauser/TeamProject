using UnityEngine;

public static class PortalRaycast
{
    public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, LayerMask layerMask, int maxPortalBounces = 3)
    {
        float remainingDistance = maxDistance;
        Vector3 currentOrigin = origin;
        Vector3 currentDirection = direction;

        for (int bounce = 0; bounce <= maxPortalBounces; bounce++)
        {
            if (Physics.Raycast(currentOrigin, currentDirection, out hit, remainingDistance, layerMask))
            {
                Portal portal = hit.collider.GetComponent<Portal>();
                if (portal == null)
                    portal = hit.collider.GetComponentInParent<Portal>();

                if (portal != null && portal.LinkedPortal != null)
                {
                    Matrix4x4 m = portal.GetTransformMatrix();

                    currentOrigin = m.MultiplyPoint3x4(hit.point);
                    currentOrigin += portal.LinkedPortal.transform.forward * 0.01f;

                    currentDirection = m.MultiplyVector(currentDirection);

                    remainingDistance -= hit.distance;
                    continue;
                }

                return true;
            }

            hit = default;
            return false;
        }

        hit = default;
        return false;
    }

    public static bool RaycastAll(Vector3 origin, Vector3 direction, out RaycastHit[] hits, float maxDistance, LayerMask layerMask, int maxPortalBounces = 3)
    {
        System.Collections.Generic.List<RaycastHit> allHits = new System.Collections.Generic.List<RaycastHit>();

        float remainingDistance = maxDistance;
        Vector3 currentOrigin = origin;
        Vector3 currentDirection = direction;

        for (int bounce = 0; bounce <= maxPortalBounces; bounce++)
        {
            RaycastHit[] segmentHits = Physics.RaycastAll(currentOrigin, currentDirection, remainingDistance, layerMask);
            System.Array.Sort(segmentHits, (a, b) => a.distance.CompareTo(b.distance));

            Portal portalHit = null;
            float portalDistance = float.MaxValue;

            foreach (RaycastHit h in segmentHits)
            {
                Portal p = h.collider.GetComponent<Portal>();
                if (p == null)
                    p = h.collider.GetComponentInParent<Portal>();

                if (p != null && p.LinkedPortal != null)
                {
                    if (h.distance < portalDistance)
                    {
                        portalHit = p;
                        portalDistance = h.distance;
                    }
                }
                else if (h.distance < portalDistance)
                {
                    allHits.Add(h);
                }
            }

            if (portalHit != null)
            {
                Matrix4x4 m = portalHit.GetTransformMatrix();

                Vector3 hitPoint = currentOrigin + currentDirection * portalDistance;
                currentOrigin = m.MultiplyPoint3x4(hitPoint);
                currentOrigin += portalHit.LinkedPortal.transform.forward * 0.01f;

                currentDirection = m.MultiplyVector(currentDirection);
                remainingDistance -= portalDistance;
                continue;
            }

            break;
        }

        hits = allHits.ToArray();
        return hits.Length > 0;
    }
}
