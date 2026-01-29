using TMPro;
using UnityEngine;

public class KnockedOut : MonoBehaviour
{
    [SerializeField] TMP_Text prompt;
    [SerializeField] float koDuration;

    EnemyAI_Base enemy;
    PlayerController player;

    bool playerInRange;
    bool knockedOut;

    Vector3 enemyPos;

    Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponentInParent<EnemyAI_Base>();
        if(prompt != null)
        {
            prompt.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInRange || player == null || enemy == null) return;

        if(InputManager.instance.interactKeyPressed())
        {
            enemy.takeKnockOut(koDuration);

            if (prompt != null) prompt.gameObject.SetActive(false);

            playerInRange = false;
            player = null;
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

        playerInRange = false;
        player = null;

        if (prompt != null)
            prompt.gameObject.SetActive(false);
    }
}
