using UnityEngine;
using TMPro;
public class printSource : MonoBehaviour
{
    [Header("Settings")]
    public string requiredGadget = "Fingerprint Lifter";

    [SerializeField] TMP_Text promptText;
    private bool playerInRange = false;
    private bool printLifted = false;

    void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }
    void Update()
    {
        if (!playerInRange || printLifted)
            return;

        bool hasGadget = gadgetInventory.instance != null && gadgetInventory.instance.HasGadget(requiredGadget);

        if (promptText != null)
        {
            if (hasGadget)
                promptText.text = "Press E to lift Fingerprint";
            else
                promptText.text = "Missing: " + requiredGadget;
        }

        if (hasGadget && Input.GetKeyDown(KeyCode.E))
        {
            LiftPrint();
        }
        //if (playerInRange && !printLifted && Input.GetKeyDown(KeyCode.E))
        //{
        //    LiftPrint();
        //}
    }

    void LiftPrint()
    {
        printLifted = true;

        FeedbackUI.instance?.ShowFeedback("Fingerprint lifted!");

        printManager.instance.hasFingerprint = true;

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !printLifted)
        {
            playerInRange = true;
            if(promptText != null)
            {
                //promptText.text = "Press E to lift Fingerprint";
                promptText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
    }
}
