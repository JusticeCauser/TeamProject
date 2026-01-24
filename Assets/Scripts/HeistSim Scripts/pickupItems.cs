using UnityEngine;
using TMPro;
public class pickupItems : MonoBehaviour
{
    [SerializeField] itemStats item;
    [SerializeField] TMP_Text promptText;

    private bool playerInRange = false;
    private bool itemPickedUp = false;
    private void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange || item == null || itemPickedUp)
            return;

        if(promptText != null)
        {
           // promptText.text = "Press E to take " + item.itemName + " ($" + item.itemValue + ")";
           //this will show what the player currently has set for their interact key, tostring needed to display keycode name
            promptText.text = "Press " + InputManager.instance.getinteractKey().ToString() + " to take " + item.itemName + " ($" + item.itemValue + ")";
        }

        if (InputManager.instance.interactKeyPressed())
        {
            PlayerController player = PlayerController.instance;

            if (player != null)
            {
                itemPickedUp = true;
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
