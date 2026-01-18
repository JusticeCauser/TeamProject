using UnityEngine;

public class HackerInteractor : MonoBehaviour
{
    [Header("Input")]
    public KeyCode hackKey = KeyCode.H;
    public KeyCode interactKey = KeyCode.E;

    [Header("Search")]
    public float range = 3f;
    public LayerMask keypadLayer;
    public bool requireLineOfSight = true;

    [Header("UI")]
    public KeypadUI keypadPanel;
    public HackMinigameUI hackPanel;
    public PromptUI promptUI;

    private KeypadTarget cachedNearest;

    private void Update()
    {
        cachedNearest = FindClosestKeypad();
        UpdatePrompt(cachedNearest);

        if (Input.GetKeyDown(hackKey))
            OpenHack();

        if (Input.GetKeyDown(interactKey))
            OpenManual();
    }

    private void UpdatePrompt(KeypadTarget target)
    {
        if (promptUI == null)
            return;

        if ((keypadPanel != null && keypadPanel.IsOpen) || (hackPanel != null && hackPanel.gameObject.activeSelf))
        {
            promptUI.Hide();
            return;
        }

        if (target == null)
        {
            promptUI.Hide();
            return;
        }

        promptUI.Show($"[{interactKey}] Enter Code\n[{hackKey}] Hack");
    }

    private void OpenManual()
    {
        var target = FindClosestKeypad();
        if (target == null)
            return;

        keypadPanel.Open(target);
        promptUI?.Hide();
    }

    private void OpenHack()
    {
        var target = FindClosestKeypad();
        if (target == null)
            return;
        
        hackPanel.Open(target);
        promptUI?.Hide();
    }

    private KeypadTarget FindClosestKeypad()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, keypadLayer);
        KeypadTarget closest = null;
        float bestDist = float.MaxValue;

        foreach (var h in hits)
        {
            var kp = h.GetComponentInParent<KeypadTarget>();
            if (kp == null)
                continue;

            float d = Vector3.Distance(transform.position, kp.transform.position);
            if (d >= bestDist)
                continue;

            if (requireLineOfSight && !HasLineOfSight(kp.transform))
                continue;

            bestDist = d;
            closest = kp;
        }
        return closest;
    }

    private bool HasLineOfSight(Transform keypad)
    {
        Vector3 origin = transform.position + Vector3.up * 1.6f;
        Vector3 dir = keypad.position - origin;
        float dist = dir.magnitude;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist))
            return hit.transform == keypad || hit.transform.IsChildOf(keypad);

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
