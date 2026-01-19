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

        // check if player has gadget
        bool hasGadget = gadgetInventory.instance != null && gadgetInventory.instance.HasGadget(requiredGadget);

        // show prompt when in range (or not if missing gadget)
        if (promptText != null)
        {
            if (hasGadget)
            {
                promptText.text = "Press E to use " + requiredGadget;
            }
            else
            {
                promptText.text = "Missing: " + requiredGadget;
            }
        }
        // only allow interaction if player has gadget
        if (hasGadget && Input.GetKeyDown(KeyCode.E))
        {
            CutGlass();
        }
        /////////////
        //if (!playerInRange || !isLocked)
        //    return;

        //// show prompt when in range
        //if (promptText != null)
        //    promptText.text = "Press E to use " + requiredGadget;

        //// test world, assume player has gadget
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    CutGlass();
        //}
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
