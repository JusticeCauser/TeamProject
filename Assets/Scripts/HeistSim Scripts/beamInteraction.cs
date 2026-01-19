using UnityEngine;
using TMPro;
public class beamInteraction : MonoBehaviour
{
    [Header("Settings")]
    public string requiredGadget = "Pocket Mirrors";

    public Transform beamStart;
    public Transform beamEnd;
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
        if (!beamDisabled && line != null && beamStart != null && beamEnd != null)
        {
            line.SetPosition(0, beamStart.position);
            line.SetPosition(1, beamEnd.position);
        }

        if(playerInRange && !beamDisabled)
        {
            bool hasGadget = gadgetInventory.instance != null && gadgetInventory.instance.HasGadget(requiredGadget);

            if(promptText != null)
            {
                if (hasGadget)
                    promptText.text = "Press E to place Mirror";
                else
                    promptText.text = "Missing: " + requiredGadget;
            }

            if(hasGadget && Input.GetKeyDown(KeyCode.E))
            {
                DisableBeam();
            }
        }
        //// handle interaction 
        //if(playerInRange && !beamDisabled && Input.GetKeyDown(KeyCode.E))
        //{
        //    DisableBeam();
        //}
    }

    void DisableBeam()
    {
        beamDisabled = true;

        if (line != null)
            line.enabled = false;

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !beamDisabled)
        {
            playerInRange = true;
            if (promptText != null)
            {
                promptText.text = "Press E to place mirror";
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
