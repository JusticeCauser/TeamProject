using TMPro;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    [SerializeField] TMP_Text prompt;

    EnemyAI_Base enemy;
    PlayerController player;

    bool playerInRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponentInParent<EnemyAI_Base>();
        if (prompt != null)
        {
            prompt.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<PlayerController>();

        playerInRange = (player != null);

        if (prompt != null && playerInRange)
        {
            prompt.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        player = null;

        if (prompt != null)
            prompt.gameObject.SetActive(false);
    }
    void startDrag()
    {
        
    }
}
