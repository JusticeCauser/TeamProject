using UnityEngine;

public class alertSystem : MonoBehaviour
{
    [SerializeField] LayerMask enemyMask;

    public void raiseAlert(Vector3 position, float radius)
    {
        Debug.Log("ALERT SYSTEM TRIGGED AT " + position);

        Collider[] hits = Physics.OverlapSphere(position, radius, enemyMask);
        
        for(int i = 0; i < hits.Length; i++)
        {
            enemyAI_Guard guard = hits[i].GetComponent<enemyAI_Guard>();
            if(guard != null)
            {
                Debug.Log("Alerting Guard: " + guard.name);
                guard.onAlert(position);
            }
        }
    }
}
