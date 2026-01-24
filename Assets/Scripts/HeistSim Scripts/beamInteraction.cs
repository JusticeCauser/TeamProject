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

        //if(playerInRange && !beamDisabled)
        //{
        //    bool isLookingAt = false;
        //    Vector3 toBeam = (transform.position - Camera.main.transform.position).normalized;
        //    float dot = Vector3.Dot(Camera.main.transform.forward, toBeam);
        //
        //    if(dot > 0.5f)
        //    {
        //        Ray ray = new Ray(Camera.main.transform.position, toBeam);
        //        RaycastHit hit;
        //
        //        if(Physics.Raycast(ray, out hit, 5f))
        //        {
        //            if(hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
        //            {
        //                isLookingAt = true;
        //            }
        //        }
        //    }    
        //
        //    if(promptText != null)
        //    {
        //        if(isLookingAt)
        //        {
        //            bool hasGadget = gadgetInventory.instance != null && gadgetInventory.instance.HasGadget(requiredGadget);
        //
        //            promptText.text = hasGadget ?
        //                "Press E to place Mirror" :
        //                "Missing: " + requiredGadget;
        //            promptText.gameObject.SetActive(true);
        //        }
        //        else
        //        {
        //            promptText.gameObject.SetActive(false);
        //        }
        //    }
        //
        //    if(isLookingAt && Input.GetKeyDown(KeyCode.E))
        //    {
        //        bool hasGadget = gadgetInventory.instance != null && gadgetInventory.instance.HasGadget(requiredGadget);
        //
        //        if (hasGadget)
        //            DisableBeam();
        //    }
        //}

        if (playerInRange && !beamDisabled)
        {
            bool hasGadget = gadgetInventory.instance != null && gadgetInventory.instance.HasGadget(requiredGadget);

            if (promptText != null)
            {
                if (hasGadget)
                    promptText.text = "Press E to place Mirror";
                else
                    promptText.text = "Missing: " + requiredGadget;
            }

            if (hasGadget && Input.GetKeyDown(KeyCode.E))
            {
                DisableBeam();
            }
        }
        //// handle interaction 
        //if (playerInRange && !beamDisabled && Input.GetKeyDown(KeyCode.E))
        //{
        //    DisableBeam();
        //}
    }

    void DisableBeam()
    {
        beamDisabled = true;

        FeedbackUI.instance?.ShowFeedback("Beam Blocked!");

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
