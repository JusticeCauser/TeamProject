using UnityEngine;

public class AlertSystem : MonoBehaviour
{
    [SerializeField] LayerMask enemyMask;

    public void raiseBarkAlert(Vector3 position, Vector3 forward, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius, enemyMask);

        for (int i = 0; i < hits.Length; i++)
        {
            enemyAI_Guard guard = hits[i].GetComponent<enemyAI_Guard>();
            if (guard != null)
            {
                guard.onBarkAlert(position, forward);
            }
        }
    }
}
