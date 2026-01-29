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
        if(!playerInRange || player == null || enemy == null) return;

        bool canDrag = enemy.isKnockedOut;

        if (prompt != null)
        {
            prompt.gameObject.SetActive(canDrag);
        }

        if (!canDrag) return;

        if(InputManager.instance.interactKeyPressed())
        {
            if (!player.isDragging)
                player.startDrag(enemy);
            else if (player.isDragging)
                player.stopDrag();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
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

        if (player != null && player.isDragging)
        {
            player.stopDrag();
        }

        playerInRange = false;
        player = null;

        if (prompt != null)
            prompt.gameObject.SetActive(false);
    }
}
