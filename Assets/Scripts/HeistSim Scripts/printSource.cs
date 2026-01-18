using UnityEngine;
using TMPro;
public class printSource : MonoBehaviour
{
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
        if(playerInRange && !printLifted && Input.GetKeyDown(KeyCode.E))
        {
            LiftPrint();
        }
    }

    void LiftPrint()
    {
        printLifted = true;

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
                promptText.text = "Press E to lift Fingerprint";
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
