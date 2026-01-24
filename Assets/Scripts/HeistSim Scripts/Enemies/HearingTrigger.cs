using UnityEngine;

public class HearingTrigger : MonoBehaviour
{
    EnemyAI_Base enemyParent;

    void Start()
    {
        enemyParent = GetComponentInParent<EnemyAI_Base>();
        Debug.Log(enemyParent ? "HearingTrigger parent OK" : "HearingTrigger parent NULL");
    }
    private void OnTriggerEnter(Collider other)
    {
        //if (enemyParent.state == EnemyAI_Base.guardState.KnockedOut) return;
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Trigger hit: {other.name} layer={LayerMask.LayerToName(other.gameObject.layer)} tag={other.tag}");
            Debug.Log("I can hear you");
            enemyParent.playerInHearingRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (enemyParent == null) return;
        if (enemyParent.state == EnemyAI_Base.guardState.KnockedOut) return;
        if(other.CompareTag("Player"))
        {
            enemyParent.sampleHearing();
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
