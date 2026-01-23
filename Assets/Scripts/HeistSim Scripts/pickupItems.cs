using UnityEngine;
using TMPro;
public class pickupItems : MonoBehaviour
{
    [SerializeField] itemStats item;
    [SerializeField] TMP_Text promptText;

    private bool playerInRange = false;

    private void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange || item == null)
            return;

        if(promptText != null)
        {
            promptText.text = "Press E to take " + item.itemName + " ($" + item.itemValue + ")";
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerController player = PlayerController.instance;

            if (player != null)
            {
                player.grabItem(item);

                if (promptText != null)
                    promptText.gameObject.SetActive(false);

                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
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
