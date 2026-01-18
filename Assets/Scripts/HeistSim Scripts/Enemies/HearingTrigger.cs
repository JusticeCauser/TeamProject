using UnityEngine;

public class HearingTrigger : MonoBehaviour
{
    protected EnemyAI_Base enemyParent;

    void Start()
    {
        enemyParent = GetComponentInParent<EnemyAI_Base>();
        Debug.Log(enemyParent ? "HearingTrigger parent OK" : "HearingTrigger parent NULL");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Hearing enter: " + other.name + " tag=" + other.tag);
            Debug.Log("ENTERED TRIGGER");
            enemyParent.playerInHearingRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (enemyParent == null) return;
        if (enemyParent.state == EnemyAI_Base.guardState.KnockedOut) return;
        if (other.CompareTag("Player"))
        {
            Debug.Log("HEARING TRIGGER STAY");
            enemyParent.canHearPlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            enemyParent.playerInHearingRange = false;
        }
    }
}
