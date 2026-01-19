using UnityEngine;

public class CaptureTrigger : MonoBehaviour
{
    EnemyAI_Base enemyParent;
    DogAI enemyDog;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyParent = GetComponentInParent<EnemyAI_Base>();
        enemyDog = GetComponentInParent<DogAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (enemyParent != null)
            enemyParent.Capture(other.gameObject);
        if(enemyDog != null)
            enemyDog.bite(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (enemyParent != null)
            enemyParent.Capture(other.gameObject);
        if(enemyDog != null)
            enemyDog.bite(other.gameObject);
    }
}
