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

    // the interactable currently targeted this frame
    IInteractable current;

    // when true disable interaction
    public bool uiLocked;

    // cool down to prevent double triggering from input/UI transitons
    float inputCooldownUntil;

    readonly Collider[] hits = new Collider[32];

    void Update()
    {
        // if interacton is disabled (UI open) or in cool down
        // hide prompt and dont scan
        if (Time.unscaledTime < inputCooldownUntil || uiLocked)
        {
            if (promptUI != null)
                promptUI.Hide();
            current = null;
            return;
        }

        // find bet intactable nearby + infront
        ScanProximityFacing();

        // main interact key
        if (current != null && current.CanInteract && Input.GetKeyDown(KeyCode.E))
        {
            current.Interact();
        }

        // keypad hack key
        if (current != null &&  Input.GetKeyDown(KeyCode.H))
        {
            var keypad = (current as MonoBehaviour)?.GetComponentInParent<KeypadInteractable>();
            keypad?.Hack();
        }

    }

    // scans within range filters by facing/line of sight then selects best target
    void ScanProximityFacing()
    {
        current = null;

        // scan origin is chest height
        Vector3 origin = transform.position + Vector3.up * chestHeight;

        // grab nearby colliders without allocating memory
        int count = Physics.OverlapSphereNonAlloc(origin, range, hits, interactMask, QueryTriggerInteraction.Ignore);

        // best canidate tracking
        float bestScore = float.NegativeInfinity;
        IInteractable best = null;
        Vector3 bestPoint = default;

        // facing direction + angle threshold
        Vector3 forward = GetFacingDirection();
        float cosLimit = Mathf.Cos(maxAngle * Mathf.Deg2Rad);

        for (int i = 0; i < count; i++)
        {
            var col = hits[i];
            if (!col)
                continue;

            // looks up the hierarchy because collider might be on child
            var interactable = col.GetComponentInParent<IInteractable>();
            if (interactable == null)
                continue;

            // use closest point so big objects feel correct
            Vector3 point = col.ClosestPoint(origin);
            Vector3 to = (point - origin);
            float dist = to.magnitude;
            if (dist < 0.001f) dist = 0.001f;

            Vector3 dir = to / dist;

            // horizontal only facing checks
            Vector3 flatForward = Vector3.ProjectOnPlane(forward, Vector3.up);
            Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up);

            if (flatForward.sqrMagnitude < 0.0001f || flatDir.sqrMagnitude < 0.0001f)
                continue;

            flatForward.Normalize();
            flatDir.Normalize();


            // facing cone check (only interact with things roughly in front)
            float facing = Vector3.Dot(forward, dir);
            if (facing < cosLimit)
                continue;

            // dont allow interaction through walls/props
            if (requireLineOfSight)
            {
                if (Physics.Raycast(origin, dir, out RaycastHit block, dist, interactMask, QueryTriggerInteraction.Ignore))
                {
                    var hitInteractable = block.collider.GetComponentInParent<IInteractable>();
                    if (hitInteractable != interactable)
                        continue;
                }
            }

            // prefer centered + closer objects
            float score = (facing * 2.0f) - (dist * 0.75f);

            if (score > bestScore)
            {
                bestScore = score;
                best = interactable;
                bestPoint = point;
            }
        }

        // set selected target
        current = best;

        // update prompt ui
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

    // get direction camera is facing
    Vector3 GetFacingDirection()
    {
        if (cam != null)
            return cam.forward;

        return (facingAxis == FacingAxis.ZForward) ? transform.forward : transform.up;
    }

    // used by other systems to prevent instant double interactions
    public void SetInputCooldown(float seconds)
    {
        inputCooldownUntil = Time.unscaledTime + seconds;
    }

    // visualize interaciton range in the editor
    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * chestHeight;
        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Gizmos.DrawSphere(origin, range);
    }
}
