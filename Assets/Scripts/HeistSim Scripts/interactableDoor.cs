using UnityEngine;
using TMPro;

public class interactableDoor : MonoBehaviour
{
    [Header("Settings")]
    public string requiredGadget = "Lockpick";
    public bool isLocked = true;

    [Header("UI")]
    [SerializeField] TMP_Text promptText;

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

        // show prompt when in range
        if (promptText != null)
            promptText.text = "Press E to use " + requiredGadget;

        // test world, assume player has gadget
        if (Input.GetKeyDown(KeyCode.E))
        {
            UnlockDoor();
        }
    }

    void UnlockDoor()
    {
        isLocked = false;

        // rotate door
        transform.Rotate(0, 90, 0);

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptText != null && isLocked)
                promptText.gameObject.SetActive(true);
        }
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
