using UnityEngine;
using TMPro;
public class interactableGlass : MonoBehaviour
{
    [Header("Settings")]
    public string requiredGadget = "Glass Cutter";
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

        bool hasGadget = gadgetInventory.instance != null && gadgetInventory.instance.HasGadget(requiredGadget);

        if(promptText != null)
        {
            if (hasGadget)
                promptText.text = "Press E to use " + requiredGadget;
            else
                promptText.text = "Missing: " + requiredGadget;
        }

        if(hasGadget && Input.GetKeyDown(KeyCode.E))
        {
            CutGlass();
        }
    }

    void CutGlass()
    {
        isLocked = false;

        // remove pane of glass
        gameObject.SetActive(false);

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
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
    }

}
