using UnityEngine;

public class HearingTrigger : MonoBehaviour
{
    protected EnemyAI_Base enemyParent;

    void Start()
    {
        enemyParent = GetComponentInParent<EnemyAI_Base>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            enemyParent.playerInHearingRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
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
