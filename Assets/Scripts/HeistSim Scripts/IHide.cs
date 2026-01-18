using TMPro;
using UnityEngine;

public class IHide : MonoBehaviour
{
    [SerializeField] TMP_Text prompt;
    [SerializeField] GameObject hideSpot;
    PlayerController player;

    bool playerInRange;

    Vector3 hidePos;
    Vector3 outsideHide;
    Transform playerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        if (hidePos != null)
        {
            hidePos = hideSpot.transform.position;
        }

        if (prompt != null)
        {
            prompt.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInRange || player == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!player.isHiding)
            {
                outsideHide = player.transform.position;
                player.transform.position = hidePos;
                player.hide();
            }
            else if (player.isHiding)
            { 
                player.exitHide(); 
                player.transform.position = outsideHide;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        player = GameManager.instance.player.GetComponent<PlayerController>();
        if (other.CompareTag("Player") && prompt != null)
        {
            prompt.gameObject.SetActive(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && prompt != null)
        {
            prompt.gameObject.SetActive(false);
            playerInRange = false;
            player = null;
        }
    }
}
