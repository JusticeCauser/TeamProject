using UnityEngine;
using TMPro;
public class beamInteraction : MonoBehaviour
{

    [SerializeField] TMP_Text promptText;

    private LineRenderer line;
    private bool playerInRange = false;
    private bool beamDisabled = false;
    void Start()
    {
        line = GetComponent<LineRenderer>();
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }
    void Update()
    {
        if (playerInRange && !beamDisabled && Input.GetKeyDown(KeyCode.E))
        {
            // disable beam
            if (line != null)
                line.enabled = false;

            beamDisabled = true;

            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !beamDisabled)
        {
            playerInRange = true;
            if (promptText != null)
                promptText.gameObject.SetActive(true);
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
