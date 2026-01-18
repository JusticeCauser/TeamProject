using UnityEngine;

public class Interactor : MonoBehaviour
{
    public enum FacingAxis { ZForward, YUp }

    [Header("Facing Axis")]
    [SerializeField] FacingAxis facingAxis = FacingAxis.ZForward;

    [Header("Refs")]
    [SerializeField] Transform cam;
    [SerializeField] PromptUI promptUI;

    [Header("Interaction")]
    [SerializeField] float range = 2.5f;
    [SerializeField] float chestHeight = 1.4f;
    [SerializeField] LayerMask interactMask = ~0;

    [Header("Facing")]
    [SerializeField, Range(10f, 120f)] float maxAngle = 55f;
    [SerializeField] bool requireLineOfSight = true;

    [Header("Debug")]
    [SerializeField] bool debugDraw = false;

    IInteractable current;

    public bool uiLocked;
    float inputCooldownUntil;

    readonly Collider[] hits = new Collider[32];

    void Update()
    {
        ScanProximityFacing();

        if (current != null && Input.GetKeyDown(KeyCode.E))
        {
            current.Interact();
        }

        if (current != null &&  Input.GetKeyDown(KeyCode.H))
        {
            var keypad = (current as MonoBehaviour)?.GetComponentInParent<KeypadInteractable>();
            keypad?.Hack();
        }

        if (Time.unscaledTime < inputCooldownUntil)
        {
            if (promptUI != null)
                promptUI.Hide();
            current = null;
            return;
        }
        if (uiLocked)
        {
            if (promptUI != null)
                promptUI.Hide();
            current = null;
            return;
        }
    }

    void ScanProximityFacing()
    {
        current = null;

        Vector3 origin = transform.position + Vector3.up * chestHeight;
        int count = Physics.OverlapSphereNonAlloc(origin, range, hits, interactMask, QueryTriggerInteraction.Ignore);

        float bestScore = float.NegativeInfinity;
        IInteractable best = null;
        Vector3 bestPoint = default;

        Vector3 forward = GetFacingDirection();
        float cosLimit = Mathf.Cos(maxAngle * Mathf.Deg2Rad);

        for (int i = 0; i < count; i++)
        {
            var col = hits[i];
            if (!col)
                continue;

            var interactable = col.GetComponentInParent<IInteractable>();
            if (interactable == null)
                continue;

            Vector3 point = col.ClosestPoint(origin);
            Vector3 to = (point - origin);
            float dist = to.magnitude;
            if (dist < 0.001f) dist = 0.001f;

            Vector3 dir = to / dist;

            Vector3 flatForward = Vector3.ProjectOnPlane(forward, Vector3.up);
            Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up);

            if (flatForward.sqrMagnitude < 0.0001f || flatDir.sqrMagnitude < 0.0001f)
                continue;

            flatForward.Normalize();
            flatDir.Normalize();

            float facing = Vector3.Dot(forward, dir);
            if (facing < cosLimit)
                continue;

            if (requireLineOfSight)
            {
                if (Physics.Raycast(origin, dir, out RaycastHit block, dist, interactMask, QueryTriggerInteraction.Ignore))
                {
                    var hitInteractable = block.collider.GetComponentInParent<IInteractable>();
                    if (hitInteractable != interactable)
                        continue;
                }
            }

            float score = (facing * 2.0f) - (dist * 0.75f);

            if (score > bestScore)
            {
                bestScore = score;
                best = interactable;
                bestPoint = point;
            }
        }

        current = best;

        if (promptUI != null)
        {
            if (current != null)
            {
                promptUI.Show(current.PromptText);
            }
            else
            {
                promptUI.Hide();
            }
        }

        if (debugDraw)
        {
            Debug.DrawLine(origin, origin + forward * range, Color.green);
            if (current != null) Debug.DrawLine(origin, bestPoint, Color.cyan);
        }
    }

    Vector3 GetFacingDirection()
    {
        if (cam != null)
            return cam.forward;

        return (facingAxis == FacingAxis.ZForward) ? transform.forward : transform.up;
    }

    public void SetInputCooldown(float seconds)
    {
        inputCooldownUntil = Time.unscaledTime + seconds;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * chestHeight;
        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Gizmos.DrawSphere(origin, range);
    }
}
