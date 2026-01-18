using UnityEngine;
using TMPro;
using System.Collections;
public class printLock : MonoBehaviour
{
    [SerializeField] TMP_Text promptText;
    public GameObject scanDoor;

    private bool playerInRange = false;
    private bool unlocked = false;
    void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }
    void Update()
    {
        if(playerInRange && !unlocked && Input.GetKeyDown(KeyCode.E))
        {
            AttemptUnlock();
        }
    }

    void AttemptUnlock()
    {
        if(printManager.instance != null && printManager.instance.hasFingerprint)
        {
            // success
            unlocked = true;

            // open door/disable lock etc
            if(scanDoor != null)
            {
                StartCoroutine(SlideDoor());
                //scanDoor.transform.position += scanDoor.transform.right * 2f;
            }

            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
        else
        {
            // fail no print
            if(promptText != null)
            {
                promptText.text = "No fingerprint data!";
            }
        }
    }

    IEnumerator SlideDoor()
    {
        Vector3 startPos = scanDoor.transform.position;
        Vector3 endPos = startPos + scanDoor.transform.right * 2f;
        float duration = 1.5f;
        float elapsed = 0f;

        while(elapsed < duration)
        {
            scanDoor.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        scanDoor.transform.position = endPos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !unlocked)
        {
            playerInRange = true;
            if(promptText != null)
            {
                promptText.text = "Press E to unlock (requires fingerprint)";
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
