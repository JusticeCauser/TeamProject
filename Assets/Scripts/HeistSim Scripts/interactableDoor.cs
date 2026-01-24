using UnityEngine;
using TMPro;
using System.Collections;

public class interactableDoor : MonoBehaviour
{
    [Header("Settings")]
    public string requiredGadget = "Lockpick";
    public bool isLocked = true;

    [SerializeField] Transform doorMesh;

    [Header("UI")]
    [SerializeField] TMP_Text promptText;
    [SerializeField] float lookDotThreshold = 0.7f;

    private bool playerInRange = false;

    private void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);

    }

    private void Update()
    {
        if (!playerInRange || !isLocked)
            return;

        // check if player is looking
        bool isLookingAt = false;
        if(playerInRange && doorMesh != null)
        {
            Vector3 toDoor = (doorMesh.position - Camera.main.transform.position).normalized;
            float dot = Vector3.Dot(Camera.main.transform.forward, toDoor);
            isLookingAt = dot > lookDotThreshold;
        }
        // check if player has gadget
        bool hasGadget = gadgetInventory.instance != null && gadgetInventory.instance.HasGadget(requiredGadget);

        // show prompt when in range (or not if missing gadget)
        if (promptText != null)
        {
            if(isLookingAt)
            {
                promptText.gameObject.SetActive(true);

            if (hasGadget)
                promptText.text = "Press E to use " + requiredGadget;
            else
                promptText.text = "Missing: " + requiredGadget;
            }
            else
            {
                promptText.gameObject.SetActive(false);
            }
        }
        // only allow interaction if player has gadget
        if (isLookingAt && hasGadget && Input.GetKeyDown(KeyCode.E))
        {
            UnlockDoor();
        }
        //// test world, assume player has gadget
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    UnlockDoor();
        //}
    }

    void UnlockDoor()
    {
        isLocked = false;

        FeedbackUI.instance?.ShowFeedback("Lock Picked!");

        StartCoroutine(SwingDoor());

        if (promptText != null)
            promptText.gameObject.SetActive(false);
        // rotate door
        //transform.Rotate(0, 90, 0);

        //if (promptText != null)
        //    promptText.gameObject.SetActive(false);
    }

    IEnumerator SwingDoor()
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, -90, 0);
        float duration = 1.5f;
        float elapsed = 0f;

        while(elapsed < duration)
        {
            transform.rotation = Quaternion.Lerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRot;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
            if (promptText != null && isLocked)
                promptText.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
    }
}
