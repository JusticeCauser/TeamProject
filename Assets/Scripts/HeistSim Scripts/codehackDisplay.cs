using UnityEngine;
using TMPro;
using System.Collections;
public class codehackDisplay : MonoBehaviour
{
    [Header("Settings")]
    public string requiredGadget = "Code Hacker";

    [Header("References")]
    [SerializeField] TMP_Text promptText;
    public GameObject displayCase;

    private bool playerInRange = false;
    private bool unlocked = false;

    private void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange || unlocked)
            return;

        // player has gadget?
        bool hasGadget = gadgetInventory.instance != null && gadgetInventory.instance.HasGadget(requiredGadget);

        if(promptText != null)
        {
            if (hasGadget)
                promptText.text = "Press E to hack";
            else
                promptText.text = "Missing: " + requiredGadget;
        }

        if(hasGadget && Input.GetKeyDown(KeyCode.E))
        {
            HackDisplay();
        }
    }

    void HackDisplay()
    {
        unlocked = true;

        if(displayCase != null)
        {
            StartCoroutine(OpenCase());
        }

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    IEnumerator OpenCase()
    {
        Quaternion startRot = displayCase.transform.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(60, 0, 0);

        Vector3 startPos = displayCase.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 0.08f, 0.6f);

        float duration = 1.5f;
        float elapsed = 0f;

        while(elapsed < duration)
        {
            displayCase.transform.localRotation = Quaternion.Lerp(startRot, endRot, elapsed / duration);
            displayCase.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        displayCase.transform.localRotation = endRot;
        displayCase.transform.localPosition = endPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !unlocked)
        {
            playerInRange = true;
            if (promptText != null)
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
